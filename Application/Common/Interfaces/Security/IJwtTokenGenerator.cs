namespace ProjectPlanner.Application.Common.Interfaces.Security;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(int userId, string username);
    string GenerateRefreshToken();
}