var table;

var tparams = {
    hasCount: true
}
$(document).ready(function () {
    var table = $("#webabyTable").webabyTable({
        style: 'table-hover table-bordered',
        dataUrl: '/Collaborate/CollaborateList',
        model: "Collaborate", // ten table,
        editController: '/Collaborate',
        modalTitle: "lịch đi công tác",
        checkAll: false,
        searchElement: "#product_search",
        modal: {
            type: 1,
            width: '700px'
        },
        paging: {
            options: [20, 30, 50]
        },
        loadModalCallback: function () {
           
            initEditForm(table);

        },
        loadDataCallback: function () {
            $('.btn-edit').unbind().click(function() {
                var id = $(this).closest('tr').attr('dataid');
                table.createOrUpdateObject(id);
            });
            $('.btn-delete').unbind().click(function () {
                var id = $(this).closest('tr').attr('dataid');
                if (confirm('Bạn chắc chắn ?')) {
                    $.ajax({
                        url: "Collaborate/delete",
                        type: "POST",
                        data: {
                            id: id
                        },
                        success: function () {
                            table.loadData();
                        }
                    });
                }
            });
        },
        selectRowCallback: function (tr) {

        },
        params: {
            search: tparams
        },
        columns: [
            {
                type: "text",
                title: "Tiêu đề",
                attribute: 'TieuDe'
            },
            {
                type: "text",
                title: "Nhân sự",
                style: 'width: 250px',
                attribute: 'Name',
                render: function (row) {
                    if (row.ObjTaiKhoan != null)
                        return row.ObjTaiKhoan.hoten;
                    return t;
                }
            },
            {
                type: "date",
                title: "Từ ngày",
                attribute: 'TuNgay',
                style: 'width: 120px;'
            },
            {
                type: "date",
                title: "Đến ngày",
                attribute: 'TuNgay',
                style: 'width: 120px;'
            },
            {
                type: "text",
                title: 'Nơi công tác',
                attribute: 'InStock',
                render: function (row) {
                    var str = [];
                    if (row.Orgs != null) {
                        $(row.Orgs).each(function() {
                            str.push(this.tenphongban);
                        });
                    }
                    return str.join(', ');
                }
            },
            {
                type: "text",
                title: "Mô tả",
                attribute: 'MoTa'
            },
            {
                type: "text",
                title: "Người duyệt",
                attribute: 'EmailNguoiDuyet'
            },
            {
                type: "text",
                title: "Trạng thái",
                attribute: 'Status',
                render: function(row) {
                    switch (row.Status) {
                    case 0:
                            return 'Lưu tạm';
                    case 1:
                            return 'Đã gửi email';
                    case 2:
                            return 'Đã duyệt';
                    case 3:
                        return 'Không đồng ý';
                    default:
                    }
                }
            },
            {
                type: "text",
                title: "Người duyệt",
                attribute: 'EmailNguoiDuyet',
                render: function() {
                    return '<a href="#"class="btn-edit"><i class="fa fa-edit"></i></a>' +
                        '<a href="#" class="btn-delete" style="color: #fb0525; margin-left: 10px"><i class="fa fa-times"></i></a>';
                }
            }
        ]
    });

    $('#projectCareModal').on('hide.bs.modal', function (e) {
        table.loadData();
    });
    
    $('#add_btn').click(function () {
        table.createOrUpdateObject(null);
    });

    $('')
    
});