﻿
var configChosenDangkyvanphongpham = {
    '.chosen-select-plan': { width: "290px", disable_search_threshold: 10 },
    '.chosen-select-chonthang': { width: "120px", disable_search_threshold: 10 },
    '.chosen-select-loaikehoach': { width: "200px", disable_search_threshold: 10 }
}

var dataHeader_Milestones =
[{
    col_class: 'header-box',
    col_id: '',
    col_value: [{
        colspan: 1,
        col_class: 'ovh col1',
        col_id: '',
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
        col_value: 'Mã ĐK'
    },
    {
        colspan: 1,
        col_class: 'ovh col4',
        col_id: '',
        col_value: 'Họ và tên'
    },
    {
        colspan: 1,
        col_class: 'ovh col5',
        col_id: '',
        col_value: 'Chức danh'
    },
     {
         colspan: 1,
         col_class: 'ovh col6',
         col_id: '',
         col_value: 'Phòng ban'
     },
     {
         colspan: 1,
         col_class: 'ovh col7',
         col_id: '',
         col_value: 'Ngày đăng ký'
     },
      {
          colspan: 1,
          col_class: 'ovh col8',
          col_id: '',
          col_value: 'Tổng tiền'
      },

    {
        colspan: 1,
        col_class: 'ovh col9',
        col_id: '',
        col_value: 'Ghi chú'
    }]
}];


$(document).ready(function (e) {
    //$('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
    //chosen combobox jquery
    //for (var selector in configChosenDangkyvanphongpham) {
    //    $(selector).chosen(configChosenDangkyvanphongpham[selector]);
    //}
    GetData_Table(true);

});

function GetData_Table(isHeader, mpb, stt) {
    //var makehoach = CheckNullOrEmpty(mkh) == true ? "0" : maphongban;
    //var STT = CheckNullOrEmpty(stt) == true ? "0" : stt;
    debugger;

    var DataJson = "";
    fncGetData_Table(linkContent + 'Home/SelectRows_DSDangkyvpp_Table', DataJson, isHeader);
}








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