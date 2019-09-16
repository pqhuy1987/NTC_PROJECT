using System;
using System.Linq;
using RICONS.DataServices.Context;
using RICONS.DataServices.Executes.ConstructionMeetings;

namespace RICONS.DataServices.Executes
{
    public partial class DataService
    {
        public ConstructionMeeting ConstructionMeetingCommand(ConstructionMeetingEditModel model)
        {
            CheckDbConnect();
            var d = Context.ConstructionMeetings.FirstOrDefault(x => x.Id == model.Id) ?? new ConstructionMeeting()
            {
                Id = 0,
                CreatedDate = DateTime.Now
            };
            d.Date = model.Date;
            d.GioHop = model.GioHop;
            d.LoaiCuocHop = model.LoaiCuocHop;
            d.ThanhPhanThamDu = model.ThanhPhanThamDu;
            d.FileDinhKem = model.FileDinhKem;
            d.GiamDoc = model.GiamDoc;
            d.CongTruong = model.CongTruong;
            if (d.Id == 0)
            {
                Context.ConstructionMeetings.Add(d);
            }
            
            Context.SaveChanges();
            return d;
        }

       
        public bool DeleteConstructionMeeting(int id)
        {
            CheckDbConnect();
            var d = Context.ConstructionMeetings.FirstOrDefault(x => x.Id == id);
            if (d != null)
            {
                d.Status = -1;
                Context.SaveChanges();
            }
            return true;
        }

    }
}
