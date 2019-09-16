using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RICONS.Web.Models
{
    public class AddStaffModels
    {
        public string stt { get; set; }
        public int mabosungnhansu { get; set; }
        public string mahoso { get; set; }
        public string maphongban { get; set; }
        public string tenduan { get; set; }
        public string phongban_congtruong { get; set; }
        public string goithau { get; set; }
        public string diachi { get; set; }
        public string ngayyeucau { get; set; }
        public int soluongnhansuhientai { get; set; }

        public int tongsonhansu { get; set; }

        public string congnghiep { get; set; }
        public string thuongmai { get; set; }
        public string dandung { get; set; }
        public string nghiduong { get; set; }
        public string hatang { get; set; }
        public string khac { get; set; }
        public string khac_noidung { get; set; }

        public string thuyenchuyennoibo { get; set; }
        public string tuyenmoi { get; set; }
        public string khongbosung { get; set; }

        public string truongbophan_cht { get; set; }
        public string truongbophan_cht_ngayky { get; set; }
        public string truongbophan_cht_duyet { get; set; }
        public string truongbophan_cht_email { get; set; }

        public string giamdocduan_ptgd { get; set; }
        public string giamdocduan_ptgd_ngayky { get; set; }
        public string giamdocduan_ptgd_duyet { get; set; }
        public string giamdocduan_ptgd_email { get; set; }

        public string phongqtnnl { get; set; }
        public string phongqtnnl_ngayky { get; set; }
        public string phongqtnnl_duyet { get; set; }
        public string phongqtnnl_email { get; set; }

        public string bantonggiamdoc { get; set; }
        public string bantonggiamdoc_ngayky { get; set; }
        public string bantonggiamdoc_duyet { get; set; }
        public string bantonggiamdoc_email { get; set; }

        public string ngaylapdanhsachdieudong { get; set; }
        public string nguoilap_dieudong { get; set; }
        public string nguoilapemail_dieudong { get; set; }
        public string bangiamdoc_dieudong { get; set; }
        public string bangiamdocemail_dieudong { get; set; }

        public string nguoilap_dieudong_duyet { get; set; }
        public string bangiamdoc_dieudong_duyet { get; set; }
        public string bangiamdoc_dieudong_ngayduyet { get; set; }

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }

    }

    public class ddns_tiendo_giatriModels
    {
        public string stt { get; set; }
        public int matiendo { get; set; }
        public int mabosungnhansu { get; set; }
        public string hangmucthicong { get; set; }
        public string giatri { get; set; }
        public string batdau { get; set; }
        public string ketthuc { get; set; }
        public string duphong { get; set; }
        public string ghichu { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class ddns_congtruong_kehoachbosungnhansuModels
    {
        public string stt { get; set; }
        public int makehoach { get; set; }
        public int mabosungnhansu { get; set; }
        public string vitricongtac { get; set; }

        public int soluong { get; set; }
        public string thoigianbosung { get; set; }

        public int soluong1 { get; set; }
        public string thoigianbosung1 { get; set; }

        public int soluong2 { get; set; }
        public string thoigianbosung2 { get; set; }

        public string ghichu { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class ddns_vanphong_kehoachbosungnhansuModels
    {
        public string stt { get; set; }
        public int makehoach { get; set; }
        public int mabosungnhansu { get; set; }
        public string vitricongtac { get; set; }
        public int soluong { get; set; }
        public string chuyenmon { get; set; }
        public string trinhdo { get; set; }
        public string thoigiantiepnhan { get; set; }
        public string tieuchuan_ghichu { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class ddns_lapdanhsachdieudongModels
    {
        public string stt { get; set; }
        public int madanhsach { get; set; }
        public int mabosungnhansu { get; set; }
        public string manhanvien { get; set; }
        public string hovaten { get; set; }
        public string noilamviec_cu { get; set; }
        public string chucdanh_cu { get; set; }
        public string noilamviec_moi { get; set; }
        public string chucdanh_moi { get; set; }
        public string ngaydieudongdukien { get; set; }
        public string ghichu { get; set; }
        public string vungmien { get; set; }
        public string chuyenracongtruong { get; set; }
        public string thongtinlienlac { get; set; }
        public string nhansuden_ct_pb { get; set; }
        public string dienthoai { get; set; }
        public string email { get; set; }

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }

    }

    public class ddns_lapdanhsach_dieudong_thongbaoModels
    {
        public string stt { get; set; }
        public int madanhsach { get; set; }
        public int mabosungnhansu { get; set; }
        public string manhanvien { get; set; }
        public string hovaten { get; set; }
        public string noilamviec_cu { get; set; }
        public string chucdanh_cu { get; set; }
        public string noilamviec_moi { get; set; }
        public string chucdanh_moi { get; set; }
        public string ngaydieudongdukien { get; set; }
        public string ghichu { get; set; }
        public string nhansuden_ct_pb { get; set; }
        public string nhansuden_ct_pb_ngayden { get; set; }

        public string nhansuden_ct_pb_ngaytra { get; set; }

        public string ngaysinh { get; set; }
        public string trinhdo { get; set; }
        public string kinhnghiem { get; set; }
        public string thamnien { get; set; }
        public string dienthoai { get; set; }
        public string email { get; set; }

        public string chuyenracongtruong { get; set; }
        public string thongtinlienlac { get; set; }

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }

    }



    public class ddns_paystaffModels
    {
        public string stt { get; set; }
        public int matranhansu { get; set; }
        public string mahoso { get; set; }
        public string mabosungnhansu { get; set; }

        public string maphongban { get; set; }
        public string tenduan { get; set; }
        public string phongban_congtruong { get; set; }
        public string goithau { get; set; }
        public string diachi { get; set; }
        public string ngayyeucau { get; set; }
        public int soluongnhansuhientai { get; set; }

        public int tongsonhansu { get; set; }

        public string congnghiep { get; set; }
        public string thuongmai { get; set; }
        public string dandung { get; set; }
        public string nghiduong { get; set; }
        public string hatang { get; set; }
        public string khac { get; set; }
        public string khac_noidung { get; set; }

        public string tiendovagiatrithang { get; set; }
        public string ngaykhoicong { get; set; }
        public string ngayhoanthanh { get; set; }


        public string dieudongdiduankhac { get; set; }
        public string dieudongvephongban { get; set; }
        public string dexuatchonghiviec { get; set; }


        public string truongbophan_cht { get; set; }
        public string truongbophan_cht_ngayky { get; set; }
        public string truongbophan_cht_duyet { get; set; }
        public string truongbophan_cht_email { get; set; }

        public string giamdocduan_ptgd { get; set; }
        public string giamdocduan_ptgd_ngayky { get; set; }
        public string giamdocduan_ptgd_duyet { get; set; }
        public string giamdocduan_ptgd_email { get; set; }

        public string phongqtnnl { get; set; }
        public string phongqtnnl_ngayky { get; set; }
        public string phongqtnnl_duyet { get; set; }
        public string phongqtnnl_email { get; set; }

      

        //public string ngaylapdanhsachdieudong { get; set; }
        //public string nguoilap_dieudong { get; set; }
        //public string nguoilapemail_dieudong { get; set; }
        //public string bangiamdoc_dieudong { get; set; }
        //public string bangiamdocemail_dieudong { get; set; }

        public string nguoilap_dieudong_duyet { get; set; }
        public string bangiamdoc_dieudong_duyet { get; set; }
        public string bangiamdoc_dieudong_ngayduyet { get; set; }

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }

    }

    public class ddns_paystaff_congtruong_kehoachbosungnhansuModels
    {
        public string stt { get; set; }
        public int makehoach { get; set; }
        public int matranhansu { get; set; }
        public string vitricongtac { get; set; }

        public int soluong { get; set; }
        public string thoigianchuyentra { get; set; }

        public int soluong1 { get; set; }
        public string thoigianchuyentra1 { get; set; }

        public int soluong2 { get; set; }
        public string thoigianchuyentra2 { get; set; }

        public string ghichu { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }

    }

    public class ddns_paystaff_vanphong_kehoachbosungnhansuModels
    {
        public string stt { get; set; }
        public int makehoach { get; set; }
        public int matranhansu { get; set; }
        public string manhanvien { get; set; }
        public string hovaten { get; set; }
        public string trinhdo { get; set; }
        public string chuyenmon { get; set; }
        public string vitricongviec { get; set; }
        public string thoigianchuyentra { get; set; }
        public string lydochuyentra { get; set; }
        public string ghichu { get; set; }
    
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }

    }




    


}