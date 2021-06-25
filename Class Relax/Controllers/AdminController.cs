using Class_Relax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Mail;
using System.Net;
using System.Security;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Web.UI.WebControls;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Threading;
using System.Web.Helpers;

using System.Globalization;

using System.Threading.Tasks;

using Class_Relax;

using System.Data.Entity.Validation;


namespace Class_Relax.Controllers
{
    
    public class AdminController : Controller
    {

        //string connectionString = "Data Source=NIDA\\MSSQLSERVERR;Initial Catalog=ClassRelax;User ID=sa;Password=classrelax2018;MultipleActiveResultSets=True;Application Name=EntityFramework";
        string connectionString = "workstation id=ClassRelax.mssql.somee.com;packet size=4096;user id=classrelax_SQLLogin_1;pwd=cpn2irgqgq;data source=ClassRelax.mssql.somee.com;persist security info=False;initial catalog=ClassRelax";



        [HttpGet]
        public ActionResult Index(int? I)
        {
            string message = "";
            if (!this.Request.IsAuthenticated)
            {

                message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }
            string v = System.Web.HttpContext.Current.User.Identity.Name.ToString();


            string fName = GetFirstName(v);
            string lName = GetLastName(v);
            string fullName = fName + " " + lName;
            string UserFullName = fullName;
            System.Web.HttpContext.Current.Session.Add("UserFullName", UserFullName);
            return View();

        }

        [HttpGet]
        public ActionResult List()
        {
            string message = "";

            if (this.Request.IsAuthenticated)
            {
                string v = System.Web.HttpContext.Current.User.Identity.Name.ToString();



                if (!isAManaOrAdmin(v))
                {

                    return RedirectToAction("Index", "Users");

                }
                ClassRelaxDbContext db = new ClassRelaxDbContext();
                return View(db.Videos.ToList());

            }
            else
            {

                message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }

            
        }

        [HttpGet]
        public ActionResult AddVideo()
        {
            string message = "";

            if (this.Request.IsAuthenticated)
            {
                string v = System.Web.HttpContext.Current.User.Identity.Name.ToString();



                if (!isAManaOrAdmin(v))
                {

                    return RedirectToAction("Index", "Users");

                }
               
                    return View();
              
            }
            else
            {

                message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }

        }


        [HttpPost]

        public ActionResult AddVideo(Videos model)
        {
            string message = "";
            string len_s ;
            string link = model.Url;
            len_s = model.Length.ToString();
            int len = lengthVideo(len_s);
            string types = model.Type.ToString();
            string styles = model.MTag.ToString();
            string _1tag = model.Tag1.ToString();
            string _2tag = model.Tag2.ToString();
            string _3tag = model.Tag3.ToString();
            if (len == 0 || _1tag == null || _2tag == null || _3tag == null )
            {
                message = "נא למלא טופס";
                ViewBag.Message = message;
              return View(model);

            }
        
            if (ModelState.IsValid)
            {

                if ((isURLExist(model.Url)) || !link.StartsWith("https://www.youtube.com/embed/") ||
                 (_1tag.Equals(_2tag) || _1tag.Equals(_3tag) || _2tag.Equals(_3tag)))
                {
                    ModelState.AddModelError("כבר קיים", "הסרטון כבר קיים במערכת");
                    return View(model);
                }
                if (!link.StartsWith("https://www.youtube.com/embed/"))
                {
                    ModelState.AddModelError("פורמט לא תקין", " לינק לא תקין, הלינק צריך להיות בפורמט https://www.youtube.com/embed/ ");
                    return View(model);
                }
                 if (_1tag.Equals(_2tag) || _1tag.Equals(_3tag) || _2tag.Equals(_3tag))
                  {
                       message = "  יש לבחור בתגיות שונות זו מזו";
                        ViewBag.Message = message;
                        return View(model);

                    }
                try
                {
                    using (ClassRelaxDbContext db = new ClassRelaxDbContext())
                    {
                        db.Videos.Add(new Video
                        {
                            Name = model.Name,
                            Url = model.Url,
                            Format = "video",
                            Type = types,
                            Length = len,
                            Tag1 = _1tag,
                            Tag2 = _2tag,
                            Tag3 = _3tag,
                            MTag = styles,
                            AvgFeedback = 0
                        });

                        db.SaveChanges();
                        return RedirectToAction("List", "Admin");
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            
            
                
            }
           
            return RedirectToAction("Index", "Admin");
        }


        protected int lengthVideo(string lenCategory)
        {

            int ans = 0;
            if (lenCategory == "2 - 3")
            {
                ans = 1;
            }
            else if (lenCategory == "3 - 5")
            {
                ans = 2;
            }
            else if (lenCategory == "5 - 10")
            {
                ans = 3;
            }
            return ans;

        }

        protected bool isAManaOrAdmin(string name)
        {

            bool ansIs = false;

            string cmdStr = "SELECT Role FROM dbo.[Users] WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@Username", name);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            object u = reader["Role"];
                            if (u != null || u != DBNull.Value)
                            {
                                ansIs = true;
                            }

                        }

                    }
                    return ansIs;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }
        }
        [NonAction]
        public string GetFirstName(string theName)
        {
            string ans = "";

            string cmdStr = "SELECT FirstName FROM dbo.[Users] WHERE Username = @Username";

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
        protected bool isURLExist(String urlLink)
        {

            using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
            {
                var v = dc.Videos.Where(a => a.Url.Equals(urlLink)).FirstOrDefault();
                return v != null;
            }
        }

    }
}