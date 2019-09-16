using System;
using System.Collections.Generic;
using System.Linq;
using RICONS.DataServices.Context;
using RICONS.DataServices.Executes.CongTruong;

namespace RICONS.DataServices.Executes
{
    public partial class DataService
    {
        public List<m_donvi_thongtingiamdoc> GetListGiamDocDuAn()
        {
            CheckDbConnect();
            var query = (from x in Context.m_donvi_phongban
                         join g in Context.m_donvi_thongtingiamdoc on x.thuocquanly equals g.mathongtin
                         where x.phongban_congtruong == 1
                         select g).Distinct().ToList();
            return query;
        }
        public List<m_donvi_phongban> GetListCongTruong(SearchCongTruongModel model)
        {
            CheckDbConnect();
            IQueryable<m_donvi_phongban> query = Context.m_donvi_phongban.Where(x => x.phongban_congtruong == 1);

            if (!string.IsNullOrEmpty(model.CongTruong))
            {
                query = query.Where(x => x.maphongban == model.CongTruong || x.sodienthoai.Trim().ToLower()==model.EmailThuKy.ToLower());
            }
            if (model.GiamDoc.HasValue)
            {
                int a=query.Count();

                var kq = query.ToList();

                query = query.Where(x => x.thuocquanly == model.GiamDoc.Value || x.thuocquanly1 == model.GiamDoc.Value || x.thuocquanly2 == model.GiamDoc.Value);
            }
            var result = query.ToList();
            return result;
        }

        public List<m_donvi_phongban> GetListAllPhongbanCongtruongbyID(string maphongban)
        {
            CheckDbConnect();
            IQueryable<m_donvi_phongban> query = Context.m_donvi_phongban.Where(x => x.maphongban == maphongban);
            var result = query.ToList();
            return result;
        }

    }
}