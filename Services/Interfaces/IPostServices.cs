namespace ShowMyLifeAPI.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int id);
        Task<Post> CreatePostAsync(Post post);
        Task UpdatePostAsync(Post post);
        Task DeletePostAsync(int id);
    }

}
