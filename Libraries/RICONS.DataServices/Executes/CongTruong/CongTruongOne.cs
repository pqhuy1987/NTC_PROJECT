using System.Linq;
using RICONS.DataServices.Context;

namespace RICONS.DataServices.Executes
{
    public partial class DataService
    {
        public m_donvi_phongban PhongBanOne(string id)
        {
            CheckDbConnect();
            var query = (from x in Context.m_donvi_phongban
                where x.maphongban == id
                select x).FirstOrDefault();
            return query;
        }
    }
}