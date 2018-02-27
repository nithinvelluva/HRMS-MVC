$(document).ready(function () {
    navigateTo(PROFILE);
    DisableFields();
    initPage();
    $('#EmpDateOfBirth').datepicker({
        format: 'yyyy-mm-dd', autoclose: true
    });    
    $('#EmpDateOfBirth').val($('#EmpDateOfBirth').val());
});
function DisableFields() {
    $('#EmpId').attr('disabled', 'disabled');
    $('#EmpDesig').attr('disabled', 'disabled');
    $('#EmpGender').attr('disabled', 'disabled');
    $('.input-field-profile').attr('disabled', 'disabled');
}
function EnableFields() {
    $('.input-field-profile').removeAttr('disabled');
}
$('#userPassword').on('change keyup paste blur', function () {
    if (this.value.toString().length < 1) {
        $('#userPassword').css('border-color', 'red');
        $('#lblpasswordSepCnfrm').text("Enter Password !!");
        $('#lblpasswordSepCnfrm').show();
    }
    else {
        $('#userPassword').css('border-color', '');
        $('#lblpasswordSepCnfrm').hide();
    }
});
$('#editBtn').click(function () {
    $('#editBtn').hide();
    $('#updateBtn').show();
    EnableFields();
})
$('#userFormSubmitBtn').click(function () {
    var empId = $('#EmpId').val();
    var userName = $('#UserName').val();
    var fisrtName = $('#EmpFirstName').val();
    var lastName = $('#EmpLastName').val();
    var empName = fisrtName + " " + lastName;
    var dob = $('#EmpDateOfBirth').val();
    var pwd = $('#Password').val();
    var cnfrmPwd = $('#userPassword').val();
    var email = $('#EmailId').val();
    var phone = $('#PhoneNumber').val();
    var gender = $('#SelGender').val();
    var role = $('#SelRole').val();

    showLoadreport("#LoadPageProf", "#profSubmitDiv");

    var patDetails = {};
    patDetails.url = "/User/ProfileDetails";
    patDetails.type = "POST";
    patDetails.data = JSON.stringify({ Id: empId, firstName: fisrtName, lastName: lastName, empname: empName, dob: dob, gender: gender, role: role, username: userName, cnfPwd: cnfrmPwd, email: email, phone: phone });
    patDetails.datatype = "json";
    patDetails.contentType = "application/json";
    patDetails.success = function (status) {
        HideLoadreport("#LoadPageProf", "#profSubmitDiv");
        if (status && status.data) {
            var response = status.data;
            if (stringIsNull(response.ErrorString)) {
                clearErrors();
                $('#updateBtn').hide();
                $('#editBtn').show();
                DisableFields();
                $('#lblpasswordSepCnfrm').hide();
                $('#userPassword').val('');
                $('#UserProfileSubmitModel').modal('toggle');
                //showMessageBox(SUCCESS, "Profile Updated Successfully.");
                ymz.jq_alert({
                    title: "HRMS",
                    text: "Profile Updated Successfully.",
                    ok_btn: "OK",
                    close_fn: function () {
                        location.reload();
                    }
                });
            }
            else {
                try {
                    var errlst = response.ErrorString.split(":");
                    var errcnt = errlst.length;
                    for (var i = 0; i < errcnt - 1; i++) {
                        if (errlst[i] == 1) {
                            $('#UserProfileSubmitModel').modal('toggle');
                            $('#lblempFirstname').show();
                            $('#EmpFirstName').css('border-color', 'red');
                        }
                        else if (errlst[i] == 2) {
                            $('#UserProfileSubmitModel').modal('toggle');
                            $('#lblempLastname').show();
                            $('#EmpLastName').css('border-color', 'red');
                        }
                        else if (errlst[i] == 3) {
                            $('#UserProfileSubmitModel').modal('toggle');
                            $('#lblgender').show();
                        }
                        else if (errlst[i] == 4) {
                            $('#UserProfileSubmitModel').modal('toggle');
                            $('#ErrlblDateOfBirth').show();
                            $('#EmpDateOfBirth').css('border-color', 'red');
                        }
                        else if (errlst[i] == 5) {
                            $('#UserProfileSubmitModel').modal('toggle');
                            $('#lblusername').show();
                            $('#UserName').css('border-color', 'red');
                        }
                        else if (errlst[i] == 7) {
                            $('#lblpasswordSepCnfrm').text('Enter Password !!')
                            $('#lblpasswordSepCnfrm').show();
                            $('#userPassword').css('border-color', 'red');
                        }
                        else if (errlst[i] == "PM") {
                            $('#lblpasswordSepCnfrm').text('Password Mismatch !!')
                            $('#lblpasswordSepCnfrm').show();
                            $('#userPassword').css('border-color', 'red');
                        }
                        else if (errlst[i] == 9) {
                            $('#ErrlblDateOfBirth').text('Age must be greater than 18 !!')
                            $('#ErrlblDateOfBirth').show();
                            $('#EmpDateOfBirth').css('border-color', 'red');
                        }
                        else if (errlst[i] == "EN") {
                            $('#lblEmailId').text("Enter Email Id !!");
                            $('#lblEmailId').show();
                            $('#EmailId').css('border-color', 'red');
                        }
                        else if (errlst[i] == "ENV") {
                            $('#lblEmailId').text("Enter Valid Email Id !!");
                            $('#lblEmailId').show();
                            $('#EmailId').css('border-color', 'red');
                        }
                        else if (errlst[i] == "PN") {
                            $('#lblphonenumber').text("Enter PhoneNumber !!");
                            $('#lblphonenumber').show();
                            $('#PhoneNumber').css('border-color', 'red');
                        }
                        else if (errlst[i] == "PNV") {
                            $('#lblphonenumber').text("Enter Valid PhoneNumber !!");
                            $('#lblphonenumber').show();
                            $('#PhoneNumber').css('border-color', 'red');
                        }
                        else if (errlst[i] == "ERROR") {
                            //showMessageBox(ERROR, "An unexpected error occured !!");
                            ymz.jq_alert({
                                title: "HRMS",
                                text: "An unexpected error occured !!",
                                ok_btn: "OK",
                                close_fn: null
                            });
                        }
                    }
                }
                catch (ex) {
                }
            }
        }
    };
    patDetails.error = function () {
        HideLoadreport("#LoadPageProf", "#profSubmitDiv");
        //showMessageBox(ERROR, "An unexpected error occured !!");
        ymz.jq_alert({
            title: "HRMS",
            text: "An unexpected error occured !!",
            ok_btn: "OK",
            close_fn: null
        });
    };
    $.ajax(patDetails);
})
$('#updateBtn').click(function () {
    var empId = $('#EmpId').val();
    var fisrtName = $('#EmpFirstName').val();
    var lastName = $('#EmpLastName').val();
    var empName = fisrtName + " " + lastName;
    var dob = $('#EmpDateOfBirth').val();
    var userName = $('#UserName').val();
    var email = $('#EmailId').val();
    var phone = $('#PhoneNumber').val();
    var gender = $('#SelGender').val();
    var role = $('#SelRole').val();

    var validFlag = false;

    if (stringIsNull(fisrtName)) {
        $('#lblempFirstname').show();
        $('#EmpFirstName').css('border-color', 'red');
        validFlag = true;
    }
    if (stringIsNull(lastName)) {
        $('#lblempLastname').show();
        $('#EmpLastName').css('border-color', 'red');
        validFlag = true;
    }
    if (stringIsNull(dob)) {
        $('#EmpDateOfBirth').css('border-color', 'red');
        $('#ErrlblDateOfBirth').text("Select Date Of Birth");
        $('#ErrlblDateOfBirth').show();
        validFlag = true;
    }
    if (stringIsNull(userName)) {
        $('#UserName').css('border-color', 'red');
        $('#lblusername').show();
        validFlag = true;
    }
    if (stringIsNull(email)) {
        $('#EmailId').css('border-color', 'red');
        $('#lblEmailId').text("Enter Email Id !!");
        $('#lblEmailId').show();
        validFlag = true;
    }
    if (stringIsNull(phone)) {
        $('#PhoneNumber').css('border-color', 'red');
        $('#lblphonenumber').show();
        validFlag = true;
    }
    if (!validFlag) {
        clearErrors();
        $('#userPassword').val('');
        $('#UserProfileSubmitModel').modal('show');
    }
})
$('#UserName').on('blur', function () {
    if (this.value.toString().length == 0) {
        $('#UserName').css('border-color', 'red');
        $('#lblusername').show();
    }
    else {
        $('#UserName').css('border-color', '');
        $('#lblusername').hide();
    }
});
$('#EmpFirstName').on('blur', function () {
    if (this.value.toString().length == 0) {
        $('#EmpFirstName').css('border-color', 'red');
        $('#lblempFirstname').show();
    }
    else {
        $('#EmpFirstName').css('border-color', '');
        $('#lblempFirstname').hide();
    }
});
$('#EmpLastName').on('blur', function () {
    if (this.value.toString().length == 0) {
        $('#EmpLastName').css('border-color', 'red');
        $('#lblempLastname').show();
    }
    else {
        $('#EmpLastName').css('border-color', '');
        $('#lblempLastname').hide();
    }
});
$('#EmpDateOfBirth').on('blur', function () {
    if (this.value.toString().length == 0) {
        $('#EmpDateOfBirth').css('border-color', 'red');
        $('#ErrlblDateOfBirth').text("Select Date Of Birth");
        $('#ErrlblDateOfBirth').show();
    }
    else {
        $('#EmpDateOfBirth').css('border-color', '');
        $('#ErrlblDateOfBirth').hide();
    }
});
$('#EmailId').on('blur', function () {
    if (this.value.toString().length == 0) {
        $('#EmailId').css('border-color', 'red');
        $('#lblEmailId').text("Enter Email Id !!");
        $('#lblEmailId').show();
    }
    else {
        $('#EmailId').css('border-color', '');
        $('#lblEmailId').hide();
    }
});
$('#PhoneNumber').on('blur', function () {
    if (this.value.toString().length == 0) {
        $('#PhoneNumber').css('border-color', 'red');
        $('#lblphonenumber').text("Enter Phone Number !!");
        $('#lblphonenumber').show();
    }
    else {
        $('#PhoneNumber').css('border-color', '');
        $('#lblphonenumber').hide();
    }
});