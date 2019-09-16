
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
        col_value: 'Số CT'
    },
    {
        colspan: 1,
        col_class: 'ovh col5',
        col_id: '',
        col_value: 'Số HĐ'
    },
     {
         colspan: 1,
         col_class: 'ovh col6',
         col_id: '',
         col_value: 'Ngày CT'
     },
     {
         colspan: 1,
         col_class: 'ovh col7',
         col_id: '',
         col_value: 'Nhà cung cấp'
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
        col_value: 'Nội dung nhập hàng'
    }]
}];


$(document).ready(function (e) {
    $('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
    //chosen combobox jquery
    //for (var selector in configChosenDangkyvanphongpham) {
    //    $(selector).chosen(configChosenDangkyvanphongpham[selector]);
    //}

    //$("#txttungayct").on('click', function () {
    //    GetData(false);
    //});

   
    var ngayBD = $('#txttungayct');
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
        if (check_over_date($(this).val(), ngayKT.val())) {
            ngayKT.datepicker("setDate", $(this).val());
        }
    });

    var ngayKT = $('#txtdenngayct');
    ngayKT.datepicker({
        dateFormat: "dd/mm/yy",
        defaultDate: new Date(),
        dayNamesShort: $.datepicker.regional.vi.dayNamesShort,
        dayNames: $.datepicker.regional.vi.dayNames,
        monthNamesShort: $.datepicker.regional.vi.monthNamesShort,
        monthNames: $.datepicker.regional.vi.monthNames,
        showAnim: "slideDown",
        firstDay: 1 // Start with Monday
    });
    ngayKT.datepicker("setDate", new Date());
    ngayKT.change(function () {
        if (!check_date(this))
            ngayKT.datepicker("setDate", new Date());
        if (check_over_date($(this).val(), ngayBD.val())) {
            ngayBD.datepicker("setDate", $(this).val());
        }
    });




    $("#btnHieuChinh").click(function () {
        var listChecked = $('.chkCheck:checked');
        var ID = $(listChecked[0]).attr('mank');
        if (listChecked.length == 2) {
            ID = $(listChecked[1]).attr('mank');
            return $(this).attr("href", $(this).attr('href') + '/?manhapkho=' + encodeURIComponent(ID));
        }
        else if (listChecked.length == 1) {
            return $(this).attr("href", $(this).attr('href') + '/?manhapkho=' + encodeURIComponent(ID));
        }
        else {
            var dialogSuccess = bootbox.dialog({
                message: "<i class='fa fa-check text-success'></i><span class='text-success'>Chọn dòng dữ liệu cần hiệu chỉnh.</span>",
                closeButton: false,
                onEscape: function () {
                }
            });
            setTimeout(function () {
                dialogSuccess.modal('hide');
            }, 1000);
            return false;
        }
            
    });

    $("#btnDeleted").click(function () {
        debugger;
        var listChecked = $('.chkCheck:checked');
        var ID = $(listChecked[0]).attr('mank');
        if (listChecked.length == 2) {
            ID = $(listChecked[1]).attr('mank');
            var dialogSuccess = bootbox.dialog({
                message: "<i class='fa fa-check text-success'></i><span class='text-success'>Xoá dữ liệu thành công.</span>",
                closeButton: false,
                onEscape: function () {
                }
            });
            setTimeout(function () {
                dialogSuccess.modal('hide');
            }, 2000);

            return $(this).attr("href", $(this).attr('href') + '/?manhapkho=' + encodeURIComponent(ID));
        }
        else if (listChecked.length == 1) {
            var dialogSuccess = bootbox.dialog({
                message: "<i class='fa fa-check text-success'></i><span class='text-success'>Xoá dữ liệu thành công.</span>",
                closeButton: false,
                onEscape: function () {
                }
            });
            setTimeout(function () {
                dialogSuccess.modal('hide');
            }, 2000);
            return $(this).attr("href", $(this).attr('href') + '/?manhapkho=' + encodeURIComponent(ID));
        }
        else {
            var dialogSuccess = bootbox.dialog({
                message: "<i class='fa fa-check text-success'></i><span class='text-success'>Chọn dòng dữ liệu cần xóa.</span>",
                closeButton: false,
                onEscape: function () {
                }
            });
            setTimeout(function () {
                dialogSuccess.modal('hide');
            }, 2000);
            return false;
        }
    });

    $(".divExentd").click(function () {
        var aa = $('#girdInfo span').text();
        var currentRow = $('#girdInfo span.currentRow').text();
        GetData(false, "", "", currentRow);

    });

    GetData(true);

});

function GetData(isHeader, mpb, stt, currentRow) {
    //var makehoach = CheckNullOrEmpty(mkh) == true ? "0" : maphongban;
    //var STT = CheckNullOrEmpty(stt) == true ? "0" : stt;

    var DataJson = "{'tungay':'" + $('#txttungayct').val() + "'," +
                      "'denngay':'" + $('#txtdenngayct').val() + "'" +
                    "}";
    debugger;
    var DataJson = "";
    fncGetData_Kiemkekho(linkContent + 'Warehousing/SelectRows_DSNhapkho', DataJson, isHeader); //fncGetData
    //fncGetData(linkContent + 'Warehousing/SelectRows_DSNhapkho', DataJson, isHeader);
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