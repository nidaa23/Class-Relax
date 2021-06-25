using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Globalization;
using System.Security.Claims;
using Class_Relax.Models;
using Class_Relax;
using System.Configuration;
using System.Web.Routing;
using System.Net.Mail;
using System.Net;
using System.Security;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Validation;

namespace Class_Relax.Controllers
{
    public class ManageController : Controller
    {
        string connectionString = "workstation id=ClassRelax.mssql.somee.com;packet size=4096;user id=classrelax_SQLLogin_1;pwd=cpn2irgqgq;data source=ClassRelax.mssql.somee.com;persist security info=False;initial catalog=ClassRelax";

        //string connectionString = "Data Source=NIDA\\MSSQLSERVERR;Initial Catalog=ClassRelax;User ID=sa;Password=classrelax2018;MultipleActiveResultSets=True;Application Name=EntityFramework";
        ClassRelaxDbContext dc = new ClassRelaxDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }
       
        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            string message = "";
            if (!this.Request.IsAuthenticated)
            {
                message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }

            
            return View();
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            string message = "";
            if (!this.Request.IsAuthenticated)
            {
                message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }
            string usName = "";
            usName = System.Web.HttpContext.Current.User.Identity.GetUserName();

          
            TempData["userName_"] = usName;
            return View();
        }
     
        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model,string name)
        {
            string message = "";
            if (name == null)
            {
                message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }
            
            
           
            if (!ModelState.IsValid )
            {
                message = "!אירעה שגיאה, נסה שוב";
                ViewBag.Message = message;
                return View();
            }
         
            string oldPwd =(model.OldPassword);
            bool pwdCorrect = IsUserExist(name, oldPwd);
            string pass = Crypto.HashPassword(model.NewPassword);
            if (!pwdCorrect)
            {
                message = "הסיסמה הנוכחית שגויה ";
                ViewBag.Message = message;
                return View();
            }
            if ( pass != null) {
                try
                {
                    updatePassword(name, pass);
                    message = "שינוי הסיסמה בוצע בהצלחה";
                    ViewBag.Message = message;
                    return View();
                }
                catch (SqlException ex)
                {
                    
                }


            }

            ViewBag.Message = message;
            return View();
        }

      
        
        [HttpGet]
        public ActionResult ChangeName()
        {
            string usName = "";
            usName = System.Web.HttpContext.Current.User.Identity.GetUserName();

            
            TempData["userName_"] = usName;
            return View();
        }
        [HttpPost]
        public ActionResult ChangeName(ChangeNameViewModel model, string name)
        {

            string message = "";
            if ( name == null)
            {
                message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }



            if (!ModelState.IsValid)
            {
                message = "!אירעה שגיאה, נסה שוב";
                ViewBag.Message = message;
                return View();
            }
            
            string fName = model.FirstName;
            string lName = model.LastName;
          
               try
                {
                    updateFstName(name,fName);
                    updateLstName(name, lName);
                    message = "שינוי השם בוצע בהצלחה";
                    ViewBag.Message = message;
                    return View();
                }
                catch (SqlException ex)
                {

                }



            ViewBag.Message = message;
            return View();
        }
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

     

      

        public enum ManageMessageId
        {
           
            ChangePasswordSuccess,
           
            SetPasswordSuccess,
            RemoveLoginSuccess,
           
            Error
        }

        #endregion
        [NonAction]
        private void updatePassword(string name, string pw)
        {
            try
            {

                string cmdStr = "UPDATE Users SET Password=@Password WHERE Username = @Username";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", name);
                        cmd.Parameters.AddWithValue("@Password", pw);


                        int rows = cmd.ExecuteNonQuery();

    //rows number of record got updated
}
                }
            }
            catch (SqlException ex)
            {
                //Log exception
                //Display Error message
            }

        }
        [NonAction]
        private void updateFstName(string name, string fstName)
        {
            try
            {

                string cmdStr = "UPDATE Users SET FirstName=@FirstName WHERE Username = @Username";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", name);
                        cmd.Parameters.AddWithValue("@FirstName", fstName);


                        int rows = cmd.ExecuteNonQuery();

                        //rows number of record got updated
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log exception
                //Display Error message
            }

        }
        [NonAction]
        private void updateLstName(string name, string lstName)
        {
            try
            {

                string cmdStr = "UPDATE Users SET LastName=@LastName WHERE Username = @Username";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username",name);
                        cmd.Parameters.AddWithValue("@LastName", lstName);


                        int rows = cmd.ExecuteNonQuery();

                        //rows number of record got updated
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log exception
                //Display Error message
            }

        }
        [NonAction]
        public int SelectTheId(string theName)
        {
            int ans = 0;

            string cmdStr = "SELECT UserID FROM dbo.[Users] WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@Username", theName);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            ans = (int)reader["UserID"];

                        }

                    }
                    connection.Close();
                    return ans;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }
        [NonAction]
        public bool IsUserExist(string name_, string pwd)
        {
            if (name_ != null && pwd != null)
            {
                try
                {
                    using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
                    {
                        var v = dc.Users.Where(a => a.Username == name_).FirstOrDefault();

                        string HashPwrd = v.Password.ToString();
                        bool exist = Crypto.ValidatePassword(pwd, HashPwrd);
                        return exist;
                    }
                }
                catch (SqlException exp)
                {

                    throw new InvalidOperationException("Data could not be read", exp);
                }
            }
            return false;
        }
    }
}