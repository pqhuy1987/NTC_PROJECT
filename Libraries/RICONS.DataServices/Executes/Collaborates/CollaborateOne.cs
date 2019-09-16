using System;
using System.Collections.Generic;
using System.Linq;
using RICONS.DataServices.Executes.Collaborates;

namespace RICONS.DataServices.Executes
{
    public partial class DataService
    {
        public CollaborateViewModel CollaborateOne(int id)
        {
            CheckDbConnect();
            var result = (from x in Context.Collaborates
                          where x.Id == id
                          select new CollaborateViewModel()
                          {
                              Id = x.Id,
                              NguoiDi = x.NguoiDi,
                              TieuDe = x.TieuDe,
                              TuNgay = x.TuNgay,
                              DenNgay = x.DenNgay,
                              NoiCongTac = x.NoiCongTac,
                              MoTa = x.MoTa,
                              EmailNguoiDuyet = x.EmailNguoiDuyet,
                              DaDuyet = x.DaDuyet,
                              CreatedDate = x.CreatedDate,
                              Status = x.Status,

                          }).FirstOrDefault();

            return result;
        }

    }
}
