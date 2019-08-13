using StandardAPI.Application.Models.Security;
using System.Threading.Tasks;

namespace StandardAPI.Application.Interfaces.Security
{
    public interface IIdentityService
    {
        Task<AuthResult> RegisterAsync(RegistrationRequest register);

        Task<AuthResult> LoginAsync(LoginRequest login);

        Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest refreshToken);
    }
}
