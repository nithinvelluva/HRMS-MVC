$(document).ready(function () {
    navigateTo(USERS);
    $('#EmpDateOfBirth').datepicker({
        format: 'yyyy-mm-dd', autoclose: true
    });
    $("#addEmpform").submit(function (event) {
        event.preventDefault();
        clearValidationError();
        var empfirstname = $('#EmployeeFirstName').val();
        var emplastname = $('#EmployeeLastName').val();
        var gender = $('#Gender').val();
        var utype = $('#roleDropDwn').val();
        var desig = $('#desigDrpDwn').val();
        var dob = $('#EmpDateOfBirth').val();
        var phone = $('#EmpPhoneNumber').val();
        var email = $('#EmpEmailId').val();

        var validFlag = false;
        if (stringIsNull(empfirstname)) {
            $('#EmployeeFirstName').css('border-color', 'red');
            $('#lblempfirstname').show();
            validFlag = true;
        }
        if (stringIsNull(gender)) {
            $('#lblgender').show();
            validFlag = true;
        }
        if (stringIsNull(utype)) {
            $('#roleDropDwn').css('border-color', 'red');
            $('#lbluserrole').show();
            validFlag = true;
        }
        if (stringIsNull(desig)) {
            $('#desigDrpDwn').css('border-color', 'red');
            $('#lblempDesig').show();
            validFlag = true;
        }
        if (stringIsNull(dob)) {
            $('#EmpDateOfBirth').css('border-color', 'red');
            $('#ErrlblDateOfBirth').text('Select Date of birth !')
            $('#ErrlblDateOfBirth').show();
            validFlag = true;
        }
        if (stringIsNull(phone)) {
            $('#EmpPhoneNumber').css('border-color', 'red');
            $('#lblphonenumber').show();
            validFlag = true;
        }
        if (stringIsNull(email)) {
            $('#EmpEmailId').css('border-color', 'red');
            $('#lblEmailId').show();
            validFlag = true;
        }

        if (!validFlag) {
            var addEmployee = {};
            addEmployee.url = "/Admin/ManageEmployee";
            addEmployee.type = "POST";
            addEmployee.data = JSON.stringify({
                EmpFirstname: empfirstname, EmpLastname: emplastname, Gender: gender, Usertype: utype, DesigType: desig,
                DateOfBirth: dob, EmailId: email, PhoneNumber: phone
            });
            addEmployee.datatype = "json";
            addEmployee.contentType = "application/json";
            addEmployee.success = function (status) {
                if (status && status.data) {
                    var response = status.data;
                    clearValidationError();
                    var errlst = response.split(":");
                    var errcnt = errlst.length;
                    for (var i = 0; i <= errcnt; i++) {

                        if (errlst[i] == 1) {
                            $('#EmpName').css('border-color', 'red');
                            $('#lblempfirstname').show();
                        }
                        else if (errlst[i] == 2) {
                            $('#lblgender').show();
                        }
                        else if (errlst[i] == 3) {
                            $('#roleDropDwn').css('border-color', 'red');
                            $('#lbluserrole').show();
                        }
                        else if (errlst[i] == 4) {
                            $('#EmpDateOfBirth').css('border-color', 'red');
                            $('#ErrlblDateOfBirth').show();
                        }
                        else if (errlst[i] == "D") {
                            $('#desigDrpDwn').css('border-color', 'red');
                            $('#lblempDesig').show();
                        }
                        else if (errlst[i] == 9) {
                            $('#EmpDateOfBirth').css('border-color', 'red');
                            $('#ErrlblDateOfBirth').text('Age must be greater than 18 !')
                            $('#ErrlblDateOfBirth').show();
                        }
                        else if (errlst[i] == "EN") {
                            $('#EmpEmailId').css('border-color', 'red');
                            $('#lblEmailId').show();
                        }
                        else if (errlst[i] == "ENV") {
                            $('#EmpEmailId').css('border-color', 'red');
                            $('#lblEmailId').text("Enter Valid Email Id !");
                            $('#lblEmailId').show();
                        }
                        else if (errlst[i] == "PN") {
                            $('#EmpPhoneNumber').css('border-color', 'red');
                            $('#lblphonenumber').show();
                        }
                        else if (errlst[i] == "PNV") {
                            $('#EmpPhoneNumber').css('border-color', 'red');
                            $('#lblphonenumber').text("Enter Valid PhoneNumber !");
                            $('#lblphonenumber').show();
                        }
                        else if (errlst[i] == "OK") {
                            ymz.jq_alert({
                                title: "HRMS",
                                text: "Employee added successfully.",
                                ok_btn: "OK",
                                close_fn: function () {
                                    location.href = "/admin/users";
                                }
                            });
                        }
                        else if (errlst[i] == "ERROR") {
                            ymz.jq_alert({
                                title: "HRMS",
                                text: "An unexpected error occured !",
                                ok_btn: "OK",
                                close_fn: null
                            });
                        }
                        else if (errlst[i] == "PD") {
                            $('#EmpPhoneNumber').css('border-color', 'red');
                            $('#lblphonenumber').text("Phone number already associated with another account !");
                            $('#lblphonenumber').show();
                        }
                        else if (errlst[i] == "ED") {
                            $('#EmpEmailId').css('border-color', 'red');
                            $('#lblEmailId').text("Email Id already associated with another account !");
                            $('#lblEmailId').show();
                        }
                        else if (errlst[i] == "IDOB") {
                            $('#EmpDateOfBirth').css('border-color', 'red');
                            $('#ErrlblDateOfBirth').text('Invalid date time format !');
                            $('#ErrlblDateOfBirth').show();
                        }
                    }
                }
            };
            addEmployee.error = function () {
                //showMessageBox(ERROR, "An unexpected error occured!");
                ymz.jq_alert({
                    title: "HRMS",
                    text: "An unexpected error occured !",
                    ok_btn: "OK",
                    close_fn: null
                });
            };
            $.ajax(addEmployee);
        }
    });
    $('#Empgendermale').change(function () {
        var checked = $(this).is(':checked');
        $('#Gender').val((checked) ? 'M' : 'F');
        $('#Empgenderfemale').prop('checked', !checked);
        $('#lblgender').hide();
    });
    $('#Empgenderfemale').change(function () {
        var checked = $(this).is(':checked');
        $('#Gender').val((checked) ? 'F' : 'M');
        $('#Empgendermale').prop('checked', !checked);
        $('#lblgender').hide();
    });
    $('#EmployeeFirstName').on('blur', function () {
        if (this.value.toString().length == 0) {
            $('#EmployeeFirstName').css('border-color', 'red');
            $('#lblempfirstname').show();
        }
        else {
            $('#EmployeeFirstName').css('border-color', '');
            $('#lblempfirstname').hide();
        }
    });
    $('#desigDrpDwn').on('blur change', function () {
        if (this.value.toString().length == 0) {
            $('#desigDrpDwn').css('border-color', 'red');
            $('#lblempDesig').show();
        }
        else {
            $('#desigDrpDwn').css('border-color', '');
            $('#lblempDesig').hide();
        }
    });
    $('#roleDropDwn').on('blur change', function () {
        if (this.value.toString().length == 0) {
            $('#roleDropDwn').css('border-color', 'red');
            $('#lbluserrole').show();
        }
        else {
            $('#roleDropDwn').css('border-color', '');
            $('#lbluserrole').hide();
        }
    });
    $('#EmpDateOfBirth').on('change blur', function () {
        if (this.value.toString().length <= 0) {
            $('#EmpDateOfBirth').css('border-color', 'red');
            $('#ErrlblDateOfBirth').text("Select Date Of Birth");
            $('#ErrlblDateOfBirth').show();
        }
        else {
            $('#EmpDateOfBirth').css('border-color', '');
            $('#ErrlblDateOfBirth').hide();
        }
    });
    $('#EmpEmailId').on('blur', function () {
        if (this.value.toString().length == 0) {
            $('#EmpEmailId').css('border-color', 'red');
            $('#lblEmailId').text("Enter Email Id !!");
            $('#lblEmailId').show();
        }
        else {
            $('#EmpEmailId').css('border-color', '');
            $('#lblEmailId').hide();
        }
    });
    $('#EmpPhoneNumber').on('blur', function () {
        if (this.value.toString().length == 0) {
            $('#EmpPhoneNumber').css('border-color', 'red');
            $('#lblphonenumber').text("Enter Phone Number !");
            $('#lblphonenumber').show();
        }
        else {
            $('#EmpPhoneNumber').css('border-color', '');
            $('#lblphonenumber').hide();
        }
    });    
});
function showLoadprofile() {
    $('#loadingDivEdit').show();
    $('.profile-content').addClass("disablediv");
    $("#waitIconEmpEdit").css("display", "block");
};
function HideLoadprofile() {
    $('#loadingDivEdit').hide();
    $('.profile-content').removeClass("disablediv");
    $("#waitIconEmpEdit").css("display", "none");
};
function ClearValues() {
    $('.inputField').val('');
    $('#Empgendermale').prop('checked', false);
    $('#Empgenderfemale').prop('checked', false);
    $('#SelGender').val('');
    $('#roleDropDwn').val(0);
    $('#desigDrpDwn').val(0);
    clearValidationError();
};
function empEdit() {
    $('#desigDrpDwnEdit').css('border-color', '');
    $('#roleDropDwnEdit').css('border-color', '');
    $('#lbluserroleEdit').hide();
    $('#lblempDesigEdit').hide();

    var role = $('#roleDropDwnEdit').val();
    var desig = $('#desigDrpDwnEdit').val();
    if (role <= 0) {
        $('#roleDropDwnEdit').css('border-color', 'red');
        $('#lbluserroleEdit').show();
    }
    if (desig <= 0) {
        $('#desigDrpDwnEdit').css('border-color', 'red');
        $('#lblempDesigEdit').show();
    }
    if (role > 0 && desig > 0) {
        var cfm = true;
        var params = {
            EmpID: ($('#EMPIDEdit').val()) ? parseInt($('#EMPIDEdit').val()) : 0,
            Usertype: role,
            DesigType: desig
        };
        var url = "/Admin/editEmpDetails";
        var callback = empEditCallback;
        if (1 == role) {
            var message = "Are you sure want to modify this user as admin ?";
            var title = "HRMS";
            ymz.jq_confirm({
                title: !stringIsNull(title) ? title : "HRMS",
                text: message,
                no_btn: "No",
                yes_btn: "Yes",
                no_fn: function () {
                    $('#roleDropDwnEdit').val(2);
                },
                yes_fn: function () {
                    AjaxCall(url, params, callback, HideLoadprofile);
                }
            });
        }
        else {
            AjaxCall(url, params, callback, HideLoadprofile);
        }
    }
};
function empDelete() {
    var empid = $('#EMPIDEdit').val();
    if (empid > 0) {
        var params = { empid: empid };
        var url = "/Admin/RemoveEmployee";
        var callback = empEditCallback;
        var message = "Are you sure want to remove this user ?";
        var title = "HRMS";
        ymz.jq_confirm({
            title: !stringIsNull(title) ? title : "HRMS",
            text: message,
            no_btn: "No",
            yes_btn: "Yes",
            no_fn: function () {
            },
            yes_fn: function () {
                AjaxCall(url, params, callback, HideLoadprofile);
            }
        });
    }
};
function empEditCallback(data) {
    if (!stringIsNull(data)) {
        var response = data.data;
        if ("OK" == response) {
            ymz.jq_alert({
                title: "HRMS",
                text: "Employee details updated",
                ok_btn: "OK",
                close_fn: function () {
                    location.reload();
                }
            });
        }
        else if ("DELETED" == response) {
            ymz.jq_alert({
                title: "HRMS",
                text: "Employee removed successfully",
                ok_btn: "OK",
                close_fn: function () {
                    location.href = "/admin/users";
                }
            });
        }
        else if ("ERROR" == response) {
            ymz.jq_alert({
                title: "An unexpected error occured!",
                text: msg,
                ok_btn: "OK",
                close_fn: null
            });
        }
        else {
            window.location.href = "/login";
        }
    }
};