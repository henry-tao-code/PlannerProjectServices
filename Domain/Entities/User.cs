namespace Domain.Entities;

public class User
{
    public int Id { get; init; }

    public string Username { get; init; } = null!;
    public string Email { get; set; } = null!;

    public string? PasswordHash { get; private set; }

    public string? AuthProvider { get; private set; }
    public string? AuthProviderId { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public uint RowVersion { get; set; }

    public ICollection<ProjectMember> ProjectMemberships { get; set; } = [];

    public static User CreateLocal(string userName, string email, string passwordHash)
    {
        ValidateCore(userName, email);

        return new User
        {
            Username = userName.Trim(),
            Email = email.Trim(),
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static User CreateOAuth(string userName, string email, string provider, string providerId)
    {
        ValidateCore(userName, email);

        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("OAuth provider details are required.");

        return new User
        {
            Username = userName.Trim(),
            Email = email.Trim(),
            AuthProvider = provider,
            AuthProviderId = providerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdatePassword(string newHash)
    {
        if (string.IsNullOrWhiteSpace(newHash))
            throw new ArgumentException("Password hash cannot be empty.");

        PasswordHash = newHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkOAuth(string provider, string providerId)
    {
        AuthProvider = provider;
        AuthProviderId = providerId;
        PasswordHash = null;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateCore(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");
    }
}