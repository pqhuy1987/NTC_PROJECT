using System;
using System.Linq;
using RICONS.DataServices.Context;
using RICONS.DataServices.Executes.Collaborates;

namespace RICONS.DataServices.Executes
{
    public partial class DataService
    {
        public Collaborate CollaborateCommand(CollaborateEditModel model)
        {
            CheckDbConnect();
            var d = Context.Collaborates.FirstOrDefault(x => x.Id == model.Id) ?? new Collaborate()
            {
                Id = 0,
                CreatedDate = DateTime.Now
            };
            d.EmailNguoiDuyet = model.EmailNguoiDuyet;
            d.NguoiDi = model.NguoiDi;
            d.TuNgay = model.TuNgay;
            d.DenNgay = model.DenNgay;
            d.NoiCongTac = model.NoiCongTac;
            d.TieuDe = model.TieuDe;
            d.MoTa = model.MoTa;
            d.DaDuyet = model.DaDuyet;
            d.Status = model.Status;
            if (d.Id == 0)
            {
                Context.Collaborates.Add(d);
            }
            
            Context.SaveChanges();
            return d;
        }

        public bool ApprovalCollaborate(int id, int status)
        {
           CheckDbConnect();
            var c = Context.Collaborates.FirstOrDefault(x => x.Id == id);
            if (c != null)
            {
                c.Status = status;
                c.NgayDuyet = DateTime.Now;
                Context.SaveChanges();
            }

            return true;
        }


        public bool DeleteCollaborate(int id)
        {
            CheckDbConnect();
            var d = Context.Collaborates.FirstOrDefault(x => x.Id == id);
            if (d != null)
            {
                d.Status = -1;
                Context.SaveChanges();
            }
            return true;
        }

    }
}
