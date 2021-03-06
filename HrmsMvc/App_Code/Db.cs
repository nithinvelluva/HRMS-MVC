﻿using HrmsMvc.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace HrmsMvc
{
    public static class Db
    {
        #region Manage Employee and Authentication

        public static DataTable Validateuser(string userName, string password)
        {
            DataTable dt = new DataTable();

            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT UserInfo.ID,Password,UserType,EmployeeInfo.EmpFirstname,EmployeeInfo.EmpLastname FROM UserInfo INNER JOIN EmployeeInfo ON UserInfo.ID = EmployeeInfo.EmpId WHERE UserName = @userName COLLATE SQL_Latin1_General_CP1_CS_AS", ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("userName", userName);
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!Helpers.Helper.matchPassword(password, dt.Rows[0]["Password"].ToString()))
                    {
                        dt = null;
                    }
                    else
                    {
                        dt.Columns.Remove("Password");
                    }
                }
            }
            catch (Exception ex)
            {
                dt = null;
            }

            return dt;
        }

        public static DataTable GetEmployeeInfo(int EmpId, string empEmail = null)
        {
            DataTable dt = new DataTable();
            string SelQuery = "";
            SqlDataAdapter da = null;

            if (string.IsNullOrEmpty(empEmail))
            {
                SelQuery = "SELECT a.UserName,b.EmpFirstname,b.EmpLastname,c.Privilage AS UserRole,b.EmpGender,b.EmpPhone,b.EmpEmail,b.EmpDob,b.EmpPhotoPath,ed.Designation,b.EmpDesignation,c.Id AS RoleId FROM EmployeeInfo b INNER JOIN UserInfo a ON a.ID = @EmpId INNER JOIN UserPrivilages c ON a.UserType=c.Id INNER JOIN EmpDesignation ed ON b.EmpDesignation = ed.ID WHERE b.EmpId = @EmpId";
                da = new SqlDataAdapter(SelQuery, ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("EmpId", EmpId);
            }
            else
            {
                SelQuery = "SELECT ei.EmpId,ei.EmpFirstname,ei.EmpLastname,ei.EmpPhone,ei.EmpEmail FROM EmployeeInfo ei WHERE ei.EmpEmail = @empEmail";
                da = new SqlDataAdapter(SelQuery, ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("empEmail", empEmail);
            }

            da.Fill(dt);
            return dt;
        }

        public static string UpdateUserPassword(int empId, string currPwd = null, string nwPwd = null)
        {
            string rtrnStr = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = null;
                    if (!string.IsNullOrEmpty(currPwd))
                    {
                        cmd = new SqlCommand("SELECT ID,Password FROM UserInfo WHERE ID = @EmpId", con);
                        cmd.Parameters.AddWithValue("EmpId", empId);

                        SqlDataReader sreader = cmd.ExecuteReader();

                        if (sreader.HasRows)
                        {
                            string stordPwd = "";
                            while (sreader.Read())
                            {
                                stordPwd = sreader["Password"].ToString();
                            }

                            sreader.Close();

                            if (Helpers.Helper.matchPassword(currPwd, stordPwd))
                            {
                                cmd = new SqlCommand("UPDATE UserInfo SET Password = @nwPwd WHERE ID = @EmpId", con);
                                cmd.Parameters.AddWithValue("EmpId", empId);
                                cmd.Parameters.AddWithValue("nwPwd", Helpers.Helper.HashPassword(nwPwd));

                                int rtrn = cmd.ExecuteNonQuery();
                                rtrnStr = (rtrn <= 0) ? "ERROR:" : "OK:";
                            }
                            else
                            {
                                rtrnStr = "PMM:";
                            }
                        }
                    }
                    else
                    {
                        cmd = new SqlCommand("UPDATE UserInfo SET Password = @nwPwd WHERE ID = @EmpId", con);
                        cmd.Parameters.AddWithValue("EmpId", empId);
                        cmd.Parameters.AddWithValue("nwPwd", Helpers.Helper.HashPassword(nwPwd));

                        int rtrn = cmd.ExecuteNonQuery();
                        rtrnStr = (rtrn <= 0) ? "ERROR:" : "OK:";
                    }
                    con.Close();
                }

                return rtrnStr;
            }
            catch (Exception ex)
            {
                return "ERROR";
            }
        }

        public static List<EmployeeModel> GetEmployeeDetails(int BlockSize, int BlockNumber, int sortingVal)
        {
            int startIndex = (BlockNumber - 1) * BlockSize;
            string coloumn = "EmpFirstname";
            string sortingOrder = "asc";
            switch (sortingVal)
            {
                case 1:
                    coloumn = "EmpFirstname";
                    sortingOrder = "asc";
                    break;
                case 2:
                    coloumn = "EmpFirstname";
                    sortingOrder = "desc";
                    break;
                default:
                    coloumn = "EmpFirstname";
                    sortingOrder = "asc";
                    break;
            }

            DataTable dt = new DataTable();
            SqlDataReader sreader = null;
            int totalRows = 0;
            EmployeeModel em = null;
            List<EmployeeModel> empDetails = new List<EmployeeModel>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                con.Open();
                SqlCommand scmd = new SqlCommand("SELECT COUNT(ID) FROM UserInfo WHERE UserType <> 1", con);
                sreader = scmd.ExecuteReader();
                if (sreader.HasRows)
                {
                    while (sreader.Read())
                    {
                        totalRows = Convert.ToInt32(sreader[0].ToString());
                    }
                }
                sreader.Close();

                string SelQuery = "SELECT a.ID AS EmpID,a.UserName,a.UserType,c.Privilage AS UserRole,b.EmpFirstname,b.EmpLastname,b.EmpGender,b.EmpPhone,b.EmpEmail,b.EmpDob,b.EmpPhotoPath,ed.Designation,ed.ID AS DesigType FROM EmployeeInfo b INNER JOIN UserInfo a ON a.ID=b.EmpId INNER JOIN UserPrivilages c ON (a.UserType=c.Id AND a.UserType != 1) INNER JOIN EmpDesignation ed ON b.EmpDesignation = ed.ID" +
                    " ORDER BY b." + coloumn + " " + sortingOrder + " OFFSET @startIndex ROWS FETCH NEXT @BlockSize ROWS ONLY";
                SqlDataAdapter da = new SqlDataAdapter(SelQuery, con);
                da.SelectCommand.Parameters.AddWithValue("startIndex", startIndex);
                da.SelectCommand.Parameters.AddWithValue("BlockSize", BlockSize);
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        em = new EmployeeModel();

                        em.NoMoreData = totalRows <= (startIndex + BlockSize);
                        em.EmpID = Convert.ToInt32(row["EmpID"].ToString());
                        em.EmpFirstname = row["EmpFirstname"].ToString();
                        em.EmpLastname = row["EmpLastname"].ToString();
                        em.UserName = row["UserName"].ToString();
                        em.Gender = row["EmpGender"].ToString().TrimEnd();
                        em.PhoneNumber = row["EmpPhone"].ToString();
                        em.EmailId = row["EmpEmail"].ToString();
                        em.DateOfBirth = (row["EmpDob"].ToString()).Split(' ')[0];
                        em.UserRole = row["UserRole"].ToString().TrimEnd();
                        em.Usertype = Convert.ToInt32(row["UserType"].ToString());
                        em.DesigType = Convert.ToInt32(row["DesigType"].ToString());
                        em.Designation = row["Designation"].ToString().TrimEnd();
                        em.UserPhotoPath = row["EmpPhotoPath"].ToString();

                        empDetails.Add(em);
                    }
                }

                con.Close();
                scmd.Dispose();
            }
            return empDetails;
        }

        public static DataTable getEmployeeFullInfo()
        {
            string SelQuery = "SELECT EmpId,EmpFirstname,EmpLastname,EmpDob,EmpPhotoPath,EmpGender FROM EmployeeInfo INNER JOIN UserInfo ON EmployeeInfo.EmpId = UserInfo.Id WHERE UserInfo.UserType <> 1";
            SqlDataAdapter da = new SqlDataAdapter(SelQuery, ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static string AddEmployee(EmployeeModel em)
        {
            try
            {
                string rtrnStr = "";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("SELECT ID FROM UserInfo WHERE UPPER(UserName) = @UserNameUpper", con);
                    cmd.Parameters.AddWithValue("UserNameUpper", em.UserName.ToUpper());
                    int idDup = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("SELECT ID FROM EmployeeInfo WHERE EmpPhone = @empPhone", con);
                    cmd.Parameters.AddWithValue("empPhone", em.PhoneNumber);
                    int phoneDup = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("SELECT ID FROM EmployeeInfo WHERE UPPER(EmpEmail) = @empEmailUpper", con);
                    cmd.Parameters.AddWithValue("empEmailUpper", em.EmailId.ToUpper());
                    int emaildup = Convert.ToInt32(cmd.ExecuteScalar());

                    if (idDup > 0)
                    {
                        rtrnStr = rtrnStr + "UD:";
                    }
                    if (phoneDup > 0)
                    {
                        rtrnStr = rtrnStr + "PD:";
                    }
                    if (emaildup > 0)
                    {
                        rtrnStr = rtrnStr + "ED:";
                    }

                    if (string.IsNullOrEmpty(rtrnStr))
                    {
                        cmd = new SqlCommand("InsertEmployeeDetailsMVC", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@empfirstname", em.EmpFirstname);
                        cmd.Parameters.AddWithValue("@emplastname", em.EmpLastname);
                        cmd.Parameters.AddWithValue("@empusername", em.UserName);
                        cmd.Parameters.AddWithValue("@emppassword", em.Password);
                        cmd.Parameters.AddWithValue("@empgender", em.Gender);
                        cmd.Parameters.AddWithValue("@empphone", em.PhoneNumber);
                        cmd.Parameters.AddWithValue("@empmail", em.EmailId);
                        cmd.Parameters.AddWithValue("@usertype", Convert.ToInt32(em.Usertype));
                        cmd.Parameters.AddWithValue("@designation", Convert.ToInt32(em.DesigType));
                        cmd.Parameters.AddWithValue("@empdob", Convert.ToDateTime(em.DateOfBirth));
                        cmd.Parameters.AddWithValue("@empid", 0);
                        cmd.Parameters.AddWithValue("@UserId", 0);
                        cmd.Parameters.AddWithValue("@casual", 0);
                        cmd.Parameters.AddWithValue("@festive", 0);
                        cmd.Parameters.AddWithValue("@sick", 0);
                        cmd.Parameters.AddWithValue("@year", DateTime.Now.Year);
                        cmd.Parameters.AddWithValue("@empphotopath", em.UserPhotoPath);

                        cmd.Parameters["@UserId"].Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        int employee_id = Convert.ToInt32(cmd.Parameters["@UserId"].Value.ToString());
                        rtrnStr = (employee_id > 0) ? "OK:" : "ERROR:";
                    }
                    con.Close();
                }

                return rtrnStr;
            }
            catch (Exception ex)
            {
                return "ERROR:";
            }
        }

        public static bool RemoveEmployee(int EmpID)
        {
            bool flag = false;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("RemoveEmployee", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@empid", EmpID);
                    cmd1.ExecuteNonQuery();

                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public static string UpdateProfile(EmployeeModel em, bool UsrIconUp = false)
        {
            string rtrnStr = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = null;

                    if (!UsrIconUp)
                    {
                        cmd = new SqlCommand("SELECT ID,Password FROM UserInfo WHERE ID = @EmpId", con);
                        cmd.Parameters.AddWithValue("EmpId", em.EmpID);

                        SqlDataReader sreader = cmd.ExecuteReader();

                        if (sreader.HasRows)
                        {
                            string stordPwd = "";
                            while (sreader.Read())
                            {
                                stordPwd = sreader["Password"].ToString();
                            }

                            sreader.Close();

                            if (Helpers.Helper.matchPassword(em.Password, stordPwd))
                            {
                                cmd = new SqlCommand("UPDATE UserInfo SET Password = @Password,UserType = @Usertype WHERE ID = @EmpID", con);
                                cmd.Parameters.AddWithValue("Password", Helpers.Helper.HashPassword(em.Password));
                                cmd.Parameters.AddWithValue("Usertype", em.Usertype);
                                cmd.Parameters.AddWithValue("EmpID", em.EmpID);
                                cmd.ExecuteNonQuery();

                                cmd = new SqlCommand("UPDATE EmployeeInfo SET EmpFirstname='" + em.EmpFirstname + "',EmpLastname='" + em.EmpLastname + "',EmpPhone='" + em.PhoneNumber + "',EmpEmail='" + em.EmailId + "',EmpDob='" + em.DateOfBirth + "' WHERE EmpId='" + em.EmpID + "'", con);
                                //cmd.Parameters.AddWithValue("EmpName", em.EmpName);
                                //cmd.Parameters.AddWithValue("PhoneNumber", em.PhoneNumber);
                                //cmd.Parameters.AddWithValue("EmailId", em.EmailId);
                                //cmd.Parameters.AddWithValue("DateOfBirth", em.DateOfBirth);
                                //cmd.Parameters.AddWithValue("EmpID", em.EmpID);

                                int rtrnRw = cmd.ExecuteNonQuery();
                                if (rtrnRw <= 0)
                                {
                                    rtrnStr = "ERROR:";
                                }
                            }
                            else
                            {
                                rtrnStr = "PM:";
                            }
                        }
                    }
                    else
                    {
                        cmd = new SqlCommand("UPDATE EmployeeInfo SET EmpPhotoPath=@UserPhotoPath WHERE EmpId = @EmpID", con);
                        cmd.Parameters.AddWithValue("UserPhotoPath", em.UserPhotoPath);
                        cmd.Parameters.AddWithValue("EmpID", em.EmpID);

                        int rtrnRw = cmd.ExecuteNonQuery();
                        if (rtrnRw <= 0)
                        {
                            rtrnStr = "ERROR";
                        }
                        else
                        {
                            rtrnStr = "OK";
                        }
                    }
                }

                return rtrnStr;
            }
            catch (Exception ex)
            {
                return "ERROR:";
            }
        }

        public static string UpdateEmployeeInfoDashboard(EmployeeModel em)
        {
            string rtrnStr = "OK";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = null;
                    cmd = new SqlCommand("UPDATE UserInfo SET UserType = @Usertype WHERE ID = @EmpID", con);
                    cmd.Parameters.AddWithValue("Usertype", em.Usertype);
                    cmd.Parameters.AddWithValue("EmpID", em.EmpID);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("UPDATE EmployeeInfo SET EmpDesignation='" + em.DesigType + "' WHERE EmpId='" + em.EmpID + "'", con);

                    int rtrnRw = cmd.ExecuteNonQuery();
                    if (rtrnRw <= 0)
                    {
                        rtrnStr = "ERROR";
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                rtrnStr = "ERROR";
            }

            return rtrnStr;
        }

        public static IEnumerable<SelectListItem> GetRoleSelectList()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [UserPrivilages]", ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
            da.Fill(dt);
            return
                dt.AsEnumerable().Select(u => new SelectListItem
                {
                    Value = Convert.ToString(u.Field<int>("Id")),
                    Text = u.Field<string>("Privilage")
                });
        }

        public static DataTable getEmployeeDesignationList()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = null;
            da = new SqlDataAdapter("SELECT * FROM EmpDesignation", ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
            da.Fill(dt);
            return dt;
        }

        public static void createResetPasswordToken(string empId, string token)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO PasswordResetToken VALUES(@empId,@token,@created)", con);
                cmd.Parameters.AddWithValue("empId", empId);
                cmd.Parameters.AddWithValue("token", token);
                cmd.Parameters.AddWithValue("created", DateTime.UtcNow.AddHours(4).ToString());
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void updateResetPasswordToken(string empId, string token)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE PasswordResetToken SET Created = @created WHERE EmpId = @empId AND Token = @token", con);
                cmd.Parameters.AddWithValue("empId", empId);
                cmd.Parameters.AddWithValue("token", token);
                cmd.Parameters.AddWithValue("created", DateTime.UtcNow.ToString());
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static bool validateResetPasswordToken(string empId, string token)
        {
            bool rtrnFlag = false;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                con.Open();
                SqlDataReader sreader = null;

                SqlCommand cmd = new SqlCommand("SELECT Token,Created FROM PasswordResetToken WHERE EmpId = @empId AND Token = @token", con);
                cmd.Parameters.AddWithValue("empId", empId);
                cmd.Parameters.AddWithValue("token", token);
                sreader = cmd.ExecuteReader();
                string created = "";
                if (sreader.HasRows)
                {
                    while (sreader.Read())
                    {
                        created = sreader["Created"].ToString();
                    }
                }
                sreader.Close();

                if (DateTime.UtcNow <= Convert.ToDateTime(created))
                {
                    rtrnFlag = true;
                }

                con.Close();
            }

            return rtrnFlag;
        }

        public static List<EmployeeModel> searchUser(string searchText)
        {
            DataTable dt;
            List<EmployeeModel> empDetails = new List<EmployeeModel>();
            try
            {
                dt = new DataTable();
                EmployeeModel em = null;
                SqlDataAdapter da = null;
                searchText = "'%" + searchText + "%'";
                searchText = "SELECT ei.EmpId AS EmpID,EmpFirstname,EmpLastname,EmpGender,EmpPhotoPath,ed.Designation FROM EmployeeInfo ei INNER JOIN EmpDesignation ed ON ei.EmpDesignation = ed.ID INNER JOIN UserInfo ui ON ei.EmpId = ui.Id WHERE ui.UserType <> 1 AND (ei.EmpFirstname LIKE " + searchText + " OR ei.EmpLastname LIKE " + searchText + " )";
                da = new SqlDataAdapter(searchText, ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        em = new EmployeeModel();
                        em.EmpID = Convert.ToInt32(row["EmpID"].ToString());
                        em.EmpFirstname = row["EmpFirstname"].ToString();
                        em.EmpLastname = row["EmpLastname"].ToString();
                        em.Gender = row["EmpGender"].ToString().TrimEnd();

                        em.Designation = row["Designation"].ToString().TrimEnd();
                        em.UserPhotoPath = row["EmpPhotoPath"].ToString();
                        empDetails.Add(em);
                    }
                }
            }
            catch (Exception ex)
            {
                empDetails = null;
            }
            return empDetails;
        }

        #endregion

        #region Manage Attendance        

        public static List<AttendanceModel> GetEmpPunchDetails(int empid, string startDate = null, string endDate = null, bool isReport = false, int BlockNumber = 1, int BlockSize = 1)
        {
            try
            {
                string sdate = "";
                string edate = "";
                int startIndex = BlockNumber;

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    sdate = startDate.ToString().Split(' ')[0];
                    edate = endDate.ToString().Split(' ')[0] + " " + "23:59:58.000";
                }
                else if (string.IsNullOrEmpty(endDate))
                {
                    sdate = startDate.ToString().Split(' ')[0];
                    edate = DateTime.UtcNow.ToString().Split(' ')[0] + " " + "23:59:58.000";
                }

                DataTable dt = new DataTable();
                SqlDataAdapter da = null;
                SqlDataReader sreader = null;
                int totalRows = 0;

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    SqlCommand scmd = new SqlCommand("SELECT COUNT(PunchinTime) FROM Attendance WHERE (PunchinTime BETWEEN @sdate AND @edate) AND EmpId = @empid", con);
                    scmd.Parameters.AddWithValue("sdate", sdate);
                    scmd.Parameters.AddWithValue("edate", edate);
                    scmd.Parameters.AddWithValue("empid", empid);

                    sreader = scmd.ExecuteReader();
                    if (sreader.HasRows)
                    {
                        while (sreader.Read())
                        {
                            totalRows = Convert.ToInt32(sreader[0].ToString());
                        }
                    }
                    sreader.Close();

                    if (isReport)
                    {
                        da = new SqlDataAdapter("SELECT PunchinTime,PunchoutTime FROM Attendance WHERE (PunchinTime BETWEEN @sdate AND @edate) AND EmpId = @empid ORDER BY PunchinTime", con);
                    }
                    else
                    {
                        da = new SqlDataAdapter("SELECT PunchinTime,PunchoutTime FROM Attendance WHERE (PunchinTime BETWEEN @sdate AND @edate) AND EmpId = @empid ORDER BY PunchinTime OFFSET @startIndex ROWS FETCH NEXT @blockSize ROWS ONLY", con);
                        da.SelectCommand.Parameters.AddWithValue("startIndex", startIndex);
                        da.SelectCommand.Parameters.AddWithValue("blockSize", BlockSize);
                    }
                    da.SelectCommand.Parameters.AddWithValue("sdate", sdate);
                    da.SelectCommand.Parameters.AddWithValue("edate", edate);
                    da.SelectCommand.Parameters.AddWithValue("empid", empid);
                    da.Fill(dt);

                    con.Close();
                    scmd.Dispose();
                }
                List<AttendanceModel> attList = new List<AttendanceModel>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DateTime pi = new DateTime();
                    DateTime po = new DateTime();

                    AttendanceModel am = null;
                    foreach (DataRow row in dt.Rows)
                    {
                        am = new AttendanceModel();
                        am.RowCount = totalRows;
                        if (!row["PunchinTime"].Equals(DBNull.Value))
                        {
                            pi = Convert.ToDateTime(row["PunchinTime"]);
                            am.PunchinTime = pi.ToString("yyyy-MM-dd hh:mm:ss tt");
                        }

                        if (!row["PunchoutTime"].Equals(DBNull.Value))
                        {
                            po = Convert.ToDateTime(row["PunchoutTime"]);
                            am.PunchoutTime = po.ToString("yyyy-MM-dd hh:mm:ss tt");
                        }
                        else
                        {
                            po = pi;
                        }

                        TimeSpan ts = po - pi;
                        am.Duration = Helpers.Helper.getDuration(ts);
                        am = Helpers.Helper.convertDateTimeFormat(am);
                        attList.Add(am);
                    }
                }
                return attList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static AttendanceModel GetEmployeeAttendanceDetails(int empId)
        {
            try
            {
                DataTable dt = new DataTable();
                string date = DateTime.Now.ToString().Split(' ')[0];
                SqlDataAdapter da = new SqlDataAdapter("SELECT TOP 1 PunchinTime,PunchoutTime FROM Attendance WHERE EmpId = @empId ORDER BY PunchinTime DESC", ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("empId", empId);
                da.Fill(dt);

                AttendanceModel am = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    am = new AttendanceModel();
                    am.PunchinTime = (!string.IsNullOrEmpty(dt.Rows[0]["PunchinTime"].ToString())) ? (dt.Rows[0]["PunchinTime"]).ToString() : "";
                    am.PunchoutTime = (!string.IsNullOrEmpty(dt.Rows[0]["PunchoutTime"].ToString())) ? (dt.Rows[0]["PunchoutTime"]).ToString() : "";
                }
                return am;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool AddAttendance(AttendanceModel am, int type)
        {
            bool rtrn = false;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    if (type == 1)
                    {
                        SqlCommand cmd1 = new SqlCommand("INSERT INTO Attendance(EmpId,PunchinTime,Notes) VALUES(@EmpID,@PunchinTime,@Notes) ", con);
                        cmd1.Parameters.AddWithValue("EmpID", am.EmpID);
                        cmd1.Parameters.AddWithValue("PunchinTime", am.PunchinTime);
                        cmd1.Parameters.AddWithValue("Notes", am.Notes);
                        cmd1.ExecuteNonQuery();
                    }
                    else if (type == 2)
                    {
                        string pin = am.PunchinTime;

                        SqlCommand cmd4 = new SqlCommand("UPDATE Attendance SET PunchoutTime = @PunchoutTime where ID = (Select ID from Attendance where CONVERT(datetime,PunchinTime) = @pin AND EmpId = @EmpID)", con);
                        cmd4.Parameters.AddWithValue("PunchoutTime", am.PunchoutTime);
                        cmd4.Parameters.AddWithValue("pin", pin);
                        cmd4.Parameters.AddWithValue("EmpID", am.EmpID);
                        cmd4.ExecuteNonQuery();
                    }

                    con.Close();
                    rtrn = true;
                }
            }
            catch (Exception ex)
            {
                rtrn = false;
            }
            return rtrn;
        }

        #endregion

        #region Manage Leaves

        public static IEnumerable<SelectListItem> GetLeaveTypes()
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id,Type FROM LeaveType", ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.Fill(dt);

                return dt.AsEnumerable().Select(u => new SelectListItem
                {
                    Value = Convert.ToString(u.Field<int>("Id")),
                    Text = u.Field<string>("Type")
                });

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable GetLeaveStatistics(int EmpId)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = null;
                da = new SqlDataAdapter("SELECT [EmpId],[CasualLeave],[FestiveLeave],[SickLeave],[LossOfPay] FROM LeaveStatistics WHERE [EmpId] = @EmpId AND Year = @year", ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("EmpId", EmpId);
                da.SelectCommand.Parameters.AddWithValue("year", DateTime.Now.Year);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<LeaveModel> GetEmployeeLeaveDetails(int EmpId, int userType, LeaveModel lv = null, bool IsReport = false, int BlockNumber = 1, int BlockSize = 1, bool IsCalendarLoad = false)
        {
            LeaveModel li = null;
            List<LeaveModel> linfo = new List<LeaveModel>();
            SqlDataAdapter da = null;
            DataTable dt = new DataTable();
            SqlDataReader sreader = null;
            int totalRows = 0;
            int startIndex = BlockNumber;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                con.Open();
                SqlCommand scmd = new SqlCommand("SELECT COUNT(FromDate) FROM EmployeeLeaveInfo WHERE ((FromDate BETWEEN @FromDate AND @ToDate) OR (ToDate BETWEEN @FromDate AND @ToDate)) AND EmpId = @empid", con);
                scmd.Parameters.AddWithValue("FromDate", lv._fromdate);
                scmd.Parameters.AddWithValue("ToDate", lv._todate);
                scmd.Parameters.AddWithValue("empid", EmpId);

                sreader = scmd.ExecuteReader();
                if (sreader.HasRows)
                {
                    while (sreader.Read())
                    {
                        totalRows = Convert.ToInt32(sreader[0].ToString());
                    }
                }
                sreader.Close();

                if (userType == 1)
                {
                    da = new SqlDataAdapter("SELECT eli.EmpId,eli.FromDate,eli.ToDate,lt.Type,eli.Status,eli.Comments,eli.Id As LeaveID,eli.DurationType,lt.Id AS LeaveTypeID,eli.LeaveSessionType FROM EmployeeLeaveInfo eli INNER JOIN LeaveType lt ON eli.LeaveType=lt.Id WHERE CONVERT(date,FromDate) BETWEEN @startDate AND @endDate AND eli.Status != 3 ORDER BY eli.FromDate DESC", ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                    da.SelectCommand.Parameters.AddWithValue("startDate", lv._fromdate);
                    da.SelectCommand.Parameters.AddWithValue("endDate", lv._todate);
                }
                else if (userType != 1)
                {
                    if (IsReport && !IsCalendarLoad)
                    {
                        da = new SqlDataAdapter("SELECT eli.EmpId,eli.FromDate,eli.ToDate,lt.Type,eli.Status,eli.Comments,eli.Id As LeaveID,eli.DurationType,lt.Id AS LeaveTypeID,eli.LeaveSessionType FROM EmployeeLeaveInfo eli INNER JOIN LeaveType lt ON eli.LeaveType=lt.Id WHERE eli.EmpId=@EmpId AND eli.Status IN (1,2) AND ((FromDate BETWEEN @FromDate AND @ToDate) OR (ToDate BETWEEN @FromDate AND @ToDate)) ORDER BY eli.FromDate", con);
                        da.SelectCommand.Parameters.AddWithValue("EmpId", EmpId);
                    }
                    else if (!IsReport && IsCalendarLoad)
                    {
                        if (lv != null && lv.EmpID > 0)
                        {
                            da = new SqlDataAdapter("SELECT eli.EmpId,eli.FromDate,eli.ToDate,lt.Type,eli.Status,eli.Comments,eli.Id As LeaveID,eli.DurationType,lt.Id AS LeaveTypeID,eli.LeaveSessionType,einfo.* FROM EmployeeLeaveInfo eli INNER JOIN LeaveType lt ON eli.LeaveType=lt.Id INNER JOIN EmployeeInfo einfo ON eli.EmpId = einfo.EmpId WHERE eli.EmpId=@EmpId AND ((FromDate BETWEEN @FromDate AND @ToDate) OR (ToDate BETWEEN @FromDate AND @ToDate)) ORDER BY eli.FromDate DESC", con);
                            da.SelectCommand.Parameters.AddWithValue("EmpId", EmpId);
                        }
                        else
                        {
                            da = new SqlDataAdapter("SELECT eli.EmpId,eli.FromDate,eli.ToDate,lt.Type,eli.Status,eli.Comments,eli.Id As LeaveID,eli.DurationType,lt.Id AS LeaveTypeID,eli.LeaveSessionType,einfo.* FROM EmployeeLeaveInfo eli INNER JOIN LeaveType lt ON eli.LeaveType=lt.Id INNER JOIN EmployeeInfo einfo ON eli.EmpId = einfo.EmpId WHERE ((FromDate BETWEEN @FromDate AND @ToDate) OR (ToDate BETWEEN @FromDate AND @ToDate)) ORDER BY eli.FromDate DESC", con);
                        }
                    }
                    else if (!IsReport && !IsCalendarLoad)
                    {
                        da = new SqlDataAdapter("SELECT eli.EmpId,eli.FromDate,eli.ToDate,lt.Type,eli.Status,eli.Comments,eli.Id As LeaveID,eli.DurationType,lt.Id AS LeaveTypeID,eli.LeaveSessionType FROM EmployeeLeaveInfo eli INNER JOIN LeaveType lt ON eli.LeaveType=lt.Id WHERE eli.EmpId=@EmpId AND ((FromDate BETWEEN @FromDate AND @ToDate) OR (ToDate BETWEEN @FromDate AND @ToDate)) ORDER BY eli.FromDate OFFSET @startIndex ROWS FETCH NEXT @blockSize ROWS ONLY", con);
                        da.SelectCommand.Parameters.AddWithValue("startIndex", startIndex);
                        da.SelectCommand.Parameters.AddWithValue("blockSize", BlockSize);
                        da.SelectCommand.Parameters.AddWithValue("EmpId", EmpId);
                    }

                    da.SelectCommand.Parameters.AddWithValue("FromDate", lv._fromdate);
                    da.SelectCommand.Parameters.AddWithValue("ToDate", lv._todate);
                }
                da.Fill(dt);

                con.Close();
                scmd.Dispose();
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    li = new LeaveModel();
                    li.RowCount = totalRows;
                    li._lvId = Convert.ToInt32(row["LeaveID"].ToString());
                    li.EmpID = Convert.ToInt32(row["EmpId"].ToString());
                    li._fromdate = row["FromDate"].ToString().Split(' ')[0];
                    li._todate = row["ToDate"].ToString().Split(' ')[0];
                    li._leaveType = !string.IsNullOrEmpty(row["LeaveTypeID"].ToString()) ? Convert.ToInt32(row["LeaveTypeID"].ToString()) : 0;
                    li._strLvType = row["Type"].ToString();
                    li._leavedurationtype = row["DurationType"].ToString();
                    li._leaveDurTypeInt = (li._leavedurationtype.Equals("Full Day")) ? 1 : 2;
                    li._comments = row["Comments"].ToString();
                    li._leaveHalfDaySession = Convert.ToInt32(row["LeaveSessionType"].ToString());

                    if (!IsReport && IsCalendarLoad)
                    {
                        li.UserPhotoPath = row["EmpPhotoPath"].ToString();
                        li.EmpFirstname = row["EmpFirstName"].ToString();
                        li.EmpLastname = row["EmpLastName"].ToString();
                        li.Gender = row["EmpGender"].ToString();
                    }

                    DateTime fd = Convert.ToDateTime(li._fromdate.ToString());
                    DateTime td = Convert.ToDateTime(li._todate.ToString());
                    li.RtrnArry = CalculateLeaveStatistics(fd, td, li._strLvType.TrimEnd(), li._leavedurationtype.TrimEnd(), li.EmpID);

                    switch ((!string.IsNullOrEmpty(row["Status"].ToString())) ? Convert.ToInt32(row["Status"].ToString()) : 0)
                    {
                        case -1://rejected state
                            li._status = false;
                            li._rejected = true;
                            li._cancelled = false;
                            li._leaveStatus = "REJECTED";
                            break;
                        case 1://pending state
                            li._status = false;
                            li._rejected = false;
                            li._cancelled = false;
                            li._leaveStatus = "PENDING";
                            break;
                        case 2://accepted state    
                            li._status = true;
                            li._rejected = false;
                            li._cancelled = false;
                            li._leaveStatus = "APPROVED";
                            break;
                        case 3: //cancelled state               
                            li._status = false;
                            li._rejected = false;
                            li._cancelled = true;
                            li._leaveStatus = "CANCELLED";
                            break;
                        default:
                            break;
                    }
                    linfo.Add(li);
                }
            }
            return linfo;
        }

        public static void MaintainLeaves()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("SELECT ID FROM LeaveStatistics WHERE Year = @year", con);
                    cmd1.Parameters.AddWithValue("year", DateTime.Now.Year);
                    int id = Convert.ToInt32(cmd1.ExecuteScalar());

                    if (id <= 0)
                    {
                        cmd1 = new SqlCommand("AddLeaveInfo", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@year", DateTime.Now.Year);
                        cmd1.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static GenericCallbackModel AddLeave(LeaveModel lm)
        {
            string returnStr = null;
            int leaveId = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    int existId = 0;
                    bool flag = false;
                    int status = 1;

                    SqlCommand scmd = new SqlCommand("SELECT Id FROM EmployeeLeaveInfo WHERE FromDate = '" + lm._fromdate + "' AND ToDate = '" + lm._todate + "' AND EmpId = @empId AND LeaveType = @lvType AND Status NOT IN (3,-1) ", con);
                    scmd.Parameters.AddWithValue("empId", lm.EmpID);
                    scmd.Parameters.AddWithValue("lvType", lm._leaveType);

                    SqlDataReader sreader = scmd.ExecuteReader();
                    if (sreader.HasRows)
                    {
                        while (sreader.Read())
                        {
                            existId = Convert.ToInt32(sreader[0].ToString());
                        }
                    }
                    sreader.Close();
                    if (existId > 0)
                    {
                        con.Close();
                        returnStr = "EXISTS";
                    }
                    else
                    {
                        try
                        {
                            SqlCommand cmd2 = new SqlCommand("AddEmployeeLeave", con);
                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Parameters.AddWithValue("@empId", lm.EmpID);
                            cmd2.Parameters.AddWithValue("@fromdate", lm._fromdate);
                            cmd2.Parameters.AddWithValue("@todate", lm._todate);
                            cmd2.Parameters.AddWithValue("@leaveType", lm._leaveType);
                            cmd2.Parameters.AddWithValue("@status", status);
                            cmd2.Parameters.AddWithValue("@comments", (lm._comments != null) ? lm._comments : "");
                            cmd2.Parameters.AddWithValue("@leavedurationtype", lm._leavedurationtype);
                            cmd2.Parameters.AddWithValue("@leaveId", 0);
                            cmd2.Parameters.AddWithValue("@leaveSessionType", lm._leaveHalfDaySession);
                            cmd2.Parameters.AddWithValue("@calendarEntryId", 0);                            

                            cmd2.Parameters["@leaveId"].Direction = ParameterDirection.Output;
                            cmd2.ExecuteNonQuery();
                            leaveId = Convert.ToInt32(cmd2.Parameters["@leaveId"].Value.ToString());

                            con.Close();
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            flag = false;
                        }

                        if (flag)
                        {
                            UpdateLeaveStatistics(lm);
                            con.Close();
                            returnStr = "OK";
                        }
                        else
                        {
                            con.Close();
                            returnStr = "ERROR";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnStr = null;
            }
            return new GenericCallbackModel { ID = leaveId, Message = returnStr };
        }

        public static GenericCallbackModel UpdateLeave(LeaveModel lm)
        {
            string rtrnStr = null;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    ArrayList rtrnArrExis = new ArrayList();
                    SqlCommand scmd = null;
                    string query;
                    bool lvDupFlag = false;
                    int leaveStatus = 0;
                    if (lm == null) return null;

                    if (lm._status && !lm._cancelled && !lm._rejected) // Approved state.
                    {
                        leaveStatus = 2;
                        query = "UPDATE EmployeeLeaveInfo SET Status = @status WHERE [Id] = @lvId";
                        scmd = new SqlCommand(query, con);
                        scmd.Parameters.AddWithValue("status", leaveStatus);
                        scmd.Parameters.AddWithValue("lvId", lm._lvId);
                        scmd.ExecuteNonQuery();
                        con.Close();
                        rtrnStr = "OK";
                    }
                    else
                    {
                        #region Duplicate checking only for PENDING State.
                        if (!lm._cancelled && !lm._status && !lm._rejected)
                        {
                            leaveStatus = 1;
                            int existId = 0;
                            scmd = new SqlCommand("SELECT Id FROM EmployeeLeaveInfo WHERE FromDate=@FromDate AND ToDate=@ToDate AND EmpId=@EmpId AND LeaveType=@leaveType AND Id !=@lvId AND Status NOT IN (3,-1)", con);
                            scmd.Parameters.AddWithValue("FromDate", lm._fromdate);
                            scmd.Parameters.AddWithValue("ToDate", lm._todate);
                            scmd.Parameters.AddWithValue("EmpId", lm.EmpID);
                            scmd.Parameters.AddWithValue("leaveType", lm._leaveType);
                            scmd.Parameters.AddWithValue("lvId", lm._lvId);

                            SqlDataReader sreader = scmd.ExecuteReader();

                            if (sreader.HasRows)
                            {
                                while (sreader.Read())
                                {
                                    existId = Convert.ToInt32(sreader[0].ToString());
                                }
                            }
                            sreader.Close();
                            if (existId > 0)
                            {
                                con.Close();
                                rtrnStr = "EXISTS";
                                lvDupFlag = true;
                            }
                        }
                        #endregion
                        if (!lvDupFlag)
                        {
                            if (lm._rejected || lm._cancelled)
                            {
                                LeaveModel existLeaveModel = leaveDetailsFetch(lm._lvId);
                                existLeaveModel.RtrnArry = lm.RtrnArry;
                                existLeaveModel._cancelled = lm._cancelled;
                                existLeaveModel._rejected = lm._rejected;
                                existLeaveModel._status = lm._status;
                                leaveStatus = (lm._rejected) ? -1 : 3;
                                lm = existLeaveModel;
                            }

                            DateTime frmdt = new DateTime();
                            DateTime todt = new DateTime();
                            string LvDurType = "";

                            frmdt = Convert.ToDateTime(lm._fromdate.ToString());
                            todt = Convert.ToDateTime(lm._todate.ToString());
                            LvDurType = lm._leavedurationtype.ToString().TrimEnd();

                            rtrnArrExis = CalculateLeaveStatistics(frmdt, todt, lm._strLvType, LvDurType, lm.EmpID);

                            query = "UPDATE EmployeeLeaveInfo SET FromDate = @fromdate,ToDate = @todate,Comments = @comments,DurationType = @leavedurationtype,Status = @status,LeaveSessionType = @leaveHalfDaySession WHERE [Id] = @lvId";
                            scmd = new SqlCommand(query, con);
                            scmd.Parameters.AddWithValue("fromdate", lm._fromdate);
                            scmd.Parameters.AddWithValue("todate", lm._todate);
                            scmd.Parameters.AddWithValue("comments", lm._comments);
                            scmd.Parameters.AddWithValue("leavedurationtype", lm._leavedurationtype);
                            scmd.Parameters.AddWithValue("leaveHalfDaySession", lm._leaveHalfDaySession);
                            scmd.Parameters.AddWithValue("status", leaveStatus);

                            scmd.Parameters.AddWithValue("lvId", lm._lvId);
                            scmd.ExecuteNonQuery();
                            con.Close();

                            UpdateLeaveStatistics(lm, rtrnArrExis);

                            con.Close();
                            rtrnStr = "OK";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rtrnStr = "ERROR";
            }
            return new GenericCallbackModel { ID = lm._lvId, Message = rtrnStr };
        }

        public static ArrayList CalculateLeaveStatistics(DateTime from, DateTime to, string leaveType, string LeaveDurationType, int empId)
        {
            int startYear = from.Year;
            int endYear = to.Year;
            double prvLvs = 0.0;
            double nxtLvs = 0.0;
            double leaves = 0.0;
            double NoLeaves = 0.0;
            bool flag = false;
            ArrayList rtrnArr = new ArrayList();

            if (startYear != endYear)
            {
                DateTime lastDay = new DateTime(startYear, 12, 31);
                DateTime firstDay = new DateTime(endYear, to.Month, 1);
                bool prvFlag = false;
                bool nxtFlag = false;

                //for both years.
                if (LeaveDurationType == "Half Day")
                {
                    //previous year
                    prvLvs = Helpers.Helper.GetBusinessDaysCount(from, lastDay);
                    prvLvs = 0.5 * prvLvs;

                    //next year
                    nxtLvs = Helpers.Helper.GetBusinessDaysCount(firstDay, to);
                    nxtLvs = 0.5 * nxtLvs;
                }
                else if (LeaveDurationType == "Full Day")
                {
                    //previous year
                    prvLvs = Helpers.Helper.GetBusinessDaysCount(from, lastDay);

                    //next year
                    nxtLvs = Helpers.Helper.GetBusinessDaysCount(firstDay, to);
                }

                NoLeaves = EnoughLeaves(empId, leaveType, startYear);

                if (NoLeaves == 0.0 || prvLvs > NoLeaves)
                {
                    prvFlag = false;
                }
                else if (prvLvs <= NoLeaves && NoLeaves >= 0)
                {
                    prvFlag = true;
                }

                NoLeaves = EnoughLeaves(empId, leaveType, endYear);

                if (NoLeaves == 0.0 || nxtLvs > NoLeaves)
                {
                    nxtFlag = false;
                }
                else if (nxtLvs <= NoLeaves && NoLeaves >= 0)
                {
                    nxtFlag = true;
                }

                if (!prvFlag || !nxtFlag)
                {
                    flag = false;
                }
                else if (prvFlag && nxtFlag)
                {
                    flag = true;
                }

                rtrnArr.Add(new List<int> { startYear, endYear });
                rtrnArr.Add(new List<double> { prvLvs, nxtLvs });
            }
            else
            {
                if (LeaveDurationType == "Half Day")
                {
                    leaves = Helpers.Helper.GetBusinessDaysCount(from, to);
                    leaves = 0.5 * leaves;
                }
                else if (LeaveDurationType == "Full Day")
                {
                    leaves = Helpers.Helper.GetBusinessDaysCount(from, to);
                }

                NoLeaves = EnoughLeaves(empId, leaveType, startYear);

                if (NoLeaves == 0.0 || leaves > NoLeaves)
                {
                    flag = false;
                }
                else if (leaves <= NoLeaves && NoLeaves >= 0)
                {
                    flag = true;
                }

                rtrnArr.Add(new List<int> { startYear });
                rtrnArr.Add(new List<double> { leaves });
            }

            rtrnArr.Add(flag);
            return rtrnArr;
        }

        private static void UpdateLeaveStatistics(LeaveModel linfo, ArrayList rtrnArrExis = null)
        {
            double leaves = 0.0;
            List<int> yearArr = (List<int>)linfo.RtrnArry[0];
            List<double> leaveArr = (List<double>)linfo.RtrnArry[1];
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                con.Open();

                for (int i = 0; i < yearArr.Count; i++)
                {
                    leaves = (rtrnArrExis == null) ? leaveArr[i] : ((List<double>)rtrnArrExis[1])[i];

                    switch (linfo._strLvType.TrimEnd())
                    {
                        case "Casual":
                            linfo._casualLeave = leaveArr[i];
                            break;
                        case "Festive":
                            linfo._festiveLeave = leaveArr[i];
                            break;
                        case "Sick":
                            linfo._sickLeave = leaveArr[i];
                            break;
                        default:
                            break;
                    }

                    SqlCommand cmd3 = new SqlCommand("SELECT CasualLeave,FestiveLeave,SickLeave,LossOfPay FROM LeaveStatistics WHERE EmpId = @EmpId AND Year = @year", con);
                    cmd3.Parameters.AddWithValue("EmpId", linfo.EmpID);
                    cmd3.Parameters.AddWithValue("year", yearArr[i]);
                    SqlDataReader rd3 = cmd3.ExecuteReader();
                    double cl = 0.0;
                    double sl = 0.0;
                    double fl = 0.0;
                    //double lp = 0.0;

                    if (rd3.HasRows)
                    {
                        while (rd3.Read())
                        {
                            cl = Convert.ToDouble(rd3[0]) - linfo._casualLeave;
                            fl = Convert.ToDouble(rd3[1]) - linfo._festiveLeave;
                            sl = Convert.ToDouble(rd3[2]) - linfo._sickLeave;
                            //lp = Convert.ToDouble(rd3[3]) - linfo.LossOfPay;

                            switch (linfo._strLvType.TrimEnd())
                            {
                                case "Casual":
                                    cl = (!linfo._cancelled && !linfo._rejected) ? ((linfo._lvId > 0) ? (cl + leaves) : cl) : (cl + 2 * leaves);
                                    break;
                                case "Festive":
                                    fl = (!linfo._cancelled && !linfo._rejected) ? ((linfo._lvId > 0) ? (fl + leaves) : fl) : (fl + 2 * leaves);
                                    break;
                                case "Sick":
                                    sl = (!linfo._cancelled && !linfo._rejected) ? ((linfo._lvId > 0) ? (sl + leaves) : sl) : (sl + 2 * leaves);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    rd3.Close();

                    SqlCommand cmd4 = new SqlCommand("UPDATE LeaveStatistics SET CasualLeave=@cl ,FestiveLeave=@fl,SickLeave=@sl WHERE EmpId=@EmpId AND Year = @year", con);
                    cmd4.Parameters.AddWithValue("cl", cl);
                    cmd4.Parameters.AddWithValue("fl", fl);
                    cmd4.Parameters.AddWithValue("sl", sl);
                    cmd4.Parameters.AddWithValue("EmpId", linfo.EmpID);
                    cmd4.Parameters.AddWithValue("year", yearArr[i]);
                    cmd4.ExecuteNonQuery();
                }
                con.Close();
            }
        }

        public static double EnoughLeaves(int EmpId, string LeaveTypeStr, int startYear)
        {
            try
            {
                string colName = "";
                double NoLeaves = 0.0;
                switch (LeaveTypeStr)
                {
                    case "Casual":
                        colName = "CasualLeave";
                        break;

                    case "Festive":
                        colName = "FestiveLeave";
                        break;

                    case "Sick":
                        colName = "SickLeave";
                        break;

                    default:
                        break;
                }
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();
                    SqlCommand scmd = new SqlCommand("SELECT " + colName + " FROM LeaveStatistics WHERE  EmpId=@empId AND Year = @year", con);
                    scmd.Parameters.AddWithValue("empId", EmpId);
                    scmd.Parameters.AddWithValue("year", startYear);
                    SqlDataReader sreader = scmd.ExecuteReader();

                    if (sreader.HasRows)
                    {
                        while (sreader.Read())
                        {
                            NoLeaves = Convert.ToDouble(sreader[0].ToString());
                        }
                    }
                    con.Close();
                }
                return NoLeaves;
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }

        public static LeaveModel leaveDetailsFetch(int leave_event_id)
        {
            LeaveModel li = null;
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = null;

                da = new SqlDataAdapter("SELECT info.*,type.Type As LeaveTypeStr FROM EmployeeLeaveInfo info INNER JOIN LeaveType type ON info.LeaveType = type.Id WHERE info.[Id] = @leave_event_id", ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("leave_event_id", leave_event_id);
                da.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    li = new LeaveModel();
                    li._lvId = Convert.ToInt32(dt.Rows[0]["Id"].ToString());
                    li.EmpID = Convert.ToInt32(dt.Rows[0]["EmpId"].ToString());
                    li._fromdate = dt.Rows[0]["FromDate"].ToString().Split(' ')[0];
                    li._todate = dt.Rows[0]["ToDate"].ToString().Split(' ')[0];
                    li._leaveType = !string.IsNullOrEmpty(dt.Rows[0]["LeaveType"].ToString()) ? Convert.ToInt32(dt.Rows[0]["LeaveType"].ToString()) : 0;
                    li._leavedurationtype = dt.Rows[0]["DurationType"].ToString();
                    li._leaveDurTypeInt = (li._leavedurationtype.Equals("Full Day")) ? 1 : 2;
                    li._comments = dt.Rows[0]["Comments"].ToString();
                    li._leaveHalfDaySession = Convert.ToInt32(dt.Rows[0]["LeaveSessionType"].ToString());
                    li._strLvType = (!string.IsNullOrEmpty(dt.Rows[0]["LeaveTypeStr"].ToString())) ? dt.Rows[0]["LeaveTypeStr"].ToString().TrimEnd() : "";

                    DateTime fd = Convert.ToDateTime(li._fromdate.ToString());
                    DateTime td = Convert.ToDateTime(li._todate.ToString());

                    switch ((!string.IsNullOrEmpty(dt.Rows[0]["Status"].ToString())) ? Convert.ToInt32(dt.Rows[0]["Status"].ToString()) : 0)
                    {
                        case -1://rejected state
                            li._status = false;
                            li._rejected = true;
                            li._cancelled = false;
                            li._leaveStatus = "REJECTED";
                            break;
                        case 1://pending state
                            li._status = false;
                            li._rejected = false;
                            li._cancelled = false;
                            li._leaveStatus = "PENDING";
                            break;
                        case 2://accepted state    
                            li._status = true;
                            li._rejected = false;
                            li._cancelled = false;
                            li._leaveStatus = "APPROVED";
                            break;
                        case 3: //cancelled state               
                            li._status = false;
                            li._rejected = false;
                            li._cancelled = true;
                            li._leaveStatus = "CANCELLED";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                li = null;
            }
            return li;
        }
        #endregion

        #region Manage calendar operations

        public static DataTable getFilterEventsByEmployee(string start_date, string end_date, int emp_id)
        {
            DataTable dt = new DataTable();
            if (emp_id > 0)
            {
                SqlDataAdapter da = null;
                //string selQuery = "select info.*,calEmp.employee_id,dates.start_date,dates.end_date,eInfo.EmpName,calStatus.status as status_name from calendar_event_info info inner join calendar_event_info_dates dates on info.id = dates.task_id inner join calendar_event_employees calEmp on info.id = calEmp.task_id inner join EmployeeInfo eInfo ON eInfo.EmpId = @employee_id inner join calendar_event_status calStatus on info.status = calStatus.id where info.status = 1  and  calEmp.employee_id = @employee_id and  (start_date <= @start_date and end_date >= @end_date or start_date between @start_date and @end_date or end_date between @start_date and @end_date) order by start_date asc";
                string selQuery = "select DISTINCT info.*,calEmp.employee_id,dates.start_date,dates.end_date,eInfo.EmpFirstname,eInfo.EmpLastname,calStatus.status as status_name,eInfo.EmpPhotoPath,eInfo.EmpGender from calendar_event_info info inner join calendar_event_info_dates dates on info.id = dates.task_id LEFT OUTER JOIN calendar_event_employees calEmp on info.id = calEmp.task_id LEFT OUTER JOIN EmployeeInfo eInfo ON eInfo.EmpId = calEmp.employee_id inner join calendar_event_status calStatus on info.status = calStatus.id where calEmp.task_id IN (select task_id from calendar_event_employees where employee_id = @employee_id) and  (start_date <= @start_date and end_date >= @end_date or start_date between @start_date and @end_date or end_date between @start_date and @end_date) order by start_date asc";
                da = new SqlDataAdapter(selQuery, ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("employee_id", emp_id);
                da.SelectCommand.Parameters.AddWithValue("start_date", start_date);
                da.SelectCommand.Parameters.AddWithValue("end_date", end_date);
                da.Fill(dt);
            }
            return dt;
        }

        public static DataTable getFullEvents(string start_date, string end_date)
        {
            SqlDataAdapter da = null;
            //string selQuery = "select info.*,calEmp.employee_id,dates.start_date,dates.end_date,eInfo.EmpName,calStatus.status as status_name from calendar_event_info info inner join calendar_event_info_dates dates on info.id = dates.task_id inner join calendar_event_employees calEmp on info.id = calEmp.task_id inner join EmployeeInfo eInfo ON eInfo.EmpId = calEmp.employee_id inner join calendar_event_status calStatus on info.status = calStatus.id where info.status = 1 and (start_date <= @start_date and end_date >= @end_date or start_date between @start_date and @end_date or end_date between @start_date and @end_date) order by start_date asc";
            string selQuery = "select DISTINCT info.*,calEmp.employee_id,dates.start_date,dates.end_date,eInfo.EmpFirstname,eInfo.EmpLastname,eInfo.EmpPhotoPath,eInfo.EmpGender,calStatus.status as status_name from calendar_event_info info inner join calendar_event_info_dates dates on info.id = dates.task_id LEFT OUTER JOIN calendar_event_employees calEmp on info.id = calEmp.task_id LEFT OUTER JOIN EmployeeInfo eInfo ON eInfo.EmpId = calEmp.employee_id inner join calendar_event_status calStatus on info.status = calStatus.id where (start_date <= @start_date and end_date >= @end_date or start_date between @start_date and @end_date or end_date between @start_date and @end_date) order by start_date asc";
            da = new SqlDataAdapter(selQuery, ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
            da.SelectCommand.Parameters.AddWithValue("start_date", start_date);
            da.SelectCommand.Parameters.AddWithValue("end_date", end_date);

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static DataTable getEmployeeBirthdayEvents(string st_date, string en_date)
        {
            DataTable dt = getEmployeeFullInfo();
            List<Dictionary<string, object>> rows = null;

            if (dt != null && dt.Rows.Count > 0)
            {
                rows = new List<Dictionary<string, object>>();

                DataColumn Id = dt.Columns.Add("Id", typeof(Int32));
                DataColumn employee_id = dt.Columns.Add("employee_id", typeof(int));
                DataColumn event_type = dt.Columns.Add("event_type", typeof(Int32));
                DataColumn heading = dt.Columns.Add("heading", typeof(String));
                DataColumn note = dt.Columns.Add("note", typeof(String));
                DataColumn status = dt.Columns.Add("status", typeof(Int32));
                DataColumn start_date = dt.Columns.Add("start_date", typeof(String));
                DataColumn end_date = dt.Columns.Add("end_date", typeof(String));

                DataColumn start_time = dt.Columns.Add("start_time", typeof(String));
                DataColumn end_time = dt.Columns.Add("end_time", typeof(String));
                DataColumn duration = dt.Columns.Add("duration", typeof(String));
                DataColumn is_view = dt.Columns.Add("is_view", typeof(bool));
                DataColumn is_edit = dt.Columns.Add("is_edit", typeof(bool));
                DataColumn is_leave_event = dt.Columns.Add("is_leave_event", typeof(bool));

                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime birthday = Convert.ToDateTime(dr["EmpDob"].ToString());
                    var birthday_year = birthday.Year;
                    var birthday_month = birthday.Month;
                    var birthday_day = birthday.Day;

                    DateTime start_dt = Convert.ToDateTime(st_date);
                    DateTime end_dt = Convert.ToDateTime(en_date);

                    var start_year = start_dt.Year;
                    var end_year = end_dt.Year;

                    var flag = false;

                    if (start_year != end_year)
                    {
                        var birthday1 = new DateTime(start_year, birthday_month, birthday_day);
                        var birthday2 = new DateTime(end_year, birthday_month, birthday_day);
                        if (birthday1 >= start_dt && birthday1 <= end_dt)
                        {
                            flag = true;
                            birthday = birthday1;
                        }
                        else if (birthday2 >= start_dt && birthday1 <= end_dt)
                        {
                            flag = true;
                            birthday = birthday2;
                        }
                    }
                    else
                    {
                        birthday = new DateTime(start_year, birthday_month, birthday_day);
                        if (birthday >= start_dt && birthday <= end_dt)
                        {
                            flag = true;
                        }
                    }
                    if (start_year < birthday_year)
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        row = new Dictionary<string, object>();
                        dr["Id"] = 0;
                        dr["employee_id"] = 0;
                        dr["event_type"] = 2;
                        dr["heading"] = "";
                        dr["note"] = "";
                        dr["status"] = 1;
                        dr["start_date"] = birthday.ToString();
                        dr["end_date"] = birthday.ToString();
                        dr["start_time"] = "00:00";
                        dr["end_time"] = "23:59";
                        dr["duration"] = "24";
                        dr["is_view"] = false;
                        dr["is_edit"] = false;
                        dr["is_leave_event"] = false;

                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                    }
                    else
                    {
                        dr.Delete();
                    }
                }

                dt.Columns.Remove("EmpId");
                dt.Columns.Remove("EmpDob");
                dt.AcceptChanges();
            }
            return dt;
        }

        public static DataTable getEmployeeLeaveEvents(LeaveModel lm)
        {
            List<Dictionary<string, object>> rows = null;
            IList<LeaveModel> leaveData = Db.GetEmployeeLeaveDetails(lm.EmpID, 0, lm, false, 0, 0, true);
            DataTable dt = new DataTable();
            if (leaveData != null && leaveData.Count > 0)
            {
                rows = new List<Dictionary<string, object>>();

                DataColumn Id = dt.Columns.Add("Id", typeof(Int32));
                DataColumn employee_id = dt.Columns.Add("employee_id", typeof(int));
                DataColumn event_type = dt.Columns.Add("event_type", typeof(Int32));
                DataColumn heading = dt.Columns.Add("heading", typeof(String));
                DataColumn note = dt.Columns.Add("note", typeof(String));
                DataColumn status = dt.Columns.Add("status", typeof(Int32));
                DataColumn start_date = dt.Columns.Add("start_date", typeof(String));
                DataColumn end_date = dt.Columns.Add("end_date", typeof(String));

                DataColumn start_time = dt.Columns.Add("start_time", typeof(String));
                DataColumn end_time = dt.Columns.Add("end_time", typeof(String));
                DataColumn duration = dt.Columns.Add("duration", typeof(String));
                DataColumn is_view = dt.Columns.Add("is_view", typeof(bool));
                DataColumn is_edit = dt.Columns.Add("is_edit", typeof(bool));
                DataColumn is_leave_event = dt.Columns.Add("is_leave_event", typeof(bool));

                //Additional columns                
                DataColumn EmpPhotoPath = dt.Columns.Add("EmpPhotoPath", typeof(String));
                DataColumn EmpFirstname = dt.Columns.Add("EmpFirstname", typeof(String));
                DataColumn EmpLastname = dt.Columns.Add("EmpLastname", typeof(String));
                DataColumn EmpGender = dt.Columns.Add("EmpGender", typeof(String));

                Dictionary<string, object> row;
                DataRow dr = null;
                foreach (var data in leaveData)
                {
                    row = new Dictionary<string, object>();
                    dr = dt.NewRow();

                    dr["Id"] = data._lvId;
                    dr["employee_id"] = data.EmpID;
                    dr["event_type"] = 3;
                    dr["heading"] = "";
                    dr["note"] = data._comments;
                    dr["status"] = 0;
                    dr["start_date"] = data._fromdate.ToString();
                    dr["end_date"] = data._todate.ToString();
                    dr["start_time"] = "00:00";
                    dr["end_time"] = "23:59";
                    dr["duration"] = "24";
                    dr["is_view"] = true;
                    dr["is_edit"] = true;
                    dr["is_leave_event"] = true;

                    dr["EmpPhotoPath"] = data.UserPhotoPath.ToString();
                    dr["EmpFirstname"] = data.EmpFirstname.ToString();
                    dr["EmpLastname"] = data.EmpLastname.ToString();
                    dr["EmpGender"] = data.Gender.ToString().TrimEnd();

                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    dt.Rows.Add(dr);
                }
                dt.AcceptChanges();
            }

            return dt;
        }

        public static DataTable fetchEventDetailsById(int eventid)
        {
            DataTable dt = null;
            if (eventid > 0)
            {
                SqlDataAdapter da = null;
                string selQuery = "select info.*,dates.start_date,dates.end_date,archive.filename,archive.filepath,archive.date,emp.employee_id,archive.id as archive_id,empInfo.EmpFirstname,empInfo.EmpLastname,empInfo.EmpPhotoPath,empInfo.EmpGender,log.id as log_id,log.created_by as log_employee_id,log.created_at as log_date,log.event_log,log_emp_info.EmpFirstname as log_emp_name,log_emp_info.EmpPhotoPath as log_emp_photo,log_emp_info.EmpGender as log_emp_gender from calendar_event_info info inner join calendar_event_info_dates dates on info.id = dates.task_id LEFT OUTER JOIN calendar_event_archive archive ON info.id = archive.task_id inner join calendar_event_employees emp ON emp.task_id = info.id inner join EmployeeInfo empInfo ON empInfo.EmpId = emp.employee_id LEFT OUTER JOIN calendar_event_log log ON info.id=log.task_id LEFT OUTER JOIN EmployeeInfo log_emp_info ON log.created_by = log_emp_info.EmpId where info.id = @eventid";
                da = new SqlDataAdapter(selQuery, ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("eventid", eventid);

                dt = new DataTable();
                da.Fill(dt);
            }
            return dt;
        }

        public static int taskSave(CalendarEventInfo event_info, CalendarEventLog event_log, LeaveModel lm = null)
        {
            int task_id = 0;
            if (event_info != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand("CreateCalendarTask", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@event_type", event_info.event_type);
                        cmd.Parameters.AddWithValue("@heading", event_info.heading);
                        cmd.Parameters.AddWithValue("@note", event_info.note);
                        cmd.Parameters.AddWithValue("@status", event_info.status);
                        cmd.Parameters.AddWithValue("@task_id", 0);
                        cmd.Parameters.AddWithValue("@start_date", event_info.event_dates[0].start_date);
                        cmd.Parameters.AddWithValue("@end_date", event_info.event_dates[0].end_date);
                        cmd.Parameters.AddWithValue("@employee_id", string.Join(",", event_info.employee.Select(n => n.ToString()).ToArray()));
                        cmd.Parameters.AddWithValue("@isLeaveTask", false);
                        cmd.Parameters.AddWithValue("@leaveId", 0);

                        cmd.Parameters["@task_id"].Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        task_id = Convert.ToInt32(cmd.Parameters["@task_id"].Value.ToString());
                        if (task_id > 0)
                        {
                            event_log.Id = task_id;
                            Db.createTaskLog(event_log);
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return task_id;
        }

        public static int createTaskLog(CalendarEventLog event_log)
        {
            int event_log_id = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO calendar_event_log VALUES(@task_id,@created_by,@created_at,@event_log)", con);
                    cmd.Parameters.AddWithValue("task_id", event_log.Id);
                    cmd.Parameters.AddWithValue("created_by", event_log.creator_employee.EmpID);
                    cmd.Parameters.AddWithValue("created_at", DateTime.Now.ToString());
                    cmd.Parameters.AddWithValue("event_log", event_log.event_log);
                    event_log_id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return event_log_id;
        }

        public static CalendarEventArchive getArchiveData(int arcive_id)
        {
            CalendarEventArchive archive = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM calendar_event_archive WHERE Id = @archive_id", con);
                    cmd.Parameters.AddWithValue("archive_id", arcive_id);
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        archive = new CalendarEventArchive();
                        archive.archive_id = arcive_id;
                        archive.filename = rd["filename"].ToString();
                        archive.filepath = rd["filepath"].ToString();
                        archive.date = rd["date"].ToString();
                        archive.Id = Convert.ToInt32(rd["task_id"].ToString());
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return archive;
        }

        public static int taskEdit(CalendarEventInfo event_info, CalendarEventLog event_log, LeaveModel lm = null)
        {
            int task_id = 0;
            if (event_info != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand("EditCalendarTask", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@task_id", event_info.Id);
                        cmd.Parameters.AddWithValue("@heading", event_info.heading);
                        cmd.Parameters.AddWithValue("@note", event_info.note);
                        cmd.Parameters.AddWithValue("@status", event_info.status);
                        cmd.Parameters.AddWithValue("@start_date", event_info.event_dates[0].start_date);
                        cmd.Parameters.AddWithValue("@end_date", event_info.event_dates[0].end_date);
                        cmd.Parameters.AddWithValue("@employee_id", string.Join(",", event_info.employee.Select(n => n.ToString()).ToArray()));

                        cmd.ExecuteNonQuery();
                        task_id = event_info.Id;
                        if (task_id > 0)
                        {
                            Db.createTaskLog(event_log);
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return task_id;
        }

        public static int taskRemove(int task_id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("RemoveCalendarTask", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@task_id", task_id);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return task_id;
        }

        internal static int removeTaskArchive(int archive_id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM calendar_event_archive WHERE Id = @archive_id", con);
                    cmd.Parameters.AddWithValue("@archive_id", archive_id);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return archive_id;
        }

        public static string taskFilesSave(List<CalendarEventArchive> uploaded_files)
        {
            string return_str = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = null;
                    foreach (var item in uploaded_files)
                    {
                        string query = "INSERT INTO calendar_event_archive VALUES(@filename,@filepath,@date,@task_id)";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("filepath", item.filename);
                        cmd.Parameters.AddWithValue("filename", item.filepath);
                        cmd.Parameters.AddWithValue("date", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("task_id", item.Id);
                        cmd.ExecuteNonQuery();
                        return_str = "OK";

                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return return_str;
        }

        public static IList<CalendarEventTreeItems> getTreeViewItems(int task_id)
        {
            DataTable dt = new DataTable();
            List<CalendarEventTreeItems> treeItems = new List<CalendarEventTreeItems>();
            SqlDataAdapter da = null;
            try
            {
                string selQuery = "SELECT * FROM calendar_event_treeitems WHERE task_id = @task_id";
                da = new SqlDataAdapter(selQuery, ConfigurationManager.ConnectionStrings["hrmscon"].ConnectionString);
                da.SelectCommand.Parameters.AddWithValue("task_id", task_id);
                da.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    treeItems = dt.AsEnumerable().Select(row => new CalendarEventTreeItems
                    {
                        id = row.Field<int?>("Id").GetValueOrDefault(),
                        name = row.Field<string>("name"),
                        text = row.Field<string>("text"),
                        parentid = row.Field<int?>("parent_id").GetValueOrDefault(),
                        node_type = row.Field<string>("node_type"),
                        type = (row.Field<string>("node_type").ToString().Equals("folder")) ? "root" : "child",
                        is_edit = (row.Field<string>("node_type").ToString().Equals("folder")) ? false : true
                    }).ToList();
                    var tree = BuildTree(treeItems);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return treeItems;
        }

        static List<CalendarEventTreeItems> BuildTree(this List<CalendarEventTreeItems> items)
        {
            items.ForEach(i => i.children = items.Where(ch => ch.parentid == i.id).ToList());
            items.RemoveAll(i => i.parentid > 0);
            return items.Where(i => i.parentid == 0).ToList();
        }

        #endregion
    }
}