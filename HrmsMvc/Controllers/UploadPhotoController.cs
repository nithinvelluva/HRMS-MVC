using HrmsMvc.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace HrmsMvc.Controllers
{
    [RequireHttps]
    public class UploadPhotoController : Controller
    {
        [HttpPost]
        public string Upload(HttpPostedFileBase myFile)
        {
            try
            {
                if (myFile != null && myFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(myFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/UserIcons/"), fileName);
                    myFile.SaveAs(path);
                }
                return "OK";
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFile()
        {
            string _imgname = string.Empty;
            if (Session["USER"] != null)
            {
                int EmpID = (Session["USER"] as EmployeeModel).EmpID;

                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                    if (pic.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(pic.FileName);
                        var _ext = Path.GetExtension(pic.FileName);
                        _imgname = Guid.NewGuid().ToString();
                        _imgname = "HRMS_" + _imgname + _ext;

                        try
                        {
                            var _comPath = Server.MapPath("~/Content/UserIcons/") + _imgname;// +_ext;

                            ViewBag.Msg = _comPath;
                            var path = _comPath;

                            var storagePath = Server.MapPath("~/Content/UserIcons/");

                            if (!Directory.Exists(storagePath))
                            {
                                Directory.CreateDirectory(storagePath);
                            }

                            // Saving Image in Original Mode
                            pic.SaveAs(path);

                            // resizing image
                            //MemoryStream ms = new MemoryStream();
                            WebImage img = new WebImage(_comPath);

                            if (img.Width > 200)
                                img.Resize(200, 200);
                            img.Save(_comPath);
                            // end resize

                            EmployeeModel em = new EmployeeModel();
                            em.EmpID = EmpID;
                            em.UserPhotoPath = _imgname;

                            string rtrnStr = Db.UpdateProfile(em, true);

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
        }

        public string UpdateUserPhoto(string userphotopath, string prvUserPhotoPath, bool CancelFlag)
        {
            if (Session["USER"] != null)
            {
                try
                {
                    string rtrnStr = "";
                    int EmpId = (Session["USER"] as EmployeeModel).EmpID;

                    if (EmpId > 0 && !string.IsNullOrEmpty(userphotopath))
                    {
                        if (!CancelFlag)
                        {
                            if (!string.IsNullOrEmpty(prvUserPhotoPath))
                            {
                                var _comPath = Server.MapPath("~/Content/UserIcons/") + prvUserPhotoPath.Replace("../Content/UserIcons/", "");
                                if (System.IO.File.Exists(_comPath.ToString()))
                                {
                                    System.IO.File.Delete(_comPath);
                                }
                            }
                            EmployeeModel em = new EmployeeModel();
                            em.EmpID = EmpId;
                            em.UserPhotoPath = (userphotopath.IndexOf("../Content/UserIcons/") >= 0) ? 
                                userphotopath.Replace("../Content/UserIcons/", "") : userphotopath;

                            rtrnStr = Db.UpdateProfile(em, true);
                        }
                        else
                        {
                            var _comPath = Server.MapPath("~/Content/UserIcons/") + userphotopath.Replace("../Content/UserIcons/", "");
                            System.IO.File.Delete(_comPath);
                            rtrnStr = "CANCEL";
                        }
                    }

                    return rtrnStr;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return null;
        }

        public JsonResult Rebind()
        {
            string imgName = Session["val"].ToString();
            return Json(imgName, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void Capture()
        {
            var stream = Request.InputStream;
            string dump;
            string _imgname = string.Empty;

            using (var reader = new StreamReader(stream))
            {
                dump = reader.ReadToEnd();
                var _ext = ".jpg";
                _imgname = Guid.NewGuid().ToString();
                _imgname = "HRMS_" + _imgname + _ext;

                var path = Server.MapPath("~/Content/UserIcons/") + _imgname;

                var storagePath = Server.MapPath("~/Content/UserIcons/");
                if (!Directory.Exists(storagePath))
                {
                    Directory.CreateDirectory(storagePath);
                }

                System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));

                ViewBag.ImgPath = path;
                ViewBag.ImgName = _imgname;

                //// resizing image
                //MemoryStream ms = new MemoryStream();
                //WebImage img = new WebImage(_comPath);

                //if (img.Width > 200)
                //    img.Resize(200, 200);
                // img.Save(_comPath);
                // end resize               
            }

            Session["val"] = _imgname;
        }

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;

            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }
    }
}