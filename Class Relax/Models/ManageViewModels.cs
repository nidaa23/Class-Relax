using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Class_Relax.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "* שדה חובה")]
        [StringLength(100, ErrorMessage =  "סיסמה צריכה להיות בת לפחות 6 תווים *", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "סימאות לא תואמות *")]
        public string ConfirmPassword { get; set; }
    }
   

    public class ChangePasswordViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "* שדה חוב")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [StringLength(100, ErrorMessage = "סיסמה צריכה להיות בת לפחות 6 תווים *", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "סימאות לא תואמות*")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeNameViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        [MinLength(2, ErrorMessage = "יש לבחור שם פרטי המכיל לפחות 2 תווים")]
        public string FirstName{ get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        
      
        public string LastName { get; set; }

    }
}