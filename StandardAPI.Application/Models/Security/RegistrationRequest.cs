using System.ComponentModel.DataAnnotations;

namespace StandardAPI.Application.Models.Security
{
    public class RegistrationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
