var blockNumber = 1;  //Infinite Scroll starts from second block
var NoMoreData = false;
var inProgress = false;
$(document).ready(function () {
    blockNumber = 1;
    $('#cardContainer').empty();
    showLoad();
    UpdateEmpTable();
    clearValidationError();

    navigateTo(USERS);
    $.typeahead({
        input: '.js-typeahead-user-v2',
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

                                if (data && data.data && data.data.length > 0) {
                                    arr = [];
                                    arr.length = 0;
                                    data = data.data;
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
                href: function (item) { },
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
                    console.log('selected employee :', item);
                    if (item.id > 0) {
                        viewEmpDetails(item.id);
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
        if (!$(e.target).is('.js-typeahead-user-v2')) {
            $('.js-typeahead-user-v2').val('');
            $('.typeahead__list').empty();
        }
    });
    $('#user_list_sorting_order').on('change', function () {
        blockNumber = 1;
        $('#cardContainer').empty();
        showLoad();
        UpdateEmpTable();
    });
});
function showLoad() {
    $("#imgProg").show();
    $(".usercontainer").addClass("disablediv");
    $(".WaitdivPage").css("display", "block");
};
function HideLoad() {
    $("#imgProg").hide();
    $(".usercontainer").removeClass("disablediv");
    $(".WaitdivPage").css("display", "none");
};
function viewEmpDetails(empId) {   
    location.href = "/admin/viewEmployeeDetails?empId=" + empId;
};
$("div.dashboardContentDiv").scroll(function () {
    if ($(".wrapper").scrollTop() == $(".wrapper").height() - $(".dashboardContentDiv").height() && !NoMoreData && !inProgress) {
        inProgress = true;
        $("#loadingDiv").show();
        UpdateEmpTable(blockNumber);
    }
});
function UpdateEmpTable(currBlockNumber) {
    //console.log(currBlockNumber);    
    $.ajax({
        url: "/Admin/GetEmployeeData",
        type: "POST",
        data: JSON.stringify({ BlockNumber: currBlockNumber, sortingVal: $('#user_list_sorting_order').val() }),
        datatype: "json",
        contentType: "application/json",
        success: function (status) {
            if (status && "OK" == status.UpdateStatus && status.data) {
                var response = status.data;
                if (response && response.length > 0) {
                    blockNumber = blockNumber + 1;                    
                    NoMoreData = response[0].NoMoreData;
                    var maleAvatarPath = "../Content/Images/male-avatar.png";
                    var femaleAvatarPath = "../Content/Images/female-avatar.png";
                    for (var i = 0; i <= response.length - 1 ; i++) {
                        if (response[i].Usertype != 1) {
                            response[i].UserPhotoPath = (stringIsNull(response[i].UserPhotoPath)
                                ? (("M" == response[i].Gender.trim())
                                ? maleAvatarPath : femaleAvatarPath) : UserPhotoBaseUrl + response[i].UserPhotoPath);
                            $('#cardContainer').append('<div class="col-lg-2 col-md-3 col-sm-3 col-xs-12"><div class="matcard card" onclick="viewEmpDetails(' + response[i].EmpID + ')" data-id="' + response[i].EmpID + '"><img class="usrAvatar" src=' + response[i].UserPhotoPath + '><div class="text-center"><h5><b class="admin_dashboard_overflow_text" title="' + response[i].EmpFirstname + " " + response[i].EmpLastname + '">' + response[i].EmpFirstname + " " + response[i].EmpLastname + '</b></h5><p class="admin_dashboard_overflow_text" title="' + response[i].Designation + '">' + response[i].Designation + '</p></div></div></div>');
                        }
                    }
                }
            }
            HideLoad();
            $("#loadingDiv").hide();
            inProgress = false;
        },
        error: function () {
            HideLoad();
            $("#loadingDiv").hide();
            ymz.jq_alert({
                title: "HRMS",
                text: "An unexpected error occured !!",
                ok_btn: "OK",
                close_fn: null
            });
        }
    });
};
function AddEmployee() {    
    location.href = "/admin/addEmployee";
};
function reloadEmpData() {
    blockNumber = 1;
    showLoad();
    $('#cardContainer').empty();    
    UpdateEmpTable(1);
};