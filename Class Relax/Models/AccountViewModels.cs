using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Linq;
using System.Web;

using Class_Relax.Models;


namespace Class_Relax.Models
{

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]

        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [DataType(DataType.Password)]
       
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
     
        [Display(Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [MinLength(2, ErrorMessage = "יש לבחור שם משתמש המכיל לפחות 2 תווים")]
        public string Username { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [MinLength(2, ErrorMessage = "יש לבחור שם פרטי המכיל לפחות 2 תווים")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "סיסמה צריכה להיות בת לפחות 6 תווים *")]
        public string Password { get; set; }

       
        public bool IsEmailVerified{ get; set;  }

        public Guid ActiviationCode { get; set; }
        public string ResetPwdCode{get; set;}
        }

    public class ResetPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [StringLength(100, ErrorMessage = "סיסמה צריכה להיות בת לפחות 6 תווים*", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "סימאות לא תואמות*")]
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }

    }

    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
