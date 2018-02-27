$(document).ready(function () {
    navigateTo(CHANGEPASSWORD);
});
function chngPwdSubmit() {
    clearErrors();
    var currntPwd = $('#currntPwd').val();
    var nwPwd = $('#nwPwd').val();
    var nwPwdCnfm = $('#nwPwdCnfrm').val();
    var validFlag = true;
    if (stringIsNull(currntPwd)) {
        $('#currntPwd').css('border-color', 'red');
        $('#ErrPwdCurr').text("Enter current password !!");
        $('#ErrPwdCurr').show();
        validFlag = false;
    }
    if (stringIsNull(nwPwd)) {
        $('#nwPwd').css('border-color', 'red');
        $('#ErrnwPwd').text("Enter new password !!");
        $('#ErrnwPwd').show();
        validFlag = false;
    }
    if (stringIsNull(nwPwdCnfm)) {
        $('#nwPwdCnfrm').css('border-color', 'red');
        $('#ErrnwPwdCnfm').text("Enter new password again to confirm !!");
        $('#ErrnwPwdCnfm').show();
        validFlag = false;
    }
    if (validFlag) {
        if (nwPwd != nwPwdCnfm) {
            $('#nwPwdCnfrm').css('border-color', 'red');
            $('#ErrnwPwdCnfm').text("Confirm Password doesn't match !!");
            $('#ErrnwPwdCnfm').show();
        }
        else {
            showLoadreport("#LoadPageChngpwd", ".chngPwdFormDiv");
            var url = "/User/ChangeUserPassword";
            var params = { empId: empId, currPwd: currntPwd, nwPwd: nwPwd, nwPwdCnfm: nwPwdCnfm };
            var callback = ChangeUserPasswordCallback;
            ajaxRequest(url, params, callback);
        }
    }
};
function ChangeUserPasswordCallback(status) {
    HideLoadreport("#LoadPageChngpwd", ".chngPwdFormDiv");
    if (!status || !status.data) {
        //showMessageBox(ERROR, "An unexpected error occured!!");
        ymz.jq_alert({
            title: "HRMS",
            text: "An unexpected error occured !",
            ok_btn: "OK",
            close_fn: null
        });
    }
    else {
        try {
            var errlst = status.data.split(":");
            var errcnt = errlst.length;

            for (var i = 0; i <= errcnt - 1; i++) {
                if (errlst[i] == "ERROR") {
                    //showMessageBox(ERROR, "An unexpected error occured !!");
                    ymz.jq_alert({
                        title: "HRMS",
                        text: "An unexpected error occured !",
                        ok_btn: "OK",
                        close_fn: null
                    });
                }
                else if (errlst[i] == "CP") {
                    $('#currntPwd').css('border-color', 'red');
                    $('#ErrPwdCurr').text("Enter current password !!");
                    $('#ErrPwdCurr').show();
                }
                else if (errlst[i] == "NP") {
                    $('#nwPwd').css('border-color', 'red');
                    $('#ErrnwPwd').text("Enter new password !!");
                    $('#ErrnwPwd').show();
                }
                else if (errlst[i] == "NPC") {
                    $('#nwPwdCnfrm').css('border-color', 'red');
                    $('#ErrnwPwdCnfm').text("Enter new password again to confirm !!");
                    $('#ErrnwPwdCnfm').show();
                }
                else if (errlst[i] == "NPM") {
                    $('#ErrnwPwdCnfm').text("Confirm password doesn't match !!");
                    $('#ErrnwPwdCnfm').show();
                }
                else if (errlst[i] == "PMM") {
                    $('#currntPwd').css('border-color', 'red');
                    $('#ErrPwdCurr').text("Incorrect password !!");
                    $('#ErrPwdCurr').show();
                }
                else if (errlst[i] == "OK") {
                    clearFileds();
                    //showMessageBox(SUCCESS, "Password updated successfully.You need to re login.");
                    ymz.jq_alert({
                        title: "HRMS",
                        text: "Password updated successfully.You need to re login.",
                        ok_btn: "OK",
                        close_fn: function () {
                            window.location.reload();
                        }
                    });
                }
            }
        }
        catch (ex) {
        }
    }
};
function clearFileds() {
    $('#currntPwd').val('');
    $('#nwPwd').val('');
    $('#nwPwdCnfrm').val('');
}
$('#currntPwd').on('change blur', function () {
    if (this.value.toString().length <= 0) {
        $('#currntPwd').css('border-color', 'red');
        $('#ErrPwdCurr').text("Enter current password !!");
        $('#ErrPwdCurr').show();
    }
    else {
        $('#currntPwd').css('border-color', '');
        $('#ErrPwdCurr').hide();
    }
});
$('#nwPwd').on('change blur', function () {
    if (this.value.toString().length <= 0) {
        $('#nwPwd').css('border-color', 'red');
        $('#ErrnwPwd').show();
    }
    else {
        $('#nwPwd').css('border-color', '');
        $('#ErrnwPwd').hide();
    }
});
$('#nwPwdCnfrm').on('change keyup paste blur', function () {
    if (this.value.toString().length <= 0) {
        $('#nwPwdCnfrm').css('border-color', 'red');
        $('#ErrnwPwdCnfm').text("Enter new password again to confirm !!");
        $('#ErrnwPwdCnfm').show();
    }
    else {
        if (($('#nwPwd').val().length > 0 && $('#nwPwdCnfrm').val().length > 0) && ($('#nwPwd').val() != $('#nwPwdCnfrm').val())) {
            $('#nwPwdCnfrm').css('border-color', 'red');
            $('#ErrnwPwdCnfm').text("Confirm password doesn't match !!");
            $('#ErrnwPwdCnfm').show();
        }
        else {
            $('#nwPwdCnfrm').css('border-color', '');
            $('#ErrnwPwdCnfm').text("Ente new password again to confirm !!");
            $('#ErrnwPwdCnfm').hide();
        }
    }
});