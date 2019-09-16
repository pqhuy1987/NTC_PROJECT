﻿using ClosedXML.Excel;
using RICONS.Core.Functions;
using RICONS.Logger;
using RICONS.Web.Data.Services;
using RICONS.Web.Models;
using Nop.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace RICONS.Web.Controllers
{
    public class AbsentController : BaseController
    {
        Log4Net _logger = new Log4Net(typeof(AbsentController));

        public static string MaDangKyNP = "";
        public ActionResult Vacationlist()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            foreach (var item in lstResult_phongban)
            {
                sbphongban.Append(string.Format("<option value='{0}'>{1}</option>", item.maphongban, item.tenphongban));
            }
            ViewBag.sbphongban = sbphongban.ToString();
            return View();
        }

        public static int maphongbanexcel = 0;

        [HttpPost]
        public JsonResult SelectRows_Danhsachnghiphep(AbsentModels model, string curentPage)
        {
            AbsentModels param = new AbsentModels();
            AbsentServices service = new AbsentServices();
            int loginid = 0;
            if (Session["userid"].ToString().Trim() != "1")
            {
                loginid = int.Parse(Session["userid"].ToString());
            }
            param.nguoitao = loginid;
            param.maphongban = model.maphongban;
            maphongbanexcel = model.maphongban;
            int tongsodong = service.CountRows(param);
            int trang=1;
            try
            {
                trang = int.Parse(curentPage);
                trang = trang + 1;
            }
            catch (Exception) { trang = 1; }
            
            int sotrang = 1;
            if (tongsodong > 20)
            {
                if (tongsodong % 20 > 0)
                {
                    sotrang = (tongsodong / 20) + 1;
                }
                else
                {
                    sotrang = (tongsodong / 20);
                }
            }

            int trangbd = 1; int trangkt = 20;
            if (trang != 1 && trang <= sotrang)
            {
                trangbd = (trangkt * (trang - 1)) + 1;
                trangkt = trangkt * trang;
            }
            
            List<AbsentModels> lstResult=new List<AbsentModels>();
            if (trang <= sotrang)
            {
                lstResult = service.SelectRows_Danhsachnghiphep(param, trangbd, trangkt);
            }
            else if (trang != 1 && trang > sotrang) trang = trang - 1;
            foreach (var list in lstResult)
            {
                string mang = "";
                if (list.nghiphep.Trim() == "1")
                    mang = mang + "Nghỉ phép"+", ";
                if (list.nghiom.Trim() == "1")
                    mang = mang + "Nghỉ ốm"+", ";
                if (list.nghithaisan.Trim() == "1")
                    mang = mang + "Nghỉ thai sản"+", ";
                if (list.nghikhongluong.Trim() == "1")
                    mang = mang + "Nghỉ không lương" + ", ";
                if (list.nghiviecrieng.Trim() == "1")
                    mang = mang + "Nghỉ việc riêng có lương" + ", ";
                if (list.nghikhac.Trim() == "1")
                    mang = mang + "Nghỉ khác" + ", ";
                if(mang.Length>0)
                {
                    mang = mang.Trim().Remove(mang.Trim().Length - 1);
                }
                list.loaiphepnam = mang;

                if (list.duyetcap1 == "0")
                    list.duyetcap1ten = "Chờ duyệt";
                else if (list.duyetcap1 == "1")
                    list.duyetcap1ten = "Đã duyệt";
                else if (list.duyetcap1 == "2")
                    list.duyetcap1ten = "Không duyệt";

                if (list.duyetcap2 == "0")
                    list.duyetcap2ten = "Chờ duyệt";
                else if (list.duyetcap2 == "1")
                    list.duyetcap2ten = "Đã duyệt";
                else if (list.duyetcap2 == "2")
                    list.duyetcap2ten = "Không duyệt";

                if (list.xacnhantruockhivaolamlai == "0")
                    list.xacnhantruockhivaolamlaiten = "No";
                else
                    list.xacnhantruockhivaolamlaiten = "Yes";

            }
            
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();

            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = trangbd;
                foreach (var item in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_Danhsachnghiphep(item, strSTT));
                    i++;
                }
                if (sbRows.Length > 0)
                    sbRows.Remove(sbRows.Length - 1, 1);
            }
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");

            sbResult.Append("\"curentPage\":\"" + "" + trang + "" + "\",");

            sbResult.Append("\"Pages\":\"" + "" + sotrang + "" + "\",");

            if (trang>1 && lstResult.Count > 0)
            {
                sbResult.Append("\"SubRow\":\"" + "true" + "\",");
                sbResult.Append("\"RowID\":\"" + model.manghiphep + "\",");
            }
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");

            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_Danhsachnghiphep(AbsentModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.manghiphep.ToString(), function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + AES.EncryptText(model.manghiphep.ToString(), function.ReadXMLGetKeyEncrypt()) + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' mdk='" + strEncryptCode + "' duyet='" + model.duyetcap1 + "' />", model.manghiphep);
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + strHTML_Checkbox + "\"");
                sbResult.Append("},");
                //stt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col2 stt\",");
                sbResult.Append("\"col_id\":\"2\",");
                sbResult.Append("\"col_value\":\"" + couter + "\"");
                sbResult.Append("},");

                //Mã đơn vị
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col3\",");
                sbResult.Append("\"col_id\":\"3\",");
                sbResult.Append("\"col_value\":\"" + model.manghiphep + "\"");
                sbResult.Append("},");

                //Mã phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + model.hovaten + "\"");
                sbResult.Append("},");


                //Tên đon vi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                //sbResult.Append("\"title\":\"" + model.madonvi + "\",");
                sbResult.Append("\"col_value\":\"" + model.ngaysinh + "\"");
                sbResult.Append("},");

                ////Tên phòng ban
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col6\",");
                //sbResult.Append("\"col_id\":\"6\",");
                //sbResult.Append("\"title\":\"" + model.machucdanh + "\",");
                //sbResult.Append("\"col_value\":\"" + model.tenchucdanh + "\"");
                //sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                sbResult.Append("\"col_value\":\"" + model.tenphongban + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");

                sbResult.Append("\"col_value\":\"" + model.ngayxinnghitu + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + model.songayxinnghi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                sbResult.Append("\"title\":\"" + model.machucdanh + "\",");
                sbResult.Append("\"col_value\":\"" + model.ngayxinnghiden + "\"");
                sbResult.Append("},");

                ////Tên phòng ban
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col11\",");
                //sbResult.Append("\"col_id\":\"11\",");
                //sbResult.Append("\"col_value\":\"" + model.songayphepconlai + "\"");
                //sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col12\",");
                sbResult.Append("\"col_id\":\"12\",");
                sbResult.Append("\"col_value\":\"" + model.lydoxinnghi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col13\",");
                sbResult.Append("\"col_id\":\"13\",");
                sbResult.Append("\"col_value\":\"" + model.loaiphepnam + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col14\",");
                sbResult.Append("\"col_id\":\"14\",");
                sbResult.Append("\"title\":\"" + model.duyetcap1 + "\",");
                sbResult.Append("\"col_value\":\"" + model.duyetcap1ten + "\""); 
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col15\",");
                sbResult.Append("\"col_id\":\"15\",");
                sbResult.Append("\"col_value\":\"" + model.duyetcap2ten + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col16\",");
                sbResult.Append("\"col_id\":\"16\",");
                sbResult.Append("\"col_value\":\"" + model.xacnhantruockhivaolamlaiten + "\"");
                sbResult.Append("},");

                //Ghi chú
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col17\",");
                sbResult.Append("\"col_id\":\"17\",");
                sbResult.Append("\"col_value\":\"''\"");
                sbResult.Append("}");

                ////dinh kem tap tin
                //string strHTML_Attachment = "";
                //#region
                ////string link = Url.Action("Edit", "Account", new { id = EncDec.EncodeCrypto(model.mataikhoan) });
                //strHTML_Attachment = "<a href='#' class='edit' ><i class='fa fa-pencil-square-o' ></i></a>&nbsp;<a href='#' class='del'><i class='fa fa-trash-o' ></i></a>";
                //#endregion
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col8\",");
                //sbResult.Append("\"col_id\":\"8\",");
                //sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                //sbResult.Append("}");

                #endregion
                sbResult.Append("]");
                sbResult.Append("},");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return sbResult;
        }

        public ActionResult Deleted(string manghiphep)
        {
            if (!IsLogged())
                return BackToLogin();
            if(manghiphep!=null)
            {
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                string manghiphepaaaa = AES.DecryptText(manghiphep, function.ReadXMLGetKeyEncrypt());
                AbsentServices service = new AbsentServices();
                bool madangky = service.DeletedRow_Dondangkynghiphep(manghiphepaaaa, Session["userid"].ToString());
                if (madangky)
                {
                    
                }
            }
            return RedirectToAction("Vacationlist");
        }

        //Thêm mới đơn đăng ký
        public ActionResult Registerforleave()
        {
            //check login
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            foreach (var item in lstResult_phongban)
            {
                sbphongban.Append(string.Format("<option value='{0}'>{1}</option>", item.maphongban, item.tenphongban));
            }
            ViewBag.sbphongban = sbphongban.ToString();

            AbsentServices service_nghiphep = new AbsentServices();
            AbsentModels param = new AbsentModels();

            int loginid = 0;
            if (Session["userid"].ToString().Trim() != "1")
            {
                loginid = int.Parse(Session["userid"].ToString());
            }
            param.nguoitao = loginid;

            List<AbsentModels> lstResult_songaynghi = service_nghiphep.SelectRows_Danhsachnghiphep_songayphep(param);
            double songayphep = double.Parse(DateTime.Now.Month.ToString("00"));
            foreach (var item in lstResult_songaynghi)
            {
                double songayxinnghi = double.Parse(item.songayxinnghi);
                if (songayphep >= 0)
                {
                    if (item.canhankh.Trim() == "1" && songayxinnghi >= 3)
                        songayphep = songayphep + 3 - songayxinnghi;

                    else if (item.conkh.Trim() == "1" && songayxinnghi >= 1)
                        songayphep = songayphep + 1 - songayxinnghi;

                    else if (item.chame_mat.Trim() == "1" && songayxinnghi >= 3)
                        songayphep = songayphep + 3 - songayxinnghi;

                    else if (item.ongba_mat.Trim() == "1")
                        songayphep = songayphep - songayxinnghi;

                    else if (item.nghiphep.Trim() == "1" && songayphep >= 1)
                        songayphep = songayphep - songayxinnghi;
                }
            }
            if (songayphep >= 1) songayphep = songayphep - 1;
            else songayphep = 0;
            List<AbsentModels> lstsongayphep = new List<AbsentModels>();
            param = new AbsentModels();
            param.songayphepconlai = songayphep.ToString();
            lstsongayphep.Insert(0, param);
            return View(lstsongayphep[0]);
        }

        [HttpPost]
        public ActionResult Registerforleave(AbsentModels model)
        {
            //check login
            if (!IsLogged())
                return BackToLogin();
            if (model.nghikhac == "on")
                model.nghikhac = "1";
            if (model.nghikhongluong == "on")
                model.nghikhongluong = "1";
            if (model.nghiom == "on")
                model.nghiom = "1";
            if (model.nghiphep == "on")
                model.nghiphep = "1";
            if (model.nghithaisan == "on")
                model.nghithaisan = "1";
            if (model.nghiviecrieng == "on")
                model.nghiviecrieng = "1";
            string aaa = model.ngaysinh;
            if (model.thangsinh != null)
                model.ngaysinh = model.ngaysinh + "/" + model.thangsinh.ToString();
            if (model.namsinh != null)
                model.ngaysinh = model.ngaysinh + "/" + model.namsinh.ToString();

            model.xoa = "0";

            model.nguoitao = int.Parse(Session["userid"].ToString());
            model.ngaytao = "GETDATE()";
            model.manghiphep = "";

            AbsentServices service = new AbsentServices();
            string madangky = service.Insert_Donxinnghiphep(model);
            #region Chucnang goi email
            DanhmucServices service_pb = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            parampb.maphongban = model.maphongban.ToString().Trim();
            List<PhongBanModels> lstResult_phongban = service_pb.SelectRows(parampb);

            if (lstResult_phongban.Count > 0)
            {
                string hotento = "";
                if (lstResult_phongban[0].hovaten.ToString() != "")
                    hotento = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(lstResult_phongban[0].hovaten.ToString().Trim().ToLower());
                if (lstResult_phongban[0].email.ToString().Trim() != "")
                {
                    #region
                    string mailto = lstResult_phongban[0].email.ToString().Trim();
                    model.tenphongban = lstResult_phongban[0].tenphongban.ToString().Trim();
                    string strEncryptCode = MaDangKyNP + "0070pXQSeNsQRuzoCmUYfuX/vA==6";
                    string mailcapquanly = "";
                    if (model.machucdanh == "1" && double.Parse(model.songayxinnghi) > 2)
                    {
                        if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "1")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("1"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "2")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("2"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "3")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("3"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "4")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("4"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "5")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("5"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "6")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("6"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "7")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("7"));

                        string[] chuoitach = mailcapquanly.Split('/');
                        strEncryptCode = strEncryptCode + "&emailto=" + chuoitach[0] + "&hotento=" + chuoitach[1];
                    }
                    else if (model.machucdanh == "2")
                    {
                        if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "1")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("1"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "2")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("2"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "3")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("3"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "4")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("4"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "5")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("5"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "6")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("6"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "7")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("7"));

                        string[] chuoicap1 = mailto.Split('/');
                        mailto = chuoicap1[0].Trim();
                        hotento = chuoicap1[1].Trim();

                        mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("1")); /// Goi cho tong giam doc
                        string[] chuoitach = mailcapquanly.Split('/');
                        strEncryptCode = strEncryptCode + "&emailto=" + chuoitach[0] + "&hotento=" + chuoitach[1];

                    }
                    #endregion

                    send_Mail(strEncryptCode, mailto, "Đơn xin nghỉ phép", "Nội dung", model, hotento);
                }
                return RedirectToAction("Vacationlist", "Absent");
            }
            #endregion
            return RedirectToAction("Vacationlist", "Absent");

        }

        public string send_Mail(string strEncryptCode, string mailto, string Subject, string Contents, AbsentModels model, string hotento)
        {
            _logger.Start("send_Mail");
            try
            {

                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname"));
                strEncryptCode = linkname.Trim() + strEncryptCode + "&mailcanhan=" + mailto;
                if ((model.machucdanh == "1" && double.Parse(model.songayxinnghi) > 2) || model.machucdanh == "2")
                {
                    strEncryptCode = strEncryptCode + "&hovaten=" + model.hovaten + "&tenphongban=" + model.tenphongban
                    + "&songayxinnghi=" + model.songayxinnghi + "&ngayxinnghitu=" + model.ngayxinnghitu + "&ngayxinnghiden=" + model.ngayxinnghiden
                    + "&lydoxinnghi=" + model.lydoxinnghi + "&sodienthoai=" + model.sodienthoai + "&mailcanhan=" + model.email;
                }

                //string smtp_port = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_port"));
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));
               


                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 20px; padding: 0; width: 650px; font-size: 1em;color:black;'>"); //margin: 0 auto;  de canh giua

                sb.Append("<table cellpadding='0' cellspacing='0' width='650px' >");
                sb.Append("<tbody>");
                sb.Append("<tr>");
                sb.Append("<td height='76px' width='650px' >");
                sb.Append("<table cellpadding='0' cellspacing='0' width='100%'>");

                sb.Append("<tbody>");
                sb.Append("<tr>");
                sb.Append("<td>");

                sb.Append("<div style='width:150px;float:left;height :45px; line-height:45px; padding-top:10px;'>");
                sb.Append("<img src='http://i.imgur.com/yKqNNy2.png'  alt='ddd' style='width:100px; height:45px;'/>");
                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<div style='width:400px; text-align:center;font-weight: bold;float:left;line-height:45px'>");
                sb.Append("<p style= 'width:400px;text-align:center;font-size:18px;font-weight:bold;line-height:45px;padding-left:80px;float:left;'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ĐƠN XIN NGHỈ PHÉP</p>");
                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("</tr>");
                sb.Append("</tbody>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</tbody>");
                sb.Append("</table>");
                sb.Append("<hr style=border: 1px solid #000; width: 100% />");

                sb.Append("<table style='width:650px; font-size:14px;'>");
                sb.Append("<tr><td style='padding-left:10px;'><p><strong><em><u>Kính gửi Anh/Chị:</u></em>&nbsp;" + hotento + "</strong></p></td></tr>");

                sb.Append("<tr><td style='float:left;height :22px; line-height:22px; padding-left:10px;'>" + "Họ và tên:             " + model.hovaten + "" + "</td></tr>");
                sb.Append("<tr><td style='float:left;height :22px; line-height:22px; padding-left:10px;'>" + "Công tác tại:          " + model.tenphongban + "" + "</td></tr>");
                sb.Append("<tr><td style='float:left;height :22px; line-height:22px; padding-left:10px;'>" + "Số ngày xin nghỉ:      " + model.songayxinnghi + "" + "</td></tr>");
                sb.Append("<tr><td style='float:left;height :22px; line-height:22px; padding-left:10px;'>" + "Từ ngày:               " + model.ngayxinnghitu + " đến ngày: " + model.ngayxinnghiden + "" + "</td></tr>");
                sb.Append("<tr><td style='float:left;height :22px; line-height:22px; padding-left:10px;'>" + "Lý do xin nghỉ:        " + model.lydoxinnghi + "" + "</td></tr>");
                sb.Append("<tr><td style='float:left;height :22px; line-height:22px; padding-left:10px;'>" + "Số điện thoại:         " + model.sodienthoai + "" + "</td></tr>");
                sb.Append("<tr><td style='float:left;height :22px; line-height:22px; padding-left:10px;'>" + "Email:                 " + model.email + "" + "</td></tr>");  //style='background-color:blue;color:white'
                sb.Append("<tr><td style='float:left; padding-left:10px; padding-top:10px;'><a href='" + strEncryptCode + "&dongy=1'> Đồng ý</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dongy=3'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");
                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user, mailto.Trim());
                message.From = new MailAddress(mailto.Trim(), Subject, System.Text.Encoding.UTF8);
                message.Subject = Subject;
                message.Body = sb.ToString();
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;
                SmtpClient client = new SmtpClient(smtp_host);
                client.UseDefaultCredentials = true;
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                                ex.ToString());
                }
                return "Successfull!";
            }
            catch (Exception ms)
            {
                _logger.Error(ms);
                return ms.Message;
            }
            _logger.End("send_Mail");
        }


        [HttpPost]
        public ActionResult Duyethosocap1(string manghiphep)
        {
            if (!IsLogged())
                return BackToLogin();
            return RedirectToAction("Vacationlist", "Absent");
        } 

        //Hiệu chỉnh đơn đăng ký
        public ActionResult Editforleave(string madangky)
        {
            //check login
            if (!IsLogged())
                return BackToLogin();

            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            foreach (var item in lstResult_phongban)
            {
                sbphongban.Append(string.Format("<option value='{0}'>{1}</option>", item.maphongban, item.tenphongban));
            }
            ViewBag.sbphongban = sbphongban.ToString();

            if (madangky != "0" && madangky != null)
            {
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                madangky = AES.DecryptText(madangky, function.ReadXMLGetKeyEncrypt());
                MaDangKyNP = madangky;
                AbsentModels param = new AbsentModels();
                param.manghiphep = madangky.Trim();
                AbsentServices service_dangky = new AbsentServices();
                List<AbsentModels> lstResult_dangky = service_dangky.SelectRows_Danhsachnghiphep_11111(param);
                if (lstResult_dangky.Count > 0) 
                {

                    if (lstResult_dangky[0].duyetcap1 == "1")
                    {
 
                    }

                    string[] chuoi = lstResult_dangky[0].ngaysinh.Split('/');
                    if (chuoi.Length == 3)
                    {
                        lstResult_dangky[0].ngaysinh = chuoi[0];
                        lstResult_dangky[0].thangsinh = chuoi[1];
                        lstResult_dangky[0].namsinh = chuoi[2];
                    }
                    else if (chuoi.Length == 2)
                    {
                        lstResult_dangky[0].thangsinh = chuoi[0];
                        lstResult_dangky[0].namsinh = chuoi[1];
                    }
                    else
                    {
                        lstResult_dangky[0].namsinh = chuoi[0];
                    }

                    if (lstResult_dangky.Count > 0)
                        return View(lstResult_dangky[0]);
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Editforleave(string madangky, AbsentModels model,string luu)
        {
            if (!IsLogged())
                return BackToLogin();

            if (model.nghiphep == "on") model.nghiphep = "1";
            else model.nghikhac = "0";

            if (model.nghikhongluong == "on") model.nghikhongluong = "1";
            else model.nghikhac = "0";

            if (model.nghithaisan == "on") model.nghithaisan = "1";
            else model.nghikhac = "0";

            if (model.conkh == "on") model.conkh = "1";
            else model.conkh = "0";

            if (model.canhankh == "on") model.canhankh = "1";
            else model.canhankh = "0";

            if (model.chame_mat == "on") model.chame_mat = "1";
            else model.chame_mat = "0";

            if (model.ongba_mat == "on") model.ongba_mat = "1";
            else model.ongba_mat = "0";

            if (model.thangsinh.ToString() != "")
                model.ngaysinh = model.ngaysinh + "/" + model.thangsinh.ToString();
            if (model.namsinh.ToString() != "")
                model.ngaysinh = model.ngaysinh + "/" + model.namsinh.ToString();

            model.xoa = "0";
            model.nguoitao = int.Parse(Session["userid"].ToString());
            model.ngaytao = "GETDATE()";
            model.manghiphep = MaDangKyNP;
            AbsentServices service = new AbsentServices();
            model.manghiphep = service.Insert_Donxinnghiphep(model);

            DanhmucServices service_pb = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            parampb.maphongban = model.maphongban.ToString().Trim();
            List<PhongBanModels> lstResult_phongban = service_pb.SelectRows(parampb);

            if (lstResult_phongban.Count > 0)
            {
                string hotento = "";
                if (lstResult_phongban[0].hovaten.ToString() != "")
                    hotento = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(lstResult_phongban[0].hovaten.ToString().Trim().ToLower());
                if (lstResult_phongban[0].email.ToString().Trim() != "")
                {

                    #region
                    string mailto = lstResult_phongban[0].email.ToString().Trim();
                    model.tenphongban = lstResult_phongban[0].tenphongban.ToString().Trim();
                    string strEncryptCode = MaDangKyNP + "0070pXQSeNsQRuzoCmUYfuX/vA==6";
                    string mailcapquanly = "";
                    if (model.machucdanh == "1" && double.Parse(model.songayxinnghi) > 2)
                    {
                        if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "1")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("1"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "2")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("2"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "3")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("3"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "4")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("4"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "5")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("5"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "6")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("6"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "7")
                            mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("7"));

                        string[] chuoitach = mailcapquanly.Split('/');
                        strEncryptCode = strEncryptCode + "&emailto=" + chuoitach[0] + "&hotento=" + chuoitach[1];
                    }
                    else if (model.machucdanh == "2")
                    {
                        if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "1")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("1"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "2")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("2"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "3")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("3"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "4")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("4"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "5")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("5"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "6")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("6"));

                        else if (lstResult_phongban[0].thuocquanly.ToString().Trim() == "7")
                            mailto = string.Format(Functiontring.ReturnStringFormatEmail("7"));

                        string[] chuoicap1 = mailto.Split('/');
                        mailto = chuoicap1[0].Trim();
                        hotento = chuoicap1[1].Trim();

                        mailcapquanly = string.Format(Functiontring.ReturnStringFormatEmail("1")); /// Goi cho tong giam doc
                        string[] chuoitach = mailcapquanly.Split('/');
                        strEncryptCode = strEncryptCode + "&emailto=" + chuoitach[0] + "&hotento=" + chuoitach[1];

                    }
                    #endregion

                    send_Mail(strEncryptCode, mailto, "Đơn xin nghỉ phép", "Nội dung", model, hotento);
                }
                return RedirectToAction("Vacationlist", "Absent");
            }
            else
            { 
                return RedirectToAction("Editforleave"); 
            }
            

            //return View();
        }

        public ActionResult ExportLicensing()
        {
            //check login
            if (!IsLogged())
                return BackToLogin();

            List<AbsentModels> lstResult = new List<AbsentModels>();

            AbsentModels param = new AbsentModels();
            AbsentServices service = new AbsentServices();
            param.maphongban = maphongbanexcel;

            int loginid = 0;
            if (Session["loginid"].ToString() != "admin")
            {
                loginid = int.Parse(Session["userid"].ToString());
            }
            param.nguoitao = loginid;

            lstResult = service.SelectRows_Danhsachnghiphep(param,1,5000);
            DataTable dt = new DataTable();
            dt.TableName = "Employee";
            dt.Columns.Add("hovaten");
            dt.Columns.Add("ngaynghi");
            dt.Columns.Add("songay");
            dt.Columns.Add("nghiden");
            foreach (var item in lstResult)
            {
                DataRow row = dt.NewRow();
                dt.Rows.Add(row);
                row["hovaten"] = item.hovaten;
                row["ngaynghi"] = item.ngayxinnghitu;
                row["songay"] = item.songayxinnghi;
                row["nghiden"] = item.ngayxinnghiden;
                dt.AcceptChanges();
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= EmployeeReport.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            return RedirectToAction("Vacationlist");
        }
    }
}