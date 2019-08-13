using System.ComponentModel.DataAnnotations;

namespace StandardAPI.Application.Models.Security
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}