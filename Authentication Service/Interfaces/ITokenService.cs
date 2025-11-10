using AuthenticationService.Models;
using System.Security.Claims;

namespace AuthenticationService.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        Task<ClaimsPrincipal?> GetPrincipalFromExpiredTokenAsync(string token);
        Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user);
        Task<bool> ValidateRefreshTokenAsync(string token, Guid userId);
        Task RevokeRefreshTokenAsync(string token, string reason);
        Task RevokeAllUserRefreshTokensAsync(Guid userId);
    }
}
