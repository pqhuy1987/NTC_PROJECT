﻿var loaiCuocHop = ['Họp với chủ đầu tư', 'Họp với BCH công trường'];

var year, month;

var Meeting = function (obj) {
    var s = this;
    $.extend(s, ko.mapping.fromJS(obj));
    s.title = ko.observable(loaiCuocHop[obj.LoaiCuocHop]);
}

var Day = function (d) {
    var s = this;
    s.d = ko.observable(d.format("D"));
    s.fullDate = ko.observable(d.format("MM/DD/YYYY"));
    s.meetings = ko.observableArray([]);
}

var Week = function (n) {
    var s = this;
    s.n = ko.observable(n);
    s.days = ko.observableArray([]);
}

var HopCongTruongViewModel = function () {
    var s = this;
    s.weeks = ko.observableArray([]);
    s.searchParams = {

    };
    s.changeMonth = function (year, m) {
        var daunam = moment([year, 0, 1]);
        s.weeks.removeAll();
        for (var i = 0; i < 6; i++) {
            var w = moment([year, m - 1, 1]).add(i, 'weeks');
            var wi = w.diff(daunam, 'weeks') + 1;

            var rangs = w.diff(daunam, 'days');
            rangs = rangs % 7;
            if (rangs + daunam.days() > 7) {
                wi++;
            }
            var wobj = new Week(wi);
            for (var j = 0; j < 7; j++) {
                wobj.days.push(new Day(w.day(j + 1)));
            }
            s.weeks.push(wobj);
        }
    }
    s.initModal = function (t, content) {
        var modal = '#ConstructionMeetingEditModal';
        if ($(modal).length == 0) {
            var html = '<div class="modal" id="ConstructionMeetingEditModal" data-backdrop="static"  role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">';
            html += '    <div class="modal-dialog" style="width: 700px">';
            html += '<div class="modal-content">';
            html += '      <div class="modal-header pt-10 pl-15 pr-15 pb-10 bg-primary">';
            html += '          <button class="close" aria-hidden="true" data-dismiss="modal" type="button">×</button>';
            html += '              <h6 class="modal-title text-bold">' + (t == 0 ? 'Thêm' : 'Sửa') + ' cuộc họp</h6>';
            html += '          </div>' +
                '<form action="" id="ConstructionMeetingForm" class="form-horizontal" enctype="multipart/form-data" method="post" >' +
                '<div class="modal-body pt-15 pl-15 pr-15 pb-5">';
            
            html += '</div>';
            html += '<div class="panel-footer panel-footer p-15">' +
                '<div class="text-right">' +
                '<button class="btn btn-sm btn-default form-cancel mr-10" data-dismiss="modal">' +
                '<i class="fa fa-remove"></i> Thoát' +
                '</button>' +
                '<button class="btn btn-sm btn-fill btn-primary m-r-5 btn-submit" type="button" ' +
                'data-loading-text="<i class=' + "'icon-spinner4 fa-spin'" + '></i> Đang xử lý ...">' +
                '<i class="fa fa-save"></i> Lưu lại' +
                '</button>' +
                '</div>' +
                '</div>';
            html += '</form></div></div></div>';
            $("body").append(html);
        }
        var title = t == 0 ? 'Thêm' : 'Sửa';
        $(modal + ' .modal-title').text(title + ' cuộc họp');
        $(modal + ' .modal-body').html(content);
    }
    s.addMeeting = function (obj, e) {
        var btn = $(e.currentTarget);
       
        var gd = $('.chosen-1').val();
        var ct = $('.chosen-2').val();
        if (gd != "" && ct != "") {
            btn.button('loading');
            $.ajax({
                url: '/ConstructionMeeting/constructionMeetingEdit',
                data: {
                    giamdoc: $('.chosen-1').val(),
                    congtruong: $('.chosen-2').val(),
                    date: obj.fullDate()
                },
                success: function(html) {
                    btn.button('reset');
                    s.initModal(0, html);
                    $('#ConstructionMeetingEditModal').modal('show');
                    initConstructionMeetingForm(function(m) {
                        obj.meetings.push(new Meeting(m));
                    });
                }
            });
        } else {
            alert('Chọn giám đốc và công trường');
        }
    }
    s.editMeeting = function (obj, e) {
        var btn = $(e.currentTarget);
        btn.button('loading');
        var d = convertCToJsDate(obj.Date(), 'en');
        $.ajax({
            url: '/ConstructionMeeting/constructionMeetingEdit',
            data: {
                giamdoc: obj.GiamDoc(),
                congtruong: obj.CongTruong,
                date: d,
                id : obj.Id()
            },
            success: function (html) {
                btn.button('reset');
                s.initModal(1, html);
                $('#ConstructionMeetingEditModal').modal('show');
                initConstructionMeetingForm(function (m) {
                    obj.title(loaiCuocHop[m.LoaiCuocHop]);
                    obj.GioHop(m.GioHop);
                    obj.FileDinhKem(m.FileDinhKem);
                });
            }
        });
    }
    s.removeMeeting = function (obj, e) {
        if (confirm('Bạn chắc chắn xóa ?')) {
            $.ajax({
                url: '/ConstructionMeeting/deleteconstructionMeeting',
                type: 'POST',
                data: {
                    id: obj.Id()
                },
                success: function (html) {
                    obj.Status(-1);
                }
            });
        }
    }
    s.loadMeetings = function () {
        $.ajax({
            url: '/ConstructionMeeting/GetListConstructionMeeting',
            data: s.searchParams,
            success: function (result) {
                var daunam = moment([year, 0, 1]);
                $(s.weeks()).each(function () {
                    $(this.days()).each(function () {
                        this.meetings.removeAll();
                    });
                });
                $(result).each(function () {
                    var meeting = this;
                    var date = new Date(parseInt(this.Date.replace("/Date(", "").replace(")/", ""), 10));
                    var w = moment(date);
                    var wi = w.diff(daunam, 'weeks') + 1 ;
                    var rangs = w.diff(daunam, 'days');
                    
                    rangs = rangs % 7; 
                    console.log(daunam.day() + ' - ' + w.format('DD/MM/YYYY') + ' - ' + rangs + ' - ' + wi);
                    if (daunam.day() > 0 && rangs + daunam.day() > 7) {
                        wi++;
                    }
                    //else if (rangs + daunam.day() < 7) {
                    //    wi--;
                    //}
                    var str = wi;
                    
                    $(s.weeks()).each(function () {
                        
                        if(this.n() == wi) { 
                            $(this.days()).each(function () { 
                                if (this.d() == w.format("D")) {
                                   
                                    this.meetings.push(new Meeting(meeting));
                                }
                            });
                        }
                    }); 
                });
            }
        });
    }
    s.init = function () {

        var daunam = moment([year, 0, 1]);
        var d1 = moment([year, 5, 3]);
        var d2 = moment([year, 0, 1]);
        console.log(d1.format('DD/MM/YYYY'))
        //console.log(w.diff(daunam, 'weeks'));


        year = $('.chosen-3').val();
        month = $('.chosen-4').val();
        s.changeMonth(year, month);
        s.searchParams.month = month + '/1/' + year;

        s.loadMeetings();

        $('.chosen-1').chosen({
            width: "200px"
        }).change(function () {
            var val = $(this).val();
            s.searchParams.giamdoc = val;
            $('.chosen-2').empty().append('<option value=""></option>');
            $.ajax({
                url: '/ConstructionMeeting/GetListCongTruong',
                data: {
                    giamdoc: val
                },
                success: function (result) {
                    $(result).each(function () {
                        $('.chosen-2').append('<option value="' + this.maphongban + '">' + this.tenphongban + '</option>');
                    });
                    $('.chosen-2').trigger("chosen:updated");
                }
            });
        }).change(function () {
            var val = $(this).val();
            s.searchParams.giamdoc = val;
            s.loadMeetings();
        });

        $('.chosen-2').chosen({
            width: "200px"
        }).change(function () {
            var val = $(this).val();
            s.searchParams.congtruong = val;
            s.loadMeetings();
        });

        $('.chosen-3').chosen({
            width: "150px"
        }).change(function () {
            var val = $(this).val();
            year = val;
            s.changeMonth(year, month);
            s.searchParams.month = month + '/1/' + year;
            s.changeMonth(val, $('.chosen-4').val());
            s.loadMeetings();
        });

        $('.chosen-4').chosen({
            width: "150px"
        }).change(function () {
            var val = $(this).val();
            s.searchParams.month = val + '/1/' + year;
            month = val;
            s.changeMonth(year, month);
            s.loadMeetings();
        });

        $('.chosen-5').chosen({
            width: "200px"
        }).change(function () {
            var val = $(this).val();
            s.searchParams.loaicuochop = val;
            s.loadMeetings();
        });
    }
    s.init();
}

function convertCToJsDate(value, lang) {
    var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
    var month = (date.getMonth() + 1);
    if (month < 10) {
        month = '0' + month;
    }
    if (lang == 'vn') {
        return date.getDate() + '/' + month + '/' + date.getFullYear();
    } else {
        return month + '/' + date.getDate() + '/' + date.getFullYear();
    }
}

$(document).ready(function () {
    ko.applyBindings(new HopCongTruongViewModel(), $('#apply_element')[0]);

})