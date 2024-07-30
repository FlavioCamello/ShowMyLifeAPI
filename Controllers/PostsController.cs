using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowMyLifeAPI.Services.Interfaces;
using System.Security.Claims;

namespace ShowMyLifeAPI.Controllers
{
    // Controllers/PostsController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostsController(IPostService postService, 
            IWebHostEnvironment environment, 
            IHttpContextAccessor httpContextAccessor)
        {
            _postService = postService;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Post>> CreatePost(Post post)
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            post.ChangeUserId(userId);
            await _postService.CreatePostAsync(post);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, Post post)
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id != post.Id || post.UserId != userId)
            {
                return BadRequest();
            }
            await _postService.UpdatePostAsync(post);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (post == null || post.UserId != userId)
            {
                return NotFound();
            }
            await _postService.DeletePostAsync(id);
            return NoContent();
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            var post = new Post(filePath, null, userId);

            await _postService.CreatePostAsync(post);

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }
    }

}
