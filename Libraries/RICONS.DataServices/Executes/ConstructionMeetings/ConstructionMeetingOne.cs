using System;
using System.Collections.Generic;
using System.Linq;
using RICONS.DataServices.Executes.ConstructionMeetings;

namespace RICONS.DataServices.Executes
{
    public partial class DataService
    {
        public ConstructionMeetingViewModel ConstructionMeetingOne(int id)
        {
            CheckDbConnect();
            var result = (from x in Context.ConstructionMeetings
                          where x.Id == id
                          select new ConstructionMeetingViewModel()
                          {
                              Id = x.Id,
                             CreatedDate = x.CreatedDate,
                           GiamDoc = x.GiamDoc,
                              Date = x.Date,
                              LoaiCuocHop = x.LoaiCuocHop,
                              CongTruong = x.CongTruong,
                              FileDinhKem = x.FileDinhKem,
                              GioHop = x.GioHop,
                              ThanhPhanThamDu = x.ThanhPhanThamDu,
                              Status = x.Status
                          }).FirstOrDefault();

           return result;
        }

    }
}
