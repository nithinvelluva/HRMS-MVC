function navigateTo(page) {
    var activeElement;
    var activeContentDiv;
    switch (page) {        
        case USERS:            
            activeElement = USERS_LI;
            break;
        case DASHBOARD_ATTENDANCE:
            activeElement = DASHBOARD_ATTENDANCE_LI;
            break;
        case DASHBOARD_LEAVES:
            activeElement = DASHBOARD_LEAVES_LI;
            break;
        case DASHBOARD_REPORTS:
            activeElement = DASHBOARD_REPORTS_LI;
            break;
        case TASKS:
            activeElement = TASKS_LI;
            break;
    }
    $(".main-nav li").removeClass("active");
    $(activeElement).addClass("active");
};

$(document).ready(function () {
    if ($('#admin_panel_sidebar').css('display') == 'block') {
        var sidebar_width = $('#admin_panel_sidebar').width();
        $('#dashboard_admin_panel_header').css('width', '-=' + sidebar_width + 'px');
    }
});