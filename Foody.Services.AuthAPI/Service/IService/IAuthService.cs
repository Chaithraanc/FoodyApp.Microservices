using Foody.Services.AuthAPI.Models.DTO;

namespace Foody.Services.AuthAPI.Service.IService
{
    public interface IAuthService 
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email , string roleName);
    }
}
