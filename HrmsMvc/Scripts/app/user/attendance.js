var PunchDetailsTable;
$('#lblNoResults').show();
$('#ResultTableDiv').hide();
$("#LoadPage").hide();
$(document).ready(function () {
    navigateTo(ATTENDANCE);
    $('#PunchStartDate').on('change keyup paste', function () {
        if (this.value.toString().length == 0) {
            $('#PunchStartDate').css('border-color', 'red');
            $('#lblstartDate').text('Select Start Date !')
            $('#lblstartDate').show();
        }
        else {
            var to_date_picker = $('#PunchEndDate');
            updateDateRange($(this), to_date_picker);
            $('#PunchStartDate').css('border-color', '');
            $('#lblstartDate').hide();
        }
    });
    $('#PunchEndDate').on('change keyup paste', function () {
        if (this.value.toString().length > 0) {
            $('#PunchEndDate').css('border-color', '');
            $('#lblstartDate').hide();
        }
    });
});
function checkpunchInfo() {
    clearValues();
    clearErrors();
};
function clearValues() {
    $('#PunchStartDate').val("");
    $('#PunchEndDate').val("");
    $('#ResultTableDiv').hide();
    $('#notesField').val('');
    $('#PunchStartDate').datepicker('remove');
    $('#PunchEndDate').datepicker('remove');
    $('#PunchStartDate').datepicker({
        todayHighlight: true,
        format: 'yyyy-mm-dd',
        autoclose: true
    });
    $('#PunchEndDate').datepicker({
        todayHighlight: true,
        format: 'yyyy-mm-dd',
        autoclose: true
    });
};
function punchIn() {
    showLoadreport("#imgProgAtt", ".attnCntDiv");
    var params = {
        EmpId: empId,
        punchInTime: null,
        punchOutTime: null,
        notes: $('#notesField').val(),
        type: 1,
        timeoffset: offset
    };
    var AddAttendance = {};
    AddAttendance.url = "/User/AddAttendance";
    AddAttendance.type = "POST";
    AddAttendance.data = JSON.stringify(params);
    AddAttendance.datatype = "json";
    AddAttendance.contentType = "application/json";
    AddAttendance.success = function (status) {
        setCurrentDateTime();
        $('#notesField').val('');
        var response = status.data;
        HideLoadreport("#imgProgAtt", ".attnCntDiv");
        if (!response) {
            //showMessageBox(ERROR, "An Unexpected Error Occured!!");
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !",
                ok_btn: "OK",
                close_fn: null
            });
        }
        if (response["ErrorString"].length == 0) {
            $('#punhInBtn').hide();
            $('#punhOutBtn').show();
            $('#PunchedInTime').text(response["PunchinTime"]);
            $('#punchdInDiv').show();
            $('#punchdOutDiv').hide();
        }
        else if (response["ErrorString"].length > 0) {
            //showMessageBox(ERROR, "An Unexpected Error Occured!!");
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !",
                ok_btn: "OK",
                close_fn: null
            });
        }
    };
    AddAttendance.error = function () {
        HideLoadreport("#imgProgAtt", ".attnCntDiv");
        //showMessageBox(ERROR, "An Unexpected Error Occured!!");
        ymz.jq_alert({
            title: "HRMS",
            text: "An unexpected error occured !",
            ok_btn: "OK",
            close_fn: null
        });
    };
    $.ajax(AddAttendance);
};
function punchOut() {
    showLoadreport("#imgProgAtt", ".attnCntDiv");
    var pin = $('#PunchedInTime').text();
    var params = {
        EmpId: empId,
        punchInTime: pin,
        punchOutTime: null,
        notes: $('#notesField').val(),
        type: 2,
        timeoffset: offset
    };
    var AddAttendance = {};
    AddAttendance.url = "/User/AddAttendance";
    AddAttendance.type = "POST";
    AddAttendance.data = JSON.stringify(params);
    AddAttendance.datatype = "json";
    AddAttendance.contentType = "application/json";
    AddAttendance.success = function (status) {
        setCurrentDateTime();
        $('#notesField').val('');
        var response = status.data;
        HideLoadreport("#imgProgAtt", ".attnCntDiv");
        if (!response) {
            //showMessageBox(ERROR, "An Unexpected Error Occured!!");
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !",
                ok_btn: "OK",
                close_fn: null
            });
        }
        if (!response["ErrorString"].length) {
            $('#punhInBtn').show();
            $('#punhOutBtn').hide();
            $('#PunchedInTime').text(response["PunchinTime"]);
            $('#PunchedOutTime').text(response["PunchoutTime"]);
            $('#punchdInDiv').hide();
            $('#punchdOutDiv').hide();
        }
        else if (response["ErrorString"].length) {
            //showMessageBox(ERROR, "An Unexpected Error Occured!!");
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !",
                ok_btn: "OK",
                close_fn: null
            });
        }
    };
    AddAttendance.error = function () {
        HideLoadreport("#imgProgAtt", ".attnCntDiv");
        //showMessageBox(ERROR, "An Unexpected Error Occured!!");
        ymz.jq_alert({
            title: "HRMS",
            text: "An unexpected error occured !",
            ok_btn: "OK",
            close_fn: null
        });
    };
    $.ajax(AddAttendance);
};
function getAttendanceData() {
    if (PunchDetailsTable != undefined) {
        PunchDetailsTable.destroy();
    }
    $('#lblNoResults').hide();
    $('#ResultTableDiv').show();
    HideLoadreport("#LoadPageAttnRprt", "#attnRprtDiv");
    PunchDetailsTable = $('#punchTable').DataTable({
        "pageLength": 7, "processing": true, "serverSide": true,
        "paging": true, "bLengthChange": false, "searching": false,
        "bInfo": true,
        drawCallback: function (settings) {
            var pagination = $(this).closest('.dataTables_wrapper').find('.dataTables_paginate');
            pagination.toggle(this.api().page.info().pages > 1);
        },
        "ajax": {
            "url": "/User/SearchPunchDetails",
            "type": "POST",
            "dataType": "JSON",
            "data": function (d) {
                d.EmpId = empId;
                d.StartDate = $('#PunchStartDate').val();
                d.EndDate = $('#PunchEndDate').val();
                d.timeoffset = offset;
            }
        }
        , "columnDefs": [
            {
                "render": function (data, type, row) {
                    return row["PunchinTime"].split(' ')[0]
                },
                "targets": 0
            }
        ]
        , "columns": [
            { "data": "PunchinTime" },
            { "data": "PunchinTime" },
            { "data": "PunchoutTime" },
            { "data": "Duration" }
        ]
         , "language":
            {
                "processing": "<div class='row text-center waitIcon'><i class='fa fa-refresh fa-spin fa-2x fa-fw'></i></div>"
            }
    });
};
function SearchAttendance() {
    clearErrors();
    var startDate = $('#PunchStartDate').val();
    var endDate = $('#PunchEndDate').val();
    var sDate = new Date(startDate);
    var eDate = new Date(endDate);
    var currentDate = new Date();
    var flag = false;
    if ((startDate.length == 0 && endDate.length == 0) || (startDate.length == 0 && endDate.length != 0)) {
        $('#PunchStartDate').css('border-color', 'red');
        $('#lblstartDate').text('Select Start date !!');
        $('#lblstartDate').show();
        flag = true;
    }
    else if (sDate > eDate) {
        $('#PunchStartDate').css('border-color', 'red');
        $('#lblstartDate').text('Start date must be less than or equal to End date !!');
        $('#lblstartDate').show();
        flag = true;
    }
    else if (eDate > currentDate) {
        $('#PunchEndDate').css('border-color', 'red');
        $('#lblEndDate').text('End date must be less than or equal to Current Date !!');
        $('#lblEndDate').show();
        flag = true;
    }
    if (!flag) {
        showLoadreport("#LoadPageAttnRprt", "#attnRprtDiv");
        blockNumber = 1;
        getAttendanceData();
    }
};