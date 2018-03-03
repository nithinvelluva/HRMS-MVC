using HrmsMvc.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace HrmsMvc.Controllers
{
    [RequireHttps]
    public class UserController : Controller
    {
        #region Manage User

        // GET: UserProfile
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult UserProfile()
        {
            if (Session["USER"] != null)
            {
                int EmpID = (Session["USER"] as EmployeeModel).EmpID;
                int UserType = (Session["USER"] as EmployeeModel).Usertype;

                ViewBag.EmpId = EmpID;
                ViewBag.UserType = UserType;

                if (UserType == 1)
                {
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    EmployeeModel em = new EmployeeModel();
                    DataTable dt = Db.GetEmployeeInfo(EmpID, null);

                    if (dt != null && dt.Rows.Count > 0)
                    {
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
                        }
                    }
                    ViewBag.EmpName = em.EmpFirstname + " " + em.EmpLastname;
                    ViewBag.Gender = em.Gender;
                    ViewBag.EmpEmail = em.EmailId;
                    ViewBag.UserPhotoPath = em.UserPhotoPath;

                    return View(em);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: ChangePassword
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ChangePassword()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype,
                    EmpFirstname = (Session["USER"] as EmployeeModel).EmpFirstname.ToString().TrimEnd(),
                    EmpLastname = (Session["USER"] as EmployeeModel).EmpLastname.ToString().TrimEnd()
                };

                ViewBag.EmpId = em.EmpID;
                ViewBag.UserType = em.Usertype;

                if (em.Usertype == 1)
                {
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    return View(em);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult getEmployeeInfo(string empId)
        {
            int EmpID = !string.IsNullOrEmpty(empId) ? Convert.ToInt32(empId) : 0;
            EmployeeModel em = null;
            if (EmpID > 0)
            {
                em = Helpers.Helper.getEmployeeInfoData(EmpID);
            }
            return Json(new { data = em, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ProfileDetails(string Id, string firstName, string lastName, string dob, string gender, string role, string username, string cnfPwd, string email, string phone)
        {
            Regex RegexEmail = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            Regex mobRgx = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");

            EmployeeModel em = null;
            string errStr = string.Empty;
            if (string.IsNullOrEmpty(firstName))
            {
                errStr = errStr + "1:";
            }
            if (string.IsNullOrEmpty(lastName))
            {
                errStr = errStr + "2:";
            }
            if (string.IsNullOrEmpty(gender))
            {
                errStr = errStr + "3:";
            }
            if (string.IsNullOrEmpty(dob))
            {
                errStr = errStr + "4:";
            }
            if (string.IsNullOrEmpty(username))
            {
                errStr = errStr + "5:";
            }
            if (string.IsNullOrEmpty(cnfPwd))
            {
                errStr = errStr + "7:";
            }
            if (!string.IsNullOrEmpty(dob))
            {
                TimeSpan ts = new TimeSpan();
                DateTime db = Convert.ToDateTime(dob);
                ts = DateTime.Now - db;
                if (ts.TotalDays < 6570)
                {
                    errStr = errStr + "9:";
                }
            }

            if (string.IsNullOrEmpty(email))
            {
                errStr = errStr + "EN:";
            }
            else if (!RegexEmail.IsMatch(email))
            {
                errStr = errStr + "ENV:";
            }

            if (string.IsNullOrEmpty(phone))
            {
                errStr = errStr + "PN:";
            }
            if (!string.IsNullOrEmpty(phone) && !mobRgx.IsMatch(phone))
            {
                errStr = errStr + "PNV:";
            }

            if (string.IsNullOrEmpty(errStr))
            {
                em = new EmployeeModel();
                em.EmpID = (!string.IsNullOrEmpty(Id)) ? Convert.ToInt32(Id) : 0;
                em.EmpFirstname = firstName;
                em.EmpLastname = lastName;
                em.Gender = gender;
                em.UserName = username;
                em.Password = cnfPwd;
                em.PhoneNumber = "";
                em.EmailId = "";
                em.Usertype = Convert.ToInt32(role);
                em.DateOfBirth = dob;
                em.PhoneNumber = phone;
                em.EmailId = email;
                em.UserPhotoPath = "";

                if (Convert.ToInt32(Id) > 0)
                {
                    errStr = Db.UpdateProfile(em);
                    em.ErrorString = string.IsNullOrEmpty(errStr) ? "" : errStr;
                    return Json(new { data = em, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
                }
            }
            em = new EmployeeModel();
            em.ErrorString = errStr;
            return Json(new { data = em, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangeUserPassword(string empId, string currPwd, string nwPwd, string nwPwdCnfm)
        {
            string rtrnStr = "";
            try
            {
                int empid = !string.IsNullOrEmpty(empId) ? Convert.ToInt32(empId) : 0;
                if (empid <= 0)
                {
                    rtrnStr = "ERROR:";
                }

                if (string.IsNullOrEmpty(currPwd))
                {
                    rtrnStr = rtrnStr + "CP:";
                }
                if (string.IsNullOrEmpty(nwPwd))
                {
                    rtrnStr = rtrnStr + "NP:";
                }
                if (string.IsNullOrEmpty(nwPwdCnfm))
                {
                    rtrnStr = rtrnStr + "NPC:";
                }

                if (!string.IsNullOrEmpty(nwPwd) && !string.IsNullOrEmpty(nwPwdCnfm) && nwPwd != nwPwdCnfm)
                {
                    rtrnStr = rtrnStr + "NPM:";
                }

                if (string.IsNullOrEmpty(rtrnStr))
                {
                    rtrnStr = Db.UpdateUserPassword(empid, currPwd, nwPwd);
                    Session.RemoveAll();
                }
            }
            catch (Exception ex)
            {
                rtrnStr = null;
            }
            return Json(new { data = rtrnStr, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        // GET: CaptureUserPhoto
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult CaptureUserPhoto()
        {
            return PartialView("CapturePhoto");
        }

        #endregion

        #region Manage Attendance

        // GET: Attendance
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Attendance()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype,
                    EmpFirstname = (Session["USER"] as EmployeeModel).EmpFirstname,
                    EmpLastname = (Session["USER"] as EmployeeModel).EmpLastname
                };

                ViewBag.EmpId = em.EmpID;
                ViewBag.UserType = em.Usertype;

                if (em.Usertype == 1)
                {
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    return View(em);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public JsonResult GetEmpPunchDetails(string empId, int timeoffset)
        {
            int EmpID = (!string.IsNullOrEmpty(empId)) ? Convert.ToInt32(empId) : 0;
            AttendanceModel am = null;
            if (EmpID > 0)
            {
                am = Helpers.Helper.convertDateTimeFormat(Db.GetEmployeeAttendanceDetails(EmpID));
            }
            return Json(new { data = am }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddAttendance(string EmpId, string punchInTime, string punchOutTime, string notes, int type, int timeoffset)
        {
            int EmpID = (!string.IsNullOrEmpty(EmpId)) ? Convert.ToInt32(EmpId) : 0;
            bool flag = false;
            AttendanceModel am = null;
            if (EmpID > 0)
            {
                if (type == 1) //punchin
                {
                    am = new AttendanceModel();
                    am.EmpID = EmpID;
                    am.PunchinTime = DateTime.UtcNow.ToString();
                    am.Notes = !string.IsNullOrEmpty(notes) ? notes : "";
                    flag = Db.AddAttendance(am, 1);
                }
                else if (type == 2) //punchout
                {
                    am = new AttendanceModel();
                    am.EmpID = EmpID;
                    am.PunchinTime = Db.GetEmployeeAttendanceDetails(EmpID).PunchinTime;
                    am.PunchoutTime = DateTime.UtcNow.ToString();
                    am.Notes = !string.IsNullOrEmpty(notes) ? notes : "";
                    flag = Db.AddAttendance(am, 2);
                }

                if (flag)
                {
                    am = Helpers.Helper.convertDateTimeFormat(am);
                    am.ErrorString = "";
                }
                else
                {
                    am.ErrorString = "ERROR";
                }
            }
            return Json(new { data = am }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SearchPunchDetails(string EmpId, string StartDate, string EndDate, int timeoffset)
        {
            try
            {
                int EmpID = (!string.IsNullOrEmpty(EmpId)) ? Convert.ToInt32(EmpId) : 0;
                if (EmpID > 0)
                {
                    var draw = Request.Form.GetValues("draw").FirstOrDefault();
                    var start = Request.Form.GetValues("start").FirstOrDefault();
                    var length = Request.Form.GetValues("length").FirstOrDefault();
                    string sortColumn = Request.Form.GetValues("order[0][column]")[0];
                    string sortColumnDir = Request.Form.GetValues("order[0][dir]")[0];

                    int pageSize = length != null ? Convert.ToInt32(length) : 0;
                    int skip = start != null ? Convert.ToInt32(start) : 0;
                    int totalRecords = 0;

                    List<AttendanceModel> attList = Db.GetEmpPunchDetails(EmpID, StartDate, EndDate, false, skip, pageSize);
                    if (attList != null && attList.Count > 0)
                    {
                        totalRecords = attList[0].RowCount;

                        //// Sorting.
                        //attList = SortByColumnWithOrder(sortColumn, sortColumnDir, attList);

                        //// Filter record count.
                        //int recFilter = attList.Count;

                        //var data = attList.Skip(skip).Take(pageSize).ToList();
                        var data = attList.ToList();

                        return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = new List<AttendanceModel>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = new List<AttendanceModel>() }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Manage Leaves

        // GET: Leaves
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Leaves()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype,
                    EmpFirstname = (Session["USER"] as EmployeeModel).EmpFirstname,
                    EmpLastname = (Session["USER"] as EmployeeModel).EmpLastname
                };

                ViewBag.EmpId = em.EmpID;
                ViewBag.UserType = em.Usertype;

                if (em.Usertype == 1)
                {
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    ViewBag.LeaveTypes = new SelectList(Db.GetLeaveTypes(), "Value", "Text", "");
                    ViewBag.LeaveDurTypes = new SelectList(new SelectListItem[]
                    {
                        new SelectListItem { Text = "Full Day", Value = "1"},
                        new SelectListItem { Text = "Half Day", Value = "2"}
                    }, "Value", "Text", "");

                    return View("LeaveDetails", em);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Leavereports
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult LeaveReports()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype,
                    EmpFirstname = (Session["USER"] as EmployeeModel).EmpFirstname,
                    EmpLastname = (Session["USER"] as EmployeeModel).EmpLastname,
                };

                ViewBag.EmpId = em.EmpID;
                ViewBag.UserType = em.Usertype;

                if (em.Usertype == 1)
                {
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    ViewBag.LeaveTypes = new SelectList(Db.GetLeaveTypes(), "Value", "Text", "");
                    ViewBag.LeaveDurTypes = new SelectList(new SelectListItem[]
                    {
                        new SelectListItem { Text = "Full Day", Value = "1"},
                        new SelectListItem { Text = "Half Day", Value = "2"}
                    }, "Value", "Text", "");
                    return View(em);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetLeaveDetails(string EmpId, string UserType, string month, string year)
        {
            try
            {
                string rtrnStr = string.Empty;

                int empId = (!string.IsNullOrEmpty(EmpId)) ? Convert.ToInt32(EmpId) : 0;
                int userType = (!string.IsNullOrEmpty(UserType)) ? Convert.ToInt32(UserType) : 0;

                if (empId > 0 && userType > 0)
                {
                    LeaveModel lm = new LeaveModel();
                    var strtDate = Helpers.Helper.GetFirstDayOfMonth(Convert.ToInt32(month), Convert.ToInt32(year));
                    lm._fromdate = strtDate.ToString();
                    lm._todate = strtDate.AddMonths(1).AddDays(-1).ToString();

                    var draw = Request.Form.GetValues("draw").FirstOrDefault();
                    var start = Request.Form.GetValues("start").FirstOrDefault();
                    var length = Request.Form.GetValues("length").FirstOrDefault();

                    //Get Sort columns value
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                    int pageSize = length != null ? Convert.ToInt32(length) : 0;
                    int skip = start != null ? Convert.ToInt32(start) : 0;
                    int totalRecords = 0;

                    List<LeaveModel> lvList = Db.GetEmployeeLeaveDetails(empId, userType, lm, false, skip, pageSize);
                    if (lvList != null && lvList.Count > 0)
                    {
                        totalRecords = lvList[0].RowCount;
                        var data = lvList.ToList();
                        return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = new List<LeaveModel>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = new List<LeaveModel>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetLeaveStatistics(string EmpId)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            try
            {
                int EmpID = (!string.IsNullOrEmpty(EmpId)) ? Convert.ToInt32(EmpId) : 0;
                if (EmpID > 0)
                {
                    DataTable dt = Db.GetLeaveStatistics(EmpID);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        Dictionary<string, object> row;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                            rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { data = rows }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddLeave(LeaveModel leave_model)
        {
            string rtrnStr = string.Empty;
            try
            {
                int empId = leave_model.EmpID;
                if (empId > 0)
                {
                    if (leave_model._leaveType <= 0)
                    {
                        rtrnStr = rtrnStr + "1";
                    }

                    if (string.IsNullOrEmpty(leave_model._fromdate))
                    {
                        rtrnStr = rtrnStr + "2";
                    }
                    if (string.IsNullOrEmpty(leave_model._todate))
                    {
                        rtrnStr = rtrnStr + "3";
                    }

                    if (!string.IsNullOrEmpty(leave_model._fromdate) && (!string.IsNullOrEmpty(leave_model._todate)))
                    {
                        DateTime frmdt = Convert.ToDateTime(leave_model._fromdate);
                        DateTime todt = Convert.ToDateTime(leave_model._todate);

                        if (todt < frmdt)
                        {
                            rtrnStr = rtrnStr + "4";
                        }
                    }

                    if (leave_model._leaveDurTypeInt <= 0)
                    {
                        rtrnStr = rtrnStr + "5";
                    }

                    if (string.IsNullOrEmpty(rtrnStr))
                    {
                        DateTime frmdt = Convert.ToDateTime(leave_model._fromdate);
                        DateTime todt = Convert.ToDateTime(leave_model._todate);
                        string LeaveTypeStr = leave_model._strLvType.TrimEnd();
                        bool flag = false;
                        ArrayList rtrnArr = new ArrayList();

                        rtrnArr = Db.CalculateLeaveStatistics(frmdt, todt, LeaveTypeStr, leave_model._leavedurationtype, empId);
                        if (rtrnArr != null)
                        {
                            flag = Convert.ToBoolean(rtrnArr[2]);
                        }

                        if (!flag)
                        {
                            rtrnStr = rtrnStr + "7"; //"No enough leaves"
                        }
                        else
                        {
                            LeaveModel lm = new LeaveModel();
                            lm = leave_model;
                            lm._status = false;
                            lm._rejected = false;
                            lm._strLvType = LeaveTypeStr;
                            lm.RtrnArry = rtrnArr;

                            if (leave_model._lvId <= 0)//Add leave
                            {
                                GenericCallbackModel gcm = new GenericCallbackModel();
                                gcm = Db.AddLeave(lm);
                                rtrnStr = (gcm != null) ? gcm.Message : null;
                                lm._lvId = (gcm != null) ? gcm.ID : 0;
                                if (lm._lvId > 0)
                                {
                                    AddLeaveCalendarEntry(lm);
                                }
                            }
                            else//Edit leave
                            {
                                GenericCallbackModel gcm = new GenericCallbackModel();
                                gcm = Db.UpdateLeave(lm, leave_model.Usertype);
                                rtrnStr = (gcm != null) ? gcm.Message : null;
                                lm._calendarEntryId = (gcm != null) ? gcm.SecID : 0;
                                if (lm._calendarEntryId > 0)
                                {
                                    AddLeaveCalendarEntry(lm);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rtrnStr = null;
            }
            return Json(new { data = rtrnStr, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        private int AddLeaveCalendarEntry(LeaveModel lm)
        {
            int row_id = 0;
            if (lm != null)
            {
                CalendarEventInfo event_info = new CalendarEventInfo();
                event_info.event_type = 1;
                event_info.status = 1;
                List<CalendarEventDate> event_dates = new List<CalendarEventDate>();
                event_dates.Add(
                    new CalendarEventDate
                    {
                        start_date = lm._fromdate,
                        end_date = lm._todate
                    });
                switch (lm._leavedurationtype)
                {
                    case "Full Day":
                        event_dates[0].start_date = event_dates[0].start_date + " " + "09:00:00";
                        event_dates[0].end_date = event_dates[0].end_date + " " + "18:00:00";
                        break;
                    case "Half Day":
                        switch (lm._leaveHalfDaySession)
                        {
                            case 1://morning session
                                event_dates[0].start_date = event_dates[0].start_date + " " + "09:00:00";
                                event_dates[0].end_date = event_dates[0].end_date + " " + "13:00:00";
                                break;
                            case 2://afternoon session
                                event_dates[0].start_date = event_dates[0].start_date + " " + "14:00:00";
                                event_dates[0].end_date = event_dates[0].end_date + " " + "18:00:00";
                                break;
                            default:
                                event_dates[0].start_date = event_dates[0].start_date + " " + "09:00:00";
                                event_dates[0].end_date = event_dates[0].end_date + " " + "13:00:00";
                                break;
                        }
                        break;
                    default:
                        event_dates[0].start_date = event_dates[0].start_date + " " + "09:00:00";
                        event_dates[0].end_date = event_dates[0].end_date + " " + "18:00:00";
                        break;
                }
                event_info.event_dates = event_dates;
                List<int> employee = new List<int>();
                employee.Add(lm.EmpID);
                event_info.employee = employee;
                event_info.heading = "Applied for leave";
                event_info.note = "";

                CalendarEventLog event_log = new CalendarEventLog()
                {
                    creator_employee = new EmployeeModel()
                    {
                        EmpID = lm.EmpID
                    },
                    event_log = "Leave added."
                };
                if(lm._lvId <= 0)
                {
                    row_id = Db.taskSave(event_info, event_log, lm);
                }
                else
                {
                    event_info.Id = lm._calendarEntryId;
                    row_id = Db.taskEdit(event_info, event_log, lm);
                }
            }
            return row_id;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SentQuery(string SenterMail, string emailSubject, string emailBody)
        {
            string rtrnStr = null;
            try
            {
                Helpers.Helper.sentEmail("Query from " + SenterMail + " : " + emailSubject, emailBody, "tnoreply001@gmail.com");
                rtrnStr = "OK";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { data = rtrnStr }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Manage UserReports

        // GET: Employee reports
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Reports()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype,
                    EmpFirstname = (Session["USER"] as EmployeeModel).EmpFirstname,
                    EmpLastname = (Session["USER"] as EmployeeModel).EmpLastname
                };

                ViewBag.EmpId = em.EmpID;
                ViewBag.UserType = em.Usertype;

                if (em.Usertype == 1)
                {
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    ViewBag.YearList = new SelectList(new SelectListItem[]
                   {
                        new SelectListItem { Text = DateTime.Now.AddYears(-5).Year.ToString(), Value = DateTime.Now.AddYears(-5).Year.ToString()},
                        new SelectListItem { Text = DateTime.Now.AddYears(-4).Year.ToString(), Value = DateTime.Now.AddYears(-4).Year.ToString()},
                        new SelectListItem { Text = DateTime.Now.AddYears(-3).Year.ToString(), Value =  DateTime.Now.AddYears(-3).Year.ToString()},
                        new SelectListItem { Text = DateTime.Now.AddYears(-2).Year.ToString(), Value =  DateTime.Now.AddYears(-2).Year.ToString()},
                        new SelectListItem { Text = DateTime.Now.AddYears(-1).Year.ToString(), Value =  DateTime.Now.AddYears(-1).Year.ToString()},
                        new SelectListItem { Text = DateTime.Now.AddYears(0).Year.ToString(), Value =  DateTime.Now.AddYears(0).Year.ToString()}

                   }, "Value", "Text", "");

                    List<string> monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames.ToList();
                    monthNames.Remove("");

                    ViewBag.MonthList = new SelectList(new SelectListItem[]
                    {
                        new SelectListItem { Text = monthNames[0], Value = "1"},
                        new SelectListItem { Text = monthNames[1], Value = "2"},
                        new SelectListItem { Text = monthNames[2], Value = "3"},
                        new SelectListItem { Text = monthNames[3], Value = "4"},
                        new SelectListItem { Text = monthNames[4], Value = "5"},
                        new SelectListItem { Text = monthNames[5], Value = "6"},
                        new SelectListItem { Text = monthNames[6], Value = "7"},
                        new SelectListItem { Text = monthNames[7], Value = "8"},
                        new SelectListItem { Text = monthNames[8], Value = "9"},
                        new SelectListItem { Text = monthNames[9], Value = "10"},
                        new SelectListItem { Text = monthNames[10], Value = "11"},
                        new SelectListItem { Text = monthNames[11], Value = "12"}
                    }, "Value", "Text", "");

                    return View("EmployeeReports", em);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        //GET:GetUserReport
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetUserReport(int EmpId, string year, string month)
        {
            UserReportModel um = null;
            try
            {
                int Year = (!string.IsNullOrEmpty(year)) ? Convert.ToInt32(year) : 0;
                int Month = (!string.IsNullOrEmpty(month)) ? Convert.ToInt32(month) : 0;
                string rtrnStr = string.Empty;

                if (Year > 0 && Month > 0)
                {
                    var startDate = Helpers.Helper.GetFirstDayOfMonth(Month, Year);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    LeaveModel lm = new LeaveModel();
                    lm._fromdate = startDate.ToString();
                    lm._todate = endDate.ToString();

                    List<AttendanceModel> punchList = Db.GetEmpPunchDetails(EmpId, startDate.ToString(), endDate.ToString(), true);
                    List<LeaveModel> lvList = Db.GetEmployeeLeaveDetails(EmpId, 2, lm, true);

                    if (lvList != null && punchList != null)
                    {
                        if (lvList.Count <= 0 && punchList.Count <= 0)
                        {
                            rtrnStr = null;
                        }
                        else
                        {
                            double lvcount = 0.0; double attCnt = 0.0;
                            List<int> yearLst;
                            List<double> lvLst;
                            foreach (var item in lvList)
                            {
                                yearLst = (List<int>)item.RtrnArry[0];
                                lvLst = (List<double>)item.RtrnArry[1];

                                foreach (int yearItem in yearLst)
                                {
                                    if (yearItem == startDate.Year)
                                    {
                                        lvcount = lvcount + lvLst[yearLst.IndexOf(yearItem)];
                                    }
                                }
                            }

                            foreach (var item in punchList)
                            {
                                attCnt = attCnt + Convert.ToDouble(item.Duration);
                            }

                            um = new UserReportModel();
                            um.TotalDays = DateTime.DaysInMonth(Year, Month);
                            um.Holidays = um.TotalDays - Helpers.Helper.GetBusinessDaysCount(startDate, endDate);

                            um.WorkingHours = Math.Round(attCnt, 2);
                            um.ActiveDays = Math.Round(attCnt / 8, 2);

                            um.LeaveDays = lvcount;
                            um.WorkingDays = um.TotalDays - um.Holidays;
                            um.Month = month;
                            um.Year = year;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(new { data = um }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}