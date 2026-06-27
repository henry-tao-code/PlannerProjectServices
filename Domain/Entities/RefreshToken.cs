namespace Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsRevoked => RevokedAt != null;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    // --- Relationship ---
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // --- Optional metadata (VERY useful in real apps) ---
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
}