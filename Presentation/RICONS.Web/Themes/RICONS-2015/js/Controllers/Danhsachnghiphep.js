
var configChosenDanhsachnghiphep = {
    '.chosen-select-plan': { width: "290px", disable_search_threshold: 10 },
    '.chosen-select-loaikehoach': { width: "210px", disable_search_threshold: 10 }
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
        col_value: 'Ngày sinh'
    },
    //{
    //    colspan: 1,
    //    col_class: 'ovh col6',
    //    col_id: '',
    //    col_value: 'Tên chức danh'
    //},
    {
        colspan: 1,
        col_class: 'ovh col7',
        col_id: '',
        col_value: 'Tên phòng ban'
    },
    {
        colspan: 1,
        col_class: 'ovh col8',
        col_id: '',
        col_value: 'Nghỉ từ'
    },
    {
        colspan: 1,
        col_class: 'ovh col9',
        col_id: '',
        col_value: 'Số ngày'
    },
    {
        colspan: 1,
        col_class: 'ovh col10',
        col_id: '',
        col_value: 'Nghỉ đến'
    },
    //{
    //    colspan: 1,
    //    col_class: 'ovh col11',
    //    col_id: '',
    //    col_value: 'Phép CL'
    //},
    {
        colspan: 1,
        col_class: 'ovh col12',
        col_id: '',
        col_value: 'Lý do nghỉ'
    },
    {
        colspan: 1,
        col_class: 'ovh col13',
        col_id: '',
        col_value: 'Loại phép'
    },
    {
        colspan: 1,
        col_class: 'ovh col14',
        col_id: '',
        col_value: 'Trưởng BP'
    },
    {
        colspan: 1,
        col_class: 'ovh col15',
        col_id: '',
        col_value: 'Ban GĐ'
    },
    {
        colspan: 1,
        col_class: 'ovh col16',
        col_id: '',
        col_value: 'XN LVL'
    },
    {
    colspan: 1,
    col_class: 'ovh col17',
    col_id: '',
    col_value: ''
    }]
}];


$(document).ready(function (e) {
    $('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
    //chosen combobox jquery
    for (var selector in configChosenDanhsachnghiphep) {
        $(selector).chosen(configChosenDanhsachnghiphep[selector]);
    }

    $("#filter01").chosen().change(function () {
        GetData(false);
    });

    
    $(".divExentd").click(function () {
        var aa= $('#girdInfo span').text();
        var currentRow = $('#girdInfo span.currentRow').text();
        GetData(false,"","",currentRow);

    });


    $("#btnHieuChinh").click(function () {
        var listChecked = $('.chkCheck:checked');
        var duyet = $(listChecked[0]).attr('duyet');
        var ID = $(listChecked[0]).attr('mdk');
        if (listChecked.length ==3)
        {
            duyet = $(listChecked[2]).attr('duyet');
            ID = $(listChecked[2]).attr('mdk');
            if (duyet != "1") {
                return $(this).attr("href", $(this).attr('href') + '/?madangky=' + encodeURIComponent(ID));
            }
            else {
                alert('Đơn xin nghỉ phép đã duyệt hoặc chọn không đúng 1 dòng');
                return false;
            }
        }
        else if (duyet != "1" && listChecked.length ==1) {
            return $(this).attr("href", $(this).attr('href') + '/?madangky=' + encodeURIComponent(ID));
        }
        else {
            alert('Đơn xin nghỉ phép đã duyệt hoặc chọn không đúng 1 dòng');
            return false;
        }
    });
    
    $("#btnDeleted").click(function () {
        var listChecked = $('.chkCheck:checked');
        var duyet = $(listChecked[0]).attr('duyet');
        var ID = $(listChecked[0]).attr('mdk');
        if (listChecked.length == 3) {
            duyet = $(listChecked[2]).attr('duyet');
            ID = $(listChecked[2]).attr('mdk');
            if (duyet != "1") {
                return $(this).attr("href", $(this).attr('href') + '/?manghiphep=' + encodeURIComponent(ID));
            }
            else {
                return false;
            }
        }
        else if (duyet != "1" && listChecked.length == 1) {
            return $(this).attr("href", $(this).attr('href') + '/?manghiphep=' + encodeURIComponent(ID));
        }
        else {
            
            return false;
        }
    });

    $("#filter01").chosen().change(function () {
        444
    });
    GetData(true);
});

function GetData(isHeader, mkh, stt, currentRow) {
    var makehoach = CheckNullOrEmpty(mkh) == true ? "0" : mkh;
    var STT = CheckNullOrEmpty(stt) == true ? "0" : stt;
    //if (isNaN(currentRow) == false) {currentRow = parseInt(currentRow)+1;}
    //else { currentRow = 1;}
    var DataJson = "{'maphongban':'" + $('#filter01').val() + "'," +
                     "'makehoach':'" + makehoach + "'," +
                      "'curentPage':'" + currentRow + "'" +
                    "}";
    fncGetData(linkContent + 'Absent/SelectRows_Danhsachnghiphep', DataJson, isHeader);
}

function ShowSubLine(e) {
    //tim nhung dong sub
    var mdk = $(e).attr('mdk');
    var tableContent = $("#tableContent");
    var rows = tableContent.find('tr[subparent="' + mdk + '"]');
    if(rows.length > 0)
    {
        rows.slideToggle();
    }
    else
    {
        GetData(false, mdk, $(e).parent().parent().find(".stt").html().trim());
    }
}