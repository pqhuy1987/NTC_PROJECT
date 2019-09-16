
var configChosenDangkyvanphongpham = {
    '.chosen-select-plan': { width: "290px", disable_search_threshold: 10 },
    '.chosen-select-chonthang': { width: "120px", disable_search_threshold: 10 },
    '.chosen-select-loaikehoach': { width: "200px", disable_search_threshold: 10 }
}

var dataHeader_Milestones =
[{
    col_class: 'header-box',
    col_id: '',
    col_value: [//{
    //    colspan: 1,
    //    col_class: 'ovh col1',
    //    col_id: '',
    //    col_value: '<input type="checkbox" onclick="SelectAll(this);" class="chkCheck" />'
    //},
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
        col_value: 'Tên văn phòng phẩm'
    },
    {
        colspan: 1,
        col_class: 'ovh col4',
        col_id: '',
        col_value: 'Đơn vị tính'
    },
    {
        colspan: 1,
        col_class: 'ovh col5',
        col_id: '',
        col_value: 'Đơn giá'
    },
    {
        colspan: 1,
        col_class: 'ovh col6',
        col_id: '',
        col_value: 'Sl nhập'
    },
    {
        colspan: 1,
        col_class: 'ovh col7',
        col_id: '',
        col_value: 'Sl xuất'
    },
     {
         colspan: 1,
         col_class: 'ovh col8',
         col_id: '',
         col_value: 'Sl tồn'
     },
    {
        colspan: 1,
        col_class: 'ovh col9',
        col_id: '',
        col_value: 'Thành tiền tồn'
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
    //debugger;

    var DataJson = "";
    fncGetData_Kiemkekho(linkContent + 'ExportWarehousing/SelectRows_DShangtonkho', DataJson, isHeader);
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