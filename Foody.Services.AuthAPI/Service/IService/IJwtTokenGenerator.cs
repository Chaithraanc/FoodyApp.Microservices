using Foody.Services.AuthAPI.Models;

namespace Foody.Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser , IEnumerable<string> roles);
    }
}
