using HrmsMvc.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace HrmsMvc.Controllers
{
    [RequireHttps]
    public class AdminController : Controller
    {
        // GET: Users
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Users()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype
                };

                switch (em.Usertype)
                {
                    case 1:
                        ViewBag.EmpId = em.EmpID;
                        ViewBag.UserType = em.Usertype;

                        return View();
                    case 2:
                        return RedirectToAction("UserProfile", "User");
                    default:
                        return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Attendance
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Attendance()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype
                };

                switch (em.Usertype)
                {
                    case 1:
                        ViewBag.EmpId = em.EmpID;
                        ViewBag.UserType = em.Usertype;

                        return View();
                    case 2:
                        return RedirectToAction("UserProfile", "User");
                    default:
                        return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: LeaveInfo
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Leaves()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype
                };

                switch (em.Usertype)
                {
                    case 1:
                        ViewBag.EmpId = em.EmpID;
                        ViewBag.UserType = em.Usertype;

                        return View();
                    case 2:
                        return RedirectToAction("UserProfile", "User");
                    default:
                        return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        //GET: EmployeeReports
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult EmployeeReports()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype
                };

                switch (em.Usertype)
                {
                    case 1:
                        ViewBag.EmpId = em.EmpID;
                        ViewBag.UserType = em.Usertype;

                        return View();
                    case 2:
                        return RedirectToAction("UserProfile", "User");
                    default:
                        return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: ViewEmployeeDetails
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ViewEmployeeDetails(string empId)
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = null;
                em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype
                };

                switch (em.Usertype)
                {
                    case 1:
                        ViewBag.EmpId = em.EmpID;
                        ViewBag.UserType = em.Usertype;

                        int EmpID = !string.IsNullOrEmpty(empId) ? Convert.ToInt32(empId) : 0;
                        em = new EmployeeModel();
                        if (EmpID > 0)
                        {
                            em = Helpers.Helper.getEmployeeInfoData(EmpID);
                        }
                        DataTable dt = Db.getEmployeeDesignationList();
                        em.DesisnationList = new List<DesignationModel>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            em.DesisnationList.Add(new DesignationModel
                            {
                                ID = int.Parse(dr["ID"].ToString()),
                                designation = dr["Designation"].ToString()
                            });
                        }
                        em.UserRoleList = new List<UserRoleModel>(Db.GetRoleSelectList().Select(u => new UserRoleModel
                        {
                            ID = Convert.ToInt32(u.Value),
                            Privilage = u.Text.ToString()
                        }));
                        return PartialView("ViewEmployeeDetails", em);
                    case 2:
                        return RedirectToAction("UserProfile", "User");
                    default:
                        return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: AddEmployee
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult AddEmployee()
        {
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    EmpID = (Session["USER"] as EmployeeModel).EmpID,
                    Usertype = (Session["USER"] as EmployeeModel).Usertype
                };

                switch (em.Usertype)
                {
                    case 1:
                        ViewBag.EmpId = em.EmpID;
                        ViewBag.UserType = em.Usertype;

                        em = new EmployeeModel();
                        DataTable dt = Db.getEmployeeDesignationList();
                        em.DesisnationList = new List<DesignationModel>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            em.DesisnationList.Add(new DesignationModel
                            {
                                ID = int.Parse(dr["ID"].ToString()),
                                designation = dr["Designation"].ToString()
                            });
                        }

                        em.UserRoleList = new List<UserRoleModel>(Db.GetRoleSelectList().Select(u => new UserRoleModel
                        {
                            ID = Convert.ToInt32(u.Value),
                            Privilage = u.Text.ToString()
                        }));

                        return PartialView("AddEmployee", em);
                    case 2:
                        return RedirectToAction("UserProfile", "User");
                    default:
                        return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ManageEmployee(EmployeeModel em)
        {
            string errStr = string.Empty;
            if (string.IsNullOrEmpty(em.EmpFirstname))
            {
                errStr = errStr + "1:";
            }
            if (string.IsNullOrEmpty(em.EmpLastname))
            {
                errStr = errStr + "LN:";
            }
            if (string.IsNullOrEmpty(em.Gender))
            {
                errStr = errStr + "2:";
            }
            if (em.Usertype <= 0)
            {
                errStr = errStr + "3:";
            }
            if (em.DesigType <= 0)
            {
                errStr = errStr + "D:";
            }
            if (string.IsNullOrEmpty(em.DateOfBirth))
            {
                errStr = errStr + "4:";
            }
            if (!string.IsNullOrEmpty(em.DateOfBirth))
            {
                TimeSpan ts = new TimeSpan();
                DateTime dob = new DateTime();
                if (DateTime.TryParse(em.DateOfBirth, out dob))
                {
                    ts = DateTime.Now - dob;
                    if (ts.TotalDays < 6570)
                    {
                        errStr = errStr + "9:";
                    }
                }
                else
                {
                    errStr = errStr + "IDOB:";
                }
            }

            if (string.IsNullOrEmpty(em.EmailId))
            {
                errStr = errStr + "EN:";
            }
            else if (!Helpers.Helper.RegexEmail.IsMatch(em.EmailId))
            {
                errStr = errStr + "ENV:";
            }

            if (string.IsNullOrEmpty(em.PhoneNumber))
            {
                errStr = errStr + "PN:";
            }
            if (!string.IsNullOrEmpty(em.PhoneNumber) && !Helpers.Helper.mobRgx.IsMatch(em.PhoneNumber))
            {
                errStr = errStr + "PNV:";
            }
            if (string.IsNullOrEmpty(errStr))
            {
                EmployeeModel empModel = new EmployeeModel();
                empModel = em;                
                empModel.Password = Helpers.Helper.HashPassword("hrms123");
                empModel.UserPhotoPath = !string.IsNullOrEmpty(em.UserPhotoPath) ? em.UserPhotoPath : "";
                empModel.UserName = em.EmpFirstname + "." + em.EmpLastname;
                errStr = Db.AddEmployee(empModel);
            }

            return Json(new { data = errStr, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveEmployee(string EmpId)
        {
            string rtrn = "NOSESSION";
            if (Session["USER"] != null)
            {
                EmployeeModel em = new EmployeeModel()
                {
                    Usertype = (Session["USER"] as EmployeeModel).Usertype
                };
                if (em.Usertype == 1)
                {
                    if (!string.IsNullOrEmpty(EmpId))
                    {
                        bool flag = Db.RemoveEmployee(Convert.ToInt32(EmpId));
                        rtrn = (flag) ? "DELETED" : "ERROR";
                    }
                }
            }

            return Json(new { data = rtrn, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetEmployeeData(int BlockNumber = 1)
        {
            List<EmployeeModel> empDetails = new List<EmployeeModel>();
            int BlockSize = 8;
            empDetails = Db.GetEmployeeDetails(BlockSize, BlockNumber);
            return Json(new { data = empDetails, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult editEmpDetails(EmployeeModel em)
        {
            int empId = em.EmpID;
            string rtrn = "";
            if (empId > 0)
            {
                int roleid = em.Usertype;
                int desigid = em.DesigType;

                EmployeeModel emModel = new EmployeeModel();
                emModel = em;
                rtrn = Db.UpdateEmployeeInfoDashboard(emModel);
            }

            return Json(new { data = rtrn, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult searchUser(string searchText)
        {
            List<EmployeeModel> empDetails = new List<EmployeeModel>();
            if (!string.IsNullOrEmpty(searchText))
            {
                empDetails = Db.searchUser(searchText);
            }
            return Json(new { data = empDetails, UpdateStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }        
    }
}