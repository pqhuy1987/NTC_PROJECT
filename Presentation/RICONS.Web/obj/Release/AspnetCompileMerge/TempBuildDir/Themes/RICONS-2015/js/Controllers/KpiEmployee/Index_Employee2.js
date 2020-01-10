/// <reference path="Index_Employee.js" />

var configChosenDangkyvanphongpham = {
    '.chosen-select-plan': { width: "290px", disable_search_threshold: 10 },
    '.chosen-select-chonthang': { width: "120px", disable_search_threshold: 10 },
    '.chosen-select-loaikehoach1': { width: "100%", disable_search_threshold: 10 },
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
        var ID = $(listChecked[0]).attr('codeid');
        if (listChecked.length == 2) {
            ID = $(listChecked[1]).attr('codeid');
            var nguoilapkpi_dagui = $(listChecked[0]).attr('nguoilapkpi_dagui');
            if (nguoilapkpi_dagui == "0")
            {
                return $(this).attr("href", $(this).attr('href') + '/?makpinhanvien=' + ID);
                return false;
            }
            else {
                alert('KPI đã gửi không được xoá');
                return false;
            }
                
        }
        else if (listChecked.length == 1) {
            var nguoilapkpi_dagui = $(listChecked[0]).attr('nguoilapkpi_dagui');
            if (nguoilapkpi_dagui == "0") {
                return $(this).attr("href", $(this).attr('href') + '/?makpinhanvien=' + ID);
                return false;
            }
            else {
                alert('KPI đã gửi không được xoá');
                return false;
            }
        }
        else {
            alert('Chọn 1 dòng dữ liệu cần xoá');
            return false;

        }

    });

    $('#txttrangthu').on('keypress', function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            GetData(true);
        }
    });
   
    $("#btnNext").click(function () {
        var trangthu = $('#txttrangthu').val();
        var currentRow = $('#girdInfo span.Pages').text();
        if (parseInt(trangthu) < parseInt(currentRow)) {
            document.getElementById('txttrangthu').value = parseInt(trangthu) + 1;
            var thang = $('#txtthang').val();
            GetData(true);
        }
        else if (parseInt(trangthu) == parseInt(currentRow) && parseInt(trangthu) == 0) {
            document.getElementById('txttrangthu').value = parseInt(trangthu) + 1;
            GetData(true);
        }
    });

    $("#btnPre").click(function () {
        var trangthu = $('#txttrangthu').val();
        if (parseInt(trangthu) > 1) {
            document.getElementById('txttrangthu').value = parseInt(trangthu) - 1;
            GetData(true);
        }
    });

    $("#filter01").chosen().change(function () {
        GetData(true);
    });

    $("#filter02").chosen().change(function () {
        GetData(true);
    });


    GetData(true);

});


function GetData(isHeader, stt ) {
    var currentRow = $("#txttrangthu").val();
    var maphongban = $("#filter01").val();

    var trinhtranghoso = $("#filter02").val();

    var STT = CheckNullOrEmpty(stt) == true ? "0" : stt;

    var DataJson = "{'curentPage':'" + currentRow + "'," +
                    "'maphongban':'" + maphongban + "'," +
                    "'loaibaocao':'" + trinhtranghoso + "'" +
                   "}";

    fncGetData_Timekeeping(linkContent + 'WeedMeeting2/SelectRows_WeedMeeting', DataJson, isHeader);
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