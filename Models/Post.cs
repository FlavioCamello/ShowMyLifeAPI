public class Post
{
    public int Id { get; private set; }
    public string PhotoPath { get; private set; }
    public string Text { get; private set; }
    public int Likes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; }

    // Constructor
    public Post(string photoPath, string text, int userId)
    {
        PhotoPath = photoPath;
        Text = text;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        Likes = 0;

        Validate();
    }

    public void Edit(string newText)
    {
        if (string.IsNullOrEmpty(newText))
        {
            throw new ArgumentException("Text cannot be null or empty.");
        }

        Text = newText;
    }

    public void ChangeUserId(int newUserId)
    {
        if (newUserId != 0)
        {
            throw new ArgumentException("UserId cannot be 0.");
        }

        UserId = newUserId;
    }


    public void Like()
    {
        Likes++;
    }

    public void Dislike()
    {
        if (Likes > 0)
        {
            Likes--;
        }
    }

    private void Validate()
    {
        if (string.IsNullOrEmpty(PhotoPath))
        {
            throw new ArgumentException("PhotoPath cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(Text))
        {
            throw new ArgumentException("Text cannot be null or empty.");
        }

        if (UserId == default)
        {
            throw new ArgumentException("UserId must be provided.");
        }
    }
}
