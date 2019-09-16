(function ($) {
    $.fn.btnSelect = function (options) {
        var s = this;
        s.selector = this.selector;
        s.setting = $.extend({
            onSelect: null
        }, options);
        $(s.selector).find('.dropdown-menu a').click(function () {
            $(s.selector).find('.btn span').text($(this).text());
            if (s.setting.onSelect != null) {
                s.setting.onSelect($(this).attr('dataid'));
            }
        });
        s.init = function () {

        }
        s.init();
        return s;
    }
}(jQuery));

var icons = {
    'warning': 'pe-7s-info',
    'success': 'pe-7s-check',
    'error': 'fa fa-frown-o'
};

var daysOfWeek = [ "CN", "T2", "T3", "T4", "T5", "T6", "T7" ],
    monthNames = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];

var newGuid = function (length) {
    var text = "";
    if (length == null) {
        length = 10;
    }
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (var i = 0; i < length; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
},
    convertIsoDateToJsDate = function (value, lang, hasTime) {
        if (typeof value === 'string') {
            var a =
                /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)(?:([\+-])(\d{2})\:(\d{2}))?Z?$/.exec(value);
            if (a) {
                var utcMilliseconds = Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4], +a[5], +a[6]);
                var date = new Date(utcMilliseconds);
                var month = (date.getMonth() + 1);
                if (month < 10) {
                    month = '0' + month;
                }
                lang = lang != null ? lang : 'vn';
                var str = '';
                if (lang == 'vn') {
                    str = date.getDate() + '/' + month + '/' + date.getFullYear();
                } else {
                    str = month + '/' + date.getDate() + '/' + date.getFullYear();
                }
                if (hasTime) {
                    var hours = date.getHours();
                    var minutes = date.getMinutes();

                    if (minutes < 10)
                        minutes = '0' + minutes;
                    str += ' ' + hours + ":" + minutes + " ";
                }
                return str;
            }
        }
        return value;
    },
    convertCToJsDate = function (value, lang, hasTime) {
        if (!app.hasValue(value))
            return '';
        var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
        var month = (date.getMonth() + 1);
        if (month < 10) {
            month = '0' + month;
        }
        lang = lang != null ? lang : 'vn';
        var str = '';
        if (lang == 'vn') {
            str = date.getDate() + '/' + month + '/' + date.getFullYear();
        } else {
            str = month + '/' + date.getDate() + '/' + date.getFullYear();
        }
        if (hasTime) {
            var hours = date.getHours();
            var minutes = date.getMinutes();

            if (minutes < 10)
                minutes = '0' + minutes;
            str += ' ' + hours + ":" + minutes + " ";
        }
        return str;
    },
    app_confirm = function (type, title, text, callback) {
        var option = {
            title: "Bạn có chắc chắn?",
            type: type,
            closeOnConfirm: true
        };
        switch (type) {
            case 'success':
                {

                }
                break;
            case 'warning':
                {
                    option.showCancelButton = true;
                    option.confirmButtonColor = "#DD6B55";
                    option.confirmButtonText = "Đồng ý";
                    option.cancelButtonText = "Thoát";
                }
                break;
            case 'error':
                {

                }
                break;
        }
        if (title != null) {
            option.title = title;
        }
        if (text != null) {
            option.text = text;
        }
        swal(option, callback);
    },
    cleanJson = function (obj) {
        for (var propName in obj) {
            if (obj[propName] === null || obj[propName] === undefined) {
                delete obj[propName];
            }
        }
    },
    toAid = function (id, c, len) {
        if (len == null) {
            len = 7;
        }
        var z = len - (id + '').length;
        for (var i = 0; i < z; i++) {
            c += '0';
        }
        return c + id;
    },
    convertVnToEnDate = function (d) {
        var arr = d.split('/');
        if (arr.length >= 3) {
            return arr[1] + '/' + arr[0] + '/' + arr[2];
        }
        return '';
    },
    app_alert = function (type, message) {
        var a = '<div class="alert alert-' +
            type +
            ' fade in" role="alert">' +
            '<button class="close" type="button" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button>';
        a += message + '</div>';
        return a;
    },
    emailValid = function (val) {
        var emailPattern = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
        return emailPattern.test(val);
    },
    replaceBrToNewLine = function (str) {
        return str.replace(new RegExp('<br/>', 'g'), '\n');
    },
    replaceNewLineToBr = function (str) {
        return str.replace(new RegExp('\n', 'g'), '<br/>');
    },
    app_notify = function (type, message) {
        new PNotify({
            text: message,
            addclass: 'bg-' + type
        });
    },
    setLoading = function (mt, size) {
        if (mt == null)
            mt = 0;

        if (size == null) {
            size = 'lg';
        }

        if (size == 'lg') {
            return '<div class="arc-rotate-double" style="margin-top: ' +
                mt +
                'px;">' +
                '<div class="loader"><div class="arc-1"></div><div class="arc-2"></div></div>' +
                '</div>';
        }
        return '<div class="spinner" style="margin-top: ' +
            mt +
            'px;">' +
            '<div class="bounce1"></div>' +
            '<div class="bounce2"></div>' +
            '<div class="bounce3"></div>' +
            '</div>';
    },
    formDataToJson = function (formData) {
        var object = {};
        formData.forEach(function (value, key) {
            object[key] = value;
        });
        return object;
    },
    formatJsDate = function (value) {
        if (!app.hasValue(value))
            return '';
        var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
        return date;
    },
    formatDate = function (value, lang) {
        if (!app.hasValue(value))
            return '';
        var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
        var month = (date.getMonth() + 1);
        if (month < 10) {
            month = '0' + month;
        }
        lang = lang != null ? lang : 'vn';
        if (lang == 'vn') {
            return date.getDate() + '/' + month + '/' + date.getFullYear();
        }
        return month + '/' + date.getDate() + '/' + date.getFullYear();
    },
    formatDateTime = function (value, lang) {
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

        lang = lang != null ? lang : 'vn';
        if (lang == 'vn') {
            return date.getDate() + '/' + month + '/' + date.getFullYear() + " " + hours + ":" + minutes + " ";
        }
        return month + '/' + date.getDate() + '/' + date.getFullYear() + " " + hours + ":" + minutes + " ";
    },
    formatBeatyDateTime = function () {
        $('.beaty-datetime').each(function () {
            var t = $(this);
            if (!t.hasClass('formated')) {
                var data = t.attr('data');
                t.text(beatyTime(data));
                t.addClass('formated');
            }
        });
    },
    rateRender = function () {
        $('.rate-render').each(function () {
            if (!$(this).hasClass('rendered')) {
                var r = $(this).attr('data');
                var t = $(this).attr('data-total');
                var empty = 5 - r;
                var h = '';
                for (var i = 0; i < r; i++) {
                    h += '<i class="icon-star-full2 text-orange-400 text-size-base"></i>';
                }
                for (var i = 0; i < empty; i++) {
                    h += '<i class="icon-star-full2 text-muted text-size-base"></i>';
                }
                if (t != null) {
                    h += '<span class="text-muted position-right">(' + t + ')</span>';
                }
                $(this).html(h);
                $(this).addClass('rendered');
            }

        });
    },
    loadData = function (url, params, page, callback) {
        var dt = params.dataType != null ? params.dataType.toLowerCase() : 'json';
        if (page != null) {
            $.extend(params,
                {
                    page: page
                });
        }
        var options = {
            url: url,
            data: params,
            type: "GET",
            dataType: dt,
            success: function (result) {
                if (dt == 'json' && hasValue(result.SessionExpired) && result.SessionExpired) {
                    $('#login_modal').modal('show');
                } else if (dt == 'html' && result.indexOf("SessionExpired") >= 0) {
                    $('#login_modal').modal('show');
                }
                else {
                    callback(result);
                }
            }
        }
        if (params.cache) {
            options.cache = true;
        }
        if (params.async == false) {
            options.async = false;
        }
        return $.ajax(options);
    },
    lazyLoader = function () {
        $(".lazy").Lazy({
            beforeLoad: function (element) {
                // called before an elements gets handled
            },
            afterLoad: function (element) {
                // called after an element was successfully handled
                $(element).addClass('loaded');
            },
            onError: function (element) {
                // called whenever an element could not be handled
            },
            onFinishedAll: function () {
                // called once all elements was handled
            }
        });
    },
    postData = function (url, params, callback, forForm) {
        var options = {
            url: url,
            data: params,
            type: "POST",
            dataType: params.dataType != null ? params.dataType : 'JSON',
            success: function (result) {
                if (hasValue(result.SessionExpired) && result.SessionExpired) {
                    $('#login_modal').modal('show');
                } else {
                    callback(result);
                }
            }
        }
        if (params.async == false) {
            options.async = false;
        }
        if (forForm) {
            options.processData = false;
            options.contentType = false;
        }
        $.ajax(options);
    },
    hasValue = function (obj) {
        return typeof obj != 'undefined' && obj != '' && obj != null;
    },
    convertNiceStrToObject = function (str) {
        var objs = [];
        if (str != null && str != '') {
            var arr = str.split('|');
            $(arr).each(function () {
                if (this != '') {
                    var a = this.split(';');
                    objs.push({
                        id: a[0],
                        text: a[1]
                    });
                }
            });
        }
        return objs;
    },
    d100 = function (str) {
        str = str + str;
        var d = [];
        for (var i = 0; i < str.length; i++) {
            var b = str[i].charCodeAt().toString(2);
            var b1 = Array(8 - b.length + 1).join("0") + b;
            b1 = (b1 + '').replace(/0/g, '2').replace(/1/g, '0').replace(/2/g, '1');
            var b2 = '';
            for (var j = 0; j < 8; j++) {
                if (b1[j] == '0') {
                    b2 += String.fromCharCode((Math.floor(Math.random() * 25) + 65));
                } else {
                    b2 += String.fromCharCode((Math.floor(Math.random() * 25) + 97));
                }
            }
            d.push(b2);
        }
        return d.join('');
    },
    _substr = function (str, len) {
        if (hasValue(str)) {
            if (str.length > len) {
                return str.substring(0, len) + '...';
            }
            return str;
        }
        return '';
    },
    idImage = function (type, media, id, thumbsize, name, companyId) {
        var p = '/media' + (media > 0 ? media : '');
        if (companyId != null) {
            p += '/company/' + companyId;
        }
        p += '/' + type + '/';
        var fname = name.indexOf('.jpg') >= 0 ? name : name + '.jpg';
        if (id > 100) {
            var i = 1;
            var idname = id + '';
            while (i < idname.length - 1) {
                p += idname.substring(0, i) + '/';
                i++;
            }
        }
        p += id + "/" + fname;
        if (hasValue(thumbsize)) {
            p += '.thumb/' + thumbsize + '.jpg';
        }
        return p;
    },
    guidImage = function (type, media, guid) {
        var p = '/media' + (media > 0 ? media : '');
        p += '/' + type + '/';
        var fname = name.indexOf('.jpg') >= 0 ? name : name + '.jpg';
        p += guid[0] + '/' + guid + fname;
        return p;
    },
    idFolder = function (path, id) {
        var p = '';
        if (id > 100) {
            var i = 1;
            var idname = id + '';
            while (i < idname.length - 1) {
                p += idname.substring(0, i) + '/';
                i++;
            }
        }
        p += path + id;
        return p;
    },
    dateImage = function (type, media, id, thumbsize, name) {
        var p = '/media' + (media > 0 ? media : '') + '/' + type + '/';
        var fname = name.indexOf('.jpg') >= 0 ? name : name + '.jpg';
        if (id > 100) {
            var i = 1;
            var idname = id + '';
            while (i < idname.length - 1) {
                p += idname.substring(0, i) + '/';
                i++;
            }
        }
        p += id + "/" + fname;
        if (hasValue(thumbsize)) {
            p += '.thumb/' + thumbsize + '.jpg';
        }
        return p;
    },
    thumb = function (type, fileName, size) {
        var extension;
        var path = '';
        if (fileName == '' || fileName == null) {
            if (size != null) {
                path = '/media/default/' + type + '/' + size + '.jpg';
            } else {
                path = '/media/default/' + type + '/xs.jpg';
            }

        } else if (type == '' || type == null) {
            if (size != null) {
                extension = fileName.substr(fileName.lastIndexOf('.'));
                path = fileName + ".thumb/" + size + extension;
            } else {
                path = fileName;
            }
        } else {
            if (type == 'videos') {
                extension = '.jpg';
            } else {
                extension = fileName.substr(fileName.lastIndexOf('.'));
            }
            console.log(extension);
            if (size != null) {
                path = fileName + ".thumb/" + size + extension;
            } else {
                path = fileName;
            }
        }
        return path;
    },
    removeVnChars = function (str) {
        str = str.replace(/à|à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ.+/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ.+/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/đ/g, "d");
        return str;
    },
    beatyTime = function (dt) {
        var date = new Date(parseInt(dt.replace("/Date(", "").replace(")/", ""), 10));
        var dms = new Date().getTime() - date.getTime();
        if (Math.round(dms / (1000 * 60 * 60 * 24 * 30)) > 0) {
            return Math.round(dms / (1000 * 60 * 60 * 24 * 30)) + ' tháng trước';
        }
        if (Math.round(dms / (1000 * 60 * 60 * 24)) > 0) {
            return Math.round(dms / (1000 * 60 * 60 * 24)) + ' ngày trước';
        }
        if (Math.round(dms / (1000 * 60 * 60)) > 0) {
            return Math.round(dms / (1000 * 60 * 60)) + ' giờ trước';
        }
        if (Math.round(dms / (1000 * 60)) > 0) {
            return Math.round(dms / (1000 * 60)) + ' phút trước';
        }
        return 'Vừa xong';
    },
    formatPrice = function (str) {
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
    },
    requestAuth = function (callback) {
        if ($('#user_role').length == 0) {
            var am = '#authModal';
            var myback = function () {
                $(am).modal('hide');
                loadData('/api/authPanel',
                    {
                        dataType: 'html'
                    },
                    null,
                    function (result) {
                        $('#auth_panel').html(result);
                        if (callback != null) {
                            callback(true);
                        }
                    });
            }
            var option = {
                loginCallback: myback,
                registCallback: myback,
                forgotCallback: myback
            };
            if ($(am + ' .form-content').html() == '') {
                loadData('/api/authForm',
                    {
                        dataType: 'html'
                    },
                    null,
                    function (result) {
                        $(am + ' .form-content').html(result);
                        $(am).modal('show');
                        initAuth(option);
                    });
            } else {
                $(am).modal('show');
                initAuth(option);
            }
            $(am + ' .close').unbind().click(function () {
                $(am).modal('hide');
                callback(false);
            });
        } else {
            if (callback != null) {
                callback(true);
            }
        }
    },
    convertTimeRange = function (timeCode) {
        var d = {};
        switch (timeCode) {
            case 'HN':
                {
                    var t = moment(new Date());
                    d.from = t.format("DD/MM/YYYY");
                    d.to = t.format("DD/MM/YYYY");
                } break;
            case 'HQ':
                {
                    var tmr = moment(new Date()).add(-1, 'days');
                    d.from = tmr.format("DD/MM/YYYY");
                    d.to = tmr.format("DD/MM/YYYY");
                } break;
            case 'TN':
                {
                    d.from = moment().startOf('isoweek').format("DD/MM/YYYY");
                    d.to = moment().endOf('isoweek').format("DD/MM/YYYY");
                }
                break;
            case 'DTDHT':
                {
                    d.from = moment().startOf('isoweek').format("DD/MM/YYYY");
                    d.to = moment(new Date()).format("DD/MM/YYYY");
                }
                break;
            case 'TTR':
                {
                    var t = moment(new Date()).add(-1, 'weeks');
                    d.from = t.startOf('isoweek').format("DD/MM/YYYY");
                    d.to = t.endOf('isoweek').format("DD/MM/YYYY");
                }
                break;
            case 'THN':
                {
                    d.from = moment().startOf('month').format("DD/MM/YYYY");
                    d.to = moment().endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'THTR':
                {
                    var t = moment(new Date()).add(-1, 'months');
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'DTHDHT':
                {
                    d.from = moment().startOf('month').format("DD/MM/YYYY");
                    d.to = moment().format("DD/MM/YYYY");
                }
                break;
            case 'QN':
                {
                    d.from = moment().startOf('quarter').format("DD/MM/YYYY");
                    d.to = moment().endOf('quarter').format("DD/MM/YYYY");
                }
                break;
            case 'DQDHT':
                {
                    d.from = moment().startOf('quarter').format("DD/MM/YYYY");
                    d.to = moment().format("DD/MM/YYYY");
                }
                break;
            case '6TDN':
                {
                    var t = moment().startOf('year');
                    d.from = t.format("DD/MM/YYYY");
                    d.to = t.add(6, 'months').format("DD/MM/YYYY");
                }
                break;
            case '6TCN':
                {
                    var t = moment().endOf('year');
                    d.to = t.format("DD/MM/YYYY");
                    d.from = t.add(-6, 'months').format("DD/MM/YYYY");
                }
                break;
            case 'NN':
                {
                    d.from = moment().startOf('year').format("DD/MM/YYYY");
                    d.to = moment().endOf('year').format("DD/MM/YYYY");
                }
                break;
            case 'NTR':
                {
                    var t = moment().add(-1, 'year');
                    d.from = t.startOf('year').format("DD/MM/YYYY");
                    d.to = t.endOf('year').format("DD/MM/YYYY");
                }
                break;
            case 'DNDHT':
                {
                    d.from = moment().startOf('year').format("DD/MM/YYYY");
                    d.to = moment().format("DD/MM/YYYY");
                }
                break;
            case 'Q1':
                {
                    var t = moment().startOf('year');
                    d.from = t.startOf('quarter').format("DD/MM/YYYY");
                    d.to = t.endOf('quarter').format("DD/MM/YYYY");
                }
                break;
            case 'Q2':
                {
                    var t = moment().startOf('year').add(4, 'months');
                    d.from = t.startOf('quarter').format("DD/MM/YYYY");
                    d.to = t.endOf('quarter').format("DD/MM/YYYY");
                }
                break;
            case 'Q3':
                {
                    var t = moment().startOf('year').add(7, 'months');
                    d.from = t.startOf('quarter').format("DD/MM/YYYY");
                    d.to = t.endOf('quarter').format("DD/MM/YYYY");
                }
                break;
            case 'Q4':
                {
                    var t = moment().startOf('year').add(10, 'months');
                    d.from = t.startOf('quarter').format("DD/MM/YYYY");
                    d.to = t.endOf('quarter').format("DD/MM/YYYY");
                }
                break;
            case 'T1':
                {
                    var t = moment([new Date().getFullYear(), 0, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T2':
                {
                    var t = moment([new Date().getFullYear(), 1, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T3':
                {
                    var t = moment([new Date().getFullYear(), 2, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T4':
                {
                    var t = moment([new Date().getFullYear(), 3, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T5':
                {
                    var t = moment([new Date().getFullYear(), 4, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T6':
                {
                    var t = moment([new Date().getFullYear(), 5, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T7':
                {
                    var t = moment([new Date().getFullYear(), 6, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T8':
                {
                    var t = moment([new Date().getFullYear(), 7, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T9':
                {
                    var t = moment([new Date().getFullYear(), 8, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T10':
                {
                    var t = moment([new Date().getFullYear(), 9, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T11':
                {
                    var t = moment([new Date().getFullYear(), 10, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            case 'T12':
                {
                    var t = moment([new Date().getFullYear(), 11, 1]);
                    d.from = t.startOf('month').format("DD/MM/YYYY");
                    d.to = t.endOf('month').format("DD/MM/YYYY");
                }
                break;
            default:
        }
        return d;
    },
    createEmptyModal = function (id, title, w) {
        if ($(id).length == 0) {
            if (w == null) {
                w = 700;
            }
            var h = '<div id="' + id + '" class="modal" tabindex="-1" role="dialog" data-backdrop="static">' +
                '<div class="modal-dialog" role="document" style="width: ' + w + 'px">' +
                '<div class="modal-content">' +
                '<div class="modal-header pt-10 pl-15 pr-15 pb-10 bg-primary">' +
                '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                '<h6 class="modal-title text-bold" >' + title + '</h6>' +
                '</div>' +
                '<div class="modal-body">' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>';
            $('body').append(h);
        }
    };
var app = function () {
    "use strict";
    return {
        init: function (params) { },
        formatPrice: function (val) {
            return formatPrice(val);
        },
        sub: function (str, len) {
            return _substr(str, len);
        },
        pagination: function (element, total, currentPage, pageSize, query) {
            pagination(element, total, currentPage, pageSize, query);
        },
        niceStrToObject: function (str) {
            return convertNiceStrToObject(str);
        },
        newGuid: function (length) {
            return newGuid(length);
        },
        alert: function (type, message) {
            return app_alert(type, message);
        },
        loading: function (mt, size) {
            return setLoading(mt, size);
        },
        loadData: function (url, params, page, callback) {
            loadData(url, params, page, callback);
        },
        postData: function (url, params, callback, forForm) {
            postData(url, params, callback, forForm);
        },
        hasValue: function (v) {
            return hasValue(v);
        },
        showPanelLoading: function (panel) {
            return showPanelLoading(panel);
        },
        hidePanelLoading: function (panel) {
            return hidePanelLoading(panel);
        },
        requestAuth: function (callback) {
            requestAuth(callback);
        },
        showModalLoading: function (panel) {
            return showModalLoading(panel);
        },
        hideModalLoading: function (panel) {
            return hideModalLoading(panel);
        },
        thumb: function (type, fileName, size) {
            return thumb(type, fileName, size);
        },
        mapConditions: function () {
            return mapConditions();
        },
        convertVnToEnDate: function (d) {
            return convertVnToEnDate(d);
        },
        confirm: function (type, title, text, callback) {
            return app_confirm(type, title, text, callback);
        },
        notify: function (type, message) {
            return app_notify(type, message);
        },
        idImage: function (type, media, id, thumbsize, name, companyId) {
            return idImage(type, media, id, thumbsize, name, companyId);
        },
        guidImage: function (type, media, guid) {
            return guidImage(type, media, guid);
        },
        idFolder: function (path, id) {
            return idFolder(path, id);
        },
        beatyTime: function (time) {
            return beatyTime(time);
        },
        formatBeatyDateTime: function () {
            return formatBeatyDateTime();
        },
        formatDate: function (obj, lang) {
            return formatDate(obj, lang);
        },
        formatDateTime: function (obj, lang) {
            return formatDateTime(obj, lang);
        },
        toAid: function (id, c, len) {
            return toAid(id, c, len);
        },
        formatJsDate: function (obj) {
            return formatJsDate(obj);
        },
        convertIsoDateToJsDate: function (obj, lang, hasTime) {
            return convertIsoDateToJsDate(obj, lang, hasTime);
        },
        isEmailValid: function (str) {
            return emailValid(str);
        },
        convertCToJsDate: function (obj, lang, hasTime) {
            return convertCToJsDate(obj, lang, hasTime);
        },
        lazyLoader: function () {
            lazyLoader();
        },
        rateRender: function () {
            return rateRender();
        },
        replaceBrToNewLine: function (str) {
            return replaceBrToNewLine(str);
        },
        replaceNewLineToBr: function (str) {
            return replaceNewLineToBr(str);
        },
        createEmptyModal: function (id, title, width) {
            return createEmptyModal(id, title, width);
        },
        formDataToJson: function (formData) {
            return formDataToJson(formData);
        },
        convertTimeRange: function (timeCode) {
            return convertTimeRange(timeCode);
        },
        cleanJson: function (obj) {
            return cleanJson(obj);
        }
    }
}();