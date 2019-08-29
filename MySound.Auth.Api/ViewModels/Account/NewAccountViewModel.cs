using System.ComponentModel.DataAnnotations;

namespace MySound.Auth.Api.ViewModels.Account
{
    public class NewAccountViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}