using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RICONS.Web.Models
{
    public class WarehousingModels
    {
        public string stt { get; set; }
        public int manhapkho { get; set; }
        public string sochungtu  { get; set; }
        public string sohoadon { get; set; }
        public string ngaynhapchungtu { get; set; }
        public int nhacungcap { get; set; }
        public string tennhacungcap { get; set; }
        public string tongtien { get; set; }
        public string noidungnhaphang { get; set; }//chi load la nhanvien hay truong bo phan la xong
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class Warehousing_detailModels
    {
        public string stt { get; set; }
        public int machitiet { get; set; }
        public int manhapkho { get; set; }
        public int mavanphongpham { get; set; }
        public string tenvanphongpham { get; set; }
        public string donvitinh { get; set; }
        public string dongia { get; set; }
        public string soluong { get; set; }
        public string thanhtien { get; set; }
        public string danhmuccha { get; set; }
        public string danhmucgoc { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class Warehousing_detail1Models
    {
        public int machitiet { get; set; }
        public int manhapkho { get; set; }
        public int mavanphongpham { get; set; }
        public int soluong { get; set; }
        public int slthucxuat { get; set; }
      
    }







}