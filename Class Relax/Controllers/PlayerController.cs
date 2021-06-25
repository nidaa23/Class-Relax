using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
using System.Web.Helpers;

namespace Class_Relax.Models
{
    public class PlayerController : Controller
    {
        string ConnectionString = "workstation id=ClassRelax.mssql.somee.com;packet size=4096;user id=classrelax_SQLLogin_1;pwd=cpn2irgqgq;data source=ClassRelax.mssql.somee.com;persist security info=False;initial catalog=ClassRelax";
        //string ConnectionString = "Data Source=NIDA\\MSSQLSERVERR;Initial Catalog=ClassRelax;User ID=sa;Password=classrelax2018;MultipleActiveResultSets=True;Application Name=EntityFramework";
        // GET: Player


        [HttpGet]
        [AllowAnonymous]
        public ActionResult NewPlayer()
        {
            return View();

        }

        [HttpPost]
        public ActionResult NewPlayer(Students model)
        {
            string message = "";
            if (ModelState.IsValid)
            {

                bool isExistPin = isPINExist(model.ClassPin);
                
                if (!isExistPin)
                {
                    message = "מספר הכיתה שהזנת שגוי";
                    ViewBag.Message = message;
                    return View(model);
                }
                else
                {

                    using (ClassRelaxDbContext db = new ClassRelaxDbContext())
                    {

                        Student _stud = new Student();

                        _stud.Nickname = model.Nickname;
                        _stud.ClassPin = model.ClassPin;
                        _stud.Mood = "";
                        _stud.Feedback = 0;

                        db.Students.Add(_stud);
                        db.SaveChanges();



                    }
                    TempData["nameStudent"] = model.Nickname;
                    int is_ = (int)model.ClassPin;
                    string x = model.Nickname.ToString();
                    int id_;
                    id_ = SelectStudentID(x, is_);
                    TempData["idStudent"] = id_;
                    TempData["classPin"] = model.ClassPin;
                    TempData["nameStudent"] = model.Nickname;
                    return RedirectToAction("LoadMood", "Player", new { name = TempData["nameStudent"], is_ = TempData["idStudent"], pin = TempData["classPin"] });
                }

            }
            message = "שגיאה";
            ViewBag.Message = message;
            return View(model);
            
        }





        [HttpGet]
        public ActionResult LoadMood()
        {



            return View();
        }


        

        protected int SelectStudentID(string nameStudent, int PinC)
        {
            int id_ = 0;


            string cmdStr = "SELECT StudentID FROM dbo.[Student] WHERE Nickname = @Nickname AND ClassPin=@ClassPin";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@Nickname", nameStudent);
                        command.Parameters.AddWithValue("@ClassPin", PinC);
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            id_ = (int)reader["StudentID"];

                        }

                    }
                    connection.Close();
                    return id_;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }



        public bool isPINExist(int number)
        {
            using (ClassRelaxDbContext dc = new ClassRelaxDbContext())
            {
                var v = dc.Classes.Where(a => a.ClassPin == number).FirstOrDefault();
                return v != null;
            }
        }



    }
}