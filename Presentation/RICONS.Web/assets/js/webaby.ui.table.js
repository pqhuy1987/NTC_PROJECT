
(function ($) {
    $.fn.webabyTable = function (options) {
        var self = this;
        var selector = this.selector;
        var currentPage = 0;
        self.searchParams = {
            keyword: "",
            page: 1,
            limit: null
        };
        self.editParams = {
        };
        self.sourceData = [];
        self.count = 0;
        self.formId = '';
        // Establish our default settings
        var settings = $.extend({
            header: null,
            footer: null,
            tableSize: null,
            rootUrl: null,
            dataUrl: null,
            paging: null,
            params: null,
            autoLoad: null,
            selectRow: null,
            idAttribute: null,
            columns: [],
            editController: null,
            model: "",
            checkAll: null,
            searchElement: null,
            beforeSubmit: null,
            hasCkeditor: false,
            detailRow: null,
            autoSubmit: true,
            responsive: false,
            modal: null,
            loadDataCallback: null,
            loadModalCallback: null,
            selectRowCallback: null,
            submitFormCallback: null,
            sumInfo: null
        }, options);

        if (settings.paging != false) {
            if (settings.paging == null) {
                settings.paging = {};
            }
            self.searchParams.page = settings.paging.page != null ? settings.paging.page : 1;
            self.searchParams.limit = settings.paging.limit != null ? settings.paging.limit : 20;
        }

        if (settings.params != null) {
            if (typeof settings.params.search != 'undefined') {
                $.extend(self.searchParams, settings.params.search);
            }
            if (typeof settings.params.edit != 'undefined') {
                $.extend(self.editParams, settings.params.edit);
            }
        }
        self.showTableLoading = function () {
            var tb = $(selector).find('table');
            if ($(tb).find('.webaby-table-loading').length == 0) {
                $(tb).append('<div class="webaby-table-loading"><div><div class="icon-spinner10 spinner text-teal-600"></div></div></div>');
            }
            $(selector).find(".webaby-table-loading").show();
        }
        self.hideTableLoading = function () {
            $(selector).find(".webaby-table-loading").hide();
        }
        self.setPagination = function (total) {
            if (total == 0) {
                $(selector).find(".pagination").css('display', 'none');
            } else {
                var limit = self.searchParams.unlimited ? total : self.searchParams.limit;
                $(selector).find(".pagination").css('display', 'block');
                var pagination = $(selector).find(".pagination");
                $(pagination).html('');
                var startPageIndex;
                var numPage = parseInt(total / limit);
                var li;
                if (total % limit != 0) {
                    numPage++;
                }
                if (self.searchParams.page <= 4) {
                    startPageIndex = 1;
                } else if (self.searchParams.page >= numPage - 3) {
                    startPageIndex = numPage - 6;
                } else {
                    startPageIndex = self.searchParams.page - 3;
                }

                var length = startPageIndex + 7;
                if (numPage < 7) {
                    length = startPageIndex + numPage;
                }
                if (length > 1) {
                    if (length > 2) {
                        $(pagination).append('<li><a href="#" class="pre"><span>←</span></a></li>');
                    }
                    for (var i = startPageIndex; i < length; i++) {
                        li = '<li>';
                        if (i == self.searchParams.page) {
                            li = '<li class="active">';
                        }
                        li += '<a href="#" page=' + i + '>' + i + '</a></li>';
                        $(pagination).append(li);
                    }
                    if (length > 2) {
                        $(pagination).append('<li><a href="#" class="next"><span>→</span></a></li>');
                    }
                }
                $(pagination).find("a").unbind().click(function () {
                    self.searchParams.page = $(this).attr("page");
                    self.loadData();
                    return false;
                });
                $(pagination).find(".pre").unbind().click(function () {
                    if (self.searchParams.page > 1) {
                        self.searchParams.page--;
                        self.loadData();
                    }
                    return false;
                });
                $(pagination).find(".next").unbind().click(function () {
                    if (self.searchParams.page < numPage) {
                        self.searchParams.page++;
                        self.loadData();
                    }
                    return false;
                });
            }
        }
        self.setSubmitEvent = function (type) {
            var submitFlag = true;
            if (settings.beforeSubmit != null) {
                submitFlag = settings.beforeSubmit();
            }
            if (!submitFlag || !settings.autoSubmit) return false;

            self.submitForm(type);
        };
        self.submitForm = function () {
            //self.showOverlay();
            if (settings.hasCkeditor) {
                for (instance in CKEDITOR.instances)
                    CKEDITOR.instances[instance].updateElement();
                $.each($(".ckeditor"), function (k, i) {
                    $(i).val(CKEDITOR.instances[$(i).attr("id")].getData());
                });
            }
            if (typeof webaby !== "undefined") {
                webaby.showModalLoading();
            }
            var url = settings.editController != null ? settings.editController + '/' : '/admin/';
            url += settings.model + "Edit";
            $.ajax(
                {
                    url: url,
                    type: "POST",
                    data: new FormData($("#" + settings.model + 'Form')[0]),
                    processData: false,
                    contentType: false,
                    success: function (errorCode) {
                        if (errorCode > 0 && settings.submitFormCallback != null) {
                            settings.submitFormCallback(errorCode);
                        } else {
                            if (typeof webaby !== "undefined") {
                                webaby.hideModalLoading();
                            }
                            $(".alf-table-overlay").hide();
                            self.loadData();
                            $("#formEditModal").modal("hide");
                            self.hideModal();
                        }
                    },
                    error: function () {
                        if (typeof webaby !== "undefined") {
                            webaby.hideModalLoading();
                        }
                        $(".alf-table-overlay span").text("Sorry! Cannot connect to server.");
                        $(".alf-table-overlay button").show();
                        //if fails
                    }
                });
            return true;
        },
            self.hideModal = function () {
                var type = parseInt($('#formEditModal').attr('data-type'));
                if (type == 2) {
                    $('#formEditModal').fadeOut('fast', function () {
                        $('#formEditModal').remove();
                    });
                } else {
                    $("#formEditModal").modal("hide");
                }
                $('body').removeClass('overflow-hidden');
            },
            self.showModal = function () {
                var type = parseInt($('#formEditModal').attr('data-type'));
                if (type == 2) {
                    $("#formEditModal").fadeIn('fast');
                } else {
                    $("#formEditModal").modal("show");
                }
                $('body').addClass('overflow-hidden');
            }
        self.eventModal = function () {
            $(".form-submit").unbind().click(function () {
                var btn = $(this);
                if (btn.hasClass('button-loading')) {
                    $('.form-cancel').attr('disabled', 'disabled').css('disabled');
                    btn.button('loading');
                }
                self.setSubmitEvent("edit");
            });
            $('.form-cancel').unbind().click(function () {
                self.hideModal();
            });
            $("#formEditModal form input").keypress(function (e) {
                var key = e.which;
                if (key == 13)  // the enter key code
                {
                    self.setSubmitEvent("edit");
                    return false;
                }
            });

            if (settings.hasCkeditor) {
                $.each($(".ckeditor"), function () {
                    var _eid = $(this).attr("id");
                    var editor = CKEDITOR.replace(
                        _eid, {
                            toolbarGroups: [
                                { name: 'document', groups: ['mode', 'document'] }, // Displays document group with its two subgroups.			// Group's name will be used to create voice label.
                                //'/',																// Line break - next group will be placed in new line.
                                { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
                                { name: 'insert', items: ['image', 'table', 'horizontalRule', 'specialChar'] },
                                { name: 'links' },
                                { name: 'styles' },
                                { name: 'colors' }
                            ],
                            height: 400
                        });
                });
            }
        }
        self.initModal = function (modal, content) {
            var html = '';
            if (modal.type == 2) {
                html = '<div class="wtable-epanel" id="formEditModal" data-type="' + modal.type + '">';
                if (modal.width != "") {
                    html += '<div class="wtable-epanel-content" style="width:' + modal.width + '">';
                } else {
                    html += '<div class="wtable-epanel-content">';
                }
                html += '<div class="panel panel-flat wtable-panel">';
                html += content;
                html += '</div></div></div>';
                $("body").append(html);
            } else {
                html = '<div class="modal" id="formEditModal" data-type="' + modal.type + '" data-backdrop="static"  role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">';
                if (modal.width != "") {
                    html += '    <div class="modal-dialog" style="width:' + modal.width + '">';
                } else {
                    html += '    <div class="modal-dialog" >';
                }
                html += '<div class="modal-content">';
                html += '      <div class="modal-header">';
                html += '          <button class="close" aria-hidden="true" data-dismiss="modal" type="button">×</button>';
                html += '              <h4 class="modal-title">' + modal.title + '</h4>';
                html += '          </div><div class="modal-body card">';
                html += content;
                html += '</div></div></div></div>';
                $("body").append(html);
                $('#formEditModal').on('hidden.bs.modal', function (e) {
                    $('body').removeClass('overflow-hidden');
                    $('#formEditModal').remove();
                });
            }
        }
        self.createOrUpdateObject = function (id) {
            self.showTableLoading();
            var url = settings.editController != null ? settings.editController + '/' : '/admin/';
            url += settings.model + "Edit";
            $.extend(self.editParams, {
                id: id
            });
            $.ajax({
                url: url,
                type: "GET",
                data: self.editParams,
                dataType: "html",
                success: function (result) {
                    self.hideTableLoading();
                    self.initModal({
                        title: (id != null ? 'Cập nhật ' : 'Thêm mới ') + settings.modalTitle,
                        width: settings.modal.width,
                        type: settings.modal.type != null ? settings.modal.type : 1
                    }, result);
                    self.eventModal();
                    self.showModal();

                    if (settings.loadModalCallback != null) {
                        settings.loadModalCallback();
                    }
                }
            });
        };
        self.getSelectedIds = function () {
            var ids = [];
            $.each($(selector).find("tbody tr"), function (k, tr) {
                if ($(tr).hasClass("active")) {
                    ids.push($(tr).attr("dataid"));
                }
            });
            return ids;
        }
        self.bulkDelete = function () {
            var ids = self.getSelectedIds();
            if (ids.length > 0) {
                self.deleteObjects("bulk", ids);
            } else {
                alert("Vui lòng chọn đối tượng cần xóa.");
            }
        }
        self.deleteObjects = function (type, ids) {
            app.confirm("warning", null, null, function () {
                if (typeof webaby !== "undefined") {
                    webaby.showModalLoading();
                }
                if (type == "bulk") {
                    $.ajax({
                        url: "/admin/BulkDelete",
                        type: "POST",
                        data: {
                            ids: ids,
                            model: settings.model,
                            isDelete: $('#app_comfirm_modal #delete').prop('checked')
                        },
                        success: function () {
                            if (typeof webaby !== "undefined") {
                                webaby.hideModalLoading();
                            }
                            $("#deleteModal").modal("hide");
                            self.loadData();
                        }
                    });
                } else {
                    var url = settings.editController != null ? settings.editController : '/admin';
                    $.ajax({
                        url: url + "/delete",
                        type: "POST",
                        data: {
                            model: settings.model,
                            id: ids[0],
                            isDelete: $('#app_comfirm_modal #delete').prop('checked')
                        },
                        success: function () {
                            if (typeof webaby !== "undefined") {
                                webaby.hideModalLoading();
                            }
                            $("#deleteModal").modal("hide");
                            self.loadData();
                        }
                    });
                }
                $(selector).find("#checkAll").prop("checked", false);
            });
        }
        self.setEvents = function () {

            $.each(settings.columns, function (k, col) {
                if (col.type == "option") {
                    $.each(col.render, function (h, render) {
                        if (render.click != null) {
                            $(selector).find("." + render.class).unbind().click(render.click);
                        } else {
                            switch (render.class) {
                                case "edit":
                                    {
                                        $(selector).find(".edit").unbind().click(function () {
                                            self.createOrUpdateObject($(this).parents('tr').attr("dataid"));
                                            return false;
                                        });
                                    }
                                    ;
                                    break;
                                case "delete":
                                    {
                                        $(selector).find(".delete").unbind().click(function () {
                                            self.deleteObjects(null, [$(this).parents('tr').attr("dataid")]);
                                        });
                                    }
                                    ;
                                    break;
                                case "advance-delete":
                                    {
                                        $(selector).find(".advance-delete").unbind().click(function () {
                                            var id = $(this).parents('tr').attr("dataid");
                                            app.confirm("warning", null, null, function () {
                                                var url = settings.editController != null ? settings.editController + '/' : '/admin/';
                                                url += "advanceDelete";
                                                postData(url, {
                                                    id: id,
                                                    cause: '',
                                                    model: settings.model
                                                }, function (result) {
                                                    if (result) {
                                                        new PNotify({
                                                            text: 'Xóa thành công',
                                                            addclass: 'alert bg-success alert-styled-left'
                                                        });
                                                    } else {
                                                        new PNotify({
                                                            title: 'Không thể xóa',
                                                            text: 'Có lỗi xảy ra hoặc bạn không được quyền xóa.',
                                                            addclass: 'alert bg-danger alert-styled-left'
                                                        });
                                                    }
                                                    self.loadData();
                                                });
                                            });
                                        });
                                    };
                                    break;
                                case "enable":
                                    {
                                        $(selector).find(".enable").unbind().click(function () {
                                            var id = $(this).parents('tr').attr("dataid");
                                            var url = settings.editController != null ? settings.editController + '/' : '/admin/';
                                            url += "EnableOrDisable";
                                            $.ajax({
                                                url: url,
                                                type: "POST",
                                                data: {
                                                    model: settings.model,
                                                    id: id,
                                                    isEnable: true
                                                },
                                                success: function (result) {
                                                    new PNotify({
                                                        text: 'Thao tác thành công',
                                                        addclass: 'alert bg-success alert-styled-left'
                                                    });
                                                    self.loadData();
                                                }
                                            });
                                        });
                                    }
                                    ;
                                    break;
                                case "disable":
                                    {
                                        $(selector).find(".disable").unbind().click(function () {
                                            var id = $(this).parents('tr').attr("dataid");
                                            app.confirm('warning', null, null, function () {
                                                var url = settings.editController != null ? settings.editController + '/' : '/admin/';
                                                url += "EnableOrDisable";
                                                $.ajax({
                                                    url: url,
                                                    type: "POST",
                                                    data: {
                                                        model: settings.model,
                                                        id: id,
                                                        isEnable: false
                                                    },
                                                    success: function () {
                                                        new PNotify({
                                                            text: 'Thao tác thành công',
                                                            addclass: 'alert bg-success alert-styled-left'
                                                        });
                                                        self.loadData();
                                                    }
                                                });
                                            });
                                        });
                                    }
                                    ;
                                    break;
                                case "renew":
                                    {
                                        $(selector).find(".renew").unbind().click(function () {
                                            var id = $(this).parents('tr').attr("dataid");
                                            var url = settings.editController != null ? settings.editController + '/' : '/admin/';
                                            url += "renew";
                                            var data = {
                                                model: settings.model,
                                                id: id
                                            };
                                            $.extend(data, self.editParams);

                                            $.ajax({
                                                url: url,
                                                type: "POST",
                                                data: data,
                                                success: function () {
                                                    new PNotify({
                                                        text: 'Thao tác thành công',
                                                        addclass: 'alert bg-success alert-styled-left'
                                                    });
                                                    self.loadData();
                                                }
                                            });
                                        });
                                    }
                                    break;
                                case "download":
                                    {
                                        $(selector).find(".download").unbind().click(function () {
                                            var id = $(this).attr("dataid");
                                            var url = "/admin/download?t=" + settings.model + "&i=" + id;
                                            window.open(url, '_blank');
                                        });
                                    }
                                default:
                            }
                        }
                    });
                }
            });
            if (settings.selectRow != false) {

                $(selector).find("tbody tr td").unbind().click(function (e) {

                    if (e.target != this) return;
                    console.log(123);
                    self.selectRow($(this).closest('tr'));
                });
            }
            $(selector).find('.show-detail').click(function () {
                self.selectRow($(this).closest('tr'));
            });
            $(selector).find("#checkAll").click(function () {
                if ($(this).prop("checked")) {
                    $(selector).find("tbody tr").addClass("active");
                    $(selector).find(".first-col input").prop("checked", true);
                } else {
                    $(selector).find(".first-col input").prop("checked", false);
                    $(selector).find("tbody tr").removeClass("active");
                }
            });
            $(selector).find(".orderable").unbind().click(function () {
                var ordertype = $(this).attr("currentOrder");
                if (ordertype == "desc") {
                    $(this).attr("currentOrder", "asc");
                    ordertype = "asc";
                    $(this).find("i").removeClass("fa-caret-down").addClass("fa-caret-up");
                } else {
                    $(this).attr("currentOrder", "desc");
                    ordertype = "desc";
                    $(this).find("i").removeClass("fa-caret-up").addClass("fa-caret-down");
                }
                self.loadData({
                    OrderBy: $(this).attr("orderBy"),
                    OrderType: ordertype
                });
            });
            if (settings.searchElement != null) {
                var sele = settings.searchElement;
                $(".searchColItems a").click(function () {
                    $(".curSearchCol").attr("searchcol", $(this).attr("val"));
                });
                var globalTimeout = null;
                $(sele).find('input').unbind().keyup(function () {
                    var keyword = $(this).val();
                    if (globalTimeout != null) {
                        clearTimeout(globalTimeout);
                    }
                    globalTimeout = setTimeout(function () {
                        $.extend(self.searchParams, { keyword: keyword });
                        if (self.searchParams.page != 1) {
                            self.searchParams.page = 1;
                        }
                        self.loadData();
                        clearTimeout(globalTimeout);
                    }, 300);
                });
            }

            $(selector).find('.limitation').unbind().change(function () {
                var v = $(this).val();
                if (v == 'all') {
                    self.searchParams.unlimited = true;

                } else {
                    self.searchParams.unlimited = null;
                    self.searchParams.limit = v;
                }
                self.loadData();
            });

            if (settings.detailRow != null) {
                $(selector).find("tbody tr td").mouseover(function (e) {
                    if (e.target != this) return;
                    $(this).css('cursor', 'pointer');
                }).mouseout(function (e) {
                    $(this).css('cursor', 'default');
                });
            }

        }
        self.reloadDetailRow = function () {
            var tr = $(selector).find('tr[class="active"]');
            self.loadDetailRow(tr);
        }
        self.loadDetailRow = function (tr) {
            var id = $(tr).attr('dataid');
            self.showTableLoading();
            var dr = $(tr).next();
            loadData(settings.detailRow.url, {
                id: id,
                dataType: 'html'
            }, null, function (result) {
                self.hideTableLoading();
                dr.find('td').html(result);
                var next = $(tr).next();
                settings.detailRow.event(next);
            });
        }
        self.selectRow = function (tr) {
            if (settings.checkAll == false) {
                if ($(tr).hasClass('active')) {
                    $(tr).removeClass('active');
                    $(tr).parent().find('tr[class="detail-row"]').css('display', 'none');
                } else {
                    $(tr).parent().find('tr[class="active"]').removeClass('active');
                    $(tr).parent().find('tr[class="detail-row"]').css('display', 'none');
                    if (!$(tr).hasClass('active')) {
                        $(tr).addClass('active');
                    }
                }
            } else {
                if ($(tr).prop("checked") == true) {
                    $(tr).find(".first-col input").prop("checked", false);
                    $(selector).find("#checkAll").prop("checked", false);
                } else {
                    $(tr).find(".first-col input").prop("checked", true);
                }
                if ($(tr).hasClass('active')) {
                    $(tr).removeClass('active');
                } else {
                    $(tr).addClass('active');
                }
            }

            if (settings.selectRowCallback != null) {
                settings.selectRowCallback(tr);
            }

            if (settings.detailRow != null) {
                var next = $(tr).next();
                if (!next.hasClass('detail-row')) {
                    $(tr).after('<tr class="detail-row" style="display: table-row"><td colspan="' +
                        (settings.columns.length + 1) + '"><i class="icon-spinner10 spinner" style="margin:5px"></i></td></tr>');

                    self.loadDetailRow(tr);
                }
                if ($(tr).hasClass('active')) {
                    $(next).css('display', 'table-row');
                    $('html, body').animate({
                        scrollTop: $(tr).offset().top - 10
                    }, 300);
                } else {
                    $(next).css('display', 'none');
                }
            }
        }
        self.getSelectedRow = function (handle) {
            if (handle != null) {
                var tr = $(handle).parent('tr');
                return {
                    id: $(tr).attr("dataid"),
                    code: $(tr).attr("datacode")
                }
            } else {
                var active;
                $.each($(selector).find('tbody tr'), function (k, i) {
                    if ($(i).hasClass('active')) {
                        active = i;
                        return false;
                    }
                });
                if (active != null) {
                    return {
                        id: $(active).attr("dataid"),
                        code: $(active).attr("datacode")
                    }
                }
            }
            return null;
        }
        self.getDataRow = function (dataId) {
            var idAttr = "Id";
            if (settings.idAttribute != null) {
                idAttr = settings.idAttribute;
            }
            for (var i = 0; i < self.sourceData.length; i++) {
                if (self.sourceData[i][idAttr] == dataId) {
                    return self.sourceData[i];
                }
            }
            ;
            return null;
        }

        self.getAllData = function () {
            return {
                many: self.sourceData,
                count: self.count
            };
        }
        self.search = function (conditions, callback) {
            self.searchParams.page = 1;
            $.extend(self.searchParams, conditions);
            self.loadData(null, callback);
        };
        self.formatDate = function (value) {
            if (!app.hasValue(value))
                return '';
            var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
            var month = (date.getMonth() + 1);
            if (month < 10) {
                month = '0' + month;
            }
            return date.getDate() + '/' + month + '/' + date.getFullYear();
        };
        self.formatDateTime = function (value) {
            if (!app.hasValue(value))
                return '';
            var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
            var month = (date.getMonth() + 1);
            if (month < 10) {
                month = '0' + month;
            }
            var hours = date.getHours();
            var minutes = date.getMinutes();

            if (minutes < 10)
                minutes = '0' + minutes;
            return date.getDate() + '/' + month + '/' + date.getFullYear() + " " + hours + ":" + minutes + " ";
        }
        self.formatPrice = function (str) {
            if (!hasValue(str) || str == 0) {
                return '0';
            }
            var oper = "";
            str = str + "";
            if (str[0] == '-' || str[0] == '+') {
                oper = str[0];
                str = str.substr(1);
            }
            var parts = (str + "").split("."),
                main = parts[0],
                len = main.length,
                output = "",
                i = len - 1;
            while (i >= 0) {
                output = main.charAt(i) + output;
                if ((len - i) % 3 === 0 && i > 0) {

                    output = "," + output;
                }
                --i;
            }
            // put decimal part back
            if (parts.length > 1) {
                output += "." + parts[1];
            }
            return oper + ' ' + output;
        };
        self.loadData = function (params, callback) {
            if (params != null) {
                $.extend(self.searchParams, params);
            }
            self.showTableLoading();
            $.ajax({
                url: settings.dataUrl != null ? settings.dataUrl : '/api/' + settings.model + "List",
                type: "GET",
                dataType: "JSON",
                contentType: 'application/json; charset=utf-8',
                data: self.searchParams,
                success: function (result) {
                    var tr;
                    var count = result.Count != null ? result.Count : 0;
                    var list = result.Many != null ? result.Many : result.length > 0 ? result : null;
                    $(selector).find("tbody").html("");
                    self.sourceData = list;
                    self.count = result.Count;
                    var tbody = '';
                    if (list != null && list.length > 0 && settings.columns.length > 0) {
                        var idAttr = "Id";
                        if (settings.idAttribute != null) {
                            idAttr = settings.idAttribute;
                        }
                        var sumarray = [];
                        $.each(list, function (k, item) {
                            tr = '<tr dataid="' + item[idAttr] + '"';
                            if (settings.rowStyle != null) {
                                tr += ' class="' + settings.rowStyle(item) + '" ';
                            }
                            if (typeof item['Code'] != 'undefined') {
                                tr += ' datacode="' + item['Code'] + '">';
                            } else {
                                tr += '>';
                            }

                            if (settings.checkAll == null || settings.checkAll) {
                                tr += '<td class="first-col"><div class="checkbox checkbox-info"><input type="checkbox" value="" dataId="' + item[idAttr] + '"/><label></label></div></td>';
                            }
                            if (settings.detailRow != null) {
                                tr += '<td><i class="icon-grid52 dragula-handle show-detail" title="Xem chi tiết"><i></td>';
                            }
                            for (var i = 0; i < settings.columns.length; i++) {
                                var col = settings.columns[i];
                                if (col.visible == null || col.visible()) {
                                    var model = settings.model;
                                    if (settings.columns[i].model != null) {
                                        model = settings.columns[i].model;
                                    }

                                    var td = '<td';
                                    if (col.class != null) {
                                        td += ' class="' + col.class + '"';
                                    }
                                    if (col.style != null) {
                                        td += ' style="' + col.style + '"';
                                    }
                                    switch (col.type) {
                                        case "text":
                                            {
                                                if (col.sumable) {
                                                    if (typeof sumarray[i] == "undefined") {
                                                        sumarray[i] = 0;
                                                    }
                                                }
                                                sumarray[i] += item[col.attribute];
                                                td += '>';
                                                if (col.render != null) {
                                                    td += col.render(item);
                                                }
                                                else if (col.attribute != null) {
                                                    var v = "";
                                                    if (item[col.attribute] != null)
                                                        v = item[col.attribute];
                                                    td += '<span ';
                                                    if (col.aline != null) {
                                                        td += ' class="aline"';
                                                    }
                                                    td += '>' + v + '</span>';
                                                }
                                            };
                                            break;
                                        case "checkbox":
                                            {

                                            };
                                            break;
                                        case "option":
                                            {
                                                td += ' style="width:50px;">';

                                                var option = '<ul class="icons-list"><li class="dropdown">';
                                                option += '<a class="dropdown-toggle" href="#" data-toggle="dropdown" aria-expanded="false"><i class="icon-menu9"></i></a>';
                                                option += '<ul class="dropdown-menu dropdown-menu-right" style="width: 200px">';

                                                $.each(col.render, function (h, opt) {
                                                    if (opt.condition == null || opt.condition(item)) {
                                                        option += '<li><a href="javascript:void(0)"';
                                                        if (opt.class != null) {
                                                            option += 'class="' + opt.class + '"';
                                                        }
                                                        option += '><i style="margin-right:5px" class="' + opt.icon + ' ' + opt.iconColor + '"></i>' + opt.text + '</a></li>';
                                                    }
                                                });

                                                td += option + '</ul></li></ul>';
                                            };
                                            break;
                                        case "date":
                                            {
                                                td += '>' + self.formatDate(item[col.attribute]);
                                            };
                                            break;
                                        case "datetime":
                                            {
                                                td += '>' + self.formatDateTime(item[col.attribute]);
                                            };
                                            break;
                                        case "html":
                                            {

                                                td += '>';
                                                if (col.render != null) {
                                                    td += col.render(item).replace(/\n/g, '<br/>');
                                                } else if (col.attribute != null) {
                                                    td += item[col.attribute] != null ? item[col.attribute] : "";
                                                }
                                            };
                                            break;
                                        case "price":
                                        case "number":
                                            {
                                                if (col.sumable) {
                                                    if (typeof sumarray[i] == "undefined") {
                                                        sumarray[i] = 0;
                                                    }
                                                }
                                                td += '>';
                                                var val = 0;
                                                if (col.render != null) {
                                                    val = col.render(item);
                                                } else {
                                                    val = item[col.attribute];
                                                }
                                                sumarray[i] += val;
                                                td += '<span> ' + self.formatPrice(val) + '</span>';
                                            };
                                            break;
                                        case "ai":
                                            {
                                                td += '>' + (k + 1);
                                            };
                                            break;
                                    }
                                    td += '</td>';
                                    tr += td;
                                }


                            }

                            tr += '</tr>';
                            tbody += tr;
                        });

                        // draw sum tr
                        if (settings.sumInfo != null) {
                            var tr = '<tr class="warning">';
                            if (settings.sumInfo.colspan != null) {
                                tr += '<td colspan="' + settings.sumInfo.colspan + '" style="text-align: right"><strong>Tổng</strong></td>';
                            }
                            for (var i = 0; i < settings.columns.length; i++) {
                                var col = settings.columns[i];
                                if (col.sumable) {
                                    tr += '<td><strong>' + self.formatPrice(sumarray[i]) + '</strong></td>';
                                } else {
                                    tr += '<td>&nbsp;</td>';
                                }
                            }
                            tr += '</tr>';
                            tbody += tr;
                        }
                        $(selector).find("tbody").append(tbody);
                        self.setEvents();
                    } else {
                        tr = '<tr><td colspan="' + (settings.columns.length + 1) + '"><div class="alert alert-primary  no-border" style="margin-bottom:0">Không tìm thấy kết quả</div></td></tr>';
                        tbody += tr;
                        $(selector).find("tbody").append(tbody);
                    }

                    if (settings.paging != null) {
                        $(selector).find("tfoot").css('display', '');
                        self.setPagination(count);
                    }

                    self.hideTableLoading();
                    if (settings.loadDataCallback != null) {
                        settings.loadDataCallback(result);
                    }
                    if (callback != null) {

                        callback(result);
                    }
                }
            });
        };
        self.initTable = function () {
            //$(selector).addClass('table-responsive');
            var tb = '<div class="' + (settings.responsive ? 'table-responsive' : '') + '"><table class="table ' + (settings.style != null ? settings.style : '') + '">';
            if (settings.header == null || settings.header) {
                var thead = '<thead><tr>';

                if (settings.checkAll == null || settings.checkAll) {
                    thead += '<th style="width:35px"><div class="checkbox checkbox-info"><input id="checkAll" type="checkbox"><label></label></div></th>';
                }
                if (settings.detailRow != null) {
                    thead += '<th style="width:20px">&nbsp;</th>';
                }

                $.each(settings.columns, function (k, col) {
                    if (col.visible == null || col.visible()) {
                        thead += '<th ';
                        if (col.style != null) {
                            thead += 'style="' + col.style + '" ';
                        }
                        if (col.class != null) {
                            thead += 'class="' + col.class + '" ';
                        }
                        if (col.sortable != null) {
                            thead += 'class="orderable" orderby="' + col.attribute + '" currentOrder="desc">';
                        } else {
                            thead += '>';
                        }
                        thead += (col.title != null ? col.title : "&nbsp;") +
                            (col.sortable != null ? '<i style="margin-left:5px;" class="fa fa-caret-down"></i>' : "") + "</th>";
                    }

                });
                thead += "</tr></thead>";
                tb += thead;
            }

            tb += '<tbody></tbody></table></div>';
            $(selector).append(tb);
            if (settings.paging != false) {
                var tfoot = '<div class="display-block" style="overflow: hidden; padding: 15px">' +
                    '<ul class="pagination bootpag pagination-flat pagination-sm pull-right" style="margin: 0"></ul>';
                if (settings.paging.options != null) {
                    var opts = settings.paging.options;
                    tfoot += '<div class="pull-right mr-10" style="width:180px"><span style="display: inline-block">Hiển thị </span> &nbsp;&nbsp;' +
                        '<div class="form-group form-group-xs ml-10 mr-10"  style="display: inline-block; margin-bottom: 0">' +
                        '<select class="form-control limitation" style="width: 60px;display: inline; padding: 4px">';
                    for (var i = 0; i < opts.length; i++) {
                        tfoot += '<option value="' + opts[i] + '">' + opts[i] + '</option>';
                    }
                    tfoot += '</select></div><span style="display: inline-block;"> &nbsp;&nbsp;kết quả</span></div>';
                }
                tfoot += '</div>';
                $(selector).append(tfoot);
            }

            if (settings.autoLoad != false) {
                self.loadData();
            }
        };
        self.initTable();
        return this;
    }

}(jQuery));