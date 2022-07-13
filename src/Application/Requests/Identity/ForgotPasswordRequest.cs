using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Application.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}