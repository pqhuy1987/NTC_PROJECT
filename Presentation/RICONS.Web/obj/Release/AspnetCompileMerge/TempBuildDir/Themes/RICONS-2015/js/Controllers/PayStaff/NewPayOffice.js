
var configChosenDanhmucvanphongpham = {
    '.chosen-select-plan': { width: "100%", disable_search_threshold: 10 },
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

    var ngayBD = $('#txtngayyeucau');
    ngayBD.datepicker({
        dateFormat: "dd/mm/yy",
        defaultDate: new Date(),
        dayNamesShort: $.datepicker.regional.vi.dayNamesShort,
        dayNames: $.datepicker.regional.vi.dayNames,
        monthNamesShort: $.datepicker.regional.vi.monthNamesShort,
        monthNames: $.datepicker.regional.vi.monthNames,
        showAnim: "slideDown",
        firstDay: 1 // Start with Monday
    });
    //ngayBD.datepicker("setDate", new Date());
    ngayBD.change(function () {
        if (!check_date(this))
            ngayBD.datepicker("setDate", new Date());
        if (check_over_date($(this).val(), ngayKT.val())) {
            ngayKT.datepicker("setDate", $(this).val());
        }
    });



});

function GetData(isHeader, mpb, stt) {
    debugger;
    var mabosungnhansu = $('#filter01').val();
    var DataJson = "{'mabosungnhansu':'" + mabosungnhansu + "'" +
                  "}";

    fncGetData_Hienthidulieu_theocongtruong(linkContent + 'PayStaff/SelectRows_ddns_addstaff_ddns_vanphong_kehoachbosungnhansu', DataJson, isHeader);
}

//function GetData(isHeader, mpb, stt) {
//    //debugger;
//    var mabosungnhansu = $('#txtmabosungnhansu').val();
//    var DataJson = "{'mabosungnhansu':'" + mabosungnhansu + "'}";
//    //fncGetData_loadkhong_header(linkContent + 'AddStaff/SelectRows_ddns_lapdanhsachdieudong', DataJson, isHeader);
//    fncGetData_loadkhong_header_content1(linkContent + 'AddStaff/SelectRows_layds_congtruong_kehoachbosung', DataJson, isHeader);
//}


function GetData_ddns_timkiemds_saukhiluu(isHeader, mpb, stt) {
    //debugger;
    var matranhansu = $('#txtmatranhansu').val();
    var DataJson = "{'matranhansu':'" + matranhansu + "'}";
    fncGetData_loadkhong_header(linkContent + 'PayStaff/SelectRows_ddns_timkiemds_saukhiluu', DataJson, isHeader);
    //fncGetData_loadkhong_header_lienlac(linkContent + 'AddStaff/SelectRows_ddns_lapdanhsachdieudong_thongtinlienlac', DataJson, isHeader);
    //fncGetData_loadkhong_header_content1(linkContent + 'AddStaff/SelectRows_layds_congtruong_kehoachbosung', DataJson, isHeader);
}

function GetData_lapdanhsachdieudong_timkiem(isHeader, mpb, stt) {
    debugger;
    var mabosungnhansu = $('#txtmabosungnhansu').val();
    var thongtin = $('#txtmanvs').val();
    var DataJson = "{'mabosungnhansu':'" + mabosungnhansu + "',"
                    + "'manhanvien':'" + thongtin + "'"
                    + "}";
    fncGetData_loadkhong_header(linkContent + 'PayStaff/SelectRows_ddns_lapds_chuyentravanphong_timkiem', DataJson, isHeader);
    //fncGetData_loadkhong_header_content1(linkContent + 'AddStaff/SelectRows_layds_congtruong_kehoachbosung', DataJson, isHeader);
}

function SelectRows_ddns_lapdanhsachdieudong_timkiem_lienlaccongtruong(isHeader, mpb, stt) {
    debugger;
    var mabosungnhansu = $('#txtmabosungnhansu').val();
    var thongtin = $('#txtmanvslienlac').val();
    var DataJson = "{'mabosungnhansu':'" + mabosungnhansu + "',"
                    + "'manhanvien':'" + thongtin + "'"
                    + "}";

    fncGetData_loadkhong_header_lienlac(linkContent + 'AddStaff/SelectRows_ddns_lapdanhsachdieudong_timkiem_lienlaccongtruong', DataJson, isHeader);
    //fncGetData_loadkhong_header_content1(linkContent + 'AddStaff/SelectRows_layds_congtruong_kehoachbosung', DataJson, isHeader);
}


function ShowSubLine(e) {
    //tim nhung dong sub
    var maKeHoach = "";//$(e).attr('mpb');
    var tableContent = $("#tableContent");
    var rows = tableContent.find('tr[subparent="' + maKeHoach + '"]');
    if (rows.length > 0) {
        rows.slideToggle();
    }
    else {
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