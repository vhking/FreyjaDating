using System.ComponentModel.DataAnnotations;

namespace FreyjaDating.API.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(32,MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 32 characters")]
        public string Password { get; set; }
    }
}