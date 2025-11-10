using OrderService.DTOs;

namespace OrderService.Interfaces
{
    public interface IAuthApiService
    {
        Task<UserLookupDto?> GetUserByIdAsync(Guid userId);
        Task<List<UserLookupDto>> GetUsersByIdsAsync(List<Guid> userIds);
        Task<UserLookupDto?> GetUserByEmailAsync(string email);
        Task<bool> CheckUserExistsAsync(Guid userId);
    }
}
