const SUCCESS = 1;
const WARNING = 2;
const ERROR = 3;
const CONFIRM = 4;
const months = JSON.parse('{ "January": 1, "February": 2, "March": 3, "April": 4, "May": 5, "June": 6, "July": 7, "August": 8, "September": 9, "October": 10, "November": 11, "December": 12 }');
const hostDomain = window.location.origin;
function showMessageBox(type, message) {
    switch (type) {
        case SUCCESS:
            type = "success";
            break;
        case WARNING:
            type = "warning";
            break;
        case ERROR:
            type = "error";
            break;
        default:
            type = "success";
            break;
    }
    Lobibox.notify(type, { title: "HRMS", position: 'center top', sound: false, size: 'mini', closeOnClick: true, msg: message });
};
function formValidate(page) {
    var validFlag = true;
    switch (page) {
        case LOGINFORM:
            clearValidationError();
            var username = $('#userName').val();
            var password = $('#userPassword').val();
            if (stringIsNull(username)) {
                $('#userName').css('border-color', 'red');
                $('#lbluserName').show();
                validFlag = false;
            }
            else if (!emailRegexValidator(username)) {
                $('#userName').css('border-color', 'red');
                $('#lbluserNameInvl').show();
                validFlag = false;
            }
            if (stringIsNull(password)) {
                $('#userPassword').css('border-color', 'red');
                $('#lblPaswd').show();
                validFlag = false;
            }
            break;
        case REGISTERFORM:
            clearValidationError();
            var name = $('#NameReg').val();
            var phone = $('#userPhone').val();
            var username = $('#userNameReg').val();
            var password = $('#userPasswordReg').val();
            var cnfmPwd = $('#userCnfrmPasswordReg').val();
            if (stringIsNull(name)) {
                $('#NameReg').css('border-color', 'red');
                $('#lblNameReg').show();
                validFlag = false;
            }
            if (stringIsNull(phone)) {
                $('#userPhone').css('border-color', 'red');
                $('#lbluserPhone').show();
                validFlag = false;
            }
            else if (!phoneNumberRegexValidate(phone)) {
                $('#userPhone').css('border-color', 'red');
                $('#lbluserPhoneInvl').show();
                validFlag = false;
            }
            if (stringIsNull(username)) {
                $('#userNameReg').css('border-color', 'red');
                $('#lbluserNameReg').show();
                validFlag = false;
            }
            else if (!emailRegexValidator(username)) {
                $('#userNameReg').css('border-color', 'red');
                $('#lbluserNameRegInvl').show();
                validFlag = false;
            }
            if (stringIsNull(password)) {
                $('#userPasswordReg').css('border-color', 'red');
                $('#lbluserPasswordReg').show();
                validFlag = false;
            }
            else if (!checkStrength(password)) {
                $('#userPasswordReg').css('border-color', 'red');
                $('#lbluserPasswordRegInvl').show();
                validFlag = false;
            }
            if (stringIsNull(cnfmPwd)) {
                $('#userCnfrmPasswordReg').css('border-color', 'red');
                $('#lblusrCnfmPwdReg').show();
                validFlag = false;
            }
            else if (!stringIsNull(password) && cnfmPwd != password) {
                $('#userCnfrmPasswordReg').css('border-color', 'red');
                $('#lblusrCnfmPwdRegMsmatch').show();
                validFlag = false;
            }
            break;
        case RESETPWD:
            clearValidationError();
            var usernameReset = $('#userNameReset').val();
            if (stringIsNull(usernameReset)) {
                $('#userNameReset').css('border-color', 'red');
                $('#lbluserNameReset').show();
                validFlag = false;
            }
            else if (!emailRegexValidator(usernameReset)) {
                $('#userNameReset').css('border-color', 'red');
                $('#lbluserNameResetInvl').show();
                validFlag = false;
            }
            break;
        default: validFlag = false;
            break;
    }
    return validFlag;
};
function clearValidationError() {
    $('.inputField').css('border-color', '');
    $('.errLabel').hide();
};
function stringIsNull(string) {
    return (string == null || string == undefined || string == "") ? true : false;
};
/* Password strength indicator */
function checkStrength(password) {
    var desc = [{ 'width': '0px' }, { 'width': '20%' }, { 'width': '40%' }, { 'width': '60%' }, { 'width': '80%' }, { 'width': '100%' }];
    var descClass = ['', 'progress-bar-danger', 'progress-bar-warning', 'progress-bar-warning', 'progress-bar-success'];
    var score = 0;
    //if password bigger than 5 give 1 point
    if (password.length >= 5) score++;
    //if password has both lower or uppercase characters give 1 point	
    if ((password.match(/[a-z]/)) || (password.match(/[A-Z]/))) score++;
    //if password has at least one number give 1 point
    if (password.match(/\d+/)) score++;
    //if password has at least one special caracther give 1 point
    if (password.match(/.[!,@,#,$,%,^,&,*,?,_,~,-,(,)]/)) score++;
    var isMatch = false;
    // display indicator
    if (score >= 4) {
        $("#pwd_strength").removeClass(descClass[3]).addClass(descClass[4]).css(desc[5]);
        isMatch = true;
    }
    else if (score < 4) {
        $("#pwd_strength").removeClass(descClass[score - 1]).addClass(descClass[score]).css(desc[score]);
    }
    return isMatch;
};
function emailRegexValidator(emailId) {
    var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i
    var isMatch = (pattern.test(emailId)) ? true : false;
    return isMatch;
};
function phoneNumberRegexValidate(phoneNumber) {
    var pattern = /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/;
    var isMatch = (pattern.test(phoneNumber)) ? true : false;
    return isMatch;
};
function updateDateRange(from_date_picker, to_date_picker) {
    if (to_date_picker && from_date_picker && to_date_picker.length && from_date_picker.length) {
        var start_dt = $(from_date_picker).datepicker('getDate');
        if (start_dt && !isNaN(Date.parse(start_dt))) {
            var end_dt = $(to_date_picker).datepicker('getDate');
            if ((end_dt && isNaN(Date.parse(end_dt))) || (end_dt && new Date(end_dt) < new Date(start_dt))) {
                $(to_date_picker).datepicker('update', start_dt);
                $(to_date_picker).focus();
            }
            $(to_date_picker).datepicker('setDate', start_dt);
            $(to_date_picker).blur();
        }
    }
};
function showAlertMessage(msg, isModal, type) {
    var html = [];
    html.push('<a href="javascript:void(0);" class="close" onclick="hideAlertMessage(' + isModal + ');">&times;</a>');
    html.push(msg);
    var error_container = (isModal) ? '.alertErrorModal' : '#alertError';
    $(error_container).empty();
    $(error_container).removeClass('alert-success alert-danger alert-info').addClass('alert-' + type);
    $(error_container).html(msg);
    $(error_container).show();
    (!isModal) ? window.scrollTo(0, 0) : '';
};
function hideAlertMessage(isModal) {
    var error_container = (isModal) ? '.alertErrorModal' : '#alertError';
    $(error_container).html('');
    $(error_container).hide();
};
function setLocalValue(key, value) {
    sessionStorage.setItem(key, JSON.stringify(value));
};
function getLocalValue(key) {
    return sessionStorage.getItem(key) ? JSON.parse(sessionStorage.getItem(key)) : null;
};
function AjaxCall(url, params, callback, loadHideCallback) {
    showLoadprofile();
    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(params),
        datatype: "json",
        contentType: "application/json",
        success: function (data) {
            (loadHideCallback) ? loadHideCallback() : '';
            callback(data);
        },
        error: function (data) {
            (loadHideCallback) ? loadHideCallback() : '';
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !",
                ok_btn: "OK",
                close_fn: null
            });
        }
    });
};