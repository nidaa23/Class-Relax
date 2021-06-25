using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Security;
using System.Web.Security;
using System.Security.Claims;
using Class_Relax.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Security.Principal;
using System.Web.Helpers;

namespace Class_Relax.Models
{

    public class OnActivityController : Controller
    {
        string connectionString = "workstation id=ClassRelax.mssql.somee.com;packet size=4096;user id=classrelax_SQLLogin_1;pwd=cpn2irgqgq;data source=ClassRelax.mssql.somee.com;persist security info=False;initial catalog=ClassRelax";
        //string connectionString = "Data Source=NIDA\\MSSQLSERVERR;Initial Catalog=ClassRelax;User ID=sa;Password=classrelax2018;MultipleActiveResultSets=True;Application Name=EntityFramework";



        public ActionResult WaitingForResults(int? I, string s_, string t_, int? l_, int? cp)
        {
            string message = "";
            if (!this.Request.IsAuthenticated)
            {
                message = "!משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }
            else if (I > 0 && (s_ == null || t_ == null || !l_.HasValue || !cp.HasValue))
            {
                TempData["UserId"] = I;
                return RedirectToAction("Index", "Users");
            }

            int le_ = l_ ?? default(int);
            int cp_ = cp ?? default(int);
            TempData["UserId"] = I;
            TempData["s"] = s_;
            TempData["t"] = t_;
            TempData["l"] = l_;
            TempData["cp"] = cp_;

            return View();




        }

        public ActionResult LoadingVideos(int? I, string s_, string t_, int? l_, int? cp)
        {
            string message = "";
            if (!this.Request.IsAuthenticated || I == null)
            {

                message = "!משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }
            if (I > 0 && (s_ == null || t_ == null || !l_.HasValue || !cp.HasValue))
            {
                TempData["UserId"] = I;
                return RedirectToAction("Index", "Users", new { I = TempData["UserId"] });
            }
            int cp_ = cp ?? default(int);
            int le_ = l_ ?? default(int);
            TempData["cp"] = cp_;
            int? video_1 = GetReturnValueFromVideoAlg1(s_, t_, le_, cp_);
            int? video_2 = GetReturnValueFromVideoAlg2(s_, t_, le_, cp_);
            int? video_3 = GetReturnValueFromVideoAlg3(s_, t_, le_, cp_);
            string urlVideo1 = selectUrlVideo(video_1);
            string urlVideo2 = selectUrlVideo(video_2);
            string urlVideo3 = selectUrlVideo(video_3);
            int l1 = 0;
            string t1 = " ";
            string n1 =" ";
            int l2 = 0;
            string t2 = " ";
            string n2 = " ";
            int l3 = 0;
            string t3 = " ";
            string n3 = " ";
            if (urlVideo1.Contains("https://www.youtube.com/embed/"))
            {
                urlVideo1 = urlVideo1.Replace(" ","");
                l1 = returnDur(video_1);
                t1 = returnType(video_1);
                n1 = returnName(video_1);
            }
            if (urlVideo2.Contains("https://www.youtube.com/embed/")) {
                urlVideo2 = urlVideo2.Replace(" ", "");
                l2 = returnDur(video_2);
                t2 = returnType(video_2);
                n2 = returnName(video_2);

            }
            if (urlVideo3.Contains("https://www.youtube.com/embed/"))
            {
                urlVideo3 = urlVideo3.Replace(" ", "");
                l3 = returnDur(video_3);
                t3 = returnType(video_3);
                n3 = returnName(video_3);
            }
           
            int all = countAll(cp_);
            int h = countHappy(cp_);
            int g = countGood(cp_);
            int a = countAngry(cp_);
            int b = countBored(cp_);

            int t = countTired(cp_);
            int an = countAnxious(cp_);
            int s = countSad(cp_);

            int e = countEnergetic(cp_);
         

            TempData["n1"] = n1;
            TempData["t1"] = t1;
            TempData["d1"] = l1;

            TempData["n2"] = n2;
            TempData["t2"] = t2;
            TempData["d2"] = l2;

            TempData["n3"] = n3;
            TempData["t3"] = t3;
            TempData["d3"] = l3;
            TempData["happy"] = h;
            TempData["good"] = g;
            TempData["angry"] = a;
            TempData["bored"] = b;
            TempData["tired"] = t;
            TempData["anxious"] = an;
            TempData["sad"] = s;
            TempData["energetic"] = e;
            TempData["all"] = all;
            TempData["UserId"] = I;
            TempData["v1"] = urlVideo1;
            TempData["v2"] = urlVideo2;
            TempData["v3"] = urlVideo3;
            TempData["vid1"] = video_1;
            TempData["vid2"] = video_2;
            TempData["vid3"] = video_3;
            return RedirectToAction("WatchVideo", "OnActivity", new { I = TempData["UserId"], v1 = TempData["vid1"], v2 = TempData["vid2"], v3 = TempData["vid3"], cp = TempData["cp"] });
        }




        public ActionResult WatchVideo(int? I, int? v1, int? v2, int? v3, int? cp)
        {
            string message = "";
            if (!this.Request.IsAuthenticated || I == null)
            {

                message = "!משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }
            if (I > 0 && (v1 == null || v2 == null || v3 == null || cp == null))
            {
                TempData["UserId"] = I;
                return RedirectToAction("Index", "Users", new { I = TempData["UserId"] });
            }

            TempData["UserId"] = I;
            int v_1 = v1 ?? default(int);
            int v_2 = v2 ?? default(int);
            int v_3 = v3 ?? default(int);
            int cpIs = cp ?? default(int);
            TempData["cp"] = cpIs;
            TempData["id1"] = v_1;
            TempData["id2"] = v_2;
            TempData["id3"] = v_3;
            TempData["UserId"] = I;
            return View();
        }




        public ActionResult EndActiv1(int? I, int? vd1, int? cp)
        {
            string message = "";
            if (this.Request.IsAuthenticated)
            {

                if (vd1 > 0 && cp > 0)
                {
                    int vd_ = vd1 ?? default(int);
                    int cp_ = cp ?? default(int);
                    TempData["UserId"] = I;
                    updateIDVideo(vd_, cp_);
                    updateAverageFeedback(cp_);
                    deleteStudentClass(cp_);
                    return View();

                }
                TempData["UserId"] = I;
                return RedirectToAction("Index", "Users", new { I = TempData["UserId"] });


            }
            else
            {
                message = "!משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }


        }



        public ActionResult EndActiv2(int? I, int? vd2, int? cp)
        {
            string message = "";
            if (this.Request.IsAuthenticated)
            {

                if (vd2 > 0 && cp > 0)
                {
                    int vd_ = vd2 ?? default(int);
                    int cp_ = cp ?? default(int);
                    TempData["UserId"] = I;
                    updateIDVideo(vd_, cp_);
                    updateAverageFeedback(cp_);
                    deleteStudentClass(cp_);
                    return View();

                }
                TempData["UserId"] = I;
                return RedirectToAction("Index", "Users", new { I = TempData["UserId"] });


            }
            else
            {
                message = "!משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }


        }

        public ActionResult EndActiv3(int? I, int? vd3, int? cp)
        {
            string message = "";
            if (this.Request.IsAuthenticated)
            {

                if (vd3 > 0 && cp > 0)
                {
                    int vd_ = vd3 ?? default(int);
                    int cp_ = cp ?? default(int);
                    TempData["UserId"] = I;
                    updateIDVideo(vd_, cp_);
                    updateAverageFeedback(cp_);
                    deleteStudentClass(cp_);
                    return View();

                }
                TempData["UserId"] = I;
                return RedirectToAction("Index", "Users", new { I = TempData["UserId"] });


            }
            else
            {
                message = "!משתמש לא מזוהה, נא לבצע כניסה למערכת";
                TempData["message"] = message;
                return RedirectToAction("Default", "Home");
            }


        }


        private void updateIDVideo(int id, int pinclass)
        {
            try
            {

                string cmdStr = "UPDATE Class SET IDVideo=@IDVideo WHERE Classpin=@ClassPin";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@IDVideo", id);
                        cmd.Parameters.AddWithValue("@ClassPin", pinclass);


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
        private void updateAverageFeedback(int pin)
        {

            int pinIs = pin;

            using (ClassRelaxDbContext dbContext = new ClassRelaxDbContext())
            {
                // 2. Get connection.

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // 3. Initialize and execute OACommand.
                    using (IDbCommand spCommand = con.CreateCommand())
                    {
                        spCommand.CommandText = "dbo.sp_updateAVGFeedback";
                        spCommand.CommandType = System.Data.CommandType.StoredProcedure;




                        // parameter

                        IDbDataParameter ClassPinParameter = spCommand.CreateParameter();
                        ClassPinParameter.ParameterName = "@ClassPin";
                        ClassPinParameter.Value = pinIs;
                        spCommand.Parameters.Add(ClassPinParameter);




                        con.Open();
                        spCommand.Connection = con;

                        spCommand.ExecuteNonQuery();
                    }
                }
            }
        }


        [NonAction]
        private void deleteStudentClass(int cp)
        {
            try
            {

                string cmdStr = "DELETE FROM STUDENT WHERE ClassPin=@ClassPin";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@PinCode", cp);

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




        protected string selectUrlVideo(int? idVideo_)
        {
            string ans = "";
            string ans_F = "";
            if (idVideo_.HasValue)
            {
                int idVd = idVideo_ ?? default(int);
                if (idVd > 0)
                {
                    string cmdStr = "SELECT Url FROM dbo.[Video] WHERE IDVideo = @IDVideo";

                    using (SqlConnection connection = new SqlConnection(connectionString))

                        try
                        {
                            using (SqlCommand command = new SqlCommand(cmdStr, connection))
                            {

                                command.Parameters.AddWithValue("@IDVideo", idVd);

                                connection.Open();

                                SqlDataReader reader = command.ExecuteReader();

                                while (reader.Read())
                                {

                                    ans = (string)reader["Url"];
                                    ans = ans.Replace(" ", "");
                                    ans_F = ans + "?enablejsapi=1";
                                    ans_F = ans_F.Replace(" ", ""); 
                                    return ans_F;
                                }

                            }
                            connection.Close();
                            return ans_F;

                        }
                        catch (SqlException ex)
                        {
                            throw ex;

                        }
                }
                return "";
            }
            else
            {
                return "";
            }



        }





        private int GetReturnValueFromVideoAlg1(string tag_, string theType_, int length_, int pinClass)
        {


            string tagM = tag_;
            string type_ = theType_;
            int length = length_;
            int thepin = pinClass;

            // 1. Create a new instance of the generated OpenAccessContext.
            using (ClassRelaxDbContext dbContext = new ClassRelaxDbContext())
            {
                // 2. Get connection.

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // 3. Initialize and execute OACommand.
                    using (IDbCommand spCommand = con.CreateCommand())
                    {
                        spCommand.CommandText = "dbo.sp_algGetVideoA1";
                        spCommand.CommandType = System.Data.CommandType.StoredProcedure;

                        // 4. Initialize parameters.
                        // 4.1. The 'CarId' parameter is IN parameter.
                        IDbDataParameter ClassPinParameter = spCommand.CreateParameter();
                        ClassPinParameter.ParameterName = "@ClassPin";
                        ClassPinParameter.Value = thepin;
                        // second parameter
                        IDbDataParameter TypeParameter = spCommand.CreateParameter();
                        TypeParameter.ParameterName = "@Type";
                        TypeParameter.Value = type_;
                        //third parameter
                        IDbDataParameter MTagParameter = spCommand.CreateParameter();
                        MTagParameter.ParameterName = "@MTag";
                        MTagParameter.Value = tagM;
                        //fourth parameter
                        IDbDataParameter LengthParameter = spCommand.CreateParameter();
                        LengthParameter.ParameterName = "@Length";
                        LengthParameter.Value = length;


                        // 4.2. This is the return value.
                        IDbDataParameter IDVideoParameter = spCommand.CreateParameter();
                        IDVideoParameter.Direction = ParameterDirection.ReturnValue;

                        spCommand.Parameters.Add(ClassPinParameter);
                        spCommand.Parameters.Add(TypeParameter);
                        spCommand.Parameters.Add(MTagParameter);
                        spCommand.Parameters.Add(LengthParameter);

                        spCommand.Parameters.Add(IDVideoParameter);


                        con.Open();
                        spCommand.Connection = con;

                        spCommand.ExecuteNonQuery();

                        // 5. Consume the return value.

                        return (int)(IDVideoParameter.Value);
                    }
                }
            }
        }


        private int GetReturnValueFromVideoAlg2(string tag_, string theType_, int length_, int pinClass)
        {
            string tagM = tag_;
            string type_ = theType_;
            int length = length_;
            int thepin = pinClass;

            // 1. Create a new instance of the generated OpenAccessContext.
            using (ClassRelaxDbContext dbContext = new ClassRelaxDbContext())
            {
                // 2. Get connection.

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // 3. Initialize and execute OACommand.
                    using (IDbCommand spCommand = con.CreateCommand())
                    {
                        spCommand.CommandText = "dbo.sp_algGetVideoB1";
                        spCommand.CommandType = System.Data.CommandType.StoredProcedure;

                        // 4. Initialize parameters.
                        // 4.1. The 'CarId' parameter is IN parameter.
                        IDbDataParameter ClassPinParameter = spCommand.CreateParameter();
                        ClassPinParameter.ParameterName = "@ClassPin";
                        ClassPinParameter.Value = thepin;
                        // second parameter
                        IDbDataParameter TypeParameter = spCommand.CreateParameter();
                        TypeParameter.ParameterName = "@Type";
                        TypeParameter.Value = type_;
                        //third parameter
                        IDbDataParameter MTagParameter = spCommand.CreateParameter();
                        MTagParameter.ParameterName = "@MTag";
                        MTagParameter.Value = tagM;
                        //fourth parameter
                        IDbDataParameter LengthParameter = spCommand.CreateParameter();
                        LengthParameter.ParameterName = "@Length";
                        LengthParameter.Value = length;


                        // 4.2. This is the return value.
                        IDbDataParameter IDVideoParameter = spCommand.CreateParameter();
                        IDVideoParameter.Direction = ParameterDirection.ReturnValue;

                        spCommand.Parameters.Add(ClassPinParameter);
                        spCommand.Parameters.Add(TypeParameter);
                        spCommand.Parameters.Add(MTagParameter);
                        spCommand.Parameters.Add(LengthParameter);

                        spCommand.Parameters.Add(IDVideoParameter);


                        con.Open();
                        spCommand.Connection = con;

                        spCommand.ExecuteNonQuery();

                        // 5. Consume the return value.

                        return (int)(IDVideoParameter.Value);
                    }
                }
            }
        }


        private int GetReturnValueFromVideoAlg3(string tag_, string theType_, int length_, int pinClass)
        {

            string tagM = tag_;
            string type_ = theType_;
            int length = length_;
            int thepin = pinClass;

            // 1. Create a new instance of the generated OpenAccessContext.
            using (ClassRelaxDbContext dbContext = new ClassRelaxDbContext())
            {
                // 2. Get connection.

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // 3. Initialize and execute OACommand.
                    using (IDbCommand spCommand = con.CreateCommand())
                    {
                        spCommand.CommandText = "dbo.sp_algGetVideoC1";
                        spCommand.CommandType = System.Data.CommandType.StoredProcedure;

                        // 4. Initialize parameters.
                        // 4.1. The 'CarId' parameter is IN parameter.
                        IDbDataParameter ClassPinParameter = spCommand.CreateParameter();
                        ClassPinParameter.ParameterName = "@ClassPin";
                        ClassPinParameter.Value = thepin;
                        // second parameter
                        IDbDataParameter TypeParameter = spCommand.CreateParameter();
                        TypeParameter.ParameterName = "@Type";
                        TypeParameter.Value = type_;
                        //third parameter
                        IDbDataParameter MTagParameter = spCommand.CreateParameter();
                        MTagParameter.ParameterName = "@MTag";
                        MTagParameter.Value = tagM;
                        //fourth parameter
                        IDbDataParameter LengthParameter = spCommand.CreateParameter();
                        LengthParameter.ParameterName = "@Length";
                        LengthParameter.Value = length;


                        // 4.2. This is the return value.
                        IDbDataParameter IDVideoParameter = spCommand.CreateParameter();
                        IDVideoParameter.Direction = ParameterDirection.ReturnValue;

                        spCommand.Parameters.Add(ClassPinParameter);
                        spCommand.Parameters.Add(TypeParameter);
                        spCommand.Parameters.Add(MTagParameter);
                        spCommand.Parameters.Add(LengthParameter);

                        spCommand.Parameters.Add(IDVideoParameter);


                        con.Open();
                        spCommand.Connection = con;

                        spCommand.ExecuteNonQuery();

                        // 5. Consume the return value.

                        return (int)(IDVideoParameter.Value);
                    }
                }
            }
        }


      

        /*   ***********************     Player Section            *************************    */

        [HttpGet]
        public ActionResult LoadingPlayers(string x, int? num, int? cp)
        {

            if (x == null || !num.HasValue || !cp.HasValue)
            {
                return RedirectToAction("NewPlayer", "Player");
            }
            TempData["is_"] = num;
            TempData["cp"] = cp;
            string moodIs = x.ToString();
            moodIs = pickedMood(moodIs);
            int cp_ = cp ?? default(int);
            int id_student = (int)TempData["is_"];
            UpdateStudentMood(id_student, moodIs);
            int classPin = cp_;
            incrementStudentNumber(cp_);
            return View();


        }

        [HttpGet]
        public ActionResult YourMood(string name, int? num, int? cp)
        {

            if (name == null || !num.HasValue || !cp.HasValue)
            {
                return RedirectToAction("NewPlayer", "Player");
            }
            TempData["m"] = name;
            TempData["n"] = num;
            TempData["p"] = cp;

            return View();
        }

        [NonAction]
        public bool MyMethod()
        {
            return false;
        }

        public ActionResult VideoOnScreen(int? is_, int? cp)
        {
            if (!is_.HasValue || !cp.HasValue)
            {
                return RedirectToAction("NewPlayer", "Player");
            }
            TempData["is_"] = is_;
            TempData["cp"] = cp;
            int cp_ = cp ?? default(int);
           
         
            return View();
        }
        public ActionResult Feedback(int? is_, int? cp)
        {

            if (!is_.HasValue || !cp.HasValue)
            {
                return RedirectToAction("NewPlayer", "Player");
            }

            TempData["is_"] = is_;
            TempData["cp"] = cp;

            return View();

        }

        public ActionResult AddFeedback(int? is_, int? fb)
        {

            if (!is_.HasValue || !fb.HasValue)
            {
                return RedirectToAction("NewPlayer", "Player");
            }
            int iss_ = is_ ?? default(int);
            int fb_ = fb ?? default(int);
            updateFeedbackS(iss_, fb_);

            return RedirectToAction("NewPlayer", "Player");
        }
        private void  updateFeedbackS(int id_, int fb)
        {
            try
            {

                string cmdStr = "UPDATE Student SET Feedback=@Feedback WHERE StudentID=@StudentID";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", id_);
                        cmd.Parameters.AddWithValue("@Feedback", fb);


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



        /*   count moods    */

        protected int countHappy(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin AND MOOD=@Mood";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Happy");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }

        protected int countSad(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin AND MOOD=@Mood";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Sad");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }

        protected int countBored(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin AND MOOD=@Mood";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Bored");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }

        protected int countEnergetic(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin AND MOOD=@Mood";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Energetic");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }

        protected int countAngry(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin AND MOOD=@Mood";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Angry");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }

        protected int countGood(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin AND MOOD=@Mood";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Good");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }

        protected int countTired(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin AND MOOD=@Mood";
            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Tired");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }

        protected int countAnxious(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin AND MOOD=@Mood";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Anxious");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }



        protected int countAll(int cp)
        {
            Int32 count = 0;

            string cmdStr = "SELECT COUNT(*) FROM dbo.[Student] WHERE ClassPin= @ClassPin";

            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    using (SqlCommand command = new SqlCommand(cmdStr, connection))
                    {

                        command.Parameters.AddWithValue("@ClassPin", cp);
                        command.Parameters.AddWithValue("@Mood", "Anxious");
                        connection.Open();
                        count = (Int32)command.ExecuteScalar();

                    }
                    connection.Close();
                    return count;

                }
                catch (SqlException ex)
                {
                    throw ex;

                }


        }


        protected void UpdateStudentMood(int idStudent, string yourMood)
        {

            try
            {

                string cmdStr = "UPDATE Student SET Mood=@Mood WHERE StudentID=@StudentID";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", idStudent);
                        cmd.Parameters.AddWithValue("@Mood", yourMood);


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


        protected string pickedMood(string id_img)
        {
            string ans = "";

            if (id_img.Equals("1"))
            {
                ans = "Good";
            }
            else if (id_img.Equals("2"))
            {
                ans = "Happy";
            }
            else if (id_img.Equals("3"))
            {
                ans = "Energetic";
            }
            else if (id_img.Equals("4"))
            {
                ans = "Sad";
            }
            else if (id_img.Equals("5"))
            {
                ans = "Tired";
            }
            else if (id_img.Equals("6"))
            {
                ans = "Anxious";
            }
            else if (id_img.Equals("7"))
            {
                ans = "Bored";
            }
            else if (id_img.Equals("8"))
            {
                ans = "Angry";
            }
            return ans;
        }

        private void incrementStudentNumber(int thePin)
        {
            // 2. Get connection.

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // 3. Initialize and execute OACommand.
                using (IDbCommand spCommand = con.CreateCommand())
                {
                    spCommand.CommandText = "dbo.sp_updateNumOfStudents";
                    spCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    // 4. Initialize parameters.
                    // 4.1. The 'CarId' parameter is IN parameter.
                    IDbDataParameter ClassPinParameter = spCommand.CreateParameter();
                    ClassPinParameter.ParameterName = "@ClassPin";
                    ClassPinParameter.Value = thePin;
                    // second parameter




                    spCommand.Parameters.Add(ClassPinParameter);


                    con.Open();
                    spCommand.Connection = con;

                    spCommand.ExecuteNonQuery();

                    // 5. Consume the return value.



                }
            }
        }


        protected int returnDur(int? id_)
        {
            Int32 len = 0;

            if (id_.HasValue)
            {
                int idVd = id_ ?? default(int);

                string cmdStr = "SELECT Length FROM dbo.[Video] WHERE IDVideo= @IDVideo ";

                using (SqlConnection connection = new SqlConnection(connectionString))

                    try
                    {
                        using (SqlCommand command = new SqlCommand(cmdStr, connection))
                        {

                            command.Parameters.AddWithValue("@IDVideo", idVd);

                            connection.Open();

                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {

                                len = (Int32)reader["Length"];

                                return len;
                            }

                        }
                        connection.Close();
                        return len;
                    }
                    catch (SqlException ex)
                    {
                        throw ex;

                    }
            }
            else
            {
                return 0;
            }
        }
        protected string returnType(int? id_)
        {
            string ans = "";

            if (id_.HasValue)
            {
                int idVd = id_ ?? default(int);

                string cmdStr = "SELECT Type FROM dbo.[Video] WHERE IDVideo = @IDVideo";

                using (SqlConnection connection = new SqlConnection(connectionString))

                    try
                    {
                        using (SqlCommand command = new SqlCommand(cmdStr, connection))
                        {

                            command.Parameters.AddWithValue("@IDVideo", idVd);

                            connection.Open();

                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {

                                ans = (string)reader["Type"];

                                return ans;
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
            else
            {
                return "";
            }

        }

        protected string returnName(int? id_)
        {
            string ans = "";

            if (id_.HasValue)
            {
                int idVd = id_ ?? default(int);

                string cmdStr = "SELECT Name FROM dbo.[Video] WHERE IDVideo = @IDVideo";

                using (SqlConnection connection = new SqlConnection(connectionString))

                    try
                    {
                        using (SqlCommand command = new SqlCommand(cmdStr, connection))
                        {

                            command.Parameters.AddWithValue("@IDVideo", idVd);

                            connection.Open();

                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {

                                ans = (string)reader["Name"];

                                return ans;
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
            else
            {
                return "";
            }

        }

    }
}