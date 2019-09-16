using Newtonsoft.Json.Linq;
using RICONS.Core.Functions;
using RICONS.Core.Message;
using RICONS.Logger;
using RICONS.Web.Data.Services;
using RICONS.Web.Models;
using Nop.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RICONS.Web.Controllers
{
    public class PayStaffController : BaseController
    {
        Log4Net _logger = new Log4Net(typeof(PayStaffController));

        public ActionResult Index()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            //foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
            //{
            //    sbphongban.Append(string.Format("<option value='{0}'>{1}</option>", item.maphongban, item.tenphongban));
            //}
            //ViewBag.sbphongban = sbphongban.ToString();
            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_Denghichuyentra(ddns_paystaffModels model, int curentPage)
        {
            ddns_paystaffModels param = new ddns_paystaffModels();
            AddStaffServices service = new AddStaffServices();
            param.nguoitao = int.Parse(Session["userid"].ToString());
            int tongsodong = service.CountRows_Denghichuyentranhansu(param);
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
            if (curentPage != 1 && curentPage <= sotrang)
            {
                trangbd = (trangkt * (curentPage - 1)) + 1;
                trangkt = trangkt * curentPage;
            }

            List<ddns_paystaffModels> lstResult = new List<ddns_paystaffModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_Denghichuyentranhansu(param, trangbd, trangkt);
            }
            else if (curentPage != 1 && curentPage > sotrang) curentPage = curentPage - 1;


            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            int tongdong = 0;
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = trangbd;

                foreach (var item in lstResult)
                {
                    strSTT = i.ToString();
                    if (item.truongbophan_cht_duyet.Trim() == "1")
                        item.truongbophan_cht = item.truongbophan_cht + " - Đã gửi HS";
                    else if (item.truongbophan_cht_duyet.Trim() == "0")
                        item.truongbophan_cht = item.truongbophan_cht + " - Chưa gửi HS";

                    if (item.giamdocduan_ptgd_duyet.Trim() == "1")
                        item.giamdocduan_ptgd = item.giamdocduan_ptgd + " - Đã Duyệt";
                    else if (item.giamdocduan_ptgd_duyet.Trim() == "2")
                        item.giamdocduan_ptgd = item.giamdocduan_ptgd + " - Không Duyệt";
                    else if (item.giamdocduan_ptgd_duyet.Trim() == "0")
                        item.giamdocduan_ptgd = item.giamdocduan_ptgd + " - Chờ Duyệt";

                    if (item.phongqtnnl_duyet.Trim() == "1")
                        item.phongqtnnl = item.phongqtnnl + " - Đã Duyệt";
                    else if (item.phongqtnnl_duyet.Trim() == "2")
                        item.phongqtnnl = item.phongqtnnl + " - Không Duyệt";
                    else if (item.phongqtnnl_duyet.Trim() == "0")
                        item.phongqtnnl = item.phongqtnnl + " - Chờ Duyệt";

                    sbRows.Append(PrepareDataJson_Denghichuyentra(item, strSTT));
                    i++;
                }
                tongdong = i - 1;
                if (sbRows.Length > 0)
                    sbRows.Remove(sbRows.Length - 1, 1);
            }
            if (tongsodong == 0) sotrang = 0;
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");

            sbResult.Append("\"tongdong\":\"" + "" + tongsodong + "" + "\",");

            sbResult.Append("\"Pages\":\"" + "" + sotrang + "" + "\",");
            sbResult.Append("\"SubRow\":\"" + "false" + "\",");

            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");

            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_Denghichuyentra(ddns_paystaffModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.matranhansu.ToString(), function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' matranhansu='" + strEncryptCode + "' phongban_congtruong='" + model.phongban_congtruong + "'/>", model.matranhansu);
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
                //sbResult.Append("\"col_value\":\"" + model.tenlop + "\"");

                if (model.phongban_congtruong == "1")
                    sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("EditPayBuilding", "PayStaff", new { matranhansu = model.matranhansu.ToString() }) + "'title='" + model.mahoso + "'>" + model.mahoso + "</a>\"");
                else
                    sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("EditOffice", "PayStaff", new { matranhansu = model.matranhansu.ToString() }) + "'title='" + model.mahoso + "'>" + model.mahoso + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                if (model.phongban_congtruong == "1")
                    sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("EditPayBuilding", "PayStaff", new { matranhansu = model.matranhansu.ToString() }) + "'title='" + model.ngayyeucau + "'>" + model.ngayyeucau + "</a>\"");
                else
                    sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("EditOffice", "PayStaff", new { matranhansu = model.matranhansu.ToString() }) + "'title='" + model.ngayyeucau + "'>" + model.ngayyeucau + "</a>\"");

                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                if (model.phongban_congtruong == "1")
                    sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("EditPayBuilding", "PayStaff", new { matranhansu = model.matranhansu.ToString() }) + "'title='" + model.tenduan + "'>" + model.tenduan + "</a>\"");
                else
                    sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("EditOffice", "PayStaff", new { matranhansu = model.matranhansu.ToString() }) + "'title='" + model.tenduan + "'>" + model.tenduan + "</a>\"");
                sbResult.Append("},");


                //Tên đon vi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                //sbResult.Append("\"title\":\"" + model.madonvi + "\",");
                sbResult.Append("\"col_value\":\"" + model.tongsonhansu + "\"");
                sbResult.Append("},");



                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.truongbophan_cht + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");

                sbResult.Append("\"col_value\":\"" + model.giamdocduan_ptgd + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + model.phongqtnnl + "\"");
                sbResult.Append("}");

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


        public ActionResult NewPayBuilding()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();

            List<AddStaffModels> lstResult = new List<AddStaffModels>();
            AddStaffServices services = new AddStaffServices();
            lstResult = services.SelectRow_Laydanhsach_nsu_ctruong_loadcombo("1");
            StringBuilder sbpaystaff = new StringBuilder();
            foreach (var item in lstResult)
            {
                sbpaystaff.Append(string.Format("<option value='{0}'>{1}</option>", item.mabosungnhansu, item.tenduan));
            }
            ViewBag.sbpaystaff = sbpaystaff.ToString();

            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_ddns_addstaff_ddns_congtruong_kehoachbosungnhansu(AddStaffModels model)
        {
            AddStaffServices service = new AddStaffServices();

            List<AddStaffModels> lstResults = service.SelectRows_Denghibosungnhansu_hieuchinh(model.mabosungnhansu.ToString());

            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            //if (lstResult.Count > 0)
            //{
            //    string strSTT = "";
            //    int i = 1;
            //    foreach (var item1 in lstResult)
            //    {
            //        strSTT = i.ToString();
            //        sbRows.Append(PrepareDataJson_ddns_congtruong_kehoachbosungnhansu(item1, strSTT));
            //        i++;
            //    }
            //    if (sbRows.Length > 0)
            //        sbRows.Remove(sbRows.Length - 1, 1);
            //}
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            if (lstResults.Count==1)
            {
                sbResult.Append("\"mabosungnhansu\":\"" + lstResults[0].mabosungnhansu + "\",");
                sbResult.Append("\"tenduan\":\"" + lstResults[0].tenduan + "\",");
                sbResult.Append("\"goithau\":\"" + lstResults[0].goithau + "\",");
                sbResult.Append("\"diachi\":\"" + lstResults[0].diachi + "\",");
                sbResult.Append("\"ngayyeucau\":\"" + lstResults[0].ngayyeucau + "\",");
                sbResult.Append("\"congnghiep\":\"" + lstResults[0].congnghiep + "\",");
                sbResult.Append("\"thuongmai\":\"" + lstResults[0].thuongmai + "\",");
                sbResult.Append("\"dandung\":\"" + lstResults[0].dandung + "\",");
                sbResult.Append("\"nghiduong\":\"" + lstResults[0].nghiduong + "\",");
                sbResult.Append("\"hatang\":\"" + lstResults[0].hatang + "\",");
                sbResult.Append("\"khac\":\"" + lstResults[0].khac + "\",");
                sbResult.Append("\"khac_noidung\":\"" + lstResults[0].khac_noidung + "\",");

                sbResult.Append("\"truongbophan_cht\":\"" + lstResults[0].truongbophan_cht + "\",");
                sbResult.Append("\"truongbophan_cht_email\":\"" + lstResults[0].truongbophan_cht_email + "\",");
                sbResult.Append("\"giamdocduan_ptgd\":\"" + lstResults[0].giamdocduan_ptgd + "\",");
                sbResult.Append("\"giamdocduan_ptgd_email\":\"" + lstResults[0].giamdocduan_ptgd_email + "\",");
                sbResult.Append("\"phongqtnnl\":\"" + lstResults[0].phongqtnnl + "\",");
                sbResult.Append("\"phongqtnnl_email\":\"" + lstResults[0].phongqtnnl_email + "\",");
            }
           
            sbResult.Append("\"Pages\":\"" + "212" + "\",");
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");
            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_ddns_congtruong_kehoachbosungnhansu(ddns_congtruong_kehoachbosungnhansuModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                //sbResult.Append("{");
                //sbResult.Append("\"col_class\":\"rows-box\",");
                //sbResult.Append("\"col_id\":\"" + model.makehoach + "\",");
                //sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.mabosungnhansu + "\",");
                //sbResult.Append("\"col_attr\":[{\"name\":\"" + "makehoach" + "\", \"value\":\"" + model.makehoach + "\"},{\"name\":\"" + "mabosungnhansu" + "\", \"value\":\"" + model.mabosungnhansu + "\"}],");
                //sbResult.Append("\"col_value\":[");

                //#region Data cell
                ////colum checkbox

                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh cols1 stt\",");
                //sbResult.Append("\"col_id\":\"1\",");
                //sbResult.Append("\"col_value\":\"" + couter + "\"");
                //sbResult.Append("},");

                ////ma nhân viên
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"type\":\"hidden\",");
                //sbResult.Append("\"col_class\":\"ovh cols2\",");
                //sbResult.Append("\"col_id\":\"2\",");
                //sbResult.Append("\"col_value\":\"" + model.vitricongtac + "\"");
                //sbResult.Append("},");


                ////ho va ten
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"type\":\"hidden\",");
                //sbResult.Append("\"col_class\":\"ovh cols3\",");
                //sbResult.Append("\"col_id\":\"3\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.soluong + "' ></input>" + "\"");
                //sbResult.Append("},");


                ////Ngay sinh
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh cols4\",");
                //sbResult.Append("\"col_id\":\"4\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thoigianbosung + "' ></input>" + "\"");
                //sbResult.Append("},");

                //// Don vi tinh
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh cols5\",");
                //sbResult.Append("\"col_id\":\"5\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.soluong1 + "' ></input>" + "\"");
                //sbResult.Append("},");


                ////phong ban
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh cols6\",");
                //sbResult.Append("\"col_id\":\"6\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thoigianbosung1 + "' ></input>" + "\"");
                //sbResult.Append("},");

                //// Don vi tinh
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh cols7\",");
                //sbResult.Append("\"col_id\":\"7\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.soluong2 + "' ></input>" + "\"");
                //sbResult.Append("},");


                ////phong ban
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh cols8\",");
                //sbResult.Append("\"col_id\":\"8\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thoigianbosung2 + "' ></input>" + "\"");
                //sbResult.Append("},");



                ////dong bo trong

                ////string strHTML_Attachment = "<a href='#' class='button-add'><i class='fa fa-user' style='font-size: 15px'></i></a> | <a href='#' class='button-del'><i class='fa fa-user-times' style='font-size: 15px'></i></a>";
                ////string strHTML_Attachment = "<a href='#' class='button-del'><i class='fa fa-trash-o' ></i></a>";
                ////strHTML_Attachment = "<i class='attach_file' ></i>";
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh cols9\",");
                //sbResult.Append("\"col_id\":\"9\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                //sbResult.Append("}");

                //#endregion

                //sbResult.Append("]");
                //sbResult.Append("},");

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return sbResult;
        }


        [HttpPost]
        public JsonResult Save(string DataJson)
        {
            JsonResult Data = new JsonResult();
            AddStaffServices servicevpp = new AddStaffServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = servicevpp.Save_paystaff(DataJson, nguoitao, "0");
            if (iresult != "-1")
            {
                return Json(new { success = true, matranhansu = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, matranhansu = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public string send_Mail(string matranhansu, string DataJson)
        {
            _logger.Start("send_Mail");
            try
            {

                JObject json = JObject.Parse(DataJson);
                string tenduan = json["data1"]["tenduan"].ToString();
                string mailtruongbophan = json["data1"]["truongbophan_cht_email"].ToString(); //model[0].email_nguoigoi.Trim();

                string giamdocduan_ptgd = json["data1"]["giamdocduan_ptgd"].ToString();
                string giamdocduan_ptgd_email = json["data1"]["giamdocduan_ptgd_email"].ToString();

                string tiendovagiatrithang = json["data1"]["tiendovagiatrithang"].ToString();
                string ngaykhoicong = json["data1"]["ngaykhoicong"].ToString();
                string ngayhoanthanh = json["data1"]["ngayhoanthanh"].ToString();

                //List<PaStaffModels> lstaddstaff = new List<AddStaffModels>();
                //List<ddns_tiendo_giatriModels> lstddns_tiendo_giatri = new List<ddns_tiendo_giatriModels>();
                //List<ddns_congtruong_kehoachbosungnhansuModels> lstddns_congtruong_kehoachbosungnhansu = new List<ddns_congtruong_kehoachbosungnhansuModels>();

                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname4"));
                string strEncryptCode = linkname.Trim() + matranhansu + "0070pXQSeNsQRuzoCmUYfuX" + "&mailtruongbophan=" + mailtruongbophan + "&capdoduyet=1";
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 20px; padding: 0; width: 850px; font-size: 1em;color:black;'>"); //margin: 0 auto;  de canh giua

                sb.Append("<table cellpadding='0' cellspacing='0' width='850px' >");
                sb.Append("<tr>");

                sb.Append("<td>");
                sb.Append("<div style='width:150px;float:left;height :45px; line-height:45px; padding-top:10px;'>");
                sb.Append("<img src='http://i.imgur.com/yKqNNy2.png'  alt='ddd' style='width:100px; height:45px;'/>");
                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<div style='width:400px; text-align:center;font-weight: bold;float:left;line-height:45px'>");
                sb.Append("<p style= 'width:400px;text-align:center;font-size:18px;font-weight:bold;line-height:45px;padding-left:80px;float:left;'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ĐỀ NGHỊ CHUYỂN TRẢ NHÂN SỰ DỰ ÁN</p>");
                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</body>");

                sb.Append("<hr style=border: 1px solid #000; width: 100% />");
                sb.Append("<table style='width:850px; font-size:14px;'>");
                sb.Append("<tr><td style='padding-left:10px;colspan=5'><p><strong><em><u>Kính gửi Anh/Chị:</u></em>&nbsp;" + giamdocduan_ptgd + "</strong></p></td></tr>");
                sb.Append("<tr><td style='padding-left:10px;colspan=5;padding-bottom:10px;'><p><strong> Đề nghị chuyển trả nhân sự dự án: " + tenduan + " với nội dung chi tiết sau:</strong></p></td></tr>");
                sb.Append("<tr><td style='padding-left:10px;colspan=5;padding-bottom:10px;'><p><strong> I. THÔNG TIN DỰ ÁN:</strong></p></td></tr>");
                sb.Append("<tr><td style='padding-left:10px;colspan=6;padding-bottom:10px;'><p><strong> Tiến độ và giá trị: " + tiendovagiatrithang + "(Ngày khởi công "+ngaykhoicong+"; ngày hoàn thành bàn giao "+ngayhoanthanh+")</strong></p></td></tr>");
                sb.Append("</table>");


                // KẾ HOẠCH BỔ SUNG NHÂN SỰ CHO DỰ ÁN

                sb.Append("<table style='width:850px; font-size:14px;'>");
                sb.Append("<tr><td style='padding-left:10px;colspan=5;padding-bottom:5px;padding-top:10px'><p><strong> II. KẾ HOẠCH CHUYỂN TRẢ NHÂN SỰ:</strong></p></td></tr>");
                sb.Append("</table>");

                sb.Append("<table style='width:850px; font-size:14px;'>");
                sb.Append("<tr style='float:left;height :27px; line-height:27px; padding-left:10px;border: 1px solid #ddd;'>");
                sb.Append("<td rowspan='2' style='width:40px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>STT</td>");
                sb.Append("<td rowspan='2' style='width:200px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Vị trí công tác</td>");
                sb.Append("<td colspan='2' style='width:160px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Thi công phần ngầm</td>");
                sb.Append("<td colspan='2' style='width:160px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Thi công phần thân thô</td>");
                sb.Append("<td colspan='2' style='width:160px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Thi công phần hoàn thiện</td>");
                sb.Append("<td rowspan='2' style='width:80px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Ghi chú</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td style='width:80px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Số lượng</td>");
                sb.Append("<td style='width:80px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Thời gian chuyển trả</td>");
                sb.Append("<td style='width:80px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Số lượng</td>");
                sb.Append("<td style='width:80px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Thời gian chuyển trả</td>");
                sb.Append("<td style='width:80px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Số lượng</td>");
                sb.Append("<td style='width:80px; text-align:center; font-weight:bold; font-size:14px;border: 1px solid #ddd;'>Thời gian chuyển trả</td>");
                sb.Append("</tr>");

                JArray json_congtruong = (JArray)json["data2"];

                for (int i = 0; i < json_congtruong.Count(); i++)
                {
                    sb.Append("<tr style='float:left;height :22px; line-height:24px; padding-left:10px;border: 1px solid #ddd;'>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:center; border: 1px solid #ddd;'>" + (i + 1).ToString() + "</td>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:left;   border: 1px solid #ddd;'>" + json_congtruong[i]["vitricongtac"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:center;   border: 1px solid #ddd;'>" + json_congtruong[i]["soluong"].ToString().Trim() + "" + "</td>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:center; border: 1px solid #ddd;'>" + json_congtruong[i]["thoigianchuyentra"].ToString().Trim() + "" + "</td>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:center; border: 1px solid #ddd;'>" + json_congtruong[i]["soluong1"].ToString().Trim() + "" + "</td>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:center; border: 1px solid #ddd;'>" + json_congtruong[i]["thoigianchuyentra1"].ToString().Trim() + "" + "</td>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:center;   border: 1px solid #ddd;'>" + json_congtruong[i]["soluong2"].ToString().Trim() + "" + "</td>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:center;   border: 1px solid #ddd;'>" + json_congtruong[i]["thoigianchuyentra2"].ToString().Trim() + "" + "</td>");
                    sb.Append("<td style='font-size:14px; line-height:24px; text-align:left;   border: 1px solid #ddd;'>" + json_congtruong[i]["ghichu"].ToString().Trim() + "" + "</td>");

                    sb.Append("</tr>");
                }
                sb.Append("</table>");

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý CTNS</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");


                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, giamdocduan_ptgd_email.Trim());
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Đề nghị chuyển trả nhân sự", System.Text.Encoding.UTF8);
                message.Subject = "Đề nghị chuyển trả nhân sự dự án";
                message.Body = sb.ToString();
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;
                SmtpClient client = new SmtpClient(smtp_host);
                client.UseDefaultCredentials = true;
                try
                {
                    client.Send(message);
                    return "1";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in CreateTestMessage2(): {0}", ex.ToString());
                    return "-1";
                }
            }
            catch (Exception ms)
            {
                _logger.Error(ms);
                return "-1";
            }
            _logger.End("send_Mail");
        }

        [HttpPost]
        public JsonResult Saveandsentmail(string DataJson)
        {
            JsonResult Data = new JsonResult();
            AddStaffServices servicevpp = new AddStaffServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = servicevpp.Save_paystaff(DataJson, nguoitao, "1");
            if (iresult != "-1")
            {
                string guimail = send_Mail(iresult, DataJson);
                if (guimail == "1")
                    return Json(new { success = true, matranhansu = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, matranhansu = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, matranhansu = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditPayBuilding(string matranhansu)
        {
            if (!IsLogged())
                return BackToLogin();
            List<ddns_paystaffModels> lstaddstaff = new List<ddns_paystaffModels>();
            AddStaffServices service = new AddStaffServices();
            lstaddstaff = service.SelectRows_Denghi_chuyentra_nhansu_hieuchinh(matranhansu);



            StringBuilder sbpaystaff = new StringBuilder();
            foreach (var item in lstaddstaff)
            {
                sbpaystaff.Append(string.Format("<option value='{0}'>{1}</option>", item.mabosungnhansu, item.tenduan));
            }
            ViewBag.sbpaystaff = sbpaystaff.ToString();



            return View(lstaddstaff[0]);
        }


        [HttpPost]
        public JsonResult SelectRows_ddns_paystaff_congtruong_hieuchinh(ddns_paystaff_congtruong_kehoachbosungnhansuModels model)
        {
            AddStaffServices service = new AddStaffServices();

            List<ddns_paystaff_congtruong_kehoachbosungnhansuModels> lstResult = service.SelectRows_ddns_paystaff_congtruong_hieuchinh(model.matranhansu.ToString());
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_SelectRows_ddns_paystaff_congtruong_hieuchinh(item1, strSTT));
                    i++;
                }
                if (sbRows.Length > 0)
                    sbRows.Remove(sbRows.Length - 1, 1);
            }
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            sbResult.Append("\"Pages\":\"" + "212" + "\",");
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");
            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_SelectRows_ddns_paystaff_congtruong_hieuchinh(ddns_paystaff_congtruong_kehoachbosungnhansuModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makehoach + "\",");
                sbResult.Append("\"col_id_matranhansu\":\"" + model.matranhansu + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makehoach" + "\", \"value\":\"" + model.makehoach + "\"},{\"name\":\"" + "matranhansu" + "\", \"value\":\"" + model.matranhansu + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh cols1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + couter + "\"");
                sbResult.Append("},");

                //ma nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"type\":\"hidden\",");
                sbResult.Append("\"col_class\":\"ovh cols2\",");
                sbResult.Append("\"col_id\":\"2\",");
                sbResult.Append("\"col_value\":\"" + model.vitricongtac + "\"");
                sbResult.Append("},");


                //ho va ten
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"type\":\"hidden\",");
                sbResult.Append("\"col_class\":\"ovh cols3\",");
                sbResult.Append("\"col_id\":\"3\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.soluong + "' ></input>" + "\"");
                sbResult.Append("},");


                //Ngay sinh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh cols4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thoigianchuyentra + "' ></input>" + "\"");
                sbResult.Append("},");

                // Don vi tinh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh cols5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.soluong1 + "' ></input>" + "\"");
                sbResult.Append("},");


                //phong ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh cols6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thoigianchuyentra1 + "' ></input>" + "\"");
                sbResult.Append("},");

                // Don vi tinh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh cols7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.soluong2 + "' ></input>" + "\"");
                sbResult.Append("},");


                //phong ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh cols8\",");
                sbResult.Append("\"col_id\":\"8\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thoigianchuyentra2 + "' ></input>" + "\"");
                sbResult.Append("},");



                //dong bo trong

                //string strHTML_Attachment = "<a href='#' class='button-add'><i class='fa fa-user' style='font-size: 15px'></i></a> | <a href='#' class='button-del'><i class='fa fa-user-times' style='font-size: 15px'></i></a>";
                //string strHTML_Attachment = "<a href='#' class='button-del'><i class='fa fa-trash-o' ></i></a>";
                //strHTML_Attachment = "<i class='attach_file' ></i>";
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh cols9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                sbResult.Append("}");

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


        #region DANH SÁCH NHÂN SỰ CÔNG TRƯỜNG TRẢ VỀ PHÒNG NHÂN SỰ

        public ActionResult Index_Pay_worker_officer()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_Danhsachnhansuchuyentra(ddns_lapdanhsach_dieudong_thongbaoModels model, int curentPage)
        {
            ddns_lapdanhsach_dieudong_thongbaoModels param = new ddns_lapdanhsach_dieudong_thongbaoModels();
            AddStaffServices service = new AddStaffServices();
            param.nguoitao = int.Parse(Session["userid"].ToString());
            int tongsodong = service.CountRow_Danhsachnhansutrave(param.noilamviec_moi);
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
            if (curentPage != 1 && curentPage <= sotrang)
            {
                trangbd = (trangkt * (curentPage - 1)) + 1;
                trangkt = trangkt * curentPage;
            }

            List<ddns_lapdanhsach_dieudong_thongbaoModels> lstResult = new List<ddns_lapdanhsach_dieudong_thongbaoModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_Danhsachnhansutrave(param, trangbd, trangkt);
            }
            else if (curentPage != 1 && curentPage > sotrang) curentPage = curentPage - 1;


            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            int tongdong = 0;
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = trangbd;
                foreach (var item in lstResult)
                {
                    strSTT = i.ToString();
                    if (item.nhansuden_ct_pb.ToString() == "1")
                        item.nhansuden_ct_pb = "Đã đến công trường";
                    else if (item.nhansuden_ct_pb.ToString() == "2")
                        item.nhansuden_ct_pb = "Đã chuyển trả nhân sự";
                    else if (item.nhansuden_ct_pb.ToString() == "3")
                        item.nhansuden_ct_pb = "Hoàn tất chuyển trả NS";
                    else
                        item.nhansuden_ct_pb = "Chưa đến công trường";
                    sbRows.Append(PrepareDataJson_Danhsachdenlamviec(item, strSTT));
                    i++;
                }
                tongdong = i - 1;
                if (sbRows.Length > 0)
                    sbRows.Remove(sbRows.Length - 1, 1);
            }
            if (tongsodong == 0) sotrang = 0;
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            sbResult.Append("\"tongdong\":\"" + "" + tongsodong + "" + "\",");
            sbResult.Append("\"Pages\":\"" + "" + sotrang + "" + "\",");
            sbResult.Append("\"SubRow\":\"" + "false" + "\",");
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");

            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_Danhsachdenlamviec(ddns_lapdanhsach_dieudong_thongbaoModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.madanhsach.ToString(), function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' madanhsach='" + strEncryptCode + "'/>", model.madanhsach);
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

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col2 stt\",");
                sbResult.Append("\"col_id\":\"2\",");
                sbResult.Append("\"col_value\":\"" + model.nhansuden_ct_pb + "\"");
                sbResult.Append("},");

                //Ngày đến công trường
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.nhansuden_ct_pb_ngaytra + "' ></input>" + "\"");
                sbResult.Append("},");

                //Tên đon vi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col3\",");
                sbResult.Append("\"col_id\":\"3\",");
                sbResult.Append("\"col_value\":\"" + model.hovaten + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + model.dienthoai + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + model.email + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + model.noilamviec_moi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"col_value\":\"" + model.chucdanh_moi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                sbResult.Append("\"col_value\":\"" + model.nhansuden_ct_pb_ngayden + "\"");
                sbResult.Append("}");

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

        [HttpPost]
        public JsonResult Save_Pay_employee_worker(string DataJson)
        {
            JsonResult Data = new JsonResult();
            AddStaffServices servicevpp = new AddStaffServices();
            bool iresult = servicevpp.Save_Pay_employee_worker(DataJson);
            if (iresult == true)
            {
                //send_Mail_Guimailthongbaodieudongchonhansu(json_tiendo_giatri);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save_Pay_employe_PhongNS_hoantat(string DataJson)
        {
            JsonResult Data = new JsonResult();
            AddStaffServices servicevpp = new AddStaffServices();
            bool iresult = servicevpp.Save_Pay_employe_PhongNS_hoantat(DataJson);
            if (iresult == true)
            {
                //send_Mail_Guimailthongbaodieudongchonhansu(json_tiendo_giatri);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region DANH SÁCH NHÂN SỰ VĂN PHÒNG TRẢ VỀ PHÒNG NHÂN SỰ

        [HttpPost]
        public JsonResult SelectRows_ddns_addstaff_ddns_vanphong_kehoachbosungnhansu(AddStaffModels model)
        {
            AddStaffServices service = new AddStaffServices();

            List<AddStaffModels> lstResults = service.SelectRows_Denghibosungnhansu_hieuchinh(model.mabosungnhansu.ToString());

            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            //if (lstResult.Count > 0)
            //{
            //    string strSTT = "";
            //    int i = 1;
            //    foreach (var item1 in lstResult)
            //    {
            //        strSTT = i.ToString();
            //        sbRows.Append(PrepareDataJson_ddns_congtruong_kehoachbosungnhansu(item1, strSTT));
            //        i++;
            //    }
            //    if (sbRows.Length > 0)
            //        sbRows.Remove(sbRows.Length - 1, 1);
            //}
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            if (lstResults.Count == 1)
            {
                sbResult.Append("\"mabosungnhansu\":\"" + lstResults[0].mabosungnhansu + "\",");
                sbResult.Append("\"tenduan\":\"" + lstResults[0].tenduan + "\",");
                sbResult.Append("\"goithau\":\"" + lstResults[0].goithau + "\",");
                sbResult.Append("\"diachi\":\"" + lstResults[0].diachi + "\",");
                sbResult.Append("\"ngayyeucau\":\"" + lstResults[0].ngayyeucau + "\",");
                sbResult.Append("\"congnghiep\":\"" + lstResults[0].congnghiep + "\",");
                sbResult.Append("\"thuongmai\":\"" + lstResults[0].thuongmai + "\",");
                sbResult.Append("\"dandung\":\"" + lstResults[0].dandung + "\",");
                sbResult.Append("\"nghiduong\":\"" + lstResults[0].nghiduong + "\",");
                sbResult.Append("\"hatang\":\"" + lstResults[0].hatang + "\",");
                sbResult.Append("\"khac\":\"" + lstResults[0].khac + "\",");
                sbResult.Append("\"khac_noidung\":\"" + lstResults[0].khac_noidung + "\",");

                sbResult.Append("\"truongbophan_cht\":\"" + lstResults[0].truongbophan_cht + "\",");
                sbResult.Append("\"truongbophan_cht_email\":\"" + lstResults[0].truongbophan_cht_email + "\",");
                sbResult.Append("\"giamdocduan_ptgd\":\"" + lstResults[0].giamdocduan_ptgd + "\",");
                sbResult.Append("\"giamdocduan_ptgd_email\":\"" + lstResults[0].giamdocduan_ptgd_email + "\",");
                sbResult.Append("\"phongqtnnl\":\"" + lstResults[0].phongqtnnl + "\",");
                sbResult.Append("\"phongqtnnl_email\":\"" + lstResults[0].phongqtnnl_email + "\",");
            }

            sbResult.Append("\"Pages\":\"" + "212" + "\",");
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");
            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult SelectRows_ddns_lapds_chuyentravanphong_timkiem(ddns_paystaff_vanphong_kehoachbosungnhansuModels model)
        {
            AddStaffServices service = new AddStaffServices();
            List<ddns_paystaff_vanphong_kehoachbosungnhansuModels> lstResult = service.SelectRows_ddns_lapdanhsachchuyentravephongban_vephongban_taomoi_timkiem(model.manhanvien.ToString());

            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_ddns_lapds_chuyentravanphong_timkiem(item1, strSTT));
                    i++;
                }
                if (sbRows.Length > 0)
                    sbRows.Remove(sbRows.Length - 1, 1);
            }
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            sbResult.Append("\"Pages\":\"" + "212" + "\",");
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");
            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_ddns_lapds_chuyentravanphong_timkiem(ddns_paystaff_vanphong_kehoachbosungnhansuModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makehoach + "\",");
                sbResult.Append("\"col_id_matranhansu\":\"" + model.matranhansu + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makehoach" + "\", \"value\":\"" + model.makehoach + "\"},{\"name\":\"" + "matranhansu" + "\", \"value\":\"" + model.matranhansu + "\"},{\"name\":\"" + "thongtinlienlac" + "\", \"value\":\"" + 0 + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + couter + "\"");
                sbResult.Append("},");

                //ma nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col2\",");
                sbResult.Append("\"col_id\":\"2\",");
                sbResult.Append("\"title\":\"" + model.manhanvien + "\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.hovaten + "' ></input>" + "\"");
                sbResult.Append("},");


                //ho va ten
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");

                sbResult.Append("\"col_class\":\"ovh col3\",");
                sbResult.Append("\"col_id\":\"3\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trinhdo + "' ></input>" + "\"");
               // sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                sbResult.Append("},");


                //Ngay sinh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.chuyenmon + "' ></input>" + "\"");
                //sbResult.Append("\"col_value\":\"" + "<select id='filter03' style='width:100%; class='chosen-select-loaikehoach' name='madanhmuc1'>" + pb + "</select>" + "\"");
                //sbResult.Append("\"col_value\":\"" + "<select id='filter03' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + cd + "</select>" + "\"");
                sbResult.Append("},");

                // Don vi tinh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.vitricongviec + "' ></input>" + "\"");
               // sbResult.Append("\"col_value\":\"" + "<select id='filter04' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pbfull + "</select>" + "\"");
                sbResult.Append("},");


                //phong ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thoigianchuyentra + "' ></input>" + "\"");
               // sbResult.Append("\"col_value\":\"" + "<select id='filter05' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + cdfull + "</select>" + "\"");
                sbResult.Append("},");

                //phong ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.lydochuyentra + "' ></input>" + "\"");
                sbResult.Append("},");

                //phong ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.hovaten + "' ></input>" + "\"");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                sbResult.Append("},");

                //dong bo trong   <a href='#' class='button-add'><i class='fa fa-user' style='font-size: 12px;color: #428bca'>Add </i></a>|

                string strHTML_Attachment = "<a href='#' class='button-del'><i class='fa fa-user-times' style='font-size: 12px;color: #428bca'>Del</i></a>";
                //string strHTML_Attachment = "<a href='#' class='button-del'><i class='fa fa-trash-o' ></i></a>";
                //strHTML_Attachment = "<i class='attach_file' ></i>";
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                sbResult.Append("}");

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

        [HttpPost]
        public JsonResult SelectRows_ddns_timkiemds_saukhiluu(ddns_paystaff_vanphong_kehoachbosungnhansuModels model)
        {
            AddStaffServices service = new AddStaffServices();
            List<ddns_paystaff_vanphong_kehoachbosungnhansuModels> lstResult = service.SelectRows_ddns_lapdanhsachchuyentravephongban_vephongban_taomoi_timkiem(model.manhanvien.ToString());

            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_ddns_timkiemds_saukhiluu(item1, strSTT));
                    i++;
                }
                if (sbRows.Length > 0)
                    sbRows.Remove(sbRows.Length - 1, 1);
            }
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            sbResult.Append("\"Pages\":\"" + "212" + "\",");
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");
            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_ddns_timkiemds_saukhiluu(ddns_paystaff_vanphong_kehoachbosungnhansuModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makehoach + "\",");
                sbResult.Append("\"col_id_matranhansu\":\"" + model.matranhansu + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makehoach" + "\", \"value\":\"" + model.makehoach + "\"},{\"name\":\"" + "matranhansu" + "\", \"value\":\"" + model.matranhansu + "\"},{\"name\":\"" + "thongtinlienlac" + "\", \"value\":\"" + 0 + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + couter + "\"");
                sbResult.Append("},");

                //ma nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col2\",");
                sbResult.Append("\"col_id\":\"2\",");
                sbResult.Append("\"title\":\"" + model.manhanvien + "\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.hovaten + "' ></input>" + "\"");
                sbResult.Append("},");


                //ho va ten
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");

                sbResult.Append("\"col_class\":\"ovh col3\",");
                sbResult.Append("\"col_id\":\"3\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trinhdo + "' ></input>" + "\"");
                // sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                sbResult.Append("},");


                //Ngay sinh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.chuyenmon + "' ></input>" + "\"");
                //sbResult.Append("\"col_value\":\"" + "<select id='filter03' style='width:100%; class='chosen-select-loaikehoach' name='madanhmuc1'>" + pb + "</select>" + "\"");
                //sbResult.Append("\"col_value\":\"" + "<select id='filter03' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + cd + "</select>" + "\"");
                sbResult.Append("},");

                // Don vi tinh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.vitricongviec + "' ></input>" + "\"");
                // sbResult.Append("\"col_value\":\"" + "<select id='filter04' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pbfull + "</select>" + "\"");
                sbResult.Append("},");


                //phong ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thoigianchuyentra + "' ></input>" + "\"");
                // sbResult.Append("\"col_value\":\"" + "<select id='filter05' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + cdfull + "</select>" + "\"");
                sbResult.Append("},");

                //phong ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.lydochuyentra + "' ></input>" + "\"");
                sbResult.Append("},");

                //phong ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.hovaten + "' ></input>" + "\"");
                sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                sbResult.Append("},");

                //dong bo trong   <a href='#' class='button-add'><i class='fa fa-user' style='font-size: 12px;color: #428bca'>Add </i></a>|

                string strHTML_Attachment = "<a href='#' class='button-del'><i class='fa fa-user-times' style='font-size: 12px;color: #428bca'>Del</i></a>";
                //string strHTML_Attachment = "<a href='#' class='button-del'><i class='fa fa-trash-o' ></i></a>";
                //strHTML_Attachment = "<i class='attach_file' ></i>";
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                sbResult.Append("}");

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


        public ActionResult NewPayOffice()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();

            List<AddStaffModels> lstResult = new List<AddStaffModels>();
            AddStaffServices services = new AddStaffServices();
            lstResult = services.SelectRow_Laydanhsach_nsu_ctruong_loadcombo("0");
            StringBuilder sbpaystaff = new StringBuilder();
            foreach (var item in lstResult)
            {
                sbpaystaff.Append(string.Format("<option value='{0}'>{1}</option>", item.mabosungnhansu, item.tenduan));
            }
            ViewBag.sbpaystaff = sbpaystaff.ToString();

            return View(lstphongban[0]);
        }




        #endregion

    }
}