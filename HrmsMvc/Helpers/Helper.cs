using HrmsMvc.Models;
using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace HrmsMvc.Helpers
{
    public static class Helper
    {
        public static Regex RegexEmail = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        public static Regex mobRgx = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");

        public static EmployeeModel getEmployeeInfoData(int EmpID)
        {
            EmployeeModel em = null;
            if (EmpID > 0)
            {
                DataTable dt = Db.GetEmployeeInfo(EmpID, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    em = new EmployeeModel();
                    foreach (DataRow row in dt.Rows)
                    {
                        em.EmpID = EmpID;
                        em.EmpFirstname = row["EmpFirstname"].ToString().TrimEnd();
                        em.EmpLastname = row["EmpLastname"].ToString().TrimEnd();
                        em.UserName = row["UserName"].ToString();

                        em.Gender = row["EmpGender"].ToString().TrimEnd();
                        em.PhoneNumber = row["EmpPhone"].ToString();
                        em.EmailId = row["EmpEmail"].ToString();
                        em.DateOfBirth = row["EmpDob"].ToString().Split(' ')[0];
                        em.UserPhotoPath = row["EmpPhotoPath"].ToString();
                        em.Designation = row["Designation"].ToString();
                        em.DesigType = Convert.ToInt32(row["EmpDesignation"].ToString());
                        em.Usertype = Convert.ToInt32(row["RoleId"].ToString());
                    }
                }
            }
            return em;
        }
        public static string HashPassword(string password)
        {
            //var sha1 = new SHA1CryptoServiceProvider();
            //var data = Encoding.ASCII.GetBytes(password);
            //var sha1data = sha1.ComputeHash(data);

            //return Convert.ToBase64String(sha1data);

            byte[] salt = GenerateSaltValue();
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        private static byte[] GenerateSaltValue()
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            return salt;
        }

        public static bool matchPassword(string entPassword, string strPassword)
        {
            bool flag = true;

            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(strPassword);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(entPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            /* Compare the results */
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public static AttendanceModel convertDateTimeFormat(AttendanceModel em)
        {
            if (em != null)
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                em.PunchinTime = (!string.IsNullOrEmpty(em.PunchinTime)) ?
                  (TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(em.PunchinTime), cstZone)).ToString("yyyy-MM-dd hh:mm:ss tt") : "";

                em.PunchoutTime = (!string.IsNullOrEmpty(em.PunchoutTime)) ?
                   (TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(em.PunchoutTime), cstZone)).ToString("yyyy-MM-dd hh:mm:ss tt") : "";
            }
            return em;
        }

        public static void MaintainLeaveTask()
        {
            //int year = DateTime.Now.Year;
            //DateTime firstDay = Convert.ToDateTime(new DateTime(year, 1, 1).ToShortDateString());
            //DateTime currDay = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            //if (firstDay == currDay)
            //{
            Db.MaintainLeaves();
            //}
            //await Task.Delay(1000);
        }

        public static DateTime GetFirstDayOfMonth(int iMonth, int year)
        {
            // create a datetime variable set to the passed in date
            DateTime dtFrom = new DateTime(year, iMonth, 1);

            // remove all of the days in the month
            // except the first day and set the
            // variable to hold that date
            dtFrom = dtFrom.AddDays(-(dtFrom.Day - 1));

            // return the first day of the month
            return dtFrom;
        }

        public static int GetBusinessDaysCount(DateTime firstDay, DateTime lastDay)
        {
            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;

            if (businessDays > fullWeekCount * 7)
            {
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)
                    businessDays -= 1;
            }

            businessDays -= fullWeekCount + fullWeekCount;

            return businessDays;
        }

        public static string getDuration(TimeSpan ts)
        {
            string hours = ts.TotalHours != 0 ? Convert.ToInt32(ts.TotalHours).ToString() : "00";
            string minutes = ts.Minutes != 0 ? ts.Minutes.ToString() : "00";
            string duration = hours + "." + minutes;
            return duration;
        }

        public static void sentEmail(string emailSubject, string emailBody, string toEmail)
        {
            try
            {
                string mailfrom = "tnoreply001@gmail.com";

                SmtpClient client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 25,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("tnoreply001@gmail.com", "#123Testuser"),
                    Timeout = 360000
                };
                using (var message = new MailMessage(mailfrom, toEmail)
                {
                    Subject = emailSubject,
                    Body = emailBody,
                    IsBodyHtml = true
                })
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}