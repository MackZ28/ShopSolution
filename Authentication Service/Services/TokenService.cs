using AuthenticationService.Config;
using AuthenticationService.Data;
using AuthenticationService.Interfaces;
using AuthenticationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationService.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(
            IOptions<JwtSettings> jwtSettings,
            AuthDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _userManager = userManager;
        }

        public Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new("firstName", user.FirstName),
            new("lastName", user.LastName)
        };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public Task<ClaimsPrincipal?> GetPrincipalFromExpiredTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task RevokeAllUserRefreshTokensAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task RevokeRefreshTokenAsync(string token, string reason)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateRefreshTokenAsync(string token, Guid userId)
        {
            throw new NotImplementedException();
        }

    }
}
