using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RICONS.Web.Models
{

    public class KpiLevelCompanyModels
    {
        public string stt { get; set; }
        public int makpicongty { get; set; }
        public string nam { get; set; }
        public string bophansoanthao { get; set; }
        public string ngaybanhanh { get; set; }
        public string ngaycapnhat { get; set; }
        public string lancapnhat { get; set; }

        public string nguoilapkpi { get; set; }
        public string nguoilapkpi_dagui { get; set; }
        public string nguoilapkpi_dagui_text { get; set; }
        public string nguoilapkpi_ngay { get; set; }

        public string photongxemxetkpi { get; set; }
        public string photongxemxetkpi_duyet { get; set; }
        public string photongxemxetkpi_duyet_text { get; set; }
        public string photongxemxetkpi_ngay { get; set; }

        public string tonggiamdocxemxetkpi { get; set; }
        public string tonggiamdocxemxetkpi_duyet { get; set; }
        public string tonggiamdocxemxetkpi_duyet_text { get; set; }
        public string tonggiamdocxemxetkpi_ngay { get; set; }

        public string nguoilapkpi_email { get; set; }
        public string photongxemxetkpi_email { get; set; }
        public string tonggiamdocxemxetkpi_email { get; set; } 

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class KpiLevelCompanyDetailModels
    {
        public int makpicongty_chitiet { get; set; }
        public int makpicongty { get; set; }
        public string stt { get; set; }
        public string muctieu { get; set; }
        public string trongso { get; set; }
        public string tieuchidanhgia { get; set; }
        public string cachtinh { get; set; }
        public string ngayghinhanketqua { get; set; }
        public string nguonchungminh { get; set; }
        public string nguonchungminh1 { get; set; }
        public string donvitinh { get; set; }
        public string kehoach { get; set; }
        public string thuchien { get; set; }
        public string tilehoanthanh { get; set; }
        public string ketqua { get; set; }
        public string ghichu { get; set; }
        public string thuoccap { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
        public string congty { get; set; }
    }


    public class KpiLevelDepartmentModels
    {
        public string stt { get; set; }
        public int makpiphongban { get; set; }
        public string bophansoanthao { get; set; }
        public string maphongban { get; set; }
        public string tenphongban { get; set; }
        public string ngaybanhanh { get; set; }
        public string ngaycapnhat { get; set; }
        public string lancapnhat { get; set; }

        public string nguoilapkpi { get; set; }
        public string nguoilapkpi_dagui { get; set; }
        public string nguoilapkpi_dagui_text { get; set; }
        public string nguoilapkpi_ngay { get; set; }

        public string photongxemxetkpi { get; set; }
        public string photongxemxetkpi_duyet { get; set; }
        public string photongxemxetkpi_duyet_text { get; set; }
        public string photongxemxetkpi_ngay { get; set; }

        public string tonggiamdocxemxetkpi { get; set; }
        public string tonggiamdocxemxetkpi_duyet { get; set; }
        public string tonggiamdocxemxetkpi_duyet_text { get; set; }
        public string tonggiamdocxemxetkpi_ngay { get; set; }

        public string nguoilapkpi_email { get; set; }
        public string photongxemxetkpi_email { get; set; }
        public string tonggiamdocxemxetkpi_email { get; set; }
        public string bankiemsoat_email { get; set; }
        public string bankiemsoat_duyet { get; set; }

        public string thoigianthicongtu { get; set; }
        public string thoigianthicongden { get; set; }
        public string hotengiamdocduan { get; set; }
        
        public string nam { get; set; }

        public string timkiemnam { get; set; }
        public string tinhtrangduyet { get; set; }

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class KpiLevelDepartmentDetailModels
    {
        public int makpiphongban_chitiet { get; set; }
        public int makpiphongban { get; set; }
        public string stt { get; set; }
        public string muctieu { get; set; }
        public string trongso { get; set; }
        public string tieuchidanhgia { get; set; }
        public string cachtinh { get; set; }
        public string ngayghinhanketqua { get; set; }
        public string nguonchungminh { get; set; }
        public string donvitinh { get; set; }
        public string kehoach { get; set; }
        public string thuchien { get; set; }
        public string tilehoanthanh { get; set; }
        public string ketqua { get; set; }
        public string ghichu { get; set; }
        public string thuoccap { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
        public string congty { get; set; }
        public string sttcon { get; set; }
        public string sttcha { get; set; }
    }



    public class KpiLevelDuDirectorTpModels
    {
        public string stt { get; set; }
        public int makpicanhancap { get; set; }
        public string maphongban { get; set; }
        public string tenphongban { get; set; }
        public string ngaybanhanh { get; set; }
        public string ngaycapnhat { get; set; }
        public string lancapnhat { get; set; }

        public string namkpi { get; set; }

        public string manhanvien { get; set; }
        public string hovaten { get; set; }
        public string chucdanh { get; set; }
        public string tenchucdanh { get; set; }
        public string captrentructiep { get; set; }
        public string ngaydangky { get; set; }
        public string ngaydanhgia { get; set; }

        public string nguoilapkpi { get; set; }
        public string nguoilapkpi_dagui { get; set; }
        public string nguoilapkpi_dagui_text { get; set; }
        public string nguoilapkpi_ngay { get; set; }

        public string photongxemxetkpi { get; set; }
        public string photongxemxetkpi_duyet { get; set; }
        public string photongxemxetkpi_duyet_text { get; set; }
        public string photongxemxetkpi_ngay { get; set; }

        public string bankiemsoat_email { get; set; }
        public string bankiemsoat_duyet { get; set; }
        public string nguoilapkpi_email { get; set; }
        public string photongxemxetkpi_email { get; set; }
        public string tonggiamdocxemxetkpi_email { get; set; }
        public string tonggiamdocxemxetkpi { get; set; }
        public string tonggiamdocxemxetkpi_duyet { get; set; }
        public string tonggiamdocxemxetkpi_ngay { get; set; }
        public string tonggiamdocxemxetkpi_duyet_text { get; set; }

        public string thongtintimkiem { get; set; }

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class KpiLevelDuDirectorTpDetailModels
    {
        public int makpicanhancap_chitiet { get; set; }
        public int makpicanhancap { get; set; }
        public string stt { get; set; }
        public string muctieu { get; set; }
        public string trongso { get; set; }
        public string tieuchidanhgia { get; set; }
        public string cachtinh { get; set; }
        public string ngayghinhanketqua { get; set; }
        public string nguonchungminh { get; set; }
        public string donvitinh { get; set; }
        public string kehoach { get; set; }
        public string thuchien { get; set; }
        public string tilehoanthanh { get; set; }
        public string ketqua { get; set; }
        public string ghichu { get; set; }
        public string thuoccap { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class KpiEmployeeModels
    {
        public string stt { get; set; }
        public int makpinhanvien { get; set; }
        public string maphongban { get; set; }
        public string tenphongban { get; set; }
        public string ngaybanhanh { get; set; }
        public string ngaycapnhat { get; set; }
        public string lancapnhat { get; set; }

        public string thangkpi { get; set; }
        public string namkpi { get; set; }

        public string manhanvien { get; set; }
        public string hovaten { get; set; }
        public string email { get; set; }
        public string chucdanh { get; set; }
        public string tenchucdanh { get; set; }
        public string captrentructiep { get; set; }
        public string ngaydangky { get; set; }
        public string ngaydanhgia { get; set; }

        public string nguoilapkpi { get; set; }
        public string nguoilapkpi_dagui { get; set; }
        public string nguoilapkpi_dagui_text { get; set; }
        public string nguoilapkpi_ngay { get; set; }

        public string truongphongxemxetkpi { get; set; }
        public string truongphongmanhanvien { get; set; }
        public string truongphongchucdanh { get; set; }
        public string truongphongemail { get; set; }

        public string truongphongxemxetkpi_duyet { get; set; }
        public string truongphongxemxetkpi_duyet_text { get; set; }
        public string truongphongxemxetkpi_ngay { get; set; }
        public string kpichinhsua { get; set; }
        public string khoaketqua { get; set; }

        public string trinhtranghoso { get; set; }
        
        

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class KpiEmployeeDetailModels
    {
        public int makpinhanvien_chitiet { get; set; }
        public int makpinhanvien { get; set; }
        public string stt { get; set; }
        public string muctieu { get; set; }
        public string trongso { get; set; }
        public string phuongphapdo { get; set; }
        public string nguonchungminh { get; set; }
        public string donvitinh { get; set; }
        public string kehoach { get; set; }
        public string thuchien { get; set; }
        public string tilehoanthanh { get; set; }
        public string ketqua { get; set; }
        public string kyxacnhan { get; set; }
        public string ghichu { get; set; }
        public string thuoccap { get; set; }
        public string kpichinhsua { get; set; }
        public string khoaketqua { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }


    public class KpiReport_MonthModels
    {
        public int matonghopketqua { get; set; }
        public string stt { get; set; }
        public string thangnam { get; set; }
        public string manhanvien { get; set; }
        public string hovaten { get; set; }
        public string chucdanh { get; set; }
        public string tenchucdanhns { get; set; }
        public string maphongban { get; set; }
        public string tenphongban { get; set; }
        public string hanhvithaido { get; set; }
        public string giaiquyetcongviec { get; set; }
        public string ketquakpi { get; set; }

        public string groupcha { get; set; }

        public string maphanloaiketqua { get; set; }
        public string phanloaiketqua { get; set; }
        public string ghichu { get; set; }
        public string xoa { get; set; }
        public string kiemtra { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }


    public class KpiReport_YearModels
    {
        public int matonghopketqua { get; set; }
        public string stt { get; set; }
        public string thangnam { get; set; }
        public string manhanvien { get; set; }
        public string hovaten { get; set; }
        public string chucdanh { get; set; }
        public string tenchucdanhns { get; set; }
        public string maphongban { get; set; }
        public string tenphongban { get; set; }
        public string ngaynhanviec { get; set; }

        public string thangkpi { get; set; }
        public string ketqua { get; set; }

        public string diemkpi01 { get; set; }
        public string xeploaikpi01 { get; set; }

        public string diemkpi02 { get; set; }
        public string xeploaikpi02 { get; set; }

        public string diemkpi03 { get; set; }
        public string xeploaikpi03 { get; set; }

        public string diemkpi04 { get; set; }
        public string xeploaikpi04 { get; set; }

        public string diemkpi05 { get; set; }
        public string xeploaikpi05 { get; set; }

        public string diemkpi06 { get; set; }
        public string xeploaikpi06 { get; set; }

        public string diemkpi07 { get; set; }
        public string xeploaikpi07 { get; set; }

        public string diemkpi08 { get; set; }
        public string xeploaikpi08 { get; set; }

        public string diemkpi09 { get; set; }
        public string xeploaikpi09 { get; set; }

        public string diemkpi10 { get; set; }
        public string xeploaikpi10 { get; set; }

        public string diemkpi11 { get; set; }
        public string xeploaikpi11 { get; set; }

        public string diemkpi12 { get; set; }
        public string xeploaikpi12 { get; set; }

        public string diemkpinam { get; set; }
        public string xeploaikpinam { get; set; }

        public string group { get; set; }
        
    }



}