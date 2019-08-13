using Microsoft.AspNetCore.Mvc;
using StandardAPI.Application.Interfaces.Security;
using StandardAPI.Application.Models.Security;
using System.Threading.Tasks;

namespace StandardAPI.API.Controllers
{
    [ApiVersion("1.0")]
    public class IdentityController : BaseController
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationRequest register)
        {           

            var res = await _identityService.RegisterAsync(register);

            return Ok(res);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            var res = await _identityService.LoginAsync(login);

            return Ok(res);
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest refreshToken)
        {
            var res = await _identityService.RefreshTokenAsync(refreshToken);

            return Ok(res);
        }
    }
}