using System;
using System.Collections.Generic;
using System.Web;
using RICONS.DataServices.Context;

namespace RICONS.DataServices.Executes.Collaborates
{
    public class SearchCollaborateModel
    {
        public int? NguoiDi { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? Status { get; set; }

    }
    public class CollaborateViewModel : Collaborate
    {
        public m_taikhoan ObjTaiKhoan { get; set; }
        public List<m_donvi_phongban> Orgs { get; set; }
    }
    public class CollaborateEditModel : Collaborate
    {
        public bool SendMail { get; set; }
    }
}
