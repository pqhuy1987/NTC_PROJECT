
var configChosenDangkyvanphongpham = {
    '.chosen-select-plan': { width: "290px", disable_search_threshold: 10 },
    '.chosen-select-chonthang': { width: "120px", disable_search_threshold: 10 },
    '.chosen-select-loaikehoach': { width: "77%", disable_search_threshold: 10 }
}

$(document).ready(function (e) {
    $('#ScrollContent .boxTable2').slimscroll({ height: getWindowHeight() - $('.RICONS-body header.header').height() - $(".HeaderFixed").height() });
    //chosen combobox jquery
    for (var selector in configChosenDangkyvanphongpham) {
        $(selector).chosen(configChosenDangkyvanphongpham[selector]);
    }

    $("#filter01").chosen().change(function () {
        GetData(false);
    });

    $('#btnUpdateemployeeworker').on('click', function (e) {
        e.preventDefault();
        debugger;
        var listChecked = $('.chkCheck:checked');
        var data = [];
        var aa = $(this).closest('tr');

        var hovaten1 = $(aa).find('td.col2 input').val();
        //var hovaten1 = $(aa)[0].find('td.col2 input').val();

        var DataJson = "{'data2':[";

        if (listChecked.length > 0) {
            $(listChecked).each(function (index, selectedItem) {

                var row = $(selectedItem).closest('.rows-box');
                var nhansuden_ct_pb_ngayden = $(row).find('td.col9 input').val();

                DataJson += "{";
                DataJson += "'madanhsach':'" + $(selectedItem).attr('codeid') + "',";
                DataJson += "'nhansuden_ct_pb_ngayden':'" + nhansuden_ct_pb_ngayden + "',";
                DataJson += "}";
                if (index != listChecked.length - 1)
                    DataJson += ",";
            });
            DataJson += "]}";
            $.ajax({
                type: 'POST',
                url: '/AddStaff/Save_employee_worker',
                datatype: "JSON",
                //data: { ids: DataJson }, DataJson1.toString()
                data: "DataJson=" + DataJson,
                cache: false,
                success: function (response) {
                    GetData(true);
                    alert('Cập nhật hồ sơ đến Công trường/ Phòng ban thành công!');
                },
                error: function (error) {
                    alert("Lỗi chuyển hồ sơ!");
                }
            })
        }
        else if (listChecked.length > 0) {
            alert('Chưa có dòng nào được chọn để chuyển hồ sơ!');
        }
    })



    $('#txttrangthu').on('keypress', function (event) {
        //debugger;
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            GetData(true);
        }
    });
   
    $("#btnNext").click(function () {
        debugger;
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
        debugger;
        var trangthu = $('#txttrangthu').val();
        if (parseInt(trangthu) > 1) {
            document.getElementById('txttrangthu').value = parseInt(trangthu) - 1;
            GetData(true);
        }
    });

    GetData(true);

});


function GetData(isHeader, stt ) {
    debugger;
    var currentRow = $("#txttrangthu").val();
    var noilamviec_moi = "";//$('#filter01').val();


    var STT = CheckNullOrEmpty(stt) == true ? "0" : stt;
    var DataJson = "{'noilamviec_moi':'" + noilamviec_moi + "'," +
                     "'curentPage':'" + currentRow + "'" +
                   "}";

    fncGetData_loadkhong_header(linkContent + 'AddStaff/SelectRows_Danhsachdenlamviec', DataJson, isHeader);
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