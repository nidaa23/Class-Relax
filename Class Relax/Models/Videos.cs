using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Class_Relax.Models
{
    public class Videos
    {
        [Display(Name = "Name:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]


        public string Name { get; set; }

        [Display(Name = "Url:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string Url { get; set; }

        [Display(Name = "Format: ")]
        public string Format { get; set; }

        [Display(Name = "Type: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string Type { get; set; }

        [Display(Name = "Length: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string Length { get; set; }

        [Display(Name = "Mood Tag 1: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string Tag1 { get; set; }

        [Display(Name = "Mood Tag 2:")]

        public string Tag2 { get; set; }

        [Display(Name = "Mood Tag 3:")]
        public string Tag3 { get; set; }

        [Display(Name = "Tag:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string MTag { get; set; }

        [Display(Name = "Average Feedback: ")]
        public float AvgFeedback { get; set; }

    }
}