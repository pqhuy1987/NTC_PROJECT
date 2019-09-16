using System;
using System.Web;
using RICONS.DataServices.Context;

namespace RICONS.DataServices.Executes.ConstructionMeetings
{
    public class SearchConstructionMeetingModel
    {
        public int? GiamDoc { get; set; }
        public int? CongTruong { get; set; }
        public DateTime? Month { get; set; }
        public int? LoaiCuocHop { get; set; }
    }
    public class ConstructionMeetingViewModel : ConstructionMeeting
    {

    }
    public class ConstructionMeetingEditModel : ConstructionMeeting
    {
        public string DateStr { get; set; }
        public HttpPostedFileBase FileDinhKemPostFileBase { get; set; }
    }
}
