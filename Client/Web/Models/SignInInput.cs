using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class SignInInput
    {
        [Required]
        [Display(Name ="Email adresiniz")]
        public string? Email { get; set; }
        [Display(Name = "şifreniz")]
        [Required]
        public string? Password { get; set; }
        [Display(Name = "Beni hatırla")]
        public bool IsRemember { get; set; }

    }
}
