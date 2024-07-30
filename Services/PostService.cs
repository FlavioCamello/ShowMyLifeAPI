using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ShowMyLifeAPI.Data;
using ShowMyLifeAPI.HubMessage;
using ShowMyLifeAPI.Services.Interfaces;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<PostHub> _hubContext;

    public PostService(ApplicationDbContext context, IMapper mapper, IHubContext<PostHub> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _context.Posts.Include(p => p.User).OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        return await _context.Posts.Include(p => p.User).SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceivePostNotification", $"New post by {post.User.Name}");

        return post;
    }

    public async Task UpdatePostAsync(Post post)
    {
        var existingPost = await _context.Posts.FindAsync(post.Id);
        if (existingPost == null)
        {
            throw new KeyNotFoundException("Post not found.");
        }

        existingPost.Edit(post.Text); // Using the rich model's method

        await _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}
