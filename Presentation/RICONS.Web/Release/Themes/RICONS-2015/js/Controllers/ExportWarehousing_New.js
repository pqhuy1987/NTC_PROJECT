
var configChosenDanhmucvanphongpham = {
    '.chosen-select-plan': { width: "100%", disable_search_threshold: 10 },
    '.chosen-select-chucvu': { width: "100%", disable_search_threshold: 10 },
    '.chosen-select-phongban': { width: "100%", disable_search_threshold: 10 }
}

var configChosenlocdangkyvpp = {
    '.chosen-select-dangky': { width: "100%", disable_search_threshold: 10 }
}

var dataHeader_Milestones =
[{
    col_class: 'header-box',
    col_id: '',
    col_value: [
        //{
    //    colspan: 1,
    //    col_class: 'ovh col1',
    //    col_id: '',
    //    style: '',
    //    col_value: '<input type="checkbox" onclick="SelectAll(this);" class="chkCheck" />'
    //},
    {
        colspan: 1,
        col_class: 'ovh col2 stt',
        col_id: '',
        col_value: 'STT'
    },
    //{
    //    colspan: 1,
    //    col_class: 'ovh col3',
    //    col_id: '',
    //    style: '',
    //    col_value: 'Mã VPP'
    //},
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
          col_value: 'SL tồn'
      },
       {
           colspan: 1,
           col_class: 'ovh col10',
           col_id: '',
           style: '',
           col_value: 'SL Xuất thật'
       },
        {
            colspan: 1,
            col_class: 'ovh col11',
            col_id: '',
            style: '',
            col_value: 'SL thiếu'
        },

    {
            colspan: 1,
            col_class: 'ovh col12',
            col_id: '',
            style: '',
            col_value: ''
    }]
}];


$(document).ready(function (e) {
    $('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
    $('#ScrollContent .boxTableSelected').slimscroll({ height: 100 });

    for (var selector in configChosenDanhmucvanphongpham) {
        $(selector).chosen(configChosenDanhmucvanphongpham[selector]);
    }

    for (var selector in configChosenlocdangkyvpp) {
        $(selector).chosen(configChosenlocdangkyvpp[selector]);
    }

    var ngayBD = $('#txtngaynhapchungtu');
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
    ngayBD.datepicker("setDate", new Date());
    ngayBD.change(function () {
        if (!check_date(this))
            ngayBD.datepicker("setDate", new Date());
        //if (check_over_date($(this).val(), ngayKT.val())) {
        //    ngayKT.datepicker("setDate", $(this).val());
        //}
    });

    var ngayXK = $('#txtngayxuatkho');
    ngayXK.datepicker({
        dateFormat: "dd/mm/yy",
        defaultDate: new Date(),
        dayNamesShort: $.datepicker.regional.vi.dayNamesShort,
        dayNames: $.datepicker.regional.vi.dayNames,
        monthNamesShort: $.datepicker.regional.vi.monthNamesShort,
        monthNames: $.datepicker.regional.vi.monthNames,
        showAnim: "slideDown",
        firstDay: 1 // Start with Monday
    });
    ngayXK.datepicker("setDate", new Date());
    ngayXK.change(function () {
        if (!check_date(this))
            ngayXK.datepicker("setDate", new Date());
        //if (check_over_date($(this).val(), ngayKT.val())) {
        //    ngayKT.datepicker("setDate", $(this).val());
        //}
    });

    $("#filter03").chosen().change(function () {
        //debugger;
        var idmadangky = $('#filter03').val();
        GetData(true, idmadangky);
        //if (idmadangky=0)
        //    GetData(true, idmadangky);
        //else GetData(false, idmadangky);

       
    });

   // GetData(true);

});

function GetData(isHeader, idmadangky, stt) {
    //var DataJson = "";

    var DataJson = "{'idmadangky':'" + $('#filter03').val() + "'" +"}";

    fncGetData_dangkychitietdachon(linkContent + 'ExportWarehousing/SelectRows_Dangkyvanphongpham_chitiet', DataJson, isHeader);
    //fncGetData_Kiemkekho(linkContent + 'ExportWarehousing/SelectRows_Dangkyvanphongpham_chitiet', DataJson, isHeader);
    //fncGetData(linkContent + 'Warehousing/SelectRows_DM_vanphongpham', DataJson, isHeader);
    
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
