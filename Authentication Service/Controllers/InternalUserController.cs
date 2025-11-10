using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService.DTOs;
using AuthenticationService.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/internal/[controller]")]
    //[Authorize] // Защищаем внутренние API
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null || !user.IsActive)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = new UserLookupDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("batch")]
        public async Task<IActionResult> GetUsersByIds([FromBody] List<Guid> userIds)
        {
            try
            {
                if (userIds == null || !userIds.Any())
                {
                    return BadRequest(new { message = "User IDs are required" });
                }

                if (userIds.Count > 100) // Ограничение на количество
                {
                    return BadRequest(new { message = "Maximum 100 users per request" });
                }

                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id) && u.IsActive)
                    .ToListAsync();

                var userDtos = users.Select(user => new UserLookupDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by IDs");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null || !user.IsActive)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = new UserLookupDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("exists/{userId:guid}")]
        public async Task<IActionResult> CheckUserExists(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                var exists = user != null && user.IsActive;

                return Ok(new { exists, userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists: {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}