using System;
using System.Collections.Generic;
using System.Linq;
using RICONS.DataServices.Context;
using RICONS.DataServices.Executes.ConstructionMeetings;

namespace RICONS.DataServices.Executes
{
    public partial class DataService
    {
        public List<ConstructionMeetingViewModel> ConstructionMeetingMany(SearchConstructionMeetingModel model)
        {
            CheckDbConnect();
            IQueryable<ConstructionMeeting> query = Context.ConstructionMeetings.Where(x => x.Status >= 0);

            if (model.GiamDoc.HasValue)
            {
                query = query.Where(x => x.GiamDoc == model.GiamDoc.Value);
            }

            if (model.LoaiCuocHop.HasValue)
            {
                query = query.Where(x => x.LoaiCuocHop == model.LoaiCuocHop.Value);
            }

            if (model.CongTruong.HasValue)
            {
                query = query.Where(x => x.CongTruong == model.CongTruong.Value);
            }

            if (model.Month.HasValue)
            {
                var fd = model.Month.Value.AddDays(-6);// Em coi lai cho này, Bữa a chỉnh lại nó mới chạy đúng dc. Giờ e cần chỉnh lại k
                // 06 ngày là nó lấấy của tháng trươớc rùi anh, chưứ ko phải tháng cầần lấấáy61
                // ví dụ lọc tháng == 2018/01/01  => fd = = 2018/01/01, td = 2018/01//30
                var fd1 = model.Month.Value;
                var td = fd1.AddMonths(1).AddMinutes(-1);
                query = query.Where(x => x.Date >= fd1 && x.Date <= td);
            }

            var r = (from x in query
                     select new ConstructionMeetingViewModel()
                     {
                         Id = x.Id,
                         Date = x.Date,
                         GioHop = x.GioHop,
                         LoaiCuocHop = x.LoaiCuocHop,
                         ThanhPhanThamDu = x.ThanhPhanThamDu,
                         FileDinhKem = x.FileDinhKem,
                         GiamDoc = x.GiamDoc,
                         CongTruong = x.CongTruong,
                         CreatedDate = x.CreatedDate,
                     });
            r = r.OrderBy(x => x.Id);
            var result = r.ToList();
            return result;
        }

    }
}