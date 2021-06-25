using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Class_Relax.Models;

namespace Class_Relax.Models
{
    public class NewClass
    {
        [HiddenInput(DisplayValue = false)]
        public int UserID { get; set; }

        [Display(Name = "Class Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string ClassName { get; set; }


        [Display(Name = "Duration")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string Durations { get; set; }


        [Display(Name = "Style")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string Styles { get; set; }

        [Display(Name = "Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string Types { get; set; }
    }
}
