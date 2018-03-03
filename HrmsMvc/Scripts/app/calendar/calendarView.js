var sel_employee_id = 0;
var calendarGoToDate = new Date();
var task_added_employee = [];
var files_to_upload = [];
var jstreeData = [];
var jstree_sel_node = {};
var sel_event_id = 0;
function calendarShowAlertMessage(msg, type) {
    var html = [];
    html.push('<a href="javascript:void(0);" class="close" onclick="calendarHideAlertMessage();">&times;</a>');
    html.push(msg);
    var error_container = '#alertErrorModalCal';
    $(error_container).removeClass('alert-success alert-danger alert-info').addClass('alert-' + type);
    $(error_container).html(html);
    $(error_container).show();
};
function calendarHideAlertMessage() {
    var error_container = '#alertErrorModalCal';
    $(error_container).html('');
    $(error_container).hide();
};
function modalClose() {
    initCalendarFields();
};
function calendarModalSetDate(date) {
    calendarGoToDate = date;
};
function manageEvent() {
    resetEventPopupValues();
    $('#task_status_div').hide();
    $('#eventPopupDiv').modal('show');
    eventPopupLoading(false);
};
function removeDuplicateCalendarEvents(full_calendar_obj, events_data) {
    if (events_data && events_data.length) {
        $(events_data).each(function (i, event_item) {
            full_calendar_obj.fullCalendar('removeEvents', event_item.id);
        });
    }
};
function removeEventsOnFiltering(full_calendar_obj, events_data) {
    var rendered_events = full_calendar_obj.fullCalendar('clientEvents');
    var difference = [];
    if (rendered_events && rendered_events.length) {
        $.grep(events_data, function (el) {
            if ($.inArray(el, rendered_events) == -1) {
                difference.push(el);
            }
        });
    }
    if (difference && difference.length && events_data && events_data.length) {
        $(difference).each(function (i, remove_event) {
            full_calendar_obj.fullCalendar('removeEvents', remove_event.id);
        });
    }
};
function fetchEvents() {
    var dates = GetCalendarDateRange();
    var params = {
        _empId: sel_employee_id,
        _start_date: calendarFormatDate(dates.start),
        _end_date: calendarFormatDate(dates.end),
        event_filter: 0
    };
    var url = "/calendar/getEvents";
    var callback = fetchEventsCallback;
    eventLoading(true);
    calendarAjaxGETRequest(url, params, callback);
};
function fetchEventsCallback(response) {
    if (response && response.data) {
        var event_id;
        var event_type;
        var event_employees;
        var event_title;
        var event_note;
        var event_start_date;
        var event_end_date;
        var event_start_time;
        var event_end_time;
        var event_cell_color;
        var event_duration;

        var full_calendar_obj = $('#week-calendar-view');
        if (full_calendar_obj && full_calendar_obj.length) {
            //full_calendar_obj.fullCalendar('removeEvents');//remove all events.
            removeDuplicateCalendarEvents(full_calendar_obj, response.data);//remove duplicate events.
            removeEventsOnFiltering(full_calendar_obj, response.data);//remove events on filtering.
            for (var i = 0; i < response.data.length; i++) {
                event_id = response.data[i].Id;
                event_type = response.data[i].event_type;
                event_title = response.data[i].heading;
                event_note = response.data[i].note;
                event_start_date = response.data[i].start_date;
                event_end_date = response.data[i].end_date;

                var start_hour = ((response.data[i].start_time).split(':')[0]);
                var start_min = ((response.data[i].start_time).split(':')[1]);
                var end_hour = ((response.data[i].end_time).split(':')[0]);
                var end_min = ((response.data[i].end_time).split(':')[1]);

                event_start_time = start_hour + ':' + start_min;
                event_end_time = end_hour + ':' + end_min;
                event_duration = response.data[i].duration;
                event_employees = response.data[i].employees;


                switch (event_type) {
                    case 1://tasks
                        switch (response.data[i].status) {
                            case 1: event_cell_color = '#c9e5f9';//Assigned
                                break;
                            case 2: event_cell_color = '#f6e4cc';//Ongoing
                                break;
                            case 3: event_cell_color = '#d9f900';//Review
                                break;
                            case 4: event_cell_color = '#cfeccf';//Completed
                                break;
                            default:
                        }
                        break;
                    case 2://birthday events
                        event_cell_color = '#f2f600';
                        break;
                    case 3://leaves
                        event_cell_color = '#ff0037';
                        break;
                    default:
                }

                var event = {
                    id: event_id,
                    type: event_type,
                    title: event_title,
                    description: event_note,
                    start: event_start_date,
                    end: event_end_date,
                    start_time: event_start_time,
                    end_time: event_end_time,
                    duration: event_duration,
                    color: event_cell_color,
                    event_employees: event_employees,
                    editable: false,
                    className: (event_type == 1) ? 'full_calendar_event_class' : 'full_calendar_event_other',
                    is_editable: response.data[i].is_edit,
                    status: response.data[i].status,
                    is_leave_event: response.data[i].is_leave_event,
                };

                full_calendar_obj.fullCalendar('renderEvent', event, true);
            }
        }
    }
    eventLoading(false);
};
function eventSave() {
    var event_id = $('#calendar_event_id').val();
    var employee = task_added_employee;
    var heading = $('#event-popup-heading').val();
    var notes = $('#event-popup-note').val();
    var event_start_date = $('#calendar_task_from_date').val();
    var event_end_date = $('#calendar_task_to_date').val();

    var validFlag = true;
    var mainError = 'Fix the following errors \n \n';
    if (!employee || (employee && !employee.length)) {
        validFlag = false;
        mainError += " * Please select an employee  \n \n";
    }
    if (!heading) {
        validFlag = false;
        mainError += " * Please enter name \n \n";
    }
    if (!event_start_date) {
        validFlag = false;
        mainError += " * Please select from date \n \n";
    }
    if (!event_end_date) {
        validFlag = false;
        mainError += " * Please select to date \n \n";
    }
    if (validFlag) {
        var startDt;
        var selDate;
        var event_dates = [];
        var start_time = $('#event-popup-from-time').find('#txtHours').val() + ":" + $('#event-popup-from-time').find('#txtMinutes').val() + ":" + "00";
        var end_time = $('#event-popup-to-time').find('#txtHours').val() + ":" + $('#event-popup-to-time').find('#txtMinutes').val() + ":" + "00";
        event_start_date = event_start_date + ' ' + start_time;
        event_end_date = event_end_date + ' ' + end_time;
        if (new Date(event_start_date) > new Date(event_end_date)) {
            validFlag = false;
        }
        else {
            event_dates.push({
                'start_date': event_start_date,
                'end_date': event_end_date
            });
        }
        if (validFlag) {
            eventPopupLoading(true);
            if (event_id && parseInt(event_id) > 0) {//Editing
                var params = {
                    Id: event_id,
                    employee: employee,
                    heading: heading,
                    note: notes,
                    event_dates: event_dates
                };
                var url = "/calendar/taskSave";
                var callback = evenEditCallback;
                calendarAjaxPOSTRequest(url, params, callback);
            }
            else {//Adding
                var params = {
                    Id: 0,
                    employee: employee,
                    heading: heading,
                    note: notes,
                    event_dates: event_dates
                };
                var url = "/calendar/taskSave";
                var callback = eventSaveCallback;
                calendarAjaxPOSTRequest(url, params, callback);
            }
        }
        else {
            calendarShowAlertMessage('Start date time should be less than or equal to end date time !!', 'danger');
        }
    }
    else {
        calendarShowAlertMessage(mainError, 'danger');
    }
};
function eventSaveCallback(response) {
    eventPopupLoading(false);
    if (response && "OK" == response.UpdateStatus) {
        var task_id = response.data;
        var upload_btn = $('#ssi-uploadBtn');
        if (upload_btn && upload_btn.length) {
            $(upload_btn).trigger('click');
            if (files_to_upload && files_to_upload.length > 0) {
                var formData = new FormData();
                for (var i = 0; i < files_to_upload.length; i++) {
                    formData.append(files_to_upload[i].name, files_to_upload[i]);
                }
                formData.append('event_id', task_id);
                $.ajax({
                    url: "/calendar/UploadTaskFile",
                    type: "POST",
                    processData: false,
                    contentType: false,
                    data: formData,
                    beforeSend: function () {

                    },
                    success: function (response) {
                        if (response && 'OK' == response.UpdateStatus) {
                            calendarShowAlertMessage('Task created successfully', 'success');
                            setTimeout(
                                function () {
                                    $('#eventPopupDiv').modal('hide');
                                    fetchEvents();
                                }, 2000);
                        }
                    },
                    error: function (er) {
                        calendarShowAlertMessage('An unexpected error occurred !!', 'danger');
                    }
                });
            }
            else {
                calendarShowAlertMessage('Task created successfully', 'success');
                setTimeout(
                    function () {
                        $('#eventPopupDiv').modal('hide');
                        fetchEvents();
                    }, 2000);
            }
        }

    }
    else {
        calendarShowAlertMessage('An unexpected error occurred !!', 'danger');
    }
};
function evenEditCallback(response) {
    eventPopupLoading(false);
    if (response && "OK" == response.UpdateStatus) {
        calendarShowAlertMessage('Task updated successfully', 'success');
        setTimeout(
            function () {
                $('#eventPopupDiv').modal('hide');
                if (sel_employee_id > 0 && response.employees && response.employees.length > 0
                    && -1 == $.inArray(sel_employee_id, response.employees)) {
                    $('#week-calendar-view').fullCalendar('removeEvents', response.data);
                }
                fetchEvents();
            }, 2000);
    }
    else {
        calendarShowAlertMessage('An unexpected error occurred !!', 'danger');
    }
};
function deleteEvent() {
    var eventid = $('#calendar_event_id').val();
    if (eventid) {
        var resp = window.confirm("Are you sure want to delete the event ?");
        if (resp) {
            eventPopupLoading(true);
            var url = "/calendar/taskRemove";
            var params = { event_id: eventid };
            var callback = deleteEventCallback;
            calendarAjaxPOSTRequest(url, params, callback);
        }
    }
};
function deleteEventCallback(response) {
    eventPopupLoading(false);
    if (response && "OK" == response.UpdateStatus) {
        calendarShowAlertMessage('Event removed successfully', 'success');
        setTimeout(
            function () {
                $('#eventPopupDiv').modal('hide');
                $('#week-calendar-view').fullCalendar('removeEvents', response.data);
                fetchEvents();
            }, 2000);
    }
    else {
        calendarShowAlertMessage('An unexpected error occurred !!', 'danger');
    }
};
function cancelEvent() {
    $('#eventPopupDiv').modal('hide');
};
function removeTaskArchive(archive_id) {
    var message = "Are you sure want to remove the file ?";
    var title = "HRMS";
    ymz.jq_confirm({
        title: !stringIsNull(title) ? title : "HRMS",
        text: message,
        no_btn: "No",
        yes_btn: "Yes",
        no_fn: function () {

        },
        yes_fn: function () {
            var url = "/calendar/removeTaskArchive";
            var params = { archive_id: archive_id };
            var callback = removeTaskArchiveCallback;
            calendarAjaxPOSTRequest(url, params, callback);
        }
    });
};
function removeTaskArchiveCallback(response) {
    if (response && 'OK' == response.UpdateStatus) {
        var archive_id = response.data;
        var event_archive_added_container = $('#task_uploaded_files_container_ul');
        if (event_archive_added_container && event_archive_added_container.length) {
            $(event_archive_added_container).find('li[data-id=' + archive_id + ']').remove();
            if (!$(event_archive_added_container).find('li').length) {
                $(event_archive_added_container).hide();
            }
        }
    }
};
function eventDetailsFetchCallback(response) {
    if (response && response.data) {
        var data = response.data;
        if (data) {
            $('#calendar_task_modal_header').text('Edit Task');
            $('#calendar_event_id').val(data.Id);
            $('#event-popup-heading').val(data.heading);
            $('#event-popup-note').val(data.note);
            var eventstartDt = data.start_date;
            var eventendDt = data.end_date;

            $('#calendar_task_from_date').datepicker('setDate', eventstartDt.split(' ')[0]);
            $('#calendar_task_to_date').datepicker('setDate', eventendDt.split(' ')[0]);

            var start_time = eventstartDt.split(' ')[1];
            var end_time = eventendDt.split(' ')[1];

            var fromtimehr = ((start_time).split(':')[0]);
            var fromtimemin = ((start_time).split(':')[1]);
            var totimehr = ((end_time).split(':')[0]);
            var totimemin = ((end_time).split(':')[1]);

            $('#event-popup-from-time').find('#txtHours').val(fromtimehr);
            $('#event-popup-from-time').find('#txtMinutes').val(fromtimemin);
            $('#event-popup-to-time').find('#txtHours').val(totimehr);
            $('#event-popup-to-time').find('#txtMinutes').val(totimemin);

            $('#mng-event-popup-cancel-event-btn').removeAttr('disabled');
            $('#mng-event-popup-delete-event-btn').removeAttr('disabled');

            $('#event_del_btn_div').show();
            switch (data.status) {
                case 1:
                    $('#task_status_processing').removeClass('not-completed completed').addClass('not-completed');
                    $('#task_status_review').removeClass('not-completed completed').addClass('not-completed');
                    $('#task_status_completed').removeClass('not-completed completed').addClass('not-completed');
                    break;
                case 2:
                    $('#task_status_processing').removeClass('not-completed completed').addClass('completed');
                    $('#task_status_review').removeClass('not-completed completed').addClass('not-completed');
                    $('#task_status_completed').removeClass('not-completed completed').addClass('not-completed');
                    break;
                case 3:
                    $('#task_status_processing').removeClass('not-completed completed').addClass('completed');
                    $('#task_status_review').removeClass('not-completed completed').addClass('completed');
                    $('#task_status_completed').removeClass('not-completed completed').addClass('not-completed');
                    break;
                case 4:
                    $('#task_status_processing').removeClass('not-completed completed').addClass('completed');
                    $('#task_status_review').removeClass('not-completed completed').addClass('completed');
                    $('#task_status_completed').removeClass('not-completed completed').addClass('completed');
                    break;
                default:

            }
            var user_data = (localStorage.getItem("user_data")) ? JSON.parse(localStorage.getItem("user_data")) : null;
            var event_employee_added_container = $('#event_employee_added_container');
            $(data.employees).each(function (i, item) {
                if (event_employee_added_container && event_employee_added_container.length) {
                    var userPhotoPath = (item.UserPhotoPath) ? "/Content/UserIcons/" + item.UserPhotoPath : ("M" == (item.Gender).trim()) ? "/Content/Images/male-avatar.png" : "/Content/Images/female-avatar.png";
                    var emp_icon_remove = (user_data && user_data.userType == 1) ? '<span class="addMember-remove" data-id="' + item.EmpID + '"><i class="fa fa-minus-circle" aria-hidden="true"></i></span>' : '';
                    var sel_employee_li = '<li class="addMember-item" data-id="' + item.EmpID + '"> <img class="employee-select-avatar" src="' + userPhotoPath + '" title="' + item.EmpFirstname + " " + item.EmpLastname + '">' + emp_icon_remove + '</li>'
                    if (!$(event_employee_added_container).find('li[data-id=' + item.EmpID + ']').length) {
                        $(event_employee_added_container).append(sel_employee_li);
                        task_added_employee.push(item.EmpID);
                    }
                }
            });
            var event_archive_added_container = $('#task_uploaded_files_container_ul');
            if (data.archive && data.archive.length) {
                $(data.archive).each(function (i, item) {
                    if (event_archive_added_container && event_archive_added_container.length) {
                        var icon_remove = (user_data && user_data.userType == 1) ? '<i class="icon-bin last cursor red-icon" data-id=' + item.archive_id + ' onclick="removeTaskArchive(' + item.archive_id + ')"></i>' : '';
                        var download_url = "/calendar/downloadTaskFile/" + item.archive_id;
                        var sel_archive_li = '<li class="archive-item addMember-item" data-id="' + item.archive_id + '"><i class="icon-file-text2 pull-left inskriven_skapautrde_filetxticon"></i><text id="filelabel" readonly="true" class="inskriven_skapautrde_label2 inskriven_skapautrde_filetext">' + item.filepath + '</text> <a href="' + download_url + '"><i class="icon-download2 last inskriven_skapautrde_filedownloadicon"></i></a>' + icon_remove + '</li>'
                        if (!$(event_archive_added_container).find('li[data-id=' + item.archive_id + ']').length) {
                            $(event_archive_added_container).append(sel_archive_li);
                        }
                    }
                });
                $(event_archive_added_container).show();
            }
            else {
                $(event_archive_added_container).hide();
            }
        }
    }
    eventPopupLoading(false);
};
function calendarFormatDate(date) {
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    var y = date.getFullYear();
    if (parseInt(mm) < 10) {
        mm = '0' + mm;
    }
    if (parseInt(dd) < 10) {
        dd = '0' + dd;
    }
    var formattedDate = y + '-' + mm + '-' + dd;
    return formattedDate;
};
function calendarFormatTime(time) {
    if (parseInt(time) < 10) {
        time = '0' + time;
    }
    return time;
};
function GetCalendarDateRange() {
    var calendar = $('#week-calendar-view').fullCalendar('getCalendar');
    var view = calendar.view;
    var start = view.start._d;
    var end_dt = new Date(view.end._d);
    var end = new Date(end_dt.setDate(end_dt.getDate() - 1)); //removing one day from enddate.
    var dates = { start: start, end: end };
    return dates;
};
function getCalendarWeekNumber(dt) {
    var tdt = new Date(dt.valueOf());
    var dayn = (dt.getDay() + 6) % 7;
    tdt.setDate(tdt.getDate() - dayn + 3);
    var firstThursday = tdt.valueOf();
    tdt.setMonth(0, 1);
    if (tdt.getDay() !== 4) {
        tdt.setMonth(0, 1 + ((4 - tdt.getDay()) + 7) % 7);
    }
    return 1 + Math.ceil((firstThursday - tdt) / 604800000);
};
function resetEventPopupValues() {
    $('#calendar_event_id').val(0);
    $('#calendar_task_from_date').val("").datepicker("update");
    $('#calendar_task_to_date').val("").datepicker("update");
    $('#event-popup-heading').val('');
    $('#event-popup-note').val('');
    $('#event-popup-from-time').find('#txtHours').val('08');
    $('#event-popup-from-time').find('#txtMinutes').val('00');
    $('#event-popup-to-time').find('#txtHours').val('09');
    $('#event-popup-to-time').find('#txtMinutes').val('00');
    $('#event_del_btn_div').hide();
    $('#event_employee_added_container').empty();
    $('#task_uploaded_files_container_ul').empty();
    $('#task_uploaded_files_container_ul').hide();
    task_added_employee.length = 0;
    task_added_employee = [];
    files_to_upload.length = 0;
    files_to_upload = [];
    $('#ssi-previewBox').empty();
    $('#ssi-uploadBtn').clearFiles();
    $('#calendar_task_modal_header').text('New Task');
    $('#task_status_processing').removeClass('not-completed completed').addClass('not-completed');
    $('#task_status_review').removeClass('not-completed completed').addClass('not-completed');
    $('#task_status_completed').removeClass('not-completed completed').addClass('not-completed');
    $('.status_track').find('div').attr('title', '');
    sel_event_id = 0;
};
$.fn.extend({
    clearFiles: function () {
        $(this).each(function () {
            var isIE = (window.navigator.userAgent.indexOf("MSIE ") > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./));
            if ($(this).prop("type") == 'file') {
                if (isIE == true) {
                    $(this).replaceWith($(this).val('').clone(true));
                } else {
                    $(this).val("");
                }
            }
        });
        return this;
    }
});
function eventLoading(isLoading) {
    (isLoading) ? $('#week-calendar-view-event-load').show() : $('#week-calendar-view-event-load').hide();
};
function eventPopupLoading(isLoading) {
    (isLoading) ? $('#calendar-view-event-popup-load').show() : $('#calendar-view-event-popup-load').hide();
};
function initCalendarFields() {
    sel_employee_id = 0;
    $('.calendarheader_people_sel').val(0);
    $('.calendarheader_people_sel').text($('#all_people_label').val());
};
$(document).ready(function () {
    navigateTo(TASKS);
    $('#event_file_upload_select').ssi_uploader({
        url: '/calendar/UploadTaskFile',
        maxFileSize: 10,
        allowed: ['jpg', 'gif', 'txt', 'png', 'pdf', 'doc', 'docx']
        , onEachUpload: function (file) { console.log(file) }
    });
    if ($('#ssi-previewBox').length) {
        $(document).on('click', '#ssi-previewBox', function () {
            $('#event_file_upload_select').trigger('click');
        });
    }
    if ($('#week-calendar-view') && $('#week-calendar-view').length) {
        var user_data = (localStorage.getItem("user_data")) ? JSON.parse(localStorage.getItem("user_data")) : null;
        if (user_data && user_data.userType == 2) {
            sel_employee_id = user_data.empId;
        }
        if ($('#week-calendar-view').children().length > 0) {
            $('#week-calendar-view').fullCalendar('gotoDate', calendarGoToDate);
            monthLabelOnChange();
            fetchEvents();
        }
        else {
            $('#week-calendar-view').fullCalendar({
                //defaultView: 'basicWeek',
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,basicWeek,basicDay'
                },
                weekNumbers: false,
                editable: false,
                lang: 'en',
                views: {
                    basicWeek: {
                        columnFormat: 'dddd D/M'
                    }
                },
                viewRender: function (data) { //triggered when view is rendered,i.e,initial loading and changing dates
                    monthLabelOnChange();
                    fetchEvents();
                },
                eventAfterAllRender: function (data) {
                    //monthLabelOnChange();
                },
                eventClick: function (calEvent, jsEvent, view) {
                    if (calEvent.is_editable && !calEvent.is_leave_event) {
                        eventPopupLoading(true);
                        $('#task_status_div').show();
                        $('#eventPopupDiv').modal('show');
                        var url = "/calendar/eventDetailsFetch";
                        var params = { eventid: calEvent.id };
                        var callback = eventDetailsFetchCallback;
                        calendarAjaxGETRequest(url, params, callback);

                        sel_event_id = calEvent.id;
                        $('#calendar_task_tree_container').jstree("refresh");
                        url = '/calendar/getTreeViewFiles';
                        callback = JStreeRender;
                        calendarAjaxGETRequest(url, params, callback);
                    }
                    else if (calEvent.is_editable && calEvent.is_leave_event) {
                        IsCalendarEdit = true;
                        leaveEditPopupLoading(true);
                        var url = "/user/leaveDetailsFetch";
                        var params = { leave_event_id: calEvent.id, IsCalendarEdit: true };
                        var callback = leaveDetailsFetchCallback;
                        calendarAjaxGETRequest(url, params, callback);
                        sel_event_id = calEvent.id;
                    }
                },
                eventRender: function (event, element, view) {
                    element.find(".fc-title").empty();
                    element.find(".fc-time").empty();

                    switch (event.type) {
                        case 1:
                            var event_color_class = '';
                            switch (event.status) {
                                case 1: event_icon = 'icon-stack';
                                    event_color_class = 'event_header_assigned';
                                    break;
                                case 2: event_icon = 'icon-clock';
                                    event_color_class = 'event_header_ongoing';
                                    break;
                                case 3: event_icon = 'icon-paste';
                                    event_color_class = 'event_header_review';
                                    break;
                                case 4: event_icon = 'icon-checkmark';
                                    event_color_class = 'event_header_completed';
                                    break;
                                default:

                            }
                            var people_avatar_html = '';
                            $(event.event_employees).each(function (i, item) {
                                if (i < 3) {
                                    var userPhotoPath = (item.UserPhotoPath) ? "/Content/UserIcons/" + item.UserPhotoPath : ("M" == (item.Gender).trim()) ? "/Content/Images/male-avatar.png" : "/Content/Images/female-avatar.png";
                                    people_avatar_html = people_avatar_html + '<li class="addMember-item"><img class="employee-select-avatar-sub" src="' + userPhotoPath + '" title="' + item.EmpFirstname + " " + item.EmpLastname + '"></li>';
                                }
                            });
                            if (event.event_employees.length - 3 > 0) {
                                people_avatar_html = people_avatar_html + '<li class="addMember-item" title="' + (event.event_employees.length - 3).toString() + " more" + '"><span class="task_employee_count">' + (event.event_employees.length - 3).toString() + '</span></li>';
                            }
                            var people_avatar_html_container = '<ul class="addMember-items">' + people_avatar_html + '</ul>';
                            element.find(".fc-title").append("<div class=" + event_color_class + "></div><div class='event_data_container'><i class='" + event_icon + " full_calendar_event_icon'>&nbsp;<label class='full_calendar_event_icon'>" + event.start_time + " - " + event.end_time + "</label></i><br/><label class='full_calendar_event_text'>" + event.title + "</label><br/>" + people_avatar_html_container + "</div>");
                            break;
                        case 2:
                            event_icon = 'icon-gift';
                            var durationHr = event.duration.split(':')[0];
                            var employee = event.event_employees[0];
                            var userPhotoPath = (employee.UserPhotoPath) ? "/Content/UserIcons/" + employee.UserPhotoPath : ("M" == (employee.Gender).trim()) ? "/Content/Images/male-avatar.png" : "/Content/Images/female-avatar.png";
                            var people_avatar_html = '<li class="addMember-item"><img class="employee-select-avatar-sub" src="' + userPhotoPath + '" title="' + employee.EmpFirstname + " " + employee.EmpLastname + '"></li>';
                            var people_avatar_html_container = '<ul class="addMember-items event_birthday_disp">' + people_avatar_html + '</ul>';
                            element.find(".fc-title").append("<div class='event_header_birthday'></div><div class='event_data_container'><i class='" + event_icon + " full_calendar_event_icon'></i>&nbsp;<labeL class='full_calendar_event_icon'>Birthday</label><br/>" + people_avatar_html_container + "&nbsp;<label class='full_calendar_event_text' title='" + employee.EmpFirstname + " " + employee.EmpLastname + "'>" + employee.EmpFirstname + " " + employee.EmpLastname + "</label></div>");
                            break;
                        case 3:
                            event_icon = 'fa fa-user-times';
                            var durationHr = event.duration.split(':')[0];
                            var employee = event.event_employees[0];
                            var userPhotoPath = (employee.UserPhotoPath) ? "/Content/UserIcons/" + employee.UserPhotoPath : ("M" == (employee.Gender).trim()) ? "/Content/Images/male-avatar.png" : "/Content/Images/female-avatar.png";
                            var people_avatar_html = '<li class="addMember-item"><img class="employee-select-avatar-sub" src="' + userPhotoPath + '" title="' + employee.EmpFirstname + " " + employee.EmpLastname + '"></li>';
                            var people_avatar_html_container = '<ul class="addMember-items event_birthday_disp">' + people_avatar_html + '</ul>';
                            element.find(".fc-title").append("<div class='leave_event_header'></div><div class='event_data_container'><i class='" + event_icon + " full_calendar_event_icon'></i>&nbsp;<labeL class='full_calendar_event_icon'>Leave</label><br/>" + people_avatar_html_container + "&nbsp;<label class='full_calendar_event_text' title='" + employee.EmpFirstname + " " + employee.EmpLastname + "'>" + employee.EmpFirstname + " " + employee.EmpLastname + "</label></div>");
                            break;
                        default:
                    }
                }
            });
            $('#week-calendar-view').fullCalendar('gotoDate', calendarGoToDate);
        }
    }
    if ($('.event-time-picker') && $('.event-time-picker').length) {
        $(".event-time-picker").timesetter({
            hour: {
                value: 0,
                min: 0,
                max: 24,
                step: 1,
                symbol: ""
            },
            minute: {
                value: 0,
                min: 0,
                max: 60,
                step: 1,
                symbol: ""
            },
            // increment or decrement
            direction: "increment",
            // hour textbox
            inputHourTextbox: null,
            // minutes textbox
            inputMinuteTextbox: null,
            // text to display after the input fields
            postfixText: "",
            // number left padding character ex: 00052
            numberPaddingChar: '0'
        });
    }
    function setEmptyFolders(node) {
        var js_tree_container = $('#calendar_task_tree_container');
        var tree_main_ul = js_tree_container.find('.jstree-container-ul');
        if (node && node.id && node.node_type) {
            if ('folder' == node.node_type.toLowerCase().trim()) {
                var nodeElement = $(tree_main_ul).find('li[id=' + node.id + ']');
                $(nodeElement).removeClass('jstree-leaf');
                $(node.children).each(function (index, item) {
                    setEmptyFolders(item);
                });
            }
            else {
                var nodeElement = $(tree_main_ul).find('li[id=' + node.id + ']');
                $(nodeElement).find('i').addClass('tree_view_file_open').attr('href', '/Content/Uploads/' + node.name);
            }
        }
    };
    function JStreeRender(response) {
        if (response) {
            jstreeData = response;
            var js_tree_container = $('#calendar_task_tree_container');
            if (js_tree_container.length) {
                js_tree_container.jstree({
                    core: {
                        'data': {
                            'url': '/calendar/getTreeViewFiles',
                            'type': 'GET',
                            'data': function (node) {
                                return { 'eventid': sel_event_id };
                            },
                            "dataType": "json",
                            success: function (data) {
                                jstreeData = data;
                            }
                        }
                    },
                    types: {
                        "root": {
                            "icon": "fa fa-folder fa-fw"
                        },
                        "child": {
                            "icon": "icomoon icon-file-text2"
                        },
                        "default": {}
                    },
                    plugins: ["search", "themes", "types", "checkbox", "wholerow", "dnd"]
                });

                js_tree_container.on('open_node.jstree', function (e, data) {
                    $(jstreeData).each(function (index, item) {
                        setEmptyFolders(item);
                    });
                    data.instance.set_icon(data.node, "fa fa-folder-open fa-fw");
                })
                .on('close_node.jstree', function (e, data) {
                    data.instance.set_icon(data.node, "fa fa-folder fa-fw");
                })
                .on("changed.jstree", function (e, data) {
                    jstree_sel_node = data.node;
                })
                .on("select_node.jstree", function (e, data) {
                    jstree_sel_node = data.node;
                    data.instance.toggle_node(data.node);
                })
                .on('load_node.jstree', function (event, data) {
                    $(jstreeData).each(function (index, item) {
                        setEmptyFolders(item);
                    });
                });
            }
        }
    };
    $(document).on('click', '.tree_view_file_open', function () {
        window.open($("iframe.test").attr("src"), "Window Title", "width=300,height=400,left=100,top=200");
    });
    $('#calendar_task_from_date').datepicker({ format: 'yyyy-mm-dd', autoclose: true });
    $('#calendar_task_to_date').datepicker({ format: 'yyyy-mm-dd', autoclose: true });
    function monthLabelOnChange() {
        var fc_toolbar_mnthLabel = $('.fc-toolbar').find('.fc-center').find('h2');
        var monthLabel = fc_toolbar_mnthLabel.text();
        $('.week-view-label').text(monthLabel);
    };
    function toggleCalendarDropDownIcon(dropdown) {
        $(dropdown).toggleClass("fa-chevron-down").toggleClass("fa-chevron-up");
    };
    $('.mng-event-popup-weekday-btn').on('click', function () {
        $('.mng-event-popup-weekday-btn').removeClass('mng-event-popup-weekday-btn-sel mng-event-popup-weekday-btn-unsel').addClass('mng-event-popup-weekday-btn-unsel');
        $(this).toggleClass('mng-event-popup-weekday-btn-unsel').toggleClass('mng-event-popup-weekday-btn-sel');
        $('#calendar_task_from_date').val($(this).find('.week_day_date_disp').text());
    });
    $('.mng-event-popup-weekday-btn_to').on('click', function () {
        $('.mng-event-popup-weekday-btn_to').removeClass('mng-event-popup-weekday-btn-sel mng-event-popup-weekday-btn-unsel').addClass('mng-event-popup-weekday-btn-unsel');
        $(this).toggleClass('mng-event-popup-weekday-btn-unsel').toggleClass('mng-event-popup-weekday-btn-sel');
        $('#calendar_task_to_date').val($(this).find('.week_day_date_disp_to').text());
    });
    $('#eventPopupDiv').on('hidden.bs.modal', function () {
        resetEventPopupValues();
        calendarHideAlertMessage(true);
    });
    $('#calendarheader-year-drpdwn').focusout(function () {
        $('#yrdropdownIcon').removeClass("fa-chevron-up fa-chevron-down").addClass("fa-chevron-down");
    });
    $('#calendarheader-year-drpdwn').on('click', function () {
        toggleCalendarDropDownIcon('#yrdropdownIcon');
    });
    $('#calendarheader-week-drpdwn').focusout(function () {
        $('#weekdropdownIcon').removeClass("fa-chevron-up fa-chevron-down").addClass("fa-chevron-down");
    });
    $('#calendarheader-week-drpdwn').on('click', function () {
        toggleCalendarDropDownIcon('#weekdropdownIcon');
    });
    $('#mng_event_popup_employee_drpdwn').focusout(function () {
        $('#mng-event-popup-drpDwnBtn-inskriven-icon').removeClass("fa-chevron-up fa-chevron-down").addClass("fa-chevron-down");
    });
    $('#mng_event_popup_employee_drpdwn').on('click', function () {
        toggleCalendarDropDownIcon('#mng-event-popup-drpDwnBtn-inskriven-icon');
    });
    $('#drp4').on('hide.bs.dropdown', function (event) {
        toggleCalendarDropDownIcon('#drpDwnBtnLastIcon');
        $('.typeahead_custom_searchBar').val('');
    });
    $('#drp4').on("show.bs.dropdown", function (event) {
        toggleCalendarDropDownIcon('#drpDwnBtnLastIcon');
        $('.typeahead_custom_searchBar').val('');
    });
    $('#weekview-prevbtn').on('click', function () {
        $('.fc-prev-button').trigger('click');
    });
    $('#weekview-nextbtn').on('click', function () {
        $('.fc-next-button').trigger('click');
    });
    $('#week-today-btn').on('click', function () {
        $('.fc-today-button').trigger('click');
        monthLabelOnChange();
        fetchEvents();
    });
    $(document).on('click', '.addMember-remove', function () {
        var row_id = $(this).attr('data-id');
        var container = $('#event_employee_added_container');
        if (container && container.length) {
            var row = $(container).find('.addMember-item[data-id="' + row_id + '"]');
            row.remove();
            task_added_employee.splice($.inArray(row_id, task_added_employee), 1);
        }
    });
    /*Typeahead starts*/
    $('#clearSelEmpBtn').on('click', function () {
        $('.calendarheader_people_sel').val(0);
        $('.calendarheader_people_sel').text($('#all_people_label').val());
        sel_employee_id = 0;
        $('#clearSelEmpBtn').hide();
        fetchEvents();
    });
    $.typeahead({
        input: '.js-typeahead-people-v2',
        minLength: 1,
        maxItem: 20,
        order: "asc",
        dynamic: true,
        delay: 500,
        href: "javascript:void(0);",
        template: function (query, item) {
            return '<span><img class="user_search_img" src={{userphoto}}></span>&nbsp;<span class="grunduppgifter_vvn_boldtext">{{name}}</span><br> <small style="color:#999;" class="grunduppgifter_vvn_greytext">{{designation}}</small>'
        },
        emptyTemplate: '<strong style="color:red;" class="grunduppgifter_vvn_boldtext">No results found !!</strong>',
        source: {
            user: {
                display: "name",
                ajax: function (query) {
                    var formData = {
                        searchText: query
                    }
                    return {
                        type: 'POST',
                        url: '/admin/searchUser',
                        data: JSON.stringify(formData),
                        contentType: "application/json",
                        dataType: 'JSON',
                        callback: {
                            done: function (data) {
                                var arr = [{
                                    id: 0,
                                    name: "",
                                    userphoto: '/Content/Images/avatar.png',
                                    designation: ''
                                }];
                                data = data.data;
                                if (data && data.length > 0) {
                                    arr = [];
                                    arr.length = 0;
                                    for (var i = 0; i < data.length; i++) {
                                        arr.push({
                                            id: data[i].EmpID,
                                            name: data[i].EmpFirstname + " " + data[i].EmpLastname,
                                            userphoto: (data[i].UserPhotoPath) ? '/Content/UserIcons/' + data[i].UserPhotoPath :
                                                ((data[i].Gender == 'M') ? '/Content/Images/male-avatar.png' : '/Content/Images/female-avatar.png'),
                                            designation: data[i].Designation
                                        });
                                    }
                                }
                                return arr;
                            }
                        }
                    }
                }
            },
            project: {
                display: "project",
                href: function (item) {
                },
                ajax: [{
                    type: 'POST',
                    url: '/admin/searchUser',
                    data: JSON.stringify({ searchText: '' }),
                }, "data.project"],

                template: '<span><img class="user_search_img" src={{userphoto}}></span>&nbsp;<span class="grunduppgifter_vvn_boldtext">{{name}}</span><br> <small style="color:#999;" class="grunduppgifter_vvn_greytext">{{designation}}</small>'
            }
        },
        callback: {
            onClick: function (node, a, item, event) {
                event.preventDefault();
                if (item) {
                    if (0 < item.id) {
                        $('.calendarheader_people_sel').val(item.id);
                        $('.calendarheader_people_sel').text(item.name);
                        sel_employee_id = item.id;
                        $('#clearSelEmpBtn').show();
                        $('#week-calendar-view').fullCalendar('removeEvents');
                        fetchEvents();
                    }
                }
            },
            onNavigateAfter: function (node, lis, a, item, query, event) {
                if (~[38, 40].indexOf(event.keyCode)) {
                    var resultList = node.closest("form").find("ul.typeahead__list"),
                    activeLi = lis.filter("li.active"),
                    offsetTop = activeLi[0] && activeLi[0].offsetTop - (resultList.height() / 2) || 0;
                    resultList.scrollTop(offsetTop);
                }
            },
            onClickAfter: function (node, a, item, event) {
                event.preventDefault();
            },
            onResult: function (node, query, result, resultCount) {
                if (query === "") return;
                var text = "";
                if (result.length > 0 && result.length < resultCount) {
                    text = "Showing <strong>" + result.length + "</strong> of <strong>" + resultCount + '</strong> elements matching "' + query + '"';
                } else if (result.length > 0) {
                    text = 'Showing <strong>' + result.length + '</strong> elements matching "' + query + '"';
                } else {
                    text = 'No results matching "' + query + '"';
                }
                $('#result-container').html(text);
            },
            onMouseEnter: function (node, a, item, event) {
                if (item.group === "country") {
                    $(a).append('<span class="flag-chart flag-' + item.display.replace(' ', '-').toLowerCase() + '"></span>')
                }
            },
            onMouseLeave: function (node, a, item, event) {
                $(a).find('.flag-chart').remove();
            }
        }
    });
    $('body').bind('click', function (e) {
        if (!$(e.target).is('.js-typeahead-admin-officer-v2')) {
            $('.js-typeahead-people-v2').val('');
            $('#task_add_employee').val('');
            $('.typeahead__list').empty();
        }
    });
    if ($('#task_add_employee').length) {
        $.typeahead({
            input: '#task_add_employee',
            minLength: 1,
            maxItem: 20,
            order: "asc",
            dynamic: true,
            delay: 500,
            href: "javascript:void(0);",
            template: function (query, item) {
                return '<span><img class="user_search_img" src={{userphoto}}></span>&nbsp;<span class="grunduppgifter_vvn_boldtext">{{name}}</span><br> <small style="color:#999;" class="grunduppgifter_vvn_greytext">{{designation}}</small>'
            },
            emptyTemplate: '<strong style="color:red;" class="grunduppgifter_vvn_boldtext">No results found !!</strong>',
            source: {
                user: {
                    display: "name",
                    ajax: function (query) {
                        var formData = {
                            searchText: query
                        }
                        return {
                            type: 'POST',
                            url: '/admin/searchUser',
                            data: JSON.stringify(formData),
                            contentType: "application/json",
                            dataType: 'JSON',
                            callback: {
                                done: function (data) {
                                    var arr = [{
                                        id: 0,
                                        name: "",
                                        userphoto: '/Content/Images/avatar.png',
                                        designation: ''
                                    }];
                                    data = data.data;
                                    if (data && data.length > 0) {
                                        arr = [];
                                        arr.length = 0;
                                        for (var i = 0; i < data.length; i++) {
                                            arr.push({
                                                id: data[i].EmpID,
                                                name: data[i].EmpFirstname + " " + data[i].EmpLastname,
                                                userphoto: (data[i].UserPhotoPath) ? '/Content/UserIcons/' + data[i].UserPhotoPath :
                                                    ((data[i].Gender == 'M') ? '/Content/Images/male-avatar.png' : '/Content/Images/female-avatar.png'),
                                                designation: data[i].Designation
                                            });
                                        }
                                    }
                                    return arr;
                                }
                            }
                        }
                    }
                },
                project: {
                    display: "project",
                    href: function (item) {
                    },
                    ajax: [{
                        type: 'POST',
                        url: '/admin/searchUser',
                        data: JSON.stringify({ searchText: '' }),
                    }, "data.project"],

                    template: '<span><img class="user_search_img" src={{userphoto}}></span>&nbsp;<span class="grunduppgifter_vvn_boldtext">{{name}}</span><br> <small style="color:#999;" class="grunduppgifter_vvn_greytext">{{designation}}</small>'
                }
            },
            callback: {
                onClick: function (node, a, item, event) {
                    event.preventDefault();
                    if (item) {
                        if (0 < item.id) {
                            var event_employee_added_container = $('#event_employee_added_container');
                            if (event_employee_added_container && event_employee_added_container.length) {
                                var idx = -1;
                                if (task_added_employee && task_added_employee.length) {
                                    idx = $.inArray(item.id, task_added_employee);
                                }
                                if (idx == -1) {
                                    var sel_employee_li = '<li class="addMember-item" data-id="' + item.id + '"> <img class="employee-select-avatar" src="' + item.userphoto + '" title="' + item.name + '"> <span class="addMember-remove" data-id="' + item.id + '"><i class="fa fa-minus-circle" aria-hidden="true"></i></span></li>';
                                    $(event_employee_added_container).append(sel_employee_li);
                                    task_added_employee.push(item.id);
                                } else {
                                    calendarShowAlertMessage('Employee already added', 'warning');
                                }
                            }
                        }
                    }
                },
                onNavigateAfter: function (node, lis, a, item, query, event) {
                    if (~[38, 40].indexOf(event.keyCode)) {
                        var resultList = node.closest("form").find("ul.typeahead__list"),
                        activeLi = lis.filter("li.active"),
                        offsetTop = activeLi[0] && activeLi[0].offsetTop - (resultList.height() / 2) || 0;
                        resultList.scrollTop(offsetTop);
                    }
                },
                onClickAfter: function (node, a, item, event) {
                    event.preventDefault();
                },
                onResult: function (node, query, result, resultCount) {
                    if (query === "") return;
                    var text = "";
                    if (result.length > 0 && result.length < resultCount) {
                        text = "Showing <strong>" + result.length + "</strong> of <strong>" + resultCount + '</strong> elements matching "' + query + '"';
                    } else if (result.length > 0) {
                        text = 'Showing <strong>' + result.length + '</strong> elements matching "' + query + '"';
                    } else {
                        text = 'No results matching "' + query + '"';
                    }
                    $('#result-container').html(text);
                },
                onMouseEnter: function (node, a, item, event) {
                    if (item.group === "country") {
                        $(a).append('<span class="flag-chart flag-' + item.display.replace(' ', '-').toLowerCase() + '"></span>')
                    }
                },
                onMouseLeave: function (node, a, item, event) {
                    $(a).find('.flag-chart').remove();
                }
            }
        });
    }
    /*Typeahead ends*/
});
