using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShowMyLifeAPI.Data;
using ShowMyLifeAPI.Services.Interfaces;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(ApplicationDbContext context, IMapper mapper, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.Validate(); // Validate the user using the rich model method
        user.HashPassword(_passwordHasher.HashPassword(user, user.Password)); // Hash the password
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        existingUser.Validate(); // Validate the user using the rich model method
        existingUser = _mapper.Map(user, existingUser); // Update properties

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
        if (user == null)
        {
            return null; // Authentication failed
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null; // Authentication failed
        }

        return user;
    }
}
