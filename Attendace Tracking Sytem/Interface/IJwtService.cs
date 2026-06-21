using Attendace_Tracking_Sytem.Models.Account;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IJwtService
    {
        Task<string> GenerateToken(LogInCredentials user);
    }
}
