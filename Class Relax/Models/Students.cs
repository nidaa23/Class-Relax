using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using Class_Relax.Models;


namespace Class_Relax.Models
{
    public class Students
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public string Nickname { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*שדה חובה")]
        public int ClassPin { get; set; }

        public List<Mood> MoodList { get; set; }

        public int Feedback { get; set; }

        public class Mood
        {
            public string mood1 = "Happy";
            public string mood2 = "Energetic";
            public string mood3 = "Good";
            public string mood4 = "Tired";
            public string mood5 = "Angry";
            public string mood6 = "Sad";
            public string mood7 = "Bored";
            public string mood8 = "Anxious";
        }
    }
}