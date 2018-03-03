$(document).ready(function () {
    navigateTo(LEAVES);
    $('#Fromdatetimepicker').datepicker({
        format: 'yyyy-mm-dd', autoclose: true,
        todayHighlight: true
    });
    $('#Todatetimepicker').datepicker({
        format: 'yyyy-mm-dd', autoclose: true,
        todayHighlight: true
    });
    $('#Fromdatetimepicker').on('change', function () {
        var to_date_picker = $('#Todatetimepicker');
        updateDateRange($(this), to_date_picker);
    });
    /*Handling Validation Errors..*/
    $('#LeaveTypeDropDown').on('change', function () {
        var value = $('#LeaveTypeDropDown').val();

        if (value > 0) {
            $('#ErrLvType').hide();
            $('#LeaveTypeDropDown').css('border-color', '');
            $('#LvStatiImgProg').show();
            $('#leaveBalLabel').text('0.0');
            var LeaveTypeStr = $('#LeaveTypeDropDown option:selected').text().trim();
            var flag = true;
            if (LeaveTypeStr == "Festive") {
                flag = false;
            }
            GetLeaveStatistics(false, LeaveTypeStr);

            try {
                $('#LeaveDurationDropDown').empty();
                var OptHtml = '<option value="' + 0 + '">' + "Select Leave Duration" + '</option>';
                $('#LeaveDurationDropDown').append(OptHtml);

                if (flag) {
                    $.each(LeaveDurList, function (i, option) {
                        $('#LeaveDurationDropDown').append($('<option/>').attr("value", option.Value).text(option.Text));
                    });
                }
                else {
                    $.each(LeaveDurList, function (i, option) {

                        if (option.Value == 1) {
                            $('#LeaveDurationDropDown').append($('<option/>').attr("value", option.Value).text(option.Text));
                        }
                    });
                }
            }
            catch (ex) {
            }
        }
        else {
            $('#ErrLvType').show();
            $('#LeaveTypeDropDown').css('border-color', 'red');
            $('#leaveBalLabel').text("0.0");
            $('#LvStatiImgProg').hide();
        }
    });
    $('#LeaveDurationDropDown').on('change', function () {
        if (stringIsNull($('#LeaveDurationDropDown').val())
            || $('#LeaveDurationDropDown').val() < 0) {
            $('#LeaveDurationDropDown').css('border-color', 'red');
            $('#ErrLvDur').show();
        }
        else {
            $('#LeaveDurationDropDown').css('border-color', '');
            $('#ErrLvDur').hide();
            var lvDurationTyp = $(this).val();
            (lvDurationTyp == 2) ? $('#lvSessionTypDiv').removeClass('hidden') :
            $('#lvSessionTypDiv').removeClass('hidden').addClass('hidden'); $('#LeaveSessionDropDown').val(0);
        }
    });
    $('#Fromdatetimepicker').on('change blur keyup paste', function () {
        if (this.value.toString().length < 0) {
            $('#Fromdatetimepicker').css('border-color', 'red');
            $('#Errsdate').show();
        }
        else {
            $('#Fromdatetimepicker').css('border-color', '');
            $('#Errsdate').hide();
        }
    });
    $('#Todatetimepicker').on('change blur keyup paste', function () {
        if (this.value.toString().length < 0) {
            $('#Todatetimepicker').css('border-color', 'red');
            $('#Errenddate').text("Please Select Todate !!");
            $('#Errenddate').show();
        }
        else {
            $('#Todatetimepicker').css('border-color', '');
            $('#Errenddate').hide();
        }
    });
    $('#LvStatiImgProg').hide();
});
if ($('#LvStatiTable').length) {
    var LeaveStatiTable = $('#LvStatiTable').DataTable({
        "pageLength": 5, "bFilter": false,
        "bInfo": false, "bPaginate": false, "bLengthChange": false, "ordering": false,
        "searching": false, responsive: true
    });
}
var response = "";
function showLoad() {
    $("#imgProg1").show();
    $("#LvStatiTable").addClass("disablediv");
};
function HideLoad() {
    $("#imgProg1").hide();
    $("#LvStatiTable").removeClass("disablediv");
};
function ResetValues() {
    $('#leaveBalLabel').text("0.0");
    $('#LeaveTypeDropDown').val("");
    $('#LvStatiImgProg').hide();
    $('#Fromdatetimepicker').val('');
    $('#Todatetimepicker').val('');
    $('#LeaveDurationDropDown').empty();
    var OptHtml = '<option value="' + 0 + '">' + "Select Leave Duration" + '</option>';
    $('#LeaveDurationDropDown').append(OptHtml);
    $('#LeaveSessionDropDown').val(0);
    $('#lvSessionTypDiv').removeClass('hidden').addClass('hidden');
    $('#notes').val('');
    $('.waitIconDiv').hide();
};
function GetLeaveStatistics(view, leaveType) {
    showLoad();
    var currentDate = new Date();
    var date = currentDate.toDateString();
    $('#leaveStatLabel').text("Showing leave status as of : " + date);

    $.ajax({
        url: "/User/GetLeaveStatistics",
        type: "POST",
        contentType: "application/json",
        datatype: "json",
        data: JSON.stringify({ EmpId: empId }),
        success: function (status) {
            $('#LvStatiImgProg').hide();
            HideLoad();
            response = status.data;
            if (view) {
                LeaveStatiTable.clear().draw();
                LeaveStatiTable.row.add({
                    0: "Casual",
                    1: 30,
                    2: 30 - response[0].CasualLeave,
                    3: response[0].CasualLeave
                }).draw();
                LeaveStatiTable.row.add({
                    0: "Festive",
                    1: 9,
                    2: 9 - response[0].FestiveLeave,
                    3: response[0].FestiveLeave
                }).draw();
                LeaveStatiTable.row.add({
                    0: "Sick",
                    1: 14,
                    2: 14 - response[0].SickLeave,
                    3: response[0].SickLeave
                }).draw();
            }
            else {
                switch (leaveType) {
                    case "Casual":
                        $('#leaveBalLabel').text(response[0].CasualLeave);
                        break;
                    case "Festive":
                        $('#leaveBalLabel').text(response[0].FestiveLeave);
                        break;
                    case "Sick":
                        $('#leaveBalLabel').text(response[0].SickLeave);
                        break;
                    default:
                        break;
                }
            }
        },
        error: function (response) {
            alert('An unexpected error occured !!');
        }
    });
};
function ApplyLeave() {
    clearErrors();
    var LvType = $('#LeaveTypeDropDown').val();
    var FromDate = $('#Fromdatetimepicker').val();
    var ToDate = $('#Todatetimepicker').val();
    var LvDur = $('#LeaveDurationDropDown').val();
    var LvSession = $('#LeaveSessionDropDown').val();
    var Notes = $('#notes').val();
    var flag = false;
    if (LvType <= 0) {
        $('#LeaveTypeDropDown').css('border-color', 'red');
        $('#ErrLvType').show();
        flag = true;
    }
    if (LvDur <= 0) {
        $('#LeaveDurationDropDown').css('border-color', 'red');
        $('#ErrLvDur').show();
        flag = true;
    }
    if (stringIsNull(FromDate)) {
        $('#Fromdatetimepicker').css('border-color', 'red');
        $('#Errsdate').show();
        flag = true;
    }
    if (stringIsNull(ToDate)) {
        $('#Todatetimepicker').css('border-color', 'red');
        $('#Errenddate').show();
        flag = true;
    }
    if (!stringIsNull(LvDur) && 2 == LvDur && stringIsNull(LvSession)) {
        $('#LeaveSessionDropDown').css('border-color', 'red');
        $('#ErrLvSession').show();
        flag = true;
    }
    if (!flag) {
        showLoadreport("#imgProg", ".lvApplyContent");
        var StrLvTyp = $('#LeaveTypeDropDown option:selected').text();
        var LvDurStr = $('#LeaveDurationDropDown option:selected').text();

        var url = "/User/AddLeave";
        var params = {
            _lvId: 0,
            EmpID: empId,
            _fromdate: FromDate,
            _todate: ToDate,
            _leaveType: LvType,
            _leaveDurTypeInt: LvDur,
            _comments: Notes ? Notes : "",
            _strLvType: StrLvTyp,
            _leavedurationtype: LvDurStr,
            _cancelled: false,
            _leaveHalfDaySession: (2 == LvDur) ? LvSession : 0
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            datatype: "json",
            data: JSON.stringify(params),
            success: function (status) {
                HideLoadreport("#imgProg", ".lvApplyContent");
                var response = status.data;
                var msg = '';
                if (stringIsNull(response)) {
                }
                else if (response == "OK") {
                    ResetValues();
                    clearErrors();
                    //showMessageBox(SUCCESS, "Saved Successfully");
                    msg = "Saved Successfully";
                }
                else if (response == "EXISTS") {
                    //showMessageBox(WARNING, "An Entry Exists In Selected Date Range !!");
                    msg = "An Entry Exists In Selected Date Range !!";
                }
                else if (response == "ERROR") {
                    //showMessageBox(ERROR, "An Unexpected Error Occured!!");
                    msg = "An Unexpected Error Occured!!";
                }
                else if (response == "1") {
                    $('#ErrLvType').show();
                    $('#LeaveTypeDropDown').css('border-color', 'red');
                }
                else if (response == "2") {
                    $('#Errsdate').show();
                    $('#Fromdatetimepicker').css('border-color', 'red');
                }
                else if (response == "3") {
                    $('#Errenddate').text("Please Select Todate !!");
                    $('#Errenddate').show();
                    $('#Todatetimepicker').css('border-color', 'red');
                }
                else if (response == "4") {
                    $('#Errenddate').text("Todate must be greater than equal to Fromdate !!");
                    $('#Errenddate').show();
                    $('#Todatetimepicker').css('border-color', 'red');
                }
                else if (response == "5") {
                    $('#ErrLvDur').show();
                    $('#LeaveDurationDropDown').css('border-color', 'red');
                }
                else if (response == "6") {
                    //showMessageBox(WARNING, "Only 9 Festive leaves can be availed per year !!");
                    msg = "Only 9 Festive leaves can be availed per year !!";
                }
                else if (response == "7") {
                    //showMessageBox(WARNING, "No Enough Leaves !!");
                    msg = "No Enough Leaves !!";
                }

                if (msg) {
                    ymz.jq_alert({
                        title: "HRMS",
                        text: msg,
                        ok_btn: "OK",
                        close_fn: null
                    });
                }
            },
            error: function (response) {
                HideLoadreport("#imgProg", ".lvApplyContent");
            }
        });
    }
};