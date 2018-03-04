﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HrmsMvc.Models
{
    public class LoginModel
    {
        public int Id { get; set; }

        [AllowHtml]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [AllowHtml]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public int UserType { get; set; }

        [AllowHtml]
        [Display(Name = "Please enter your email")]
        //[Required(ErrorMessage = "Please enter your email to receive password reset link")]
        //[RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter a valid email id")]
        public string UserEmail { get; set; }

        [AllowHtml]
        [ScriptIgnore]
        public string CnfrmPassword { get; set; }

        [ScriptIgnore]
        public string UserToken { get; set; }

        [ScriptIgnore]
        public string returnString { get; set; }
    }

    public class EmployeeModel
    {
        public int EmpID { get; set; }

        [AllowHtml]
        public string EmpFirstname { get; set; }

        [AllowHtml]
        public string EmpLastname { get; set; }

        public string Gender { get; set; }

        [AllowHtml]
        public string UserName { get; set; }

        [ScriptIgnore]
        [AllowHtml]
        public string Password { get; set; }

        [ScriptIgnore]
        [AllowHtml]
        public string CnfrmPassword { get; set; }

        [AllowHtml]
        public string PhoneNumber { get; set; }

        [AllowHtml]
        public string EmailId { get; set; }

        public string UserRole { get; set; }

        public int DesigType { get; set; }

        public int Usertype { get; set; }

        public string Designation { get; set; }

        [AllowHtml]
        public string DateOfBirth { get; set; }

        [AllowHtml]
        public string UserPhotoPath { get; set; }

        public string ErrorString { get; set; }

        public bool NoMoreData { get; set; }

        public int RowCount { get; set; }

        public virtual ICollection<DesignationModel> DesisnationList { get; set; }
        public virtual ICollection<UserRoleModel> UserRoleList { get; set; }
    }

    public class LeaveModel : EmployeeModel
    {
        public int _lvId { get; set; }
        public double _casualLeave { get; set; }
        public double _festiveLeave { get; set; }
        public double _sickLeave { get; set; }

        [AllowHtml]
        public string _fromdate { get; set; }
        [AllowHtml]
        public string _todate { get; set; }

        public string _leavedurationtype { get; set; }
        public int _leaveDurTypeInt { get; set; }
        public string _strLvType { get; set; }
        public int _leaveType { get; set; }

        public int _leaveHalfDaySession { get; set; } //1 - morning, 2 - afternoon

        [AllowHtml]
        public string _comments { get; set; }
        public bool _status { get; set; }
        public bool _rejected { get; set; }
        public bool _cancelled { get; set; }
        public string _leaveStatus { get; set; }        

        public ArrayList RtrnArry { get; set; }
    }

    public class AttendanceModel : EmployeeModel
    {
        public string PunchinTime { get; set; }
        public string PunchoutTime { get; set; }
        public string Duration { get; set; }
        public string Notes { get; set; }
    }

    public class UserReportModel : EmployeeModel
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public double TotalDays { get; set; }
        public double Holidays { get; set; }
        public double WorkingDays { get; set; }
        public double WorkingHours { get; set; }
        public double ActiveDays { get; set; }
        public double LeaveDays { get; set; }
    }

    public class DesignationModel
    {
        public int ID { get; set; }
        public string designation { get; set; }
    }

    public class UserRoleModel
    {
        public int ID { get; set; }
        public string Privilage { get; set; }
    }

    public class CalendarEventInfo
    {
        public int Id { get; set; }
        public IList<int> employee { get; set; }
        public int event_type { get; set; }
        public string heading { get; set; }
        public string note { get; set; }
        public int status { get; set; }
        public IList<CalendarEventDate> event_dates { get; set; }
        public IList<CalendarEventArchive> archive { get; set; }
        public IList<CalendarEventEmployees> employees { get; set; }

    }

    public class CalendarEventDate : CalendarEventInfo
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
    }

    public class CalendarEventType
    {
        public string type_name { get; set; }
    }

    public class CalendarEventEmployees : CalendarEventInfo
    {
        public int employee_id { get; set; }
    }

    public class CalendarEventArchive : CalendarEventInfo
    {
        public int archive_id { get; set; }
        public string filename { get; set; }
        public string filepath { get; set; }
        public string date { get; set; }
    }

    public class CalendarEventLog : CalendarEventInfo
    {
        public int log_id { get; set; }
        public EmployeeModel creator_employee { get; set; }
        public string created_at { get; set; }
        public string event_log { get; set; }

    }

    public class CalendarEventTreeItems:CalendarEventInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public int parentid { get; set; }
        public string node_type { get; set; }
        public bool is_edit { get; set; }
        public string type { get; set; }

        public List<CalendarEventTreeItems> children { get; set; }
    }

    public class EmployeeViewModel
    {
        [Display(Name = "Employee Details")]
        public List<EmployeeModel> EmployeeDetails { get; set; }

        [Display(Name = "Punch Details")]
        public List<EmployeeModel> PunchDetails { get; set; }

        [Display(Name = "Employee Id")]
        public string EmpId { get; set; }

        [Required(ErrorMessage = "Please enter First Name")]
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        [Required(ErrorMessage = "Please Select gender")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please Select DateOfBirth")]
        [Display(Name = "Date Of Birth")]
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please Select User Role")]
        [Display(Name = "User Role")]
        public SelectList UserRole { get; set; }

        [Required(ErrorMessage = "Please enter the Phone Number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the Email Id")]
        [Display(Name = "Email Id")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please enter the User Name")]
        [RegularExpression(@"^[^-\s][a-zA-Z0-9_\s-]+$")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public EmployeeViewModel(List<EmployeeModel> empDetails, List<EmployeeModel> punchDetails)
        {
            EmployeeDetails = empDetails;
            PunchDetails = punchDetails;
        }
    }
}