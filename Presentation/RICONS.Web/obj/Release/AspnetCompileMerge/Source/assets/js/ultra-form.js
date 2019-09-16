
(function ($) {
    $.fn.ultraForm = function (options) {
        var s = this;
        var form = this.selector;
        var setting = $.extend({
            uiType: null,
            props: null,
            autoSubmit: null,
            beforeSubmit: null,
            afterSubmit: null,
            validCallback: null,
            initCallback: null
        }, options);

        s.hasError = function () {
            var isValid = true;
            var hasError = false;
            $(setting.props).each(function () {
                isValid = s.isPropValid(this);
                if (!isValid) {
                    hasError = true;
                }
            });
            return hasError;
        }
        s.getPropData = function (prop) {
            var ele = $(form).find('[name="' + prop.name + '"]');
            switch (prop.type) {
                case 'summernote':
                    {
                        return ele.summernote('code');
                    }
                case 'file':
                    {
                        return $(form).find('[name="' + prop.name + 'PostFileBase"]').prop('files')[0];
                    }
                case 'checkbox':
                    {
                        return ele.prop('checked');
                    }
                case 'radio':
                    {
                        return $(form).find('[name="' + prop.name + '"]:checked').val();
                    }
                case 'datepicker':
                    {
                        var d = ele.val();
                        return app.convertVnToEnDate(d);
                    }
                case 'chosen':
                {
                        return ele.val();
                    }
                default:
                    {
                        return ele.val();
                    }
            }
        }
        s.isPropValid = function (prop) {
            var ele = $(form).find('[name="' + (prop.type == 'file' ? prop.name + 'PostFileBase' : prop.name) + '"]');
            var val = s.getPropData(prop);
            var fg = ele.closest('.form-group');
            var hm = fg.find('.help-msg');
            if (prop.required) {
                if (prop.type == 'file') {
                    var uploaded = $(form).find('[name="' + prop.name + 'Uploaded"]').val();

                    if (uploaded == '' && !app.hasValue(val)) {
                        fg.addClass('has-error');
                        hm.text(prop.required.message).css('display', 'block');
                        return false;
                    }
                } else {
                    if (val == '' || val == null || val == '00000000-0000-0000-0000-000000000000') {
                        fg.addClass('has-error');
                        hm.text(prop.required.message).css('display', 'block');
                        return false;
                    }
                }
            }
            if (prop.minLength != null) {
                if (val.length < prop.minLength.val) {
                    fg.addClass('has-error');
                    hm.text(prop.minLength.message).css('display', 'block');
                    return false;
                }
            }
            if (prop.maxLength != null) {
                if (val.length > prop.maxLength.val) {
                    fg.addClass('has-error');
                    hm.text(prop.maxLength.message).css('display', 'block');
                    return false;
                }
            }
            if (prop.min != null) {
                if (parseFloat(val) < prop.min.val) {
                    fg.addClass('has-error');
                    hm.text(prop.min.message).css('display', 'block');
                    return false;
                }
            }
            if (prop.max != null) {
                if (parseFloat(val) > prop.max.val) {
                    fg.addClass('has-error');
                    hm.text(prop.max.message).css('display', 'block');
                    return false;
                }
            }
            if (prop.email != null) {
                if (app.hasValue(val)) {
                    var emailPattern = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
                    if (!emailPattern.test(val)) {
                        fg.addClass('has-error');
                        hm.text(prop.email.message).css('display', 'block');
                        return false;
                    }
                }
            }
            if (prop.acceptFormats != null) {
                ele = $(form).find('[name="' + prop.name + 'PostFileBase"]');
                var fs = ele.prop('files');
                if (fs.length > 0) {
                    var fn = ele.prop('files')[0].name;
                    var ext = fn.replace(/^.*\./, '');
                    if (jQuery.inArray(ext, prop.acceptFormats) < 0) {
                        fg.addClass('has-error');
                        hm.text('Định dạng file không hợp lệ').css('display', 'block');
                        return false;
                    }
                }
            }
            fg.removeClass('has-error');
            hm.css('display', 'none');
            return true;
        }
        s.submit = function (btn) {
            if (!s.hasError()) {
                if (setting.actionType == 'ajax') {
                    var data;
                    var isOk = true;
                    if (setting.beforeSubmit != null) {
                        isOk = setting.beforeSubmit();
                    }
                    if (isOk) {
                        if (setting.autoSubmit == null || setting.autoSubmit == true) {
                            btn.button('loading');
                            data = new FormData();
                            $(setting.props).each(function () {
                                if (this.type == 'file') {
                                    data.append(this.name + 'PostFileBase', s.getPropData(this));
                                    data.append(this.name, $(form).find('[name="' + this.name + 'Uploaded"]').val());
                                } else {
                                    data.append(this.name, s.getPropData(this));
                                }
                            });
                            $.ajax(
                                {
                                    url: setting.action,
                                    type: "POST",
                                    data: data,
                                    processData: false,
                                    contentType: false,
                                    success: function (result) {
                                        btn.button('reset');
                                        if (setting.afterSubmit != null) {
                                            setting.afterSubmit(result);
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
                        } else {
                            data = new FormData();
                            $(setting.props).each(function () {
                                if (this.type == 'file') {
                                    
                                    data.append(this.name + 'PostFileBase', s.getPropData(this));
                                    data.append(this.name, $(form).find('[name="' + this.name + 'Uploaded"]').val());
                                } else {
                                    data.append(this.name, s.getPropData(this));
                                }
                            });
                            if (setting.validCallback != null) {
                                setting.validCallback(data, btn);
                            }
                        }
                    }
                } else {
                    if (typeof $(form).attr('action') == 'undefined') {
                        $(form).attr('action', setting.action);
                    }
                    if (setting.autoSubmit == null || setting.autoSubmit == true) {
                        $(form).submit();
                    }
                }
            }
        }
        s.events = function () {
            $(setting.props).each(function () {
                var prop = this;
                var ele = $(form).find('[name="' + prop.name + '"]');
                switch (prop.type) {
                    case 'summernote':
                        {
                            ele.on("summernote.change",
                                function (e) {
                                    s.isPropValid(prop);
                                });
                        } break;
                    case 'file':
                        {
                            ele = $(form).find('[name="' + prop.name + 'PostFileBase"]');
                            ele.change(function () {
                                if (s.isPropValid(prop)) {
                                    if (prop.onChange != null) {
                                        prop.onChange(s.getPropData(prop));
                                    }
                                }
                            });
                        }
                        break;
                    case 'chosen':
                        {
                            ele.change(function () {
                                s.isPropValid(prop);
                                if (prop.onChange != null) {
                                    prop.onChange($(this).val());
                                }
                            });
                        }
                        break;
                    default:
                        {
                            ele.change(function () {
                                s.isPropValid(prop);
                                if (prop.onChange != null) {
                                    prop.onChange($(this).val());
                                }
                            });
                        }
                }
            });
            $(form + ' .view-pass').on('mousedown', function () {
                var input = $(this).closest('.input-group').find('input');
                input.attr('type', 'text');
            }).on('mouseup mouseleave', function () {
                var input = $(this).closest('.input-group').find('input');
                input.attr('type', 'password');
            });
            $(form + ' .btn-submit').unbind().click(function () {
                s.submit($(this).button());
            });
            if (setting.initCallback != null) {
                setting.initCallback();
            }
        }
        s.init = function () {
            var hm = '<span class="help-block help-msg" style="display: none"></span>';
            $(setting.props).each(function () {
                var prop = this;
                var ele = $(form).find('[name="' + (prop.type == 'file' ? prop.name + 'PostFileBase' : prop.name) + '"]');
                var fg = $(ele).closest('.form-group');
                if (setting.uiType == 0) { // vertical
                    fg.append(hm);
                } else {
                    fg.find(' > div').eq(0).append(hm);
                }

                switch (this.type) {
                    case 'datepicker':
                        {
                            ele.daterangepicker({
                                singleDatePicker: true,
                                autoUpdateInput: false, 
                                locale: {
                                    format: 'DD/MM/YYYY'
                                }
                            }, function (d) {
                                $(this.element[0]).val(d.format('DD/MM/YYYY'));
                            });
                        }
                        break;
                    case 'file':
                        {

                        }
                        break;
                    case 'number':
                        {
                            ele.TouchSpin(prop.option);
                        }
                        break;
                    case 'mask':
                        {
                            ele.mask(prop.option.format);
                        }
                        break;
                    case 'money':
                        {
                            ele.number(true,0);
                        }
                        break;
                    case 'checkbox':
                    case 'radio':
                        {

                        }
                        break;
                    case 'chosen':
                        {
                            ele.chosen(prop.option);
                        }
                        break;
                    case 'summernote':
                        {
                            initSummernote({
                                system: false
                            }, prop.option);
                        } break;
                    case 'compoTree':
                        {
                            prop.option.selectCallback = function () {
                                s.isPropValid(prop);
                                if (prop.onChange != null) {
                                    prop.onChange(s.getPropData(prop));
                                }
                            }
                            ele.compoTree(prop.option);
                        }
                        break;
                }
            });
            s.events();
        }
        s.init();
        return s;
    }
}(jQuery));