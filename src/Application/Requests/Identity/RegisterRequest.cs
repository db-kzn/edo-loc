using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Application.Requests.Identity
{
    public class RegisterRequest
    {
        [Required]
        [MinLength(6)]
        public string UserName { get; set; }

        [Required]
        public string GivenName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
        public string Snils { get; set; }

        public bool ActivateUser { get; set; } = false;
        public bool AutoConfirmEmail { get; set; } = false;
    }
}