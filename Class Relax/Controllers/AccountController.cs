using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Class_Relax.Models;
using System.Collections.Generic;
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
    
    public class AccountController : Controller
    {
        string ConnectionString = "workstation id=ClassRelax.mssql.somee.com;packet size=4096;user id=classrelax_SQLLogin_1;pwd=cpn2irgqgq;data source=ClassRelax.mssql.somee.com;persist security info=False;initial catalog=ClassRelax";
        //string ConnectionString = "Data Source=NIDA\\MSSQLSERVERR;Initial Catalog=ClassRelax;User ID=sa;Password=classrelax2018;MultipleActiveResultSets=True;Application Name=EntityFramework";

        #region Private Properties    

        private ClassRelaxDbContext databaseManager = new ClassRelaxDbContext();
        #endregion

       



        #region Log in methods

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Verification.    
                if (this.Request.IsAuthenticated)
                {
                    // Info.    
                    return RedirectToAction("Index", "Users");
                }
            }
            catch (Exception ex)
            {
                // Info    
                Console.Write(ex);
            }
            // Info.    
            return RedirectToAction("Default", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            string message = "";

            // Verification
            if (ModelState.IsValid)
            {
                try
                {
                    using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
                    {
                        var v = dc.Users.Where(a => a.Username == login.Username).FirstOrDefault();
                        if (login.Username.Contains("@"))
                        {
                            var email_ = dc.Users.Where(a => a.Email == login.Username).FirstOrDefault();
                            v = email_;
                        }

                        if (v != null) { 
                         string logPwrd = login.Password;
                        string HashPwrd = v.Password;

                            if (Crypto.ValidatePassword(logPwrd, HashPwrd))
                            {


                                if (!v.IsEmailVerified)
                                {
                                    ViewBag.Message = "החשבון שלך לא מאומת";
                                    string email = v.Email.ToString();
                                    TempData["email"] = email;
                                    return RedirectToAction("UnverifiedAcc", "Account");

                                }
                                this.SignInUser(v.Username, false);


                                string name = v.Username;
                                string fName = GetFirstName(v.Username);
                                string lName = GetLastName(v.Username);
                                string fullName = fName + " " + lName;
                                string role = isAManaOrAdmin(name);
                                int i;
                                i = SelectTheId(name);

                                TempData["admin"] = role;
                                TempData["userName_"] = name;
                                TempData["UserID"] = i;
                                TempData["fstName"] = fullName;
                                TempData["I"] = i;
                                if (i == 2 || i == 5)
                                {
                                    return RedirectToAction("Index", "Admin", new { I = TempData["I"] });
                                }
                                return RedirectToAction("Index", "Users", new { I = TempData["I"] });
                            }
                            message = "אחד או יותר מהפרטים שהזנת שגויים";
                            TempData["message"] = message;
                            return RedirectToAction("Default", "Home");

                        }
                        else
                        {
                            message = "אחד או יותר מהפרטים שהזנת שגויים";
                            TempData["message"] = message;
                            return RedirectToAction("Default", "Home");

                        }

                    }
                }

                catch (SqlException exp)
                {

                    throw new InvalidOperationException("Data could not be read", exp);
                }
            }
            else
            {
               
                message = "אחד או יותר מהפרטים שהזנת שגויים";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");

            }
        }
        #endregion
        //

        #region Log Out method.    
      
        [HttpGet]
        public ActionResult UnverifiedAcc()
        {
          
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UnverifiedAcc(string e)
        {
            string linkver= GetverLink(e);
            await SendVerificationLinkEmail(e, linkver);
            return View();
        }
        [HttpPost]

        public ActionResult LogOff()
        {
            try
            {
                // Setting.    
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign Out.    
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Default", "Home");
        }
        #endregion
        #region Helpers    
        #region Sign In method.    
        /// <summary>  
        /// Sign In User method.    
        /// </summary>  
        /// <param name="username">Username parameter.</param>  
        /// <param name="isPersistent">Is persistent parameter.</param>  
        private void SignInUser(string username, bool isPersistent)
        {
            // Initialization.    
            var claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
        }
        #endregion

        //




        //
        #region Register methods
        //
        // GET: /Account/Register
        [AllowAnonymous]

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([Bind(Exclude = "IsEmailVerified,ActivationCode,ResetPwdCode")] RegisterViewModel model)
        {


            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {

                var isExist = IsEmailExist(model.Email);
                if (isExist)
                {
                    ModelState.AddModelError("מייל קיים", "כבר קיים משתמש עם כתובת מייל זהה");
                    return View(model);

                }

                var isUserExist = IsUserExist(model.Username);
                if (isUserExist)
                {
                    ModelState.AddModelError("שם משתמש קיים", "כבר קיים משתמש עם שם משתמש זהה");
                    return View(model);

                }




                string pass = Crypto.HashPassword(model.Password);
             

                Guid userCode = Guid.NewGuid();



                using (ClassRelaxDbContext db = new ClassRelaxDbContext())
                {
                    try
                    {
                        db.Users.Add(new User
                        {
                            Username = model.Username,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                        Password = pass,
                        IsEmailVerified = false,
                        ActiviationCode = userCode,
                        ResetPwdCode = "0"
                    });
                        
                       
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                            }
                        }
                    }

                }
                await SendVerificationLinkEmail(model.Email, userCode.ToString());
                message = message = " ההרשמה שלך לאתר בוצעה בהצלחה ";
                Status = true;
                ViewBag.Message = message;
                ViewBag.Status = Status;
                TempData["msg"] = message;
                TempData["email"] = model.Email;
                return RedirectToAction("ActiviationLink", "Account", new { msg = @TempData["msg"] });
            }
            else
            {
                message = "שגיאה!";

            }
            ViewBag.Message = message;
            ViewBag.Status = Status;

            return View(model);
        }

        //Verify Account 
        [HttpGet]
        public ActionResult ActiviationLink()
        {

            return View();
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;

                var v = dc.Users.Where(a => a.ActiviationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "שגיאה";
                }
            }
            ViewBag.Status = Status;
            return View();
        }
        #endregion



        #region Password reset confirmation
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
   

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
                {
                    var account = dc.Users.Where(a => a.Email == model.Email).FirstOrDefault();
                    

                    if (account != null)
                    {
                        string _email = model.Email;
                        string resetCode = Guid.NewGuid().ToString();
                        updateResetCode(_email, resetCode);

                        await SendResetPwdLinkEmail(_email, resetCode);
                        TempData["email"] = _email;

                        return RedirectToAction("ResetlinkSent", "Account", new {e=TempData["email"] });
                    }
                   
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpGet]
        public ActionResult ResetlinkSent(string e)
        {
            if (e == null)
            {
                return RedirectToAction("Default", "Home");
            }
            TempData["email"] = e;
            return View();
        }
        [NonAction]
        private async Task SendResetPwdLinkEmail(string email, string token)
        {
            var verifyUrl = "/Account/ResetPassword/" + token;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("sapirclassrelax@gmail.com", "Class Relax");
            var toEmail = new MailAddress(email);
            var FromEmailPassword = "relax1812018"; //Replace with actual password
            string subject = "שיחזור סיסמה";
            string body = "<br/><br/>שלום " +
                " לצורך שינוי הסיסמה בחשבונך, יש ללחוץ על הקישור" +
                "<br/><br/><a href=' " + link + "'>" + link + "</a>" + "<br/><br/> ',בתודה" + "<br/><br/> Class Relax";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, FromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
      
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string id)
        {


            try
            {
                using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
                {
                    var account = dc.Users.Where(a => a.ResetPwdCode == new Guid(id).ToString()).FirstOrDefault();

                    if (account == null || account.ResetPwdCode == "0")
                    {
                        return View("Error");
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }

        }


        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var message = "";

            if (!ModelState.IsValid)
            {
                message = "אירעה שגיאה";
                ViewBag.Message = message;
                return View(model);
            }
            try
            {
                using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
                {
                    var account = dc.Users.Where(a => a.Email == model.Email).FirstOrDefault();

                    if (account == null)
                    {
                        // Don't reveal that the user does not exist
                        message = "הזנת פרטים שגויים";
                        ViewBag.Message = message;
                        return View(model);

                    }
                    string _email = model.Email;
                    string pwd = model.Password;
                    string pass = Crypto.HashPassword(pwd);
                    updatePassword(_email, pass);
                    DoneResetCode(_email);
                   
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }


        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion





      
        //Methods [NON Action]
        //Method 1 : get the first name of the loged in user 
        [NonAction]
        public string GetFirstName(string theName)
        {
            string ans = "";

            string cmdStr = "SELECT FirstName FROM dbo.[Users] WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@Username", theName);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            ans = (string)reader["FirstName"];

                        }

                    }
                    return ans;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }

        }
        [NonAction]
        public string GetLastName(string theName)
        {
            string ans = "";

            string cmdStr = "SELECT LastName FROM dbo.[Users] WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@Username", theName);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            ans = (string)reader["LastName"];

                        }

                    }
                    return ans;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }

        }
        // method 2: check if the logged in user manager or user

        protected string isAManaOrAdmin(string name)
        {
            string ans = "";

            string cmdStr = "SELECT Role FROM dbo.[Users] WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@Username", name);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            object x = reader["Role"];
                            if (x == null || x == DBNull.Value)
                            {
                                ans = "user";
                            }
                            else
                            {
                                ans = "M/A";
                            }

                        }

                    }
                    return ans;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }
        }

        [NonAction]
        private void updatePassword(string email_, string pw)
        {
            try
            {

                string cmdStr = "UPDATE Users SET Password=@Password WHERE Email=@Email";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email_);
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
        private void updateResetCode(string email_, string code)
        {
            try
            {

                string cmdStr = "UPDATE Users SET ResetPwdCode=@ResetPwdCode WHERE Email=@Email";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email_);
                        cmd.Parameters.AddWithValue("@ResetPwdCode", code);


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
        private void DoneResetCode(string email_)
        {
            string code = "0";
            try
            {

                string cmdStr = "UPDATE Users SET ResetPwdCode=@ResetPwdCode WHERE Email=@Email";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email_);
                        cmd.Parameters.AddWithValue("@ResetPwdCode", code);


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
        //Method 3: id of user 
        [NonAction]
        public int SelectTheId(string theName)
        {
            int ans = 0;

            string cmdStr = "SELECT UserID FROM dbo.[Users] WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

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
        public string GetverLink(string theName)
        {
            string ans = "";

            string cmdStr = "SELECT ActiviationCode FROM dbo.[Users] WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@Username", theName);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            ans = (string)reader["ActiviationCode"];

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
        public bool IsEmailExist(string emailID)
        {
            try
            {
                using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
                {
                    var v = dc.Users.Where(a => a.Email == emailID).FirstOrDefault();
                    return v != null;
                }
            }
            catch (SqlException exp)
            {

                throw new InvalidOperationException("Data could not be read", exp);
            }
        }




        [NonAction]
        public bool IsUserExist(string usernameID)
        {
            try
            {
                using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
                {
                    var v = dc.Users.Where(a => a.Username == usernameID).FirstOrDefault();
                    return v != null;
                }
            }
            catch (SqlException exp)
            {

                throw new InvalidOperationException("Data could not be read", exp);
            }
        }

        //Method 3: 
        // 
        [NonAction]
        private async Task SendVerificationLinkEmail(string emailID, string activateCode)
        {
            var verifyUrl = "/Account/VerifyAccount/" + activateCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("sapirclassrelax@gmail.com", "Class Relax");
            var toEmail = new MailAddress(emailID);
            var FromEmailPassword = "relax1812018"; //Replace with actual password
            string subject = "החשבון שלך באתר Class Relax";
            string body = "<br/><br/>   תודה רבה על ההרשמה לאתר!" +
            "<br/><br/>    ברוך/ברוכה הבא/ה" +
           "<br/><br/>        " +
                "לחץ על הקישור על מנת להפעיל את החשבון" +
                "<br/><br/><a href=' " + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, FromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        #endregion
      
    }
}
