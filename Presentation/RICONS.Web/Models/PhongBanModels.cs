using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RICONS.Web.Models
{
    public class PhongBanModels
    {
        public string stt { get; set; }
        public string madonvi { get; set; }
        public string tendonvi { get; set; }
        public string maphongban { get; set; }
        public string tenphongban { get; set; }
        public string sodienthoai { get; set; }

        public string hovaten { get; set; }
        public string email { get; set; }
     
        public string thuocquanly { get; set; }
        public string hotenquanly { get; set; } //
        public string ghichu { get; set; } //email quản lý

        public string thuocquanly1 { get; set; }
        public string hotenquanly1 { get; set; } //
        public string ghichu1 { get; set; } //email quản lý

        public string thuocquanly2 { get; set; }
        public string hotenquanly2 { get; set; } //
        public string ghichu2 { get; set; } //email quản lý

        public string cv_thietbi { get; set; }
        public string gs_thietbi { get; set; } //

        public string cv_hsse { get; set; }
        public string gs_hsse { get; set; } //

        public string cv_qaqc { get; set; }
        public string gs_qaqc { get; set; } //

        public string cv_mep { get; set; }
        public string gs_mep { get; set; } //

        public string xoa { get; set; }
      
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
        public string phongban_congtruong { get; set; }




    }

    public class Item_weedModels
    {
        public string stt { get; set; }
        public string matuan { get; set; }
        public string tentuan { get; set; }
        public string tungay { get; set; }
        public string denngay { get; set; }
        public string thang { get; set; }
        public string nam { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }




    public class daotao_dm_giaovienModels
    {
        public string stt { get; set; }
        public int magiaovien { get; set; }
        public string hovaten { get; set; }
        public string sodienthoai { get; set; }
        public string nguongiaovien { get; set; }
        public string tennguongiaovien { get; set; }
        public string sodienthoai1 { get; set; }
        public string email { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class thongtinnhanvienModels
    {
        public string stt { get; set; }
        public string manhanvien { get; set; }
        public string hoten { get; set; }
        public string maphongban { get; set; }
        public string chucdanhkpi { get; set; }
        public string tenchucdanhkpi { get; set; }
        public string machucdanh { get; set; }
        public string captrentructiep { get; set; }
        public string captrentructiep_email { get; set; }

        public string mailnhanvien { get; set; }
        public string mailtruongphong { get; set; }
    }

    public class thongtingiamdocModels
    {
        public string stt { get; set; }
        public string mathongtin { get; set; }
        public string hovaten { get; set; }
        public string email { get; set; }
        public string xoa { get; set; }
    }

}