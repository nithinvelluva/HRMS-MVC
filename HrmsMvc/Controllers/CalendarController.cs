using HrmsMvc.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HrmsMvc.Controllers
{
    public class CalendarController : Controller
    {
        //GET calendar
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Calendar()
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

                return View("calendarView", em);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: getEvents
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult getEvents(string _empId, string _start_date, string _end_date, string event_filter = null)
        {
            if (Session["USER"] != null)
            {
                int emp_id = (!string.IsNullOrEmpty(_empId) && !string.IsNullOrWhiteSpace(_empId)) ? Convert.ToInt32(_empId) : 0;
                int event_filter_id = (!string.IsNullOrEmpty(event_filter) && !string.IsNullOrWhiteSpace(event_filter)) ? Convert.ToInt32(event_filter) : 0;
                EmployeeModel em = new EmployeeModel()
                {
                    Usertype = (Session["USER"] as EmployeeModel).Usertype,
                    EmpFirstname = (Session["USER"] as EmployeeModel).EmpFirstname.ToString().TrimEnd(),
                    EmpLastname = (Session["USER"] as EmployeeModel).EmpLastname.ToString().TrimEnd()
                };

                _start_date = _start_date + " " + "00:00:00";
                _end_date = _end_date + " " + "23:59:58";

                DataTable events_dt = new DataTable();
                DataTable birthday_events = new DataTable();
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row = null;

                if (event_filter_id == 1 || event_filter_id == 0)
                {
                    if (emp_id > 0)
                    {
                        events_dt = Db.getFilterEventsByEmployee(_start_date, _end_date, emp_id);
                    }
                    else
                    {
                        events_dt = Db.getFullEvents(_start_date, _end_date);
                    }

                    if (events_dt != null && events_dt.Rows.Count > 0)
                    {
                        DataColumn start_time = events_dt.Columns.Add("start_time", typeof(String));
                        DataColumn end_time = events_dt.Columns.Add("end_time", typeof(String));
                        DataColumn duration = events_dt.Columns.Add("duration", typeof(String));
                        DataColumn is_view = events_dt.Columns.Add("is_view", typeof(bool));
                        DataColumn is_edit = events_dt.Columns.Add("is_edit", typeof(bool));

                        foreach (DataRow dr in events_dt.Rows)
                        {
                            row = new Dictionary<string, object>();
                            dr["is_view"] = true;
                            dr["is_edit"] = true;
                            dr["start_time"] = (dr["start_date"].ToString()).Split(' ')[1];
                            dr["end_time"] = (dr["end_date"].ToString()).Split(' ')[1];
                            dr["duration"] = DateTime.Parse(dr["end_time"].ToString()).Subtract(DateTime.Parse(dr["start_time"].ToString()));
                            foreach (DataColumn col in events_dt.Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                        }
                    }
                    else
                    {
                        events_dt = new DataTable();
                        DataColumn Id = events_dt.Columns.Add("Id", typeof(Int32));
                        DataColumn employee_id = events_dt.Columns.Add("employee_id", typeof(int));
                        DataColumn event_type = events_dt.Columns.Add("event_type", typeof(Int32));
                        DataColumn heading = events_dt.Columns.Add("heading", typeof(String));
                        DataColumn note = events_dt.Columns.Add("note", typeof(String));
                        DataColumn status = events_dt.Columns.Add("status", typeof(Int32));
                        DataColumn start_date = events_dt.Columns.Add("start_date", typeof(String));
                        DataColumn end_date = events_dt.Columns.Add("end_date", typeof(String));

                        DataColumn start_time = events_dt.Columns.Add("start_time", typeof(String));
                        DataColumn end_time = events_dt.Columns.Add("end_time", typeof(String));
                        DataColumn duration = events_dt.Columns.Add("duration", typeof(String));
                        DataColumn is_view = events_dt.Columns.Add("is_view", typeof(bool));
                        DataColumn is_edit = events_dt.Columns.Add("is_edit", typeof(bool));

                    }
                }
                if (event_filter_id == 2 || event_filter_id == 0)
                {
                    birthday_events = Db.getEmployeeBirthdayEvents(_start_date, _end_date);
                }

                row = null;
                List<EmployeeModel> employees = null;
                DataTable filter_events_dt = (events_dt != null && events_dt.Rows.Count > 0) ? events_dt.AsEnumerable().GroupBy(r => r.Field<int>("Id")).Select(g => g.First()).CopyToDataTable() : null;
                foreach (DataRow dr in events_dt.Rows)
                {
                    DataRow[] emp_dr = filter_events_dt.Select("Id = " + Convert.ToInt32(dr["Id"].ToString()));
                    row = new Dictionary<string, object>();
                    if (emp_dr != null && emp_dr[0] != null)
                    {
                        var event_rows = events_dt.AsEnumerable().
                                         Where(x => x.Field<int>("Id") == Convert.ToInt32(dr["Id"].ToString()));
                        employees = new List<EmployeeModel>();
                        foreach (var event_row in event_rows)
                        {
                            if (!string.IsNullOrWhiteSpace(dr["employee_id"].ToString()))
                            {
                                employees.Add(new EmployeeModel()
                                {
                                    EmpID = Convert.ToInt32(event_row["employee_id"].ToString()),
                                    UserPhotoPath = event_row["EmpPhotoPath"].ToString(),
                                    EmpFirstname = event_row["EmpFirstname"].ToString(),
                                    EmpLastname = event_row["EmpLastname"].ToString(),
                                    Gender = event_row["EmpGender"].ToString()
                                });
                            }
                        }
                    }
                    foreach (DataColumn col in events_dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    row.Add("employees", (employees != null) ? employees.GroupBy(x => x.EmpID).Select(y => y.First()) : null);
                    row.Remove("employee_id");
                    row.Remove("EmpPhotoPath");
                    row.Remove("EmpFirstname");
                    row.Remove("EmpLastname");
                    row.Remove("EmpGender");
                    bool has = rows.Any(event_item => Convert.ToInt32(event_item["Id"].ToString()) == Convert.ToInt32(dr["Id"].ToString()));
                    if (!has)
                    {
                        rows.Add(row);
                    }
                }

                #region Birthday events
                foreach (DataRow dr in birthday_events.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in birthday_events.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }

                    if (!string.IsNullOrWhiteSpace(dr["employee_id"].ToString()))
                    {
                        employees = new List<EmployeeModel>();
                        employees.Add(new EmployeeModel()
                        {
                            EmpID = Convert.ToInt32(dr["employee_id"].ToString()),
                            UserPhotoPath = dr["EmpPhotoPath"].ToString(),
                            EmpFirstname = dr["EmpFirstname"].ToString(),
                            EmpLastname = dr["EmpLastname"].ToString(),
                            Gender = dr["EmpGender"].ToString()
                        });
                    }
                    row.Add("employees", employees.GroupBy(x => x.EmpID).Select(y => y.First()));
                    row.Remove("employee_id");
                    row.Remove("EmpPhotoPath");
                    row.Remove("EmpFirstname");
                    row.Remove("EmpLastname");
                    row.Remove("EmpGender");
                    rows.Add(row);
                }
                #endregion

                return Json(new { data = rows }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        //GET:eventDetailsFetch
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult eventDetailsFetch(string eventid)
        {
            //if (Session["USER"] != null)
            {
                int event_id = (!string.IsNullOrEmpty(eventid) && !string.IsNullOrWhiteSpace(eventid)) ? Convert.ToInt32(eventid) : 0;
                DataTable dt = Db.fetchEventDetailsById(event_id);
                Dictionary<string, object> event_item = null;

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<EmployeeModel> employees = new List<EmployeeModel>();
                    List<CalendarEventArchive> archive = new List<CalendarEventArchive>();
                    List<CalendarEventLog> event_log = new List<CalendarEventLog>();
                    event_item = new Dictionary<string, object>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(dr["employee_id"].ToString()))
                        {
                            employees.Add(new EmployeeModel()
                            {
                                EmpID = (!string.IsNullOrWhiteSpace(dr["employee_id"].ToString())) ? Convert.ToInt32(dr["employee_id"].ToString()) : 0,
                                UserPhotoPath = dr["EmpPhotoPath"].ToString(),
                                EmpFirstname = dr["EmpFirstname"].ToString(),
                                EmpLastname = dr["EmpLastname"].ToString(),
                                Gender = dr["EmpGender"].ToString()
                            });
                        }
                        if (!string.IsNullOrWhiteSpace(dr["archive_id"].ToString()))
                        {
                            archive.Add(new CalendarEventArchive()
                            {
                                archive_id = (!string.IsNullOrWhiteSpace(dr["archive_id"].ToString())) ? Convert.ToInt32(dr["archive_id"].ToString()) : 0,
                                filename = dr["filename"].ToString(),
                                filepath = dr["filepath"].ToString(),
                                date = dr["filepath"].ToString()
                            });
                        }
                        if (!string.IsNullOrWhiteSpace(dr["log_id"].ToString()))
                        {
                            event_log.Add(new CalendarEventLog()
                            {
                                log_id = (!string.IsNullOrWhiteSpace(dr["log_id"].ToString())) ? Convert.ToInt32(dr["log_id"].ToString()) : 0,
                                creator_employee = new EmployeeModel()
                                {
                                    EmpID = Convert.ToInt32(dr["log_employee_id"].ToString()),
                                    UserPhotoPath = dr["log_emp_photo"].ToString(),
                                    EmpFirstname = dr["EmpFirstname"].ToString(),
                                    EmpLastname = dr["EmpLastname"].ToString(),
                                    Gender = dr["log_emp_gender"].ToString()
                                },
                                created_at = dr["log_date"].ToString(),
                                event_log = dr["event_log"].ToString()

                            });
                        }
                    }

                    foreach (DataColumn col in dt.Columns)
                    {
                        event_item.Add(col.ColumnName, dt.Rows[0][col]);
                    }
                    event_item.Remove("employee_id");
                    event_item.Remove("EmpPhotoPath");
                    event_item.Remove("EmpFirstname");
                    event_item.Remove("EmpLastname");
                    event_item.Remove("EmpGender");
                    event_item.Add("employees", employees.GroupBy(x => x.EmpID).Select(y => y.First()));

                    event_item.Remove("filename");
                    event_item.Remove("filepath");
                    event_item.Add("archive", archive.GroupBy(x => x.archive_id).Select(y => y.First()));
                    event_item.Remove("archive_id");
                    event_item.Remove("date");

                    event_item.Remove("log_id");
                    event_item.Remove("log_employee_id");
                    event_item.Remove("log_emp_photo");
                    event_item.Remove("log_emp_name");
                    event_item.Remove("log_emp_gender");
                    event_item.Remove("log_date");
                    event_item.Remove("event_log");
                    event_item.Add("event_log", event_log.GroupBy(x => x.log_id).Select(y => y.First()));
                }
                return Json(new { data = event_item, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
            }
            //else
            //{
            //    return RedirectToAction("Login", "Login");
            //}
        }

        //POST:taskSave
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult taskSave(CalendarEventInfo event_info)
        {
            if (Session["USER"] != null)
            {
                int row_id = 0;
                List<int> employee = null;
                if (event_info != null)
                {
                    if (event_info.Id > 0)
                    {
                        event_info.status = 1;
                        CalendarEventLog event_log = new CalendarEventLog()
                        {
                            Id = event_info.Id,
                            creator_employee = new EmployeeModel()
                            {
                                EmpID = (Session["USER"] as EmployeeModel).EmpID
                            },
                            event_log = "Task edited."
                        };
                        row_id = Db.taskEdit(event_info, event_log);
                        DataTable dt = Db.fetchEventDetailsById(event_info.Id);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            employee = dt.AsEnumerable()
                              .Select(r => r.Field<int>("employee_id"))
                              .ToList();
                            employee.GroupBy(x => x).Select(y => y.First());
                        }
                    }
                    else
                    {
                        event_info.event_type = 1;
                        event_info.status = 1;
                        CalendarEventLog event_log = new CalendarEventLog()
                        {
                            creator_employee = new EmployeeModel()
                            {
                                EmpID = (Session["USER"] as EmployeeModel).EmpID
                            },
                            event_log = "Task created and assigned to employees by admin."
                        };
                        row_id = Db.taskSave(event_info, event_log);
                    }
                }
                return Json(new { data = row_id, employees = employee, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        //POST:taskRemove
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult taskRemove(int event_id)
        {
            if (Session["USER"] != null && 1 == (Session["USER"] as EmployeeModel).Usertype)
            {
                return Json(new { data = Db.taskRemove(event_id), UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        //POST:UploadTaskFile
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadTaskFile()
        {
            string status = "";
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype
                };

                if (em.Usertype == 1)
                {
                    if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                    {
                        var event_id = System.Web.HttpContext.Current.Request.Params.Get("event_id");
                        HttpFileCollectionBase files = Request.Files;
                        List<CalendarEventArchive> uploaded_task_files = new List<CalendarEventArchive>();
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string _filename;
                            string _originalFileName;
                            string _ext;

                            // Checking for Internet Explorer  
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                _filename = testfiles[testfiles.Length - 1];
                                _originalFileName = Path.GetFileName(_filename);
                                _ext = Path.GetExtension(_filename);
                            }
                            else
                            {
                                _originalFileName = Path.GetFileName(file.FileName);
                                _ext = Path.GetExtension(_originalFileName);
                            }

                            _filename = Guid.NewGuid().ToString();
                            _filename = "task_" + _filename + _ext;
                            try
                            {
                                // Get the complete folder path and store the file inside it.  
                                var _comPath = Server.MapPath("~/Content/Uploads/") + _filename;
                                var storagePath = Server.MapPath("~/Content/Uploads/");
                                if (!Directory.Exists(storagePath))
                                {
                                    Directory.CreateDirectory(storagePath);
                                }
                                file.SaveAs(_comPath);
                                uploaded_task_files.Add(new CalendarEventArchive()
                                {
                                    filename = _filename,
                                    filepath = _originalFileName,
                                    Id = Convert.ToInt32(event_id)
                                });
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }

                        if (uploaded_task_files != null && uploaded_task_files.Count() > 0)
                        {
                            status = Db.taskFilesSave(uploaded_task_files);
                        }
                    }
                }
                else
                {
                    RedirectToAction("Login");
                }
            }
            return Json(new { UpdateStatus = status }, JsonRequestBehavior.AllowGet);
        }

        //GET:downloadTaskFile
        [AcceptVerbs(HttpVerbs.Get)]
        public FileResult downloadTaskFile(string id)
        {
            int archive_id = (!string.IsNullOrEmpty(id)) ? Convert.ToInt32(id) : 0;
            if (archive_id > 0)
            {
                CalendarEventArchive archive = Db.getArchiveData(archive_id);
                if (archive != null)
                {
                    var _comPath = Server.MapPath("~/Content/Uploads/") + archive.filepath;
                    if (System.IO.File.Exists(_comPath))
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(_comPath);
                        string fileName = archive.filepath;
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                    }
                }
            }
            return null;
        }

        //POST:removeTaskArchive
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult removeTaskArchive(int archive_id)
        {
            if (Session["USER"] != null && 1 == (Session["USER"] as EmployeeModel).Usertype)
            {
                return Json(new { data = Db.removeTaskArchive(archive_id), UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        //GET:getTreeViewFiles
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult getTreeViewFiles(int eventid)
        {
            IList<CalendarEventTreeItems> rows = null;
            if (eventid > 0)
            {
                rows = Db.getTreeViewItems(eventid);
            }
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
    }
}