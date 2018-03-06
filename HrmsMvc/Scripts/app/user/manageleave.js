var IsCancel = false;
var IsCalendarEdit = false;
$(document).ready(function () {
    $(document).on('click', '.ViewLeaveDetails', function () {
        var leaveId = $(this).attr('data-id');
        IsCalendarEdit = false;
        leaveEditPopupLoading(true);
        var url = "/user/leaveDetailsFetch";
        var params = { leave_event_id: leaveId };
        var callback = leaveDetailsFetchCallback;
        calendarAjaxGETRequest(url, params, callback);
    });
    $('#EditLeaveModal').on('hidden.bs.modal', function () {
        initLeaveModal();
    });
});
function initLeaveModal() {
    $('#cntctAdmin').hide();
    $('#LeaveUpdateBtn').hide();
    $('#LeaveCancelBtn').hide();
    $('#LeaveRejectBtn').hide();
    $('#LeaveApproveBtn').hide();
    DisableLeaveFields();
};
function LeaveUpdateClick() {
    var leaveID = $('#LeaveId').val();
    var strtDate = $('#LvstartDateLabel').val();
    var toDate = $('#LvtoDateLabel').val();
    var comments = $('#cmntsArea').val();
    var lvTypInt = $('#leaveTypeLabel').val();
    var lvtypstr = $('#leaveTypeLabel option:selected').text();
    var lvdurtyp = $('#LvDurTypLabel').val();
    var lvdurtypstr = $('#LvDurTypLabel option:selected').text();
    var LvSession = $('#LeaveSessionDropDown').val();

    var validFlag = false;
    if (stringIsNull(strtDate)) {
        validFlag = true;
    }
    if (stringIsNull(toDate)) {
        validFlag = true;
    }
    if (!stringIsNull(lvdurtyp) && 2 == lvdurtyp && stringIsNull(LvSession)) {
        $('#LeaveSessionDropDown').css('border-color', 'red');
        $('#ErrLvSession').show();
        validFlag = true;
    }
    if (!validFlag) {
        showLoadleavereport("#LoadPageLvEdit", ".lvEditView");
        var params = {
            _lvId: leaveID,
            EmpID: empId,
            _fromdate: strtDate,
            _todate: toDate,
            _leaveType: lvTypInt,
            _leaveDurTypeInt: lvdurtyp,
            _comments: comments ? comments : "",
            _strLvType: lvtypstr,
            Usertype: userType,
            _leavedurationtype: lvdurtypstr,
            _leaveHalfDaySession: (2 == lvdurtyp) ? LvSession : 0,
            _cancelled: false
        };
        var url = "/User/AddLeave";
        manageLeaveAjaxRequest(params, url);
    }
    else {
        ymz.jq_alert({
            title: "HRMS",
            text: "Select Mandatory Fields !!",
            ok_btn: "OK",
            close_fn: null
        });
    }
};
function LeaveCancelClick() {
    var message = "Are you sure want to cancel the leave ?";
    var title = "HRMS";
    ymz.jq_confirm({
        title: !stringIsNull(title) ? title : "HRMS",
        text: message,
        no_btn: "No",
        yes_btn: "Yes",
        no_fn: function () {
        },
        yes_fn: function () {
            var leaveID = $('#LeaveId').val();
            var strtDate = $('#LvstartDateLabel').val();
            var toDate = $('#LvtoDateLabel').val();
            var comments = $('#cmntsArea').val();
            var lvTypInt = $('#leaveTypeLabel').val();
            var lvtypstr = $('#leaveTypeLabel option:selected').text();
            var lvdurtyp = $('#LvDurTypLabel').val();
            var lvdurtypstr = $('#LvDurTypLabel option:selected').text();
            var LvSession = $('#LeaveSessionDropDown').val();
            var params = {
                _lvId: leaveID,
                EmpID: empId,
                _fromdate: strtDate,
                _todate: toDate,
                _leaveType: lvTypInt,
                _leaveDurTypeInt: lvdurtyp,
                _comments: comments ? comments : "",
                _strLvType: lvtypstr,
                Usertype: userType,
                _leavedurationtype: lvdurtypstr,
                _leaveHalfDaySession: (2 == lvdurtyp) ? LvSession : 0,
                _cancelled: true
            };
            var url = "/User/AddLeave";
            manageLeaveAjaxRequest(params, url);
        }
    });
};
function LeaveManageClick(approve) {
    var message = "Are you sure want to continue ?";
    var title = "HRMS";
    ymz.jq_confirm({
        title: !stringIsNull(title) ? title : "HRMS",
        text: message,
        no_btn: "No",
        yes_btn: "Yes",
        no_fn: function () {
        },
        yes_fn: function () {
            var leaveID = $('#LeaveId').val();
            var strtDate = $('#LvstartDateLabel').val();
            var toDate = $('#LvtoDateLabel').val();
            var comments = $('#cmntsArea').val();
            var lvTypInt = $('#leaveTypeLabel').val();
            var lvtypstr = $('#leaveTypeLabel option:selected').text();
            var lvdurtyp = $('#LvDurTypLabel').val();
            var lvdurtypstr = $('#LvDurTypLabel option:selected').text();
            var LvSession = $('#LeaveSessionDropDown').val();
            var params = {
                _lvId: leaveID,
                EmpID: empId,
                _fromdate: strtDate,
                _todate: toDate,
                _leaveType: lvTypInt,
                _leaveDurTypeInt: lvdurtyp,
                _comments: comments ? comments : "",
                _strLvType: lvtypstr,
                Usertype: userType,
                _leavedurationtype: lvdurtypstr,
                _leaveHalfDaySession: (2 == lvdurtyp) ? LvSession : 0,
                _rejected: !approve,
                _status: approve
            };
            var url = "/admin/ManageLeave";
            manageLeaveAjaxRequest(params, url);
        }
    });    
};
function manageLeaveAjaxRequest(leave_model, url) {
    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(leave_model),
        datatype: "json",
        contentType: "application/json",
        success: function (status) {
            HideLoadLeavereport("#LoadPageLvEdit", ".lvEditView");
            if (!stringIsNull(status.data)) {
                var error = '';
                var response = status.data;
                if (response == "OK") {
                    ymz.jq_alert({
                        title: "HRMS",
                        text: "Saved Successfully",
                        ok_btn: "OK",
                        close_fn: function () {
                            $('#LeaveEditBtn').show();
                            $('#LeaveUpdateBtn').hide();
                            DisableLeaveFields();
                            $('#EditLeaveModal').modal('toggle');
                            if (!IsCalendarEdit) {
                                PopulateLeaveReportTable(currMonth, currYear, false);
                                if (IsCancel) {
                                    document.getElementById("leaveStatusLabel").style.color = '#ff0000';
                                    $('#leaveStatusLabel').text("Cancelled");
                                    $('#lv_cancel_div').hide();
                                    $('#LeaveEditBtn').hide();
                                }
                            }
                            else {
                                fetchEvents();
                            }
                        }
                    });
                }
                else if (response == "EXISTS") {
                    error = "An Entry Exists In Selected Date Range !!";
                }
                else if (response == "ERROR") {
                    error = "An Unexpected Error Occured !!";
                }
                else if (response == "1" || response == "2" || response == "3" || response == "5") {
                    error = "Select Mandatory Fields !!";
                }
                else if (response == "4") {
                    error = "To Date Must Greater than equal to From Date !!";
                }
                else if (response == "6") {
                    error = "Only 9 Festive leaves can be availed per year !!";
                }
                else if (response == "7") {
                    error = "No Enough Leaves !!";
                }

                if (error) {
                    ymz.jq_alert({
                        title: "HRMS",
                        text: error,
                        ok_btn: "OK",
                        close_fn: null
                    });
                }
            }
        },
        error: function (status) {
            HideLoadLeavereport("#LoadPageLvEdit", ".lvEditView");
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !",
                ok_btn: "OK",
                close_fn: null
            });
        }
    });
};
function EnableLeaveFields() {
    $('#LvstartDateLabel').removeAttr("disabled");
    $('#LvtoDateLabel').removeAttr("disabled");
    $('#cmntsArea').removeAttr("disabled");
    $('#leaveTypeLabel').removeAttr("disabled");
    $('#LvDurTypLabel').removeAttr("disabled");
    $('#lv_cancel_chkBx').removeAttr("disabled");
    $('#LeaveSessionDropDown').removeAttr("disabled");
};
function DisableLeaveFields() {
    $('#LvstartDateLabel').attr('disabled', 'disabled');
    $('#LvtoDateLabel').attr('disabled', 'disabled');
    $('#cmntsArea').attr('disabled', 'disabled');
    $('#leaveTypeLabel').attr('disabled', 'disabled');
    $('#LvDurTypLabel').attr('disabled', 'disabled');
    $('#lv_cancel_chkBx').attr('disabled', 'disabled');
    $('#LeaveSessionDropDown').attr('disabled', 'disabled');
};
function leaveEditPopupLoading(isLoading) {
    (isLoading) ? $('#LoadPageLvEdit').show() : $('#LoadPageLvEdit').hide();
};
function leaveDetailsFetchCallback(response) {
    if (response && response.data) {
        initLeaveModal();
        DisableLeaveFields();
        $('#EditLeaveModal').modal('show');
        var data = response.data;
        $('#LeaveId').val(data._lvId);
        var lvSessionTyp = data._leaveHalfDaySession;
        //pending state.
        if (data._leaveStatus == "PENDING") {
            $('#LeaveUpdateBtn').show();
            $('#LeaveCancelBtn').show();
            $('#LeaveRejectBtn').show();
            $('#LeaveApproveBtn').show();
            $('#leaveStatusLabel').css('color', '#ff6a00');//orange color
            EnableLeaveFields();
        }
            //approved state.
        else if (data._leaveStatus == "APPROVED") {
            $('#leaveStatusLabel').css('color', '#4cff00');//green color
        }
            //rejected state.
        else if (data._leaveStatus == "REJECTED") {
            $('#cntctAdmin').show();
            $('#leaveStatusLabel').css('color', '#ff0000');//red color
        }
            //cancel state.
        else if (data._leaveStatus == "CANCELLED") {
            $('#leaveStatusLabel').css('color', '#ff0000');//red color
        }
        $("#LvstartDateLabel").datepicker({
            format: 'yyyy-mm-dd', autoclose: true, todayHighlight: false
        });
        $("#LvtoDateLabel").datepicker({
            format: 'yyyy-mm-dd', autoclose: true, todayHighlight: false
        });
        $('#LvstartDateLabel').datepicker('setDate', data._fromdate);
        $('#LvtoDateLabel').datepicker('setDate', data._todate);
        $('#cmntsArea').val(data._comments);
        $('#leaveStatusLabel').text(data._leaveStatus);
        $('#leaveTypeLabel').empty();
        $('#leaveTypeLabel').val(data._leaveType);
        $('#LvDurTypLabel').empty();
        $.each(LeaveTypList, function (i, option) {
            if (option.Value == data._leaveType) {
                $('#leaveTypeLabel').append($('<option/>').attr("value", option.Value).text(option.Text));
            }
        });
        var strLvType = null;
        switch (data._leaveType) {
            case 1:
                strLvType = "Casual";
                break;
            case 2:
                strLvType = "Festive";
                break;
            case 5:
                strLvType = "Sick";
                break;
            default:
        }
        var durId = data._leaveDurTypeInt;
        if ("Casual" == strLvType || "Sick" == strLvType) {
            $.each(LeaveDurList, function (i, option) {
                $('#LvDurTypLabel').append($('<option/>').attr("value", option.Value).text(option.Text));
            });
        }
        else if ("Festive" == strLvType) {
            $.each(LeaveDurList, function (i, option) {
                if (option.Value == durId) {
                    $('#LvDurTypLabel').append($('<option/>').attr("value", option.Value).text(option.Text));
                }
            });
        }
        $('#LvDurTypLabel').val(durId);
        (durId == 2) ? $('#LeaveSessionDropDown').val(lvSessionTyp) : $('#LeaveSessionDropDown').val(0);
        (durId == 2) ? $('#lvSessionTypDiv').removeClass('hidden') : ($('#lvSessionTypDiv').removeClass('hidden').addClass('hidden'));
    }
    leaveEditPopupLoading(false);
};
function showLoadleavereport(activDiv, disableDiv) {
    $(activDiv).show();
    $(disableDiv).addClass("disablediv");
    $(".waitIconDiv").css("display", "block");
};
function HideLoadLeavereport(activDiv, disableDiv) {
    $(activDiv).hide();
    $(disableDiv).removeClass("disablediv");
    $(".waitIconDiv").css("display", "none");
};