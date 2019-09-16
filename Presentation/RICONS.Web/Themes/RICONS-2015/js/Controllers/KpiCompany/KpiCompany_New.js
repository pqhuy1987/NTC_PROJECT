
var configChosenDanhmucvanphongpham = {
    '.chosen-select-plan': { width: "268px", disable_search_threshold: 10 },
    '.chosen-select-nguoithuchien': { width: "80%", disable_search_threshold: 5 },
    '.chosen-select-nguoithuchien1': { width: "100%", disable_search_threshold: 5 },
    '.chosen-select-loaikehoach': { width: "100%", disable_search_threshold: 10 }
}

var dataHeader_Milestones =
[{
    col_class: 'header-box',
    col_id: '',
    col_value: [{
        colspan: 1,
        col_class: 'ovh col1',
        col_id: '',
        style: '',
        col_value: '<input type="checkbox" onclick="SelectAll(this);" class="chkCheck" />'
    },
    {
        colspan: 1,
        col_class: 'ovh col2 stt',
        col_id: '',
        col_value: 'STT'
    },
    {
        colspan: 1,
        col_class: 'ovh col3',
        col_id: '',
        style: '',
        col_value: 'Mã VPP'
    },
    {
        colspan: 1,
        col_class: 'ovh col4',
        col_id: '',
        style: '',
        col_value: 'Tên văn phòng phẩm'
    },

    {
        colspan: 1,
        col_class: 'ovh col5',
        col_id: '',
        style: '',
        col_value: 'Đơn vị tính'
    },

     {
         colspan: 1,
         col_class: 'ovh col6',
         col_id: '',
         style: '',
         col_value: 'Số lượng'
     },

    {
        colspan: 1,
        col_class: 'ovh col7',
        col_id: '',
        style: '',
        col_value: 'Số tiền'
    },

    {
        colspan: 1,
        col_class: 'ovh col8',
        col_id: '',
        style: '',
        col_value: 'Thành tiền'
    },
    {
            colspan: 1,
            col_class: 'ovh col9',
            col_id: '',
            style: '',
            col_value: ''
    }]
}];


$(document).ready(function (e) {
    $('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
    $('#ScrollContent .boxTableSelected').slimscroll({ height: 100 });

    //chosen combobox jquery
    for (var selector in configChosenDanhmucvanphongpham) {
        $(selector).chosen(configChosenDanhmucvanphongpham[selector]);
    }

    //var ngayBH = $('boxContent #txtngaybanhanh');
    //ngayBH.datepicker({
    //    dateFormat: "dd/mm/yy",
    //    defaultDate: new Date(),
    //    dayNamesShort: $.datepicker.regional.vi.dayNamesShort,
    //    dayNames: $.datepicker.regional.vi.dayNames,
    //    monthNamesShort: $.datepicker.regional.vi.monthNamesShort,
    //    monthNames: $.datepicker.regional.vi.monthNames,
    //    showAnim: "slideDown",
    //    firstDay: 1 // Start with Monday
    //});
    //ngayBH.datepicker("setDate", new Date());
    //ngayBH.change(function () {
    //    if (!check_date(this))
    //        ngayBH.datepicker("setDate", new Date());
    //    //if (check_over_date($(this).val(), ngayKT.val())) {
    //    //    ngayKT.datepicker("setDate", $(this).val());
    //    //}
    //});

    //var ngayCN = $('#txtngaycapnhat');
    //ngayCN.datepicker({
    //    dateFormat: "dd/mm/yy",
    //    defaultDate: new Date(),
    //    dayNamesShort: $.datepicker.regional.vi.dayNamesShort,
    //    dayNames: $.datepicker.regional.vi.dayNames,
    //    monthNamesShort: $.datepicker.regional.vi.monthNamesShort,
    //    monthNames: $.datepicker.regional.vi.monthNames,
    //    showAnim: "slideDown",
    //    firstDay: 1 // Start with Monday
    //});
    //ngayCN.datepicker("setDate", new Date());
    //ngayBD.change(function () {
    //    if (!check_date(this))
    //        ngayBD.datepicker("setDate", new Date());
    //    if (check_over_date($(this).val(), ngayKT.val())) {
    //        ngayKT.datepicker("setDate", $(this).val());
    //    }
    //});

    var ngaylapkpi = $('#txtnguoilapkpi_ngay');
    ngaylapkpi.datepicker({
        dateFormat: "dd/mm/yy",
        defaultDate: new Date(),
        dayNamesShort: $.datepicker.regional.vi.dayNamesShort,
        dayNames: $.datepicker.regional.vi.dayNames,
        monthNamesShort: $.datepicker.regional.vi.monthNamesShort,
        monthNames: $.datepicker.regional.vi.monthNames,
        showAnim: "slideDown",
        firstDay: 1 // Start with Monday
    });
    ngaylapkpi.datepicker("setDate", new Date());


    var ngayptgd = $('#txtphotongxemxetkpi_ngay');
    ngayptgd.datepicker({
        dateFormat: "dd/mm/yy",
        defaultDate: new Date(),
        dayNamesShort: $.datepicker.regional.vi.dayNamesShort,
        dayNames: $.datepicker.regional.vi.dayNames,
        monthNamesShort: $.datepicker.regional.vi.monthNamesShort,
        monthNames: $.datepicker.regional.vi.monthNames,
        showAnim: "slideDown",
        firstDay: 1 // Start with Monday
    });
    ngayptgd.datepicker("setDate", new Date());

    var ngaytgd = $('#txttonggiamdocxemxetkpi_ngay');
    ngaytgd.datepicker({
        dateFormat: "dd/mm/yy",
        defaultDate: new Date(),
        dayNamesShort: $.datepicker.regional.vi.dayNamesShort,
        dayNames: $.datepicker.regional.vi.dayNames,
        monthNamesShort: $.datepicker.regional.vi.monthNamesShort,
        monthNames: $.datepicker.regional.vi.monthNames,
        showAnim: "slideDown",
        firstDay: 1 // Start with Monday
    });
    ngaytgd.datepicker("setDate", new Date());

    $('#btn-inbieumau').click(function (e) {
        debugger;
        //var listChecked = $('.chkCheck:checked');
        ////var duyet = $(listChecked[0]).attr('duyet');
        //var ID = $(listChecked[0]).attr('mxk');
        //if (listChecked.length == 2) {
        //    ID = $(listChecked[1]).attr('mxk');
        //}
        //else if (listChecked.length == 1) {
        //    ID = $(listChecked[0]).attr('mxk');
        //}
        //else {
        //    alert('Đề nghị cung cấp vật tư thiết bị chọn không đúng 1 dòng');
        //    return false;
        //}

        //ShowMessageBox('Bạn muốn xuất Excel?', "Thong Bao", "ok", "question");

        var DataJson = "{'makpicongty':'" + $('#txtmakpicongty').val() + "'}";
        fncPostData(linkContent + 'KpiCompany/XuatExcel', DataJson);
    });



    //GetData(false);

});

function GetData(isHeader, mpb, stt) {
    debugger;
    var makpicongty = $('#txtmakpicongty').val();
    var DataJson = "{'makpicongty':'" + makpicongty + "'}";
    fncGetData_loadkhong_header(linkContent + 'KpiCompany/SelectRows_KpiLevelCompanyDetail_hieuchinh', DataJson, isHeader);

}

function GetData_loaddata(isHeader, mpb, stt) {
    debugger;
    var makpiphongban = $('#txtmakpiphongban').val();
    var DataJson = "{'makpiphongban':'" + makpiphongban + "'}";
    fncGetData_loadkhong_header(linkContent + 'KpiCompany/SelectRows_KpiLevelDepartmentDetail_hieuchinh', DataJson, isHeader);

}

//function GetData(isHeader, mpb, stt) {

//    var manv = $('#txtmanvs').val(); 
//    var hovaten = $('#txthovatens').val();
//    var maphongban = $('#filter02').val();

//    if ($('#chkkiemtra:checked').length == 0)
//        chkktra = 0;

//    var DataJson = "{'manv':'" + manv + "'," +
//                    "'maphongban':'" + maphongban + "'," +
//                    "'hovaten':'" + hovaten + "'" +
//                  "}";

//    fncGetData_loadkhong_header(linkContent + 'AddStaff/SelectRows_ReaddataEmployee', DataJson, isHeader);
//}


function ShowSubLine(e) {
    //tim nhung dong sub
    var maKeHoach = "";//$(e).attr('mpb');
    var tableContent = $("#tableContent");
    var rows = tableContent.find('tr[subparent="' + maKeHoach + '"]');
    if(rows.length > 0)
    {
        rows.slideToggle();
    }
    else
    {
        GetData(false, maKeHoach, $(e).parent().parent().find(".stt").html().trim());
    }
}

//ham set vi tri combobox chon gio
//function fncPostionDropDown(chosenDrop, objUpdateValue) {
//    chosenDrop = $(".chosen-drop-grid").css({
//        "top": objUpdateValue.position().top + objUpdateValue.outerHeight(true),
//        "left": objUpdateValue.position().left,
//    });
//}