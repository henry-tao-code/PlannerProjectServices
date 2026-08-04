using Konscious.Security.Cryptography;
using ProjectPlanner.Application.Common.Interfaces.Security;
using System.Security.Cryptography;
using System.Text;

namespace ProjectPlanner.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    // Standard OWASP Recommended Parameters for Argon2id
    private const int MemoryCost = 65536; // 64 MB of RAM usage
    private const int TimeCost = 3;       // 3 iterations pass
    private const int DegreeOfParallelism = 4; // 4 concurrent CPU threads
    private const int HashLength = 32;    // 32-byte hash output

    public string HashPassword(string password)
    {
        // 1. Generate a cryptographically secure secure random 16-byte salt
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // 2. Compute the raw Argon2id byte hash
        byte[] hash = ComputeArgon2IdHash(password, salt, MemoryCost, TimeCost, DegreeOfParallelism);

        // 3. Convert salt and hash arrays to Base64 strings for easy database text storage
        string saltBase64 = Convert.ToBase64String(salt);
        string hashBase64 = Convert.ToBase64String(hash);

        // 4. Return a structured cryptographic string containing all parameters needed for verification
        return $"$argon2id$m={MemoryCost},t={TimeCost},p={DegreeOfParallelism}${saltBase64}${hashBase64}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) return false;
        try
        {
            // 1. Parse the saved string parts back out using split boundaries
            var parts = passwordHash.Split('$');
            if (parts.Length != 5 || parts[1] != "argon2id") return false;

            var configParts = parts[2].Split(',');
            int memorySize = int.Parse(configParts[0].Split('=')[1]);
            int iterations = int.Parse(configParts[1].Split('=')[1]);
            int parallelism = int.Parse(configParts[2].Split('=')[1]);

            // 2. Decode the salt back into bytes from its Base64 string format
            byte[] salt = Convert.FromBase64String(parts[3]); // Index 3 is the Salt
            byte[] storedHash = Convert.FromBase64String(parts[4]);

            // 3. Re-hash the incoming text password string using the exact same extracted salt parameters
            byte[] computedHash = ComputeArgon2IdHash(password, salt, memorySize, iterations, parallelism);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
        catch
        {
            return false;
        }
    }

    private static byte[] ComputeArgon2IdHash(string password, byte[] salt, int memorySize, int iterations, int parallelism)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = memorySize,
            Iterations = iterations,
            DegreeOfParallelism = parallelism
        };

        return argon2.GetBytes(HashLength);
    }
}