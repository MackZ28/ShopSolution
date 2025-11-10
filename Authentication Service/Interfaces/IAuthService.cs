using AuthenticationService.DTOs;

namespace AuthenticationService.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<bool> RevokeTokenAsync(string token);
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<bool> AssignRoleAsync(Guid userId, string roleName);
    }
}
