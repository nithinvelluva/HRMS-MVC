var empName;
var ExtChkFlg = false;
var IsDefaultUserIcon = false;
function ajaxRequest(url, params, callback) {
    params = (params) ? params : {};
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        datatype: "json",
        data: JSON.stringify(params),
        success: function (response) {
            if (callback) {
                callback(response);
            }
        },
        error: function (response) {
            alert('An unexpected error occured !!');
        }
    });
};
function toggleCaptureWindow(IsClosable) {
    if (!IsClosable) {
        $('.cameraDiv').show();
        $('.capturePreviewDiv').hide();
    }
    else {
        $('#CapturePhotoDiv').modal('toggle');
    }
};
function UpdateUserPhoto() {
    var userPhotoPath = $('#UserPhotoPath').val();
    var prvUserPhotoPath = userPhoto;
    var params = {
        userphotopath: userPhotoPath,
        prvUserPhotoPath: prvUserPhotoPath,
        CancelFlag: false
    };
    $.ajax({
        url: "/UploadPhoto/UpdateUserPhoto",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(params),
        success: function (response) {
            if (response && "OK" == response) {
                userPhotoUploadAjaxRequest();
                $('#saveDiv').hide();
                $('#userAvatar').attr('src', userPhoto);
                $('.cameraDiv').show();
                $('.capturePreviewDiv').hide();
            }
        },
        error: function (er) {
            //showMessageBox(ERROR, "An Unexpected Error Occured !!");
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !!",
                ok_btn: "OK",
                close_fn: null
            });
        }
    });
};
function userPhotoUploadAjaxRequest() {
    var files = $("#userPhotoInput").get(0).files;
    if (files.length > 0) {
        var data = new FormData();
        data.append("MyImages", files[0]);
        $.ajax({
            url: "/UploadPhoto/UploadFile",
            type: "POST",
            processData: false,
            contentType: false,
            data: data,
            success: function (response) {
                var path = UserPhotoBaseUrl + response;
                userPhoto = path;
                $('#userAvatar').attr('src', path);
                $('#UserPhotoPath').val(path);
                $('#saveDiv').hide();
                IsDefaultUserIcon = false;
                $("#userPhotoInput").clearFiles();
            },
            error: function (er) {
                //showMessageBox(ERROR, er);
                ymz.jq_alert({
                    title: "HRMS",
                    text: "An unexpected error occured !!",
                    ok_btn: "OK",
                    close_fn: null
                });
            }
        });
    }
};
function ChangeUserPhoto(CancelFlag) {
    if (!CancelFlag) {
        var files = $("#userPhotoInput").get(0).files;
        if (files.length > 0) {
            if (!IsDefaultUserIcon) {
                UpdateUserPhoto();
            }
            else {
                userPhotoUploadAjaxRequest();
            }
        }
    }
    else {
        $('#saveDiv').hide();
        $('#userAvatar').attr('src', userPhoto);
        $('.cameraDiv').show();
        $('.capturePreviewDiv').hide();
        $("#userPhotoInput").clearFiles();
    }
};
$.fn.extend({
    clearFiles: function () {
        $(this).each(function () {
            var isIE = (window.navigator.userAgent.indexOf("MSIE ") > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./));
            if ($(this).prop("type") == 'file') {
                if (isIE == true) {
                    $(this).replaceWith($(this).val('').clone(true));
                } else {
                    $(this).val("");
                }
            }
        });
        return this;
    }
});
function ChangeCaptureUserPhoto(cancelFlag) {
    var userPhotoPath = $('#UserPhotoPath').val();
    var prvUserPhotoPath = userPhoto;
    if (IsDefaultUserIcon) {
        prvUserPhotoPath = null;
    }
    var params = {
        userphotopath: userPhotoPath,
        prvUserPhotoPath: prvUserPhotoPath,
        CancelFlag: cancelFlag
    };
    $.ajax({
        url: "/UploadPhoto/UpdateUserPhoto",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(params),
        success: function (response) {
            if (response && "OK" == response) {
                $('#saveDiv').hide();
                $('#userAvatar').attr('src', userPhotoPath);
                $('.cameraDiv').show();
                $('.capturePreviewDiv').hide();
            }
            else if ("CANCEL" == response) {
                $('#saveDiv').hide();
                $('#userAvatar').attr('src', userPhoto);
                $('.cameraDiv').show();
                $('.capturePreviewDiv').hide();
            }
            else {
                //showMessageBox(ERROR, "An Unexpected Error Occured!!");
                ymz.jq_alert({
                    title: "HRMS",
                    text: "An unexpected error occured !!",
                    ok_btn: "OK",
                    close_fn: null
                });
            }
        },
        error: function (er) {
            //showMessageBox(ERROR, "An Unexpected Error Occured !!");
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !!",
                ok_btn: "OK",
                close_fn: null
            });
        }
    });
};
function takePhotoClick() {
    toggleCaptureWindow();
    $.ajax({
        url: "/User/CaptureUserPhoto",
        dataType: "html",
        type: "GET",
        contentType: "application/json",
        success: function (data) {
            if (data) {
                $('#CapturePhotoDivContainer').html(data);
            }
        },
        error: function (data) {
        }
    });
};
function fileCheck(obj) {
    ExtChkFlg = false;
    var fileExtension = ['jpeg', 'jpg', 'png'];
    if ($.inArray($(obj).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
        //showMessageBox(WARNING, "Only '.jpeg','.jpg', '.png' formats are allowed.");
        ymz.jq_alert({
            title: "HRMS",
            text: "Only '.jpeg','.jpg', '.png' formats are allowed.",
            ok_btn: "OK",
            close_fn: null
        });
        $("#userPhotoInput").clearFiles();
    }
    else {
        ExtChkFlg = true;
    }
};
function initPage() {
    if (Modernizr.touch) {
        $(".close-overlay").removeClass("hidden");
        $(".img").click(function (e) {
            if (!$(this).hasClass("hover")) {
                $(this).addClass("hover");
            }
        });
        $(".close-overlay").click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($(this).closest(".img").hasClass("hover")) {
                $(this).closest(".img").removeClass("hover");
            }
        });
    }
    else {
        $(".img").mouseenter(function () {
            $(this).addClass("hover");
        })
        .mouseleave(function () {
            $(this).removeClass("hover");
        });
    }
};
function setUserPhoto() {
    if (stringIsNull(userPhoto)) {
        var maleAvatarPath = "../Content/Images/male-avatar.png";
        var femaleAvatarPath = "../Content/Images/female-avatar.png";
        userPhoto = ("M" == UserGender) ? maleAvatarPath : femaleAvatarPath;
        $('#userAvatar').attr('src', userPhoto);
        IsDefaultUserIcon = true;
    }
    else {
        userPhoto = UserPhotoBaseUrl + userPhoto;
        $('#userAvatar').attr('src', userPhoto);
    }
};
function getEmpDetails() {
    showLoadreport("#imgProgProf", "#EmpProfileForm");
    var params = { empId: empId };
    var getEmpDetails = {};
    getEmpDetails.url = "/User/getEmployeeInfo";
    getEmpDetails.type = "POST";
    getEmpDetails.data = JSON.stringify(params);
    getEmpDetails.datatype = "json";
    getEmpDetails.contentType = "application/json";
    getEmpDetails.success = function (status) {
        if (status && status.data) {
            var response = status.data;
            if (response) {                
                $('#SelRole').val(userType);
                $('#SelGender').val(UserGender);
                $('#EmpGender').val("M" == UserGender ? "MALE" : "FEMALE");
                empName = response.EmpName;
                $('#EmpFirstName').val(response.EmpFirstname);
                $('#EmpLastName').val(response.EmpLastname);
            }
        }
        HideLoadreport("#imgProgProf", "#EmpProfileForm");
    };
    getEmpDetails.error = function () {
        HideLoadreport("#imgProgProf", "#EmpProfileForm");
    };
    $.ajax(getEmpDetails);
};
function setCurrentDateTime() {
    var currentTime = new Date();
    var date = currentTime.toDateString();
    var time = currentTime.toLocaleTimeString();

    $('#currentDate').text(date.toString());
    $('#currentTime').text(time.toString());

    $('#DateLabelLeave').val("As of Date : " + date.toString());

    var hours = currentTime.getHours();
    var minutes = currentTime.getMinutes();
    var seconds = currentTime.getSeconds();
};
function getEmpPunchDetails() {
    showLoadreport("#imgProgAtt", ".attnCntDiv");
    $('#notesField').val('');

    var params = { EmpId: empId, timeoffset: offset };
    var GetAttendance = {};
    GetAttendance.url = "/User/GetEmpPunchDetails";
    GetAttendance.type = "POST";
    GetAttendance.data = JSON.stringify(params);
    GetAttendance.datatype = "json";
    GetAttendance.contentType = "application/json";
    GetAttendance.success = function (status) {
        var response = status.data;
        if (!response) {
            $('#punhInBtn').show();
            $('#punchdInDiv').hide();
            $('#punhOutBtn').hide();
            $('#punchdOutDiv').hide();
        }
        else if (response["PunchinTime"].length > 0 && response["PunchoutTime"].length > 0) {
            $('#punhInBtn').show();
            $('#punhOutBtn').hide();
            $('#PunchedInTime').text((response["PunchinTime"]));
            $('#PunchedOutTime').text((response["PunchoutTime"]));
            $('#punchdInDiv').hide();
            $('#punchdOutDiv').hide();
        }
        else if (response["PunchinTime"].length > 0 && response["PunchoutTime"].length == 0) {
            $('#punhInBtn').hide();
            $('#punhOutBtn').show();
            $('#PunchedInTime').text((response["PunchinTime"]));
            $('#punchdInDiv').show();
            $('#punchdOutDiv').hide();
        }
        HideLoadreport("#imgProgAtt", ".attnCntDiv");
    };
    GetAttendance.error = function () {
        HideLoadreport("#imgProgAtt", ".attnCntDiv");
    };
    $.ajax(GetAttendance);
};
function leaveRefClick() {
    $('#dropdownIcon').toggleClass("glyphicon-chevron-down").toggleClass("glyphicon-chevron-up");
};
function navigateTo(page) {
    var activeElement;
    var activeContentDiv;
    var lvLi = false;
    switch (page) {
        case PROFILE:
            clearErrors();
            getEmpDetails();
            setUserPhoto();
            activeElement = PROFILE_LI;
            break;
        case ATTENDANCE:
            clearErrors();
            setCurrentDateTime();
            getEmpPunchDetails();
            activeElement = ATTENDANCE_LI;
            break;
        case LEAVES:
            clearErrors();
            ResetValues();
            $('#leaveItemsDiv').addClass('in');
            lvLi = true;
            activeElement = LEAVES_LI;
            $(".nav-list li").removeClass("active");
            $(APPLY_LEAVE_LI).addClass("active");
            $('#dropdownIcon').toggleClass("glyphicon-chevron-down").toggleClass("glyphicon-chevron-up");
            break;
        case LEAVEREPORT:
            clearErrors();
            $("#chngDatePicker").val('');
            setLeaveReportsInfo(true);
            PopulateLeaveReportTable();
            $('#leaveItemsDiv').addClass('in');
            lvLi = true;
            activeElement = LEAVES_LI;
            $(".nav-list li").removeClass("active");
            $(LEAVE_REPORT_LI).addClass("active");
            $('#dropdownIcon').toggleClass("glyphicon-chevron-down").toggleClass("glyphicon-chevron-up");
            break;
        case REPORTS:
            clearErrors();
            var dateObj = new Date();
            var month = dateObj.getUTCMonth() + 1;
            var year = dateObj.getUTCFullYear();
            $('#YearDropDown').val(year);
            $('#MonthDropDown').val(month);
            activeElement = REPORTS_LI;
            break;
        case CHANGEPASSWORD:
            clearFileds();
            clearErrors();
            break;
        case TASKS:
            activeElement = TASKS_LI;
            break;
    }
    if (!lvLi) {
        $(".main-nav li").removeClass("active");
    }
    $(activeElement).addClass("active");
};
function clearErrors() {
    $('.errLabel').hide();
    $('.input-field').css('border-color', '');
    $('.inut-field-profile').css('border-color', '');
};
function showLoadreport(activDiv, disableDiv) {
    $(activDiv).show();
    $(disableDiv).addClass("disablediv");
    $(".waitIconDiv").css("display", "block");
};
function HideLoadreport(activDiv, disableDiv) {
    $(activDiv).hide();
    $(disableDiv).removeClass("disablediv");
    $(".waitIconDiv").css("display", "none");
};
function setLeaveReportsInfo(InitFlag) {
    var date;
    if (InitFlag) {
        var currentTime = new Date();
        date = currentTime.toDateString();
        var dateSplit = date.split(' ');
        $('#monthLabel').text("Showing Leave Reports Of : " + dateSplit[1] + "," + dateSplit[3]);
        $('#dateLabel').text(" Details As Of : " + date);
        currYear = dateSplit[3];
        currMonth = (currentTime.getMonth()) + 1;
    }
    else {
        date = $("#chngDatePicker").val();
        currMonth = months[date.split(' ')[0]];
        currYear = date.split(' ')[1];
        $('#MonthDropDown').val(currMonth);
        $('#monthLabel').text("Showing Leave Reports Of : " + date.split(' ')[0] + "," + currYear);
    }
};
function PopulateLeaveReportTable() {
    var statusIconPath;
    var statusToolTip;
    var IsEdit = false;
    var IsRejected = false;

    $('#lblLvNoResults').hide();
    showLoadreport("#imgProgLvRprt", ".TableDivLvRprt");
    if (LeaveDetailsTable != undefined) {
        LeaveDetailsTable.destroy();
    }
    LeaveDetailsTable = $('#LvReprtTable').DataTable({
        "pageLength": 7, "processing": true, "serverSide": true,
        "paging": true, "bLengthChange": false, "searching": false,
        "bInfo": true,
        drawCallback: function (settings) {
            var pagination = $(this).closest('.dataTables_wrapper').find('.dataTables_paginate');
            pagination.toggle(this.api().page.info().pages > 1);
        },
        "ajax": {
            "url": "/User/GetLeaveDetails",
            "type": "POST",
            "dataType": "JSON",
            "data": function (d) {
                d.EmpId = empId;
                d.UserType = userType;
                d.month = currMonth;
                d.year = currYear;
            }
        }
        , "columnDefs": [
            {
                "render": function (data, type, row) {
                    var result = leaveRprtTableCallBack(row);
                    statusToolTip = result["statusToolTip"];
                    IsEdit = result["IsEdit"];
                    IsRejected = result["IsRejected"];
                    return '<input type="button" data-toggle="modal" data-target="#EditLeaveModal" title="View Details" value="View" class="btn btn-primary btn-fill ViewLeaveDetails" data-IsCancel = ' + row["_cancelled"] + ' data-IsRejected = ' + IsRejected + ' data-IsEdit=' + IsEdit + ' data-empid=' + empId + ' data-empname="' + empName + '" data-fromDate="' + row["_fromdate"] + '" data-toDate="' + row["_todate"] + '" data-LeaveType="' + row["_strLvType"] + '" data-DurationType="' + row["_leavedurationtype"] + '" data-lvDurTypint="' + row["_leaveDurTypeInt"] + '" data-LeaveStatus="' + statusToolTip + '" data-leaveTypeInt="' + row["_leaveType"] + '" data-commnets="' + row["_comments"] + '"  data-id="' + row["_lvId"] + '" data-lvSessionTyp="' + row["_leaveHalfDaySession"] + '" />'
                },
                "targets": 5
            },
            {
                "render": function (data, type, row) {
                    var result = leaveRprtTableCallBack(row);
                    statusIconPath = result["statusIconPath"];
                    statusToolTip = result["statusToolTip"];
                    return '<span class="cursor"><i class="' + statusIconPath + '" aria-hidden="true" title="' + statusToolTip + '"></i></span>'
                },
                "targets": 4
            }
        ]
        , "columns": [
            { "data": "_fromdate" },
            { "data": "_todate" },
            { "data": "_strLvType" },
            { "data": "_leavedurationtype" },
            { "data": "_leaveStatus" },
            { "data": "_leaveStatus" }
        ]
        , "language":
            {
                "processing": "<div class='row text-center waitIcon'><i class='fa fa-refresh fa-spin fa-2x fa-fw'></i></div>"
            }
    });

    HideLoadreport("#imgProgLvRprt", ".TableDivLvRprt");
    $('#lblLvNoResults').hide();
    $('.TableDivLvRprt').show();
};
function leaveRprtTableCallBack(row) {
    var statusIconPath;
    var statusToolTip;
    var IsEdit = false;
    var IsRejected = false;

    if (row["_status"] && !row["_rejected"] && !row["_cancelled"])//approved.
    {
        statusIconPath = "fa fa-check fa-lg";
        statusToolTip = "Approved";
        IsEdit = false;
        IsRejected = false;
    }
    else if (!row["_status"] && !row["_rejected"] && !row["_cancelled"])//pending.
    {
        statusIconPath = "fa fa-clock-o fa-lg";
        statusToolTip = "Pending";
        IsEdit = true;
        IsRejected = false;
    }
    else if (!row["_status"] && row["_rejected"] && !row["_cancelled"])//rejected.
    {
        statusIconPath = "fa fa-times fa-lg";
        statusToolTip = "Rejected";
        IsEdit = false;
        IsRejected = true;
    }
    else if (!row["_status"] && !row["_rejected"] && row["_cancelled"])//cancelled.
    {
        statusIconPath = "fa fa-ban fa-lg";
        statusToolTip = "Cancelled";
        IsEdit = false;
        IsRejected = false;
    }
    return { statusIconPath: statusIconPath, statusToolTip: statusToolTip, IsEdit: IsEdit, IsRejected: IsRejected };
};
$(document).ready(function () {
    $('#usrAvatarExpndBtn').click(function () {
        try {
            $('#userPhotoInput').trigger('click');
        }
        catch (ex) {
            //showMessageBox(ERROR, "ERROR !!");
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !!",
                ok_btn: "OK",
                close_fn: null
            });
        }
    });
    $("#userPhotoInput").change(function () {
        if (ExtChkFlg) {
            var files = $("#userPhotoInput").get(0).files;
            if (files.length > 0) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#userAvatar').attr('src', e.target.result);
                    $('#saveDiv').show();
                }
                reader.readAsDataURL(files[0]);
            }
        }
    });
});