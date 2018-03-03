var currYear;
var currMonth;
var IsCancel = false;
var LeaveDetailsTable;
$(document).ready(function () {
    navigateTo(LEAVEREPORT);
    $('#lv_cancel_chkBx').on('change', function () {
        var isCancel = $(this).is(':checked');
        if (isCancel) {
            var message = "Are you sure want to cancel the leave ?";
            var title = "HRMS";
            ymz.jq_confirm({
                title: !stringIsNull(title) ? title : "HRMS",
                text: message,
                no_btn: "No",
                yes_btn: "Yes",
                no_fn: function () {
                    $('#lv_cancel_chkBx').prop('checked', false);
                },
                yes_fn: function () {
                    $('#leaveStatusLabel').text(isCancel ? "Cancelled" : "Pending");
                    $('#leaveStatusLabel').css('color', (isCancel) ? '#ff0000' : '#ff6a00');
                }
            });
        }
        else {
            $('#leaveStatusLabel').text(isCancel ? "Cancelled" : "Pending");
            $('#leaveStatusLabel').css('color', (isCancel) ? '#ff0000' : '#ff6a00');
        }
    });
    $("#chngDatePicker").datepicker({
        format: "MM yyyy",
        viewMode: "months",
        minViewMode: "months",
        todayHighlight: true
    });
    $(document).on('click', '.ViewLeaveDetails', function () {
        DisableLeaveFields();
        $('#lv_cancel_chkBx').prop('checked', false);
        $('#LeaveId').val($(this).attr('data-id'));
        $('#LeaveUpdateBtn').hide();
        $('#cntctAdmin').hide();
        var isEdit = $(this).attr('data-IsEdit');
        var isRejected = $(this).attr('data-IsRejected');
        var isCancel = $(this).attr('data-IsCancel');
        var lvSessionTyp = $(this).attr('data-lvsessiontyp');
        //pending state.
        if (isEdit == "true" && isRejected == "false" && isCancel == "false") {
            $('#LeaveEditBtn').show();
            $('#cntctAdmin').hide();
            $('#lv_cancel_div').show();
            $('#leaveStatusLabel').css('color', '#ff6a00');//orange color
        }
            //approved state.
        else if (isEdit == "false" && isRejected == "false" && isCancel == "false") {
            $('#LeaveEditBtn').hide();
            $('#cntctAdmin').hide();
            $('#lv_cancel_div').hide();
            $('#leaveStatusLabel').css('color', '#4cff00');//green color
        }
            //rejected state.
        else if (isEdit == "false" && isRejected == "true" && isCancel == "false") {
            $('#LeaveEditBtn').hide();
            $('#cntctAdmin').show();
            $('#lv_cancel_div').hide();
            $('#leaveStatusLabel').css('color', '#ff0000');//red color
        }
            //cancel state.
        else if (isEdit == "false" && isRejected == "false" && isCancel == "true") {
            $('#LeaveEditBtn').hide();
            $('#cntctAdmin').hide();
            $('#lv_cancel_div').hide();
            $('#leaveStatusLabel').css('color', '#ff0000');//red color
        }

        $("#LvstartDateLabel").datepicker({
            format: 'yyyy-mm-dd', autoclose: true, todayHighlight: true
        });
        $("#LvtoDateLabel").datepicker({
            format: 'yyyy-mm-dd', autoclose: true, todayHighlight: true
        });
        $('#LvstartDateLabel').datepicker('setDate', $(this).attr('data-fromDate'));
        $('#LvtoDateLabel').datepicker('setDate', $(this).attr('data-toDate'));
        $('#cmntsArea').val($(this).attr('data-commnets'));
        $('#leaveStatusLabel').text($(this).attr('data-LeaveStatus'));

        $('#leaveTypeLabel').empty();
        var id = $(this).attr('data-leaveTypeInt');
        $('#leaveTypeLabel').val(id);
        $('#LvDurTypLabel').empty();
        $.each(LeaveTypList, function (i, option) {
            if (option.Value == id) {
                $('#leaveTypeLabel').append($('<option/>').attr("value", option.Value).text(option.Text));
            }
        });
        var strLvType = ($(this).attr('data-LeaveType')).trim();
        var durId = $(this).attr('data-lvDurTypint');
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
    });
    $('#SentLeaveQueryModal').on('shown.bs.modal', function () {
        $('#senterEmail').val("nithinvelluva@gmail.com");
        $('#Emailsubject').val('');
        $('#EmailBody').val('');
    });
    $('#LvDurTypLabel').on('change', function () {
        if (stringIsNull($(this).val())
            || $(this).val() < 0) {
            $(this).css('border-color', 'red');
            $('#ErrLvDur').show();
        }
        else {
            $(this).css('border-color', '');            
            var lvDurationTyp = $(this).val();
            (lvDurationTyp == 2) ? $('#lvSessionTypDiv').removeClass('hidden') :
            $('#lvSessionTypDiv').removeClass('hidden').addClass('hidden'); $('#LeaveSessionDropDown').val(0);
        }
    });
});
function LeaveEditClick() {
    $('#LeaveEditBtn').hide();
    $('#LeaveUpdateBtn').show();
    EnableLeaveFields();
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
    IsCancel = $('#lv_cancel_chkBx').is(':checked');
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
        showLoadreport("#LoadPageLvEdit", ".lvEditView");
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
            _cancelled: IsCancel,
            _leaveHalfDaySession: (2 == lvdurtyp) ? LvSession : 0
        };
        $.ajax({
            url: "/User/AddLeave",
            type: "POST",
            data: JSON.stringify(params),
            datatype: "json",
            contentType: "application/json",
            success: function (status) {
                HideLoadreport("#LoadPageLvEdit", ".lvEditView");
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
                                PopulateLeaveReportTable(currMonth, currYear, false);
                                if (IsCancel) {
                                    document.getElementById("leaveStatusLabel").style.color = '#ff0000';
                                    $('#leaveStatusLabel').text("Cancelled");
                                    $('#lv_cancel_div').hide();
                                    $('#LeaveEditBtn').hide();
                                }
                                $('#EditLeaveModal').modal('toggle');
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
                HideLoadreport("#LoadPageLvEdit", ".lvEditView");
                ymz.jq_alert({
                    title: "HRMS",
                    text: "An unexpected error occured !",
                    ok_btn: "OK",
                    close_fn: null
                });
            }
        });
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
function ChngClick() {
    clearErrors();
    if (stringIsNull($("#chngDatePicker").val())) {
        $('#custom-search-input').css('border-color', 'red');
        $('#lblChnDate').text("Select a date !!");
        $('#lblChnDate').show();
    }
    else {
        $('#imgProgLvRprt').show();
        setLeaveReportsInfo(false);
        PopulateLeaveReportTable();
    }
};