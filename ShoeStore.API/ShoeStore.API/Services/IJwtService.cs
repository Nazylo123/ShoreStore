using ShoeStore.API.Models;

namespace ShoeStore.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserAccount user);
        bool ValidateToken(string token);
        string GetUsernameFromToken(string token);
        string GetRoleFromToken(string token);
    }
} 