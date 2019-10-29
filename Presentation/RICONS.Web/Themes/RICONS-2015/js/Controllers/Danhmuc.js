﻿
//var configChosenDanhmucphongban = {
//    '.chosen-select-plan': { width: "290px", disable_search_threshold: 10 },
//    '.chosen-select-loaikehoach': { width: "100%", disable_search_threshold: 10 }
//}

//var dataHeader_Milestones =
//[{
//    col_class: 'header-box',
//    col_id: '',
//    col_value: [{
//        colspan: 1,
//        col_class: 'ovh col1',
//        col_id: '',
//        col_value: '<input type="checkbox" onclick="SelectAll(this);" class="chkCheck" />'
//    },
//    {
//        colspan: 1,
//        col_class: 'ovh col2 stt',
//        col_id: '',
//        col_value: 'STT'
//    },
//    {
//        colspan: 1,
//        col_class: 'ovh col3',
//        col_id: '',
//        col_value: 'Mã PB'
//    },
//    {
//        colspan: 1,
//        col_class: 'ovh col4',
//        col_id: '',
//        col_value: 'Tên phòng ban'
//    },
//    {
//        colspan: 1,
//        col_class: 'ovh col5',
//        col_id: '',
//        col_value: 'Họ và tên'
//    },

//    {
//        colspan: 1,
//        col_class: 'ovh col6',
//        col_id: '',
//        col_value: 'Email TBP/CHT'
//    },
//        {
//            colspan: 1,
//            col_class: 'ovh col8',
//            col_id: '',
//            col_value: 'Email thư ký'
//        },

//    {
//        colspan: 1,
//        col_class: 'ovh col7',
//        col_id: '',
//        col_value: 'Người quản lý 1'
//    },

//    {
//        colspan: 1,
//        col_class: 'ovh col9',
//        col_id: '',
//        col_value: 'Email QL 1'
//        },

//        {
//            colspan: 1,
//            col_class: 'ovh col11',
//            col_id: '',
//            col_value: 'Người quản lý 2'
//        },

//        {
//            colspan: 1,
//            col_class: 'ovh col12',
//            col_id: '',
//            col_value: 'Email QL 2'
//        },

//        {
//            colspan: 1,
//            col_class: 'ovh col13',
//            col_id: '',
//            col_value: 'Người quản lý 3'
//        },

//        {
//            colspan: 1,
//            col_class: 'ovh col14',
//            col_id: '',
//            col_value: 'Email QL 3'
//        },



//    {
//    colspan: 1,
//    col_class: 'ovh col10',
//    col_id: '',
//    col_value: ''
//    }]
//}];


//$(document).ready(function (e) {
//    $('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
//    //chosen combobox jquery
//    for (var selector in configChosenDanhmucphongban) {
//        $(selector).chosen(configChosenDanhmucphongban[selector]);
//    }

//    $("#filter01").chosen().change(function () {
//        //GetData(false);
//    });

//    $("#btnHieuChinh").click(function () {
//        var listChecked = $('.chkCheck:checked');
//        console.log(listChecked);
//        var ID = $(listChecked[0]).attr('mpb');
//        console.log(ID);
//        //return false;
        
//        if (listChecked.length == 1) {
           

//            return $(this).attr("href", $(this).attr('href') + '/?maphongban='+encodeURIComponent( ID ) );
//        }
//        else
//            return false;
//    });

//    $("#btnDeleted").click(function () {
//        var listChecked = $('.chkCheck:checked');
//        console.log(listChecked);
//        var ID = $(listChecked[0]).attr('mpb');
//        console.log(ID);
//        //return false;

//        if (listChecked.length == 1) {


//            return $(this).attr("href", $(this).attr('href') + '/?maphongban=' + encodeURIComponent(ID));
//        }
//        else
//            return false;
//    });

//    $("#filter01").chosen().change(function () {
//        444
//    });
//    GetData(true);

  

//});

//function GetData(isHeader, mkh, stt) {
//    var makehoach = CheckNullOrEmpty(mkh) == true ? "0" : maphongban;
//    var STT = CheckNullOrEmpty(stt) == true ? "0" : stt;
//    var DataJson = "";
//    //fncGetData_Kiemkekho(linkContent + 'Danhmuc/SelectRows', DataJson, isHeader);
//    fncGetData(linkContent + 'Danhmuc/SelectRows', DataJson, isHeader);
//}

//function ShowSubLine(e) {
//    //tim nhung dong sub
//    var maKeHoach = $(e).attr('mpb');
//    var tableContent = $("#tableContent");
//    var rows = tableContent.find('tr[subparent="' + maKeHoach + '"]');
//    if(rows.length > 0)
//    {
//        rows.slideToggle();
//    }
//    else
//    {
//        GetData(false, maKeHoach, $(e).parent().parent().find(".stt").html().trim());
//    }
//}



var configChosenDanhmucphongban = {
    '.chosen-select-plan': { width: "290px", disable_search_threshold: 10 },
    '.chosen-select-loaikehoach': { width: "357px", disable_search_threshold: 10 }
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
        col_value: 'Mã PB'
    },
    {
        colspan: 1,
        col_class: 'ovh col4',
        col_id: '',
        col_value: 'Tên phòng ban'
    },
    {
        colspan: 1,
        col_class: 'ovh col5',
        col_id: '',
        col_value: 'Họ và tên'
    },

    {
        colspan: 1,
        col_class: 'ovh col6',
        col_id: '',
        col_value: 'Email TBP/CHT'
    },
        {
            colspan: 1,
            col_class: 'ovh col8',
            col_id: '',
            col_value: 'Email thư ký'
        },

    {
        colspan: 1,
        col_class: 'ovh col7',
        col_id: '',
        col_value: 'Người quản lý 1'
    },

    {
        colspan: 1,
        col_class: 'ovh col9',
        col_id: '',
        col_value: 'Email QL 1'
        },



    {
        colspan: 1,
        col_class: 'ovh col10',
        col_id: '',
        col_value: ''
    },
        {
            colspan: 1,
            col_class: 'ovh col11',
            col_id: '',
            col_value: 'Người quản lý 2'
        },

        {
            colspan: 1,
            col_class: 'ovh col12',
            col_id: '',
            col_value: 'Email QL 2'
        },

        {
            colspan: 1,
            col_class: 'ovh col13',
            col_id: '',
            col_value: 'Người quản lý 3'
        },

        {
            colspan: 1,
            col_class: 'ovh col14',
            col_id: '',
            col_value: 'Email QL 3'
        },
]
}];


//$(document).ready(function (e) {
//    $('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
//    //chosen combobox jquery
//    for (var selector in configChosenDanhmucphongban) {
//        $(selector).chosen(configChosenDanhmucphongban[selector]);
//    }

//    $("#filter01").chosen().change(function () {
//        //GetData(false);
//    });

//    GetData(true);

//});

//function GetData(isHeader, mpb, stt) {
//    //var makehoach = CheckNullOrEmpty(mkh) == true ? "0" : maphongban;
//    //var STT = CheckNullOrEmpty(stt) == true ? "0" : stt;
//    var DataJson = "";
//    //fncGetData_Kiemkekho(linkContent + 'Danhmuc/SelectRows_chucvu', DataJson, isHeader);
//    fncGetData(linkContent + 'Danhmuc/SelectRows_2', DataJson, isHeader);
//}


//function ShowSubLine(e) {
//    //tim nhung dong sub
//    var maKeHoach = "";//$(e).attr('mpb');
//    var tableContent = $("#tableContent");
//    var rows = tableContent.find('tr[subparent="' + maKeHoach + '"]');
//    if (rows.length > 0) {
//        rows.slideToggle();
//    }
//    else {
//        GetData(false, maKeHoach, $(e).parent().parent().find(".stt").html().trim());
//    }
//}