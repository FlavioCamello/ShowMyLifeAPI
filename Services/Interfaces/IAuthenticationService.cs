namespace ShowMyLifeAPI.Services.Interfaces
{
    public interface IAuthenticationService
    {
        string GenerateJwtToken(User user);
    }
}
