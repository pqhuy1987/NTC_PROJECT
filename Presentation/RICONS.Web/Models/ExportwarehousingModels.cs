using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RICONS.Web.Models
{
    public class daotao_taolopModels
    {
        public string stt { get; set; }
        public int malop { get; set; }
        public string tenlop { get; set; }
        public string tenkhoahoc { get; set; }
        public string ngayhoc { get; set; }
        public string giobatdauhoc { get; set; }
        public string tongthoigianhoc { get; set; }
        public string tongchiphidaotao { get; set; }
        public string noihoc { get; set; }
        public string chongiaovien { get; set; }
        public string tengiaovien { get; set; }

        public string hovaten { get; set; }
        public string nguongiaovien { get; set; }
        public string sodienthoai { get; set; }
        public string malop_chuoi { get; set; }
        public string tenfile { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
        public string email { get; set; }
        public string tieudeguimail { get; set; }
        public string mailcc { get; set; }
        public string kinhgui { get; set; }
        public string baocao { get; set; }
        public string noidungguimail { get; set; }
        public string maphongban { get; set; }

    }

    public class daotao_taolop_chitietModels
    {
        public string stt { get; set; }
        public int malopchitiet { get; set; }
        public int malop { get; set; }
        public string manv { get; set; }
        public string hovaten { get; set; }
        public string ngaysinh { get; set; }
        public string email { get; set; }
        public string maphongban { get; set; }
        public string tenphongban { get; set; }
        public string diemdanh { get; set; }
        public string tendiemdanh { get; set; }
        public string uploadfile { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
        public string daguimail { get; set; }
        public string xacnhanthamgia { get; set; }
    }

    public class dm_tieuchisaodaotaoModels
    {
        public int matieuchi { get; set; }
        public string tentieuchi { get; set; }
        public string danhmuccha { get; set; }
        public string danhmucgoc { get; set; }
        public string noidungtext { get; set; }
        public string kem { get; set; }
        public string trungbinh { get; set; }
        public string kha { get; set; }
        public string tot { get; set; }
        public string rattot { get; set; }
    }

    public class daotao_ykiensaodaotaoModels
    {
        public int matiepnhan { get; set; }
        public string hovaten { get; set; }
        public string maphongban { get; set; }
        public string tenphongban { get; set; }
        public string tieudekhoahoc { get; set; }
        public string tengiaovien { get; set; }
        public string ngaydaotao { get; set; }
        public int malop { get; set; }
        public string tenlop { get; set; }
        public string manv { get; set; }
        public string email { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class daotao_ykiensaodaotao_chitietModels
    {
        public int matiepnhan_tieuchidaotao { get; set; }
        public int matiepnhan { get; set; }
        public int matieuchi { get; set; }
        public string tentieuchi { get; set; }
        public string kem { get; set; }
        public string trungbinh { get; set; }
        public string kha { get; set; }
        public string tot { get; set; }
        public string rattot { get; set; }
        public string danhmuccha { get; set; }
        public string danhmucgoc { get; set; }

        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

    public class WeedMeetingModels
    {
        public string stt { get; set; }
        public int macuochop { get; set; }
        public string matuan { get; set; }
        public string tentuan { get; set; }
        public string giohop { get; set; }
        public string ngayhop { get; set; }

        public string phonghop { get; set; }
        public string tenphonghop { get; set; }
        
        public string maphongban { get; set; }
        public string tenphongban { get; set; }

        public string loaicuochop { get; set; }
        public string tenloaicuochop { get; set; }

        public string lydobuoihop { get; set; }
        public string thanhphanthamdu { get; set; }
        public string caplanhdao { get; set; }
        public string noidungcuochop { get; set; }
        public string uploadfile { get; set; }
        public string tenfile { get; set; }
        public string xoa { get; set; }
        public int nguoitao { get; set; }
        public string ngaytao { get; set; }
        public int nguoihieuchinh { get; set; }
        public string ngayhieuchinh { get; set; }
    }

}