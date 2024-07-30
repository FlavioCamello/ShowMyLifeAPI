
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShowMyLifeAPI.Models
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
