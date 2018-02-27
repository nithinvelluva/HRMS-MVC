function showLoad() {
    $(".waitIconDiv").show();
    $(".panel-body").addClass("disablediv");
    $(".waitIconDiv").css("display", "block");
}
function HideLoad() {
    $(".waitIconDiv").hide();
    $(".panel-body").removeClass("disablediv");
    $(".waitIconDiv").css("display", "none");
}
function sentResetPasswordLink() {
    if (stringIsNull($('#UserEmail').val())) {
        $('#UserEmail').addClass('input-validation-error');
        $('#emailError').text("Please enter your email to receive password reset link");
        $('#emailError').show();
    }
    else {
        var postData = { 'email': $('#UserEmail').val() };
        showLoad();
        $.ajax({
            url: "/account/forgotpassword",
            type: "POST",
            datatype: "json",
            contentType: "application/json",
            data: JSON.stringify(postData),
            success: function (data) {
                HideLoad();
                if (data) {
                    switch (data.status) {
                        case 200:
                            $('#UserEmail').val('');
                            ymz.jq_alert({
                                title: "HRMS",
                                text: data.response,
                                ok_btn: "OK",
                                close_fn: null
                            });
                            break;
                        case 404:
                            $('#UserEmail').addClass('input-validation-error');
                            $('#emailError').text(data.response);
                            $('#emailError').show();
                            break;
                        default:
                            break;
                    }
                }

            },
            error: function (data) {
                HideLoad();
            }
        });
    }
}
$(document).ready(function () {
    HideLoad();
    $('#UserEmail').on('blur', function () {
        if (this.value.toString().length <= 0) {
            $('#UserEmail').addClass('input-validation-error');
            $('#emailError').text("Please enter your email to receive password reset link");
            $('#emailError').show();
        }
        else {
            $('#UserEmail').removeClass('input-validation-error');
            $('#emailError').hide();
        }
    });
    function resetValidation() {
        $('#UserEmail').removeClass('input-validation-error');
        $('#emailError').hide();
    }
});