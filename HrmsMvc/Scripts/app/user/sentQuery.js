function SentQuery() {    
    var emailSubject = $('#Emailsubject').val();
    var emailBody = $('#EmailBody').val();
    if (emailSubject && emailBody) {
        showLoadreport("#LoadPageLvQuery", "#contactAdminDiv");
        var params = { emailSubject: emailSubject, emailBody: emailBody };
        $.ajax({
            url: "/User/SentQuery",
            type: "POST",
            contentType: "application/json",
            datatype: "json",
            data: JSON.stringify(params),
            success: function (status) {
                HideLoadreport("#LoadPageLvQuery", "#contactAdminDiv");
                if ("OK" == status.data) {
                    $('#Emailsubject').val('');
                    $('#EmailBody').val('');
                    $('#SentLeaveQueryModal').modal('toggle');                    
                    ymz.jq_alert({
                        title: "HRMS",
                        text: "Query Sent Successfully." + "\n" + "We will get back to you soon..",
                        ok_btn: "OK",
                        close_fn: null
                    });
                }
                else {                   
                    ymz.jq_alert({
                        title: "HRMS",
                        text: "An Unexpected Error Occured!!",
                        ok_btn: "OK",
                        close_fn: null
                    });
                }
            },
            error: function () {
                HideLoadreport("#LoadPageLvQuery", "#contactAdminDiv");
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
            text: "Enter mandatory fields!!",
            ok_btn: "OK",
            close_fn: null
        });
    }
};