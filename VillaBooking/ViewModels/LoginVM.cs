using System.ComponentModel.DataAnnotations;

namespace VillaBooking.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string  Password { get; set; }
        public bool  RemeberMe { get; set; }
        public string?  RedirectUrl { get; set; }
    }
}
