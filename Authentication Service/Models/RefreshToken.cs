namespace AuthenticationService.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiryTime;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; }
        public string? RevokedReason { get; set; }

        // Foreign Key
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
