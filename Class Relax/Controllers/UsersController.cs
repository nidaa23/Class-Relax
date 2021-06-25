using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Class_Relax.Models;
using System.Web.Security;
using System.Security.Claims;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Net;
using System.Security;
using Microsoft.AspNet.Identity;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Web.UI.WebControls;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Helpers;


namespace Class_Relax.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
      string ConnectionString = "workstation id=ClassRelax.mssql.somee.com;packet size=4096;user id=classrelax_SQLLogin_1;pwd=cpn2irgqgq;data source=ClassRelax.mssql.somee.com;persist security info=False;initial catalog=ClassRelax";
       //string ConnectionString = "Data Source=NIDA\\MSSQLSERVERR;Initial Catalog=ClassRelax;User ID=sa;Password=classrelax2018;MultipleActiveResultSets=True;Application Name=EntityFramework";
        private SqlConnection con;
        private ClassRelaxDbContext db = new ClassRelaxDbContext();

        [AllowAnonymous]
        [HttpGet]
        public ActionResult test()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateClass()
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


        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);

        }


        [HttpPost]
        public ActionResult CreateClass(NewClass _model)

        {
            string message = "";
            if (ModelState.IsValid)
            {
                
                int _duration = 0;
                _duration = theSelected(_model.Durations);
                int random = RandomNumbers();
                TempData["urPin"] = random;
                string usName="";
                usName = System.Web.HttpContext.Current.User.Identity.GetUserName();

                int currentID = 0;
                currentID = SelectTheId(usName);

                if (currentID == 0)
                {

                    message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                    TempData["message"] = message;
                    return RedirectToAction("Default", "Home");
                }
                else
                {

                    using (ClassRelaxDbContext db = new ClassRelaxDbContext())
                    {



                        Class _class = new Class();
                        _class.UserID = currentID;
                        _class.ClassPin = random;
                        _class.ClassName = _model.ClassName;
                        _class.Duration = _duration;
                        _class.Style = _model.Styles;
                        _class.Type = _model.Types;
                        _class.Date = System.DateTime.Today;

                        _class.IDVideo = 1;
                        _class.NumOfStudents = 0;
                        _class.ClassAvgFeedback = 0;


                        db.Classes.Add(_class);
                        db.SaveChanges();
                    }
                  
                    TempData["l"] = _duration;
                    TempData["t"] = _model.Types;
                    TempData["s"] = _model.Styles;
                    TempData["UserId"] =currentID;
                    return RedirectToAction("YourClassPin", "Users", new { I = TempData["UserId"], pinC = TempData["urPin"], l = TempData["l"], t = TempData["t"], s = TempData["s"] });
                }


            }

           
            return RedirectToAction("Index", "Users");

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
           
           
           string v = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            
           
            string fName = GetFirstName(v);
            string lName = GetLastName(v);
            string fullName = fName + " " + lName;
            string UserFullName =fullName;
            System.Web.HttpContext.Current.Session.Add("UserFullName", UserFullName);
            return View();


        }



        public int  RandomNumbers()
        {
            int min = 1000000;
            int max = 999999999;
            Random random = new Random();
            int pintemp = random.Next(min, max);
            if (isPinExist(pintemp))
            {
                RandomNumbers();
            }

            return pintemp;
        }

        public ActionResult YourClassPin(int? I, int? pinC, int? l, string t, string s)
        {
            string message = "";
            if (!this.Request.IsAuthenticated || I == null)
            {
                message = " !משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }
            if (this.Request.IsAuthenticated && (pinC == 0 || l == null || s == null || s == null))
            {
                TempData["I"] = I;
                return RedirectToAction("Index", "Users", new { I = TempData["I"] });


            }
            TempData["UserId"] = I;
            TempData["s"] = s;
            TempData["t"] = t;
            TempData["l"] = l;
            TempData["urPin"] = pinC;
            return View();


        }



        public ActionResult PastClasses()
        {
            string message = "";
            if (!this.Request.IsAuthenticated )
            {

                message = "!משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");


            }
            string v = System.Web.HttpContext.Current.User.Identity.Name.ToString();


            int I_ = SelectTheId(v);
            return View(db.Classes.Where(a => a.UserID == I_).ToList().OrderBy(a => a.Date));
        }

        [NonAction]
        public bool isPinExist(int pinclass)
        {
            using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
            {
                var v = dc.Classes.Where(a => a.ClassPin == pinclass).FirstOrDefault();
                return v != null;
            }

        }

        [NonAction]
        public int theSelected(string selected)
        {
            int y = 0;
            if (selected.Equals("2 - 3"))
            {
                y = 1;
            }
            if (selected.Equals("3 - 5"))
            {
                y = 2;
            }
            if (selected.Equals("5 - 10"))
            {
                y = 3;
            }

            return y;
        }
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

    }
}