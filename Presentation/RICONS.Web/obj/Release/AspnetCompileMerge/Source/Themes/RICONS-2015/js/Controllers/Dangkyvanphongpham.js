
var configChosenDangkyvanphongpham = {
    '.chosen-select-plan': { width: "290px", disable_search_threshold: 10 },
    '.chosen-select-chonthang': { width: "120px", disable_search_threshold: 10 },
    '.chosen-select-loaikehoach': { width: "77%", disable_search_threshold: 10 }
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
    $('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
    //chosen combobox jquery
    for (var selector in configChosenDangkyvanphongpham) {
        $(selector).chosen(configChosenDangkyvanphongpham[selector]);
    }

    $("#filter01").chosen().change(function () {
        GetData(false);
    });

    $("#btnex").on('click', function () {
        GetData(false);
    });


    $("#btnHieuChinh").click(function () {
        var listChecked = $('.chkCheck:checked');
        var ID = $(listChecked[0]).attr('mpb');
        if (listChecked.length == 2) {
            ID = $(listChecked[1]).attr('mpb');
            return $(this).attr("href", $(this).attr('href') + '/?madangky=' + encodeURIComponent(ID));
        }
        else if (listChecked.length == 1) {
            return $(this).attr("href", $(this).attr('href') + '/?madangky=' + encodeURIComponent(ID));
        }
        else {
            alert('Chọn dòng dữ liệu cần hiệu chỉnh');
            return false;
            
        }
            
    });

    $("#btnDeleted").click(function () {
        var listChecked = $('.chkCheck:checked');
        debugger;
        var ID = $(listChecked[0]).attr('mpb');
        if (listChecked.length == 2) {
            ID = $(listChecked[1]).attr('mpb');
            var daduyet = $(listChecked[1]).attr('daduyet');
            if (daduyet == "1")
            {
                alert('Văn phòng phẩm đã duyệt không được xoá');
                return false;
            }
            else
                return $(this).attr("href", $(this).attr('href') + '/?madangky=' + encodeURIComponent(ID));
        }
        else if (listChecked.length == 1) {
            var daduyet = $(listChecked[0]).attr('daduyet');
            if (daduyet == "1")
            {
                alert('Văn phòng phẩm đã duyệt không được xoá');
                return false;
            }
            else
            {
                alert('Xoá dữ liệu thành công');
                return $(this).attr("href", $(this).attr('href') + '/?madangky=' + encodeURIComponent(ID));
            }
                
        }
        else {
            alert('Chọn dòng dữ liệu cần xoá');
            return false;

        }

    });


   

    $(".divExentd").click(function () {
        var aa = $('#girdInfo span').text();
        var currentRow = $('#girdInfo span.currentRow').text();
        var xemthem="1";
        GetData(false, "", "", currentRow,xemthem);

    });


    //GetData(true);

});

function Thongbao(num) {
    var dialogSuccess = bootbox.dialog({
        message: "<i class='fa fa-check text-success'></i><span class='text-success'>" + num + "</span>",
        closeButton: false,
        onEscape: function () {
        }
    });
    setTimeout(function () {
        dialogSuccess.modal('hide');
    }, 2000);
}
function GetData(isHeader, mpb, stt, currentRow, xemthem) {
    //var makehoach = CheckNullOrEmpty(mkh) == true ? "0" : maphongban;
    //var STT = CheckNullOrEmpty(stt) == true ? "0" : stt;

    var DataJson = "{'maphongban':'" + $('#filter01').val() + "'," +
                      "'xemthem':'" + xemthem + "'," +
                      "'curentPage':'" + currentRow + "'" +
                    "}";
   // fncGetData_Kiemkekho(linkContent + 'Home/SelectRows_DSDangkyvpp', DataJson, isHeader); //fncGetData
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