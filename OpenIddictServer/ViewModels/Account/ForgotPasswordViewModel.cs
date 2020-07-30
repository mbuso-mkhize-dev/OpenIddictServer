using System.ComponentModel.DataAnnotations;

namespace OpenIddictServer.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
