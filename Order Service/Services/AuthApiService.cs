using OrderService.DTOs;
using OrderService.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OrderService.Services
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthApiService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthApiService(
            HttpClient httpClient,
            ILogger<AuthApiService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            // Передаем JWT токен из текущего запроса в Auth Service
            SetAuthorizationHeader();
        }

        private void SetAuthorizationHeader()
        {
            var currentContext = _httpContextAccessor.HttpContext;
            if (currentContext != null)
            {
                var authHeader = currentContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        AuthenticationHeaderValue.Parse(authHeader);
                }
            }
        }

        public async Task<UserLookupDto?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Fetching user data for ID: {UserId}", userId);

                var response = await _httpClient.GetAsync($"api/internal/user/{userId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("User not found: {UserId}", userId);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserLookupDto>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("Successfully fetched user: {UserId}", userId);
                return user;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when fetching user {UserId}", userId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {UserId}", userId);
                return null;
            }
        }

        public async Task<List<UserLookupDto>> GetUsersByIdsAsync(List<Guid> userIds)
        {
            try
            {
                if (!userIds.Any())
                    return new List<UserLookupDto>();

                _logger.LogInformation("Fetching batch user data for {Count} users", userIds.Count);

                var json = JsonSerializer.Serialize(userIds);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/internal/user/batch", content);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<UserLookupDto>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("Successfully fetched {Count} users", users?.Count ?? 0);
                return users ?? new List<UserLookupDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching batch users");
                return new List<UserLookupDto>();
            }
        }

        public async Task<UserLookupDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Fetching user data for email: {Email}", email);

                var response = await _httpClient.GetAsync($"api/internal/user/email/{Uri.EscapeDataString(email)}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserLookupDto>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email {Email}", email);
                return null;
            }
        }

        public async Task<bool> CheckUserExistsAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/internal/user/exists/{userId}");

                if (!response.IsSuccessStatusCode)
                    return false;

                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);

                if (result != null && result.TryGetValue("exists", out var existsValue))
                {
                    return existsValue.ToString()?.ToLower() == "true";
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists {UserId}", userId);
                return false;
            }
        }

        Task<DTOs.UserLookupDto?> IAuthApiService.GetUserByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        Task<DTOs.UserLookupDto?> IAuthApiService.GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
