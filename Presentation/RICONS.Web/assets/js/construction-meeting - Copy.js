
$(document).ready(function() {
    // Render calendar
    $('.fullcalendar-languages').fullCalendar({
        header: {
            left: '',
            center: 'title',
            right: ''
        },
        dayNamesShort: ["Chủ Nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy"],
        weekHeader: "Tuần",
        defaultDate: moment(),
        locale: 'vi',
        buttonIcons: false, 
        weekNumbers: true,
        editable: true,
        eventRender: function (event, element, view) {
            return $('<div class="fc-day-grid-event fc-h-event fc-event fc-start fc-end">' +
                '<div class="media">' +
                '<div class="media-body">' +
										'<h6 class="media-heading">James Alexander</h6>' +
										'<span class="text-muted">Lead developer</span>' +
									'</div>' +

                '<div class= "media-right media-middle"> ' +
                '<ul class= "icons-list"> ' +
                '<li class= "dropdown"> ' +
                '<a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> ' +
                '<i class= "fa fa-align-justify" ></i> ' +
					                    		'</a> ' +

                '<ul class="dropdown-menu" > ' +
                '<li > <a href="#"><i class="icon-comment-discussion pull-right"></i> Start chat</a></li> ' +
                '<li > <a href="#"><i class="icon-phone2 pull-right"></i> Make a call</a></li> ' +
                '<li > <a href="#"><i class="icon-mail5 pull-right"></i> Send mail</a></li> ' +
                '<li class= "divider" ></li > ' +
                '<li > <a href="#"><i class="icon-statistics pull-right"></i> Statistics</a></li> ' +
												'</ul> ' +
					                    	'</li> ' +
				                    	'</ul> ' +
									'</div> ' +
                '</div>');
        },
        events: [
            {
                title: 'All Day Event',
                start: '2018-11-01'
            },
            {
                title: 'Long Event',
                start: '2018-11-07',
                end: '2018-11-10'
            },
            {
                id: 999,
                title: 'Repeating Event',
                start: '2018-11-09T16:00:00'
            },
            {
                id: 999,
                title: 'Repeating Event',
                start: '2018-11-16T16:00:00'
            },
            {
                title: 'Conference',
                start: '2018-11-11',
                end: '2018-11-13'
            },
            {
                title: 'Meeting',
                start: '2018-11-12T10:30:00',
                end: '2018-11-12T12:30:00'
            },
            {
                title: 'Lunch',
                start: '2018-11-12T12:00:00'
            },
            {
                title: 'Meeting',
                start: '2018-11-12T14:30:00'
            },
            {
                title: 'Happy Hour',
                start: '2018-11-12T17:30:00'
            },
            {
                title: 'Dinner',
                start: '2018-11-12T20:00:00'
            },
            {
                title: 'Birthday Party',
                start: '2018-11-13T07:00:00'
            },
            {
                title: 'Click for Google',
                url: 'http://google.com/',
                start: '2018-11-28'
            }
        ],
        eventAfterAllRender: function(v) {
            //$('.dropdown-toggle').dropdown();
        }
    });
})