using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class User
{ 
    public int Id { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? InactivatedAt { get; private set; }
    public string Name { get; private set; }
    public DateTime BirthDate { get; private set; }

    private List<Post> _posts;
    public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();

    // Constructor
    public User(string email, string password, string role, string name, DateTime birthDate)
    {
        Email = email;
        Password = password; // You should hash passwords before storing them
        Role = role;
        Name = name;
        BirthDate = birthDate;
        CreatedAt = DateTime.UtcNow;
        _posts = new List<Post>();

        Validate();
    }

    public void AddPost(Post post)
    {
        if (post == null)
        {
            throw new ArgumentNullException(nameof(post));
        }

        _posts.Add(post);
    }

    public void Inactivate()
    {
        InactivatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrEmpty(newPassword))
        {
            throw new ArgumentException("New password cannot be null or empty.");
        }

        Password = newPassword; // You should hash passwords before storing them
    }

    public void HashPassword(string hashPassword)
    {
        if (string.IsNullOrEmpty(hashPassword))
        {
            throw new ArgumentException("Hash password cannot be null or empty.");
        }

        Password = hashPassword; // You should hash passwords before storing them
    }

    public string GenerateJwtToken(User user, string? keyJwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(keyJwt);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Email))
        {
            throw new ArgumentException("Email cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(Password))
        {
            throw new ArgumentException("Password cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(Role))
        {
            throw new ArgumentException("Role cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(Name))
        {
            throw new ArgumentException("Name cannot be null or empty.");
        }

        if (BirthDate == default)
        {
            throw new ArgumentException("BirthDate must be provided.");
        }
    }
}
