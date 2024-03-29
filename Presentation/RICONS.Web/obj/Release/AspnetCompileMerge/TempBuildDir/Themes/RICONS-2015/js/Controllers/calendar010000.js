﻿$(document).ready(function () {
    var view = "week";

    var DATA_FEED_URL = "ajax/calendar/datafeed.ashx";
    var op = {
        view: view,
        theme: 3,
        showday: new Date(),
        EditCmdhandler: Edit,
        DeleteCmdhandler: Delete,
        ViewCmdhandler: View,
        onWeekOrMonthToDay: wtd,
        onBeforeRequestData: cal_beforerequest,
        onAfterRequestData: cal_afterrequest,
        onRequestDataError: cal_onerror,
        autoload: true,
        url: DATA_FEED_URL + "?method=list",
        quickAddUrl: DATA_FEED_URL + "?method=add",
        quickUpdateUrl: DATA_FEED_URL + "?method=update",
        quickDeleteUrl: DATA_FEED_URL + "?method=remove"
    };
    var $dv = $("#calhead");
    var _MH = document.documentElement.clientHeight;
    var dvH = $dv.height();
    op.height = _MH - dvH;
    op.eventItems = [];

    var p = $("#gridcontainer").bcalendar(op).BcalGetOp();
    if (p && p.datestrshow) {
        $("#txtdatetimeshow").text(p.datestrshow);
    }
    p = $("#gridcontainer").gotoDate().BcalGetOp();
    if (p && p.datestrshow) {
        $("#txtdatetimeshow").text(p.datestrshow);
    }
    $("#caltoolbar").noSelect();

    $("#hdtxtshow").datepicker({
        picker: "#txtdatetimeshow", showtarget: $("#txtdatetimeshow"),
        onReturn: function (r) {
            $("#caltoolbar div.fcurrent").each(function () {
                $(this).removeClass("fcurrent");
            })
            $('#showdaybtn').addClass("fcurrent");
            var p = $("#gridcontainer").swtichView("day").BcalGetOp();
            p = $("#gridcontainer").gotoDate(r).BcalGetOp();
            if (p && p.datestrshow) {
                $("#txtdatetimeshow").text(p.datestrshow);
            }
        }
    });
    function cal_beforerequest(type) {
        var t = "Đang xử lý...";
        switch (type) {
            case 1:
                t = "Đang xử lý...";
                break;
            case 2:
            case 3:
            case 4:
                t = "Đang xử lý...";
                break;
        }
        $("#errorpannel").hide();
        //ShortAlertWithAutoHide(t);
    }
    function cal_afterrequest(type) {
        switch (type) {
            case 1:
                RemoveLoader();
                break;
            case 2:
            case 3:
            case 4:
                AddLoader();
                break;
        }

    }
    function cal_onerror(type, data) {
        $("#errorpannel").show();
    }
    function Edit(data) {
        var eurl = "editCalendar.aspx?id={0}&start={2}&end={3}&isallday={4}&title={1}";
        if (data) {
            if (data[0] == '0') {
                var url = "editCalendar.aspx";
                OpenModelWindow(url, { width: 500, height: 345, caption: "Tạo sự kiện mới" });
            }
            else {
                var url = StrFormat(eurl, data);
                OpenModelWindow(url, {
                    width: 500, height: 345, caption: "Chỉnh sửa sự kiện", onclose: function () {
                        $("#gridcontainer").reload();
                    }
                });
            }
        }
    }
    function View(data) {
        var str = "";
        $.each(data, function (i, item) {
            str += "[" + i + "]: " + item + "\n";
        });
        alert(str);
    }
    function Delete(data, callback) {

        $.alerts.okButton = "Đồng ý";
        $.alerts.cancelButton = "Hủy";
        hiConfirm("Bạn có muốn xóa sự kiện này", 'Thông báo', function (r) { r && callback(0); });
    }
    function wtd(p) {
        if (p && p.datestrshow) {
            $("#txtdatetimeshow").text(p.datestrshow);
        }
        $("#caltoolbar div.fcurrent").each(function () {
            $(this).removeClass("fcurrent");
        })
        $("#showdaybtn").addClass("fcurrent");
    }
    //to show day view
    $("#showdaybtn").click(function (e) {
        //document.location.href="#day";
        $("#caltoolbar div.fcurrent").each(function () {
            $(this).removeClass("fcurrent");
        })
        $(this).addClass("fcurrent");
        var p = $("#gridcontainer").swtichView("day").BcalGetOp();
        if (p && p.datestrshow) {
            $("#txtdatetimeshow").text(p.datestrshow);
        }
    });
    //to show week view
    $("#showweekbtn").click(function (e) {
        //document.location.href="#week";
        $("#caltoolbar div.fcurrent").each(function () {
            $(this).removeClass("fcurrent");
        })
        $(this).addClass("fcurrent");
        var p = $("#gridcontainer").swtichView("week").BcalGetOp();
        if (p && p.datestrshow) {
            $("#txtdatetimeshow").text(p.datestrshow);
        }

    });
    //to show month view
    $("#showmonthbtn").click(function (e) {
        //document.location.href="#month";
        $("#caltoolbar div.fcurrent").each(function () {
            $(this).removeClass("fcurrent");
        })
        $(this).addClass("fcurrent");
        var p = $("#gridcontainer").swtichView("month").BcalGetOp();
        if (p && p.datestrshow) {
            $("#txtdatetimeshow").text(p.datestrshow);
        }
    });

    $("#showreflashbtn").click(function (e) {
        $("#gridcontainer").reload();
    });

    //Add a new event
    $("#faddbtn").click(function (e) {
        //var url = "editCalendar.aspx";
        //OpenModelWindow(url, { width: 500, height: 345, caption: "Tạo sự kiện mới" });
        window.location =linkContent + "calendar/create";
    });
    //go to today
    $("#showtodaybtn").click(function (e) {
        var p = $("#gridcontainer").gotoDate().BcalGetOp();
        if (p && p.datestrshow) {
            $("#txtdatetimeshow").text(p.datestrshow);
        }


    });
    //previous date range
    $("#sfprevbtn").click(function (e) {
        var p = $("#gridcontainer").previousRange().BcalGetOp();
        if (p && p.datestrshow) {
            $("#txtdatetimeshow").text(p.datestrshow);
        }

    });
    //next date range
    $("#sfnextbtn").click(function (e) {
        var p = $("#gridcontainer").nextRange().BcalGetOp();
        if (p && p.datestrshow) {
            $("#txtdatetimeshow").text(p.datestrshow);
        }
    });
});