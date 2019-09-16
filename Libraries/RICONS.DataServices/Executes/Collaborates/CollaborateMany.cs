using System;
using System.Collections.Generic;
using System.Linq;
using RICONS.DataServices.Context;
using RICONS.DataServices.Executes.Collaborates;
using RICONS.Services.Variables;

namespace RICONS.DataServices.Executes
{
    public partial class DataService
    {
        public QueryResult<CollaborateViewModel> CollaborateMany(SearchCollaborateModel model, OptionResult option)
        {
            CheckDbConnect();
            IQueryable<Collaborate> query = Context.Collaborates.Where(x => x.Status >= 0);

            if (model.NguoiDi.HasValue)
            {
                query = query.Where(x => x.NguoiDi == model.NguoiDi.Value);
            }

            if (model.TuNgay.HasValue)
            {
                query = query.Where(x => x.TuNgay >= model.TuNgay.Value);
            }

            if (model.DenNgay.HasValue)
            {
                query = query.Where(x => x.DenNgay <= model.DenNgay.Value);
            }

            var r = (from x in query
                     join t in Context.m_taikhoan on x.NguoiDi equals t.mataikhoan
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
                         ObjTaiKhoan = t
                     });
            r = r.OrderBy(x => x.Id);

            var result = new QueryResult<CollaborateViewModel>(r, option);
            foreach (var item in result.Many)
            {
                if (!string.IsNullOrEmpty(item.NoiCongTac))
                {
                    var ids = item.NoiCongTac.Split(',').ToList();
                    item.Orgs = Context.m_donvi_phongban.Where(x => ids.Contains(x.maphongban)).ToList();
                }
            }
            return result;
        }
    }
}
