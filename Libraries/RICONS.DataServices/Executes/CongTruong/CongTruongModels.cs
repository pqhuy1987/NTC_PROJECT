using RICONS.DataServices.Context;

namespace RICONS.DataServices.Executes.CongTruong
{
    public class PhongBanViewModel : m_donvi_phongban
    {
    }

    public class SearchCongTruongModel
    {
        public int? GiamDoc { get; set; }
        public string CongTruong { get; set; }
        public string EmailThuKy { get; set; }
    }
}
