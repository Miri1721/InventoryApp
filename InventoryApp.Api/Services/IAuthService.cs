using InventoryApp.Api.DTOs;

namespace InventoryApp.Api.Services
{
    public interface IAuthService
    {
        AuthResponseDto Register(RegisterRequestDto request);
        AuthResponseDto Login(LoginRequestDto request);
    }
}