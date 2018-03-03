var currYear;
var currMonth;
var LeaveDetailsTable;
$(document).ready(function () {
    navigateTo(LEAVEREPORT);
    $("#chngDatePicker").datepicker({
        format: "MM yyyy",
        viewMode: "months",
        minViewMode: "months",
        todayHighlight: true
    });
});
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