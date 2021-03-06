﻿using HrmsMvc.Models;
using System;
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace HrmsMvc.Controllers
{
    [RequireHttps]
    public class LoginController : Controller
    {
        // GET: Login
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Login()
        {
            if (Session["USER"] != null)
            {
                int UserType = (Session["USER"] as EmployeeModel).Usertype;

                if (UserType == 1)
                {
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    return RedirectToAction("UserProfile", "User");
                }
            }
            else
            {
                return View();
            }
        }

        // POST: Login
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(UserModel lobj)
        {
            string userName = lobj.UserName;
            string password = lobj.Password;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("UserName", "Enter credentials");
                ModelState.AddModelError("Password", "Enter credentials");
            }
            if (ModelState.IsValid)
            {
                DataTable dt = Db.Validateuser(userName, password);
                if (dt != null && dt.Rows.Count > 0)
                {
                    EmployeeModel em = null;

                    foreach (DataRow row in dt.Rows)
                    {
                        em = new EmployeeModel()
                        {
                            EmpID = Convert.ToInt32(row["ID"].ToString().TrimEnd()),
                            Usertype = Convert.ToInt32(row["UserType"].ToString().TrimEnd()),
                            EmpFirstname = row["EmpFirstname"].ToString().TrimEnd(),
                            EmpLastname = row["EmpLastname"].ToString().TrimEnd()
                        };
                    }

                    Session["USER"] = em;

                    if (em.Usertype == 1)
                    {
                        return RedirectToAction("Users", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("UserProfile", "User");
                    }
                }

                ModelState.AddModelError("UserName", "Invalid credentials");
                ModelState.AddModelError("Password", "Invalid credentials");
            }
            return View(lobj);
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Login");
        }

        // GET: ForgotPassword
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ForgotPassword()
        {
            if (Session["USER"] != null)
            {
                int UserType = (Session["USER"] as EmployeeModel).Usertype;

                if (UserType == 1)
                {
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("UserDetails", "User");
                }
            }
            else
            {
                return View();
            }
        }

        // POST: ForgotPassword
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ForgotPassword(UserModel lobj)
        {
            if (string.IsNullOrEmpty(lobj.UserEmail))
            {
                TempData["returnString"] = "E";
                ModelState.AddModelError("UserEmail", "Please enter your email to receive password reset link");
            }
            else if (!Helpers.Helper.RegexEmail.IsMatch(lobj.UserEmail))
            {
                TempData["returnString"] = "IF";
                ModelState.AddModelError("UserEmail", "Enter a valid email id");
            }

            if (ModelState.IsValid)
            {
                DataTable dt = Db.GetEmployeeInfo(0, lobj.UserEmail);
                string EmpID = "";
                string empName = "";
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        EmpID = row["EmpId"].ToString();
                        empName = row["EmpFirstname"].ToString() + " " + row["EmpLastname"].ToString(); 
                    }

                    // generate password token
                    var token = Guid.NewGuid().ToString() + "_" + lobj.UserEmail + "_" + EmpID;
                    Db.createResetPasswordToken(EmpID, token);

                    //create url with password token
                    var link = Url.Action("resetpassword", "login", new { un = lobj.UserEmail, rt = token, empId = EmpID }, "https");
                    lobj.UserToken = link;
                    var rendView = RenderRazorViewToString("ResetPwdEmailTemplate", lobj);
                    Helpers.Helper.sentEmail("Hrms - Reset password link", rendView, lobj.UserEmail);
                    ModelState.Clear();
                    TempData["returnString"] = "OK";
                    return RedirectToAction("successviewforgotpassword", "login");
                }
                else
                {
                    TempData["returnString"] = "NE";
                    ModelState.AddModelError("UserEmail", "Entered email id is not registered with us");
                }
            }

            return View(lobj);
        }

        // GET: ResetPassword
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ResetPassword(string un, string rt, string empId)
        {
            if (!string.IsNullOrEmpty(un) && !string.IsNullOrEmpty(rt) && !string.IsNullOrEmpty(empId))
            {
                Session["RESETLINKPARAM"] = new UserModel() { Id = Convert.ToInt32(empId), UserToken = rt };

                if (!string.IsNullOrEmpty(empId) && !string.IsNullOrEmpty(rt))
                {
                    if (Db.validateResetPasswordToken(empId, rt))
                    {
                        return View(new UserModel());
                    }
                }
            }

            return RedirectToAction("errorpage", "login");
        }

        // POST: ResetPassword
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ResetPassword(UserModel lobj)
        {
            if (Session["RESETLINKPARAM"] != null)
            {
                int EmpID = (Session["RESETLINKPARAM"] as UserModel).Id;
                string token = (Session["RESETLINKPARAM"] as UserModel).UserToken;

                if (string.IsNullOrEmpty(lobj.Password))
                {
                    TempData["returnString"] = "E";
                    ModelState.AddModelError("Password", "Please enter password");
                }
                if (string.IsNullOrEmpty(lobj.CnfrmPassword))
                {
                    TempData["returnString"] = "E";
                    ModelState.AddModelError("CnfrmPassword", "Please enter password again to confirm");
                }
                if (!string.IsNullOrEmpty(lobj.Password) && !string.IsNullOrEmpty(lobj.CnfrmPassword) && lobj.Password != lobj.CnfrmPassword)
                {
                    TempData["returnString"] = "NM";
                    ModelState.AddModelError("CnfrmPassword", "Confirm password doesn't match");
                }

                if (ModelState.IsValid)
                {
                    string rtrnStr = Db.UpdateUserPassword(EmpID, null, lobj.Password);
                    if (rtrnStr.Equals("OK:"))
                    {
                        Db.updateResetPasswordToken(EmpID.ToString(), token);
                        Session.RemoveAll();
                        ModelState.Clear();
                        TempData["returnString"] = "OK";
                        return RedirectToAction("SuccessViewResetPassword", "Login");
                    }
                }
            }

            return View(lobj);
        }

        // GET: ErrorPage
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ErrorPage()
        {
            return View();
        }

        // GET: SuccessViewForgotPassword
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult SuccessViewForgotPassword()
        {
            return View();
        }

        // GET: SuccessViewResetPassword
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult SuccessViewResetPassword()
        {
            return View();
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            try
            {
                ViewData.Model = model;
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                             viewName);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                                 ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}