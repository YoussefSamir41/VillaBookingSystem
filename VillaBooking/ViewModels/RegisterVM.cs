using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace VillaBooking.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string Name { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare(nameof(Password))]
        [Display(Name = "Confirmed Password")]
        [DataType(DataType.Password)]
        public string ConfirmedPassword { get; set; }

        public string? Role { get; set; }
        public string? RedirectUrl { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
