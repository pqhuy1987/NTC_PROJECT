using RICONS.Logger;
using Nop.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using RICONS.Web.Data.Services;
using RICONS.Web.Models;
using RICONS.Core.Functions;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using System.Data;
using ExportHelper.Repository;
using System.IO;

namespace RICONS.Web.Controllers
{
    public class KpiCompanyController : BaseController
    {
        #region Fields
        Log4Net _logger = new Log4Net(typeof(KpiCompanyController));
        public static List<PhongBanModels> lstResult_phongban = new List<PhongBanModels>();
        public static List<ChucDanhModels> lstResult_chucdanh = new List<ChucDanhModels>();
        #endregion

        #region KPI CẤP CÔNG TY

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
        public JsonResult SelectRows_KpiLevelCompany(KpiLevelCompanyModels model, int curentPage)
        {
            KpiLevelCompanyModels param = new KpiLevelCompanyModels();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            param.nguoitao = int.Parse(Session["userid"].ToString());
            int tongsodong = service.CountRows_KpiLevelCompany(param);
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

            List<KpiLevelCompanyModels> lstResult = new List<KpiLevelCompanyModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_KpiLevelCompany(param, trangbd, trangkt);
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
                    string chuoi = "";
                    if (item.nguoilapkpi_dagui.Trim() == "1")
                        item.nguoilapkpi_dagui_text = "Đã gửi HS";
                    else if (item.nguoilapkpi_dagui.Trim() == "0")
                        item.nguoilapkpi_dagui_text = "Chưa gửi HS";
                    else if (item.nguoilapkpi_dagui.Trim() == "3")
                        item.nguoilapkpi_dagui_text = "Đã gửi HS PTGĐ không duyệt";
                    else if (item.nguoilapkpi_dagui.Trim() == "4")
                        item.nguoilapkpi_dagui_text = "Đã gửi HS TGĐ không duyệt";


                    if (item.photongxemxetkpi_duyet.Trim() == "1")
                        item.photongxemxetkpi_duyet_text = "Đã Duyệt";
                    else if (item.photongxemxetkpi_duyet.Trim() == "2")
                        item.photongxemxetkpi_duyet_text = "Không Duyệt";
                    else if (item.photongxemxetkpi_duyet.Trim() == "0")
                        item.photongxemxetkpi_duyet_text = "Chờ Duyệt";

                    if (item.tonggiamdocxemxetkpi_duyet.Trim() == "1")
                        item.tonggiamdocxemxetkpi_duyet_text = "Đã Duyệt";
                    else if (item.tonggiamdocxemxetkpi_duyet.Trim() == "2")
                        item.tonggiamdocxemxetkpi_duyet_text = "Không Duyệt";
                    else if (item.tonggiamdocxemxetkpi_duyet.Trim() == "0")
                        item.tonggiamdocxemxetkpi_duyet_text = "Chờ Duyệt";

                   
                    sbRows.Append(PrepareDataJson_KpiLevelCompany(item, strSTT));
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

        private StringBuilder PrepareDataJson_KpiLevelCompany(KpiLevelCompanyModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.makpicongty.ToString(), function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' makpicongty='" + strEncryptCode + "' phongban_congtruong='"+1+"'/>", model.makpicongty);
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
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Company_Edit", "KpiCompany", new { makpicongty = model.makpicongty.ToString() }) + "'title='QP04FM12'>" + "QP04FM12-" + model.nam + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Company_Edit", "KpiCompany", new { makpicongty = model.makpicongty.ToString() }) + "'title='" + model.bophansoanthao + "'>" + model.bophansoanthao + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Company_Edit", "KpiCompany", new { makpicongty = model.makpicongty.ToString() }) + "'title='" + model.ngaybanhanh + "'>" + model.ngaybanhanh + "</a>\"");
                sbResult.Append("},");

                //Tên đon vi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Company_Edit", "KpiCompany", new { makpicongty = model.makpicongty.ToString() }) + "'title='" + model.ngaycapnhat + "'>" + model.ngaycapnhat + "</a>\"");
                sbResult.Append("},");


                //Trình trạng hồ sơ
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col13\",");
                sbResult.Append("\"col_id\":\"13\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.nguoilapkpi_dagui_text + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");

                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi_ngay + "\"");
                sbResult.Append("},");


                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col11\",");
                sbResult.Append("\"col_id\":\"11\",");

                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");


                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col12\",");
                sbResult.Append("\"col_id\":\"12\",");
                //sbResult.Append("\"title\":\"" + model.ngayhoc + "\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi_ngay + "\"");
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

        // THÊM MỚI.
        public ActionResult Kpi_Level_Company_New()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            lstResult_phongban = new List<PhongBanModels>();
            lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            StringBuilder sbNguoiThucHien = new StringBuilder();
            foreach (var item in lstResult_phongban)
            {
                sbNguoiThucHien.Append(string.Format("<option value={0}>{1}</option>", item.maphongban, item.tenphongban));
            }
            //ViewBag.keHoachs = sbHeHoach.ToString();
            ViewBag.nguoiThucHiens = sbNguoiThucHien.ToString();
            lstphongban[0].hovaten = Session["fullname"].ToString().Trim();
            lstphongban[0].email = Session["thudientu"].ToString().Trim();
            return View(lstphongban[0]);
        }

        // CẬP NHẬT.
        public ActionResult Kpi_Level_Company_Edit(string makpicongty)
        {
            if (!IsLogged())
                return BackToLogin();

            DanhmucServices servicedm = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            lstResult_phongban = new List<PhongBanModels>();
            lstResult_phongban = servicedm.SelectRows(parampb);
            StringBuilder sbNguoiThucHien = new StringBuilder();
            string nguoncm = "";
            foreach (var item in lstResult_phongban)
            {
                nguoncm = nguoncm + "<option value=" + item.maphongban + ">" + item.tenphongban + "</option>";
                sbNguoiThucHien.Append(string.Format("<option value={0}>{1}</option>", item.maphongban, item.tenphongban));
            }
            ViewBag.nguoiThucHiens = sbNguoiThucHien.ToString();

            List<KpiLevelCompanyModels> lstaddstaff = new List<KpiLevelCompanyModels>();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            lstaddstaff = service.SelectRows_KpiLevelCompany_hieuchinh(makpicongty);
            return View(lstaddstaff[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiLevelCompanyDetail_hieuchinh(KpiLevelCompanyDetailModels model)
        {
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            List<KpiLevelCompanyDetailModels> lstResult = service.SelectRows_KpiLevelCompanyDetail_hieuchinh(model.makpicongty.ToString());
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string nguoncm = "";
                foreach (var item in lstResult_phongban)
                {
                    nguoncm = nguoncm + "<option value=" + item.maphongban + ">" + item.tenphongban + "</option>";
                }

                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_KpiLevelCompanyDetail_hieuchinh(item1, strSTT, nguoncm));
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

            //Server.HtmlEncode("<script>sbResult.ToString()</script>");

        }

        private StringBuilder PrepareDataJson_KpiLevelCompanyDetail_hieuchinh(KpiLevelCompanyDetailModels model, string couter, string nguoncm)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style="";

                if (model.stt.ToUpper() == "A") { group = "1"; kpicha = "1"; style = "background-color:#f9fd0d;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "B") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "C") { group = "3"; kpicha = "1"; style = "background-color:#5edcf1;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "D") { group = "4"; kpicha = "1"; style = "background-color:#90abe8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "")     {              style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "A") group = "1";
                else if (model.thuoccap.ToUpper() == "B") group = "2";
                else if (model.thuoccap.ToUpper() == "C") group = "3";
                else if (model.thuoccap.ToUpper() == "D") group = "4";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpicongty_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpicongty + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpicongty_chitiet" + "\", \"value\":\"" + model.makpicongty_chitiet + "\"},{\"name\":\"" + "makpicongty" + "\", \"value\":\"" + model.makpicongty + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                sbResult.Append("},");

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt=="")
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + model.muctieu + "\"");
                    sbResult.Append("},");

                    //trong so
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    // Don vi tinh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");


                    //phong ban
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col15\",");
                    sbResult.Append("\"col_id\":\"15\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    //string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("}");

                    #endregion
                }
                else
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txttrongso' name='trongso' >" + model.trongso.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tieuchidanhgia + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.cachtinh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ngayghinhanketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"" + "<select  id='filter01' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Người thực hiện' class='chosen-select-nguoithuchien'  name='nguonchungminh' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"col15 boxEdit\",");
                    sbResult.Append("\"col_id\":\"15\",");
                    sbResult.Append("\"col_value\":\"" + "<select  id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Nguồn chứng minh' class='chosen-select-nguoithuchien'  name='nguonchungminh1' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtketqua' name='ketqua' >" + model.ketqua + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                    sbResult.Append("}");

                    #endregion
                }

              

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

        // Lưu kpi cấp công ty
        [HttpPost]
        public JsonResult Save(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiLevelCompanyServices services = new KpiLevelCompanyServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_KpiLevelCompany(DataJson, nguoitao,"0", "0","0");
            if (iresult != "-1")
            {
                return Json(new { success = true, makpicongty = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpicongty = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        //Gui mail cho pho tong xem xet2 duyet KPI năm
        public string send_Mail(string makpicongty, string DataJson)
        {
            _logger.Start("send_Mail");
            try
            {
                JObject json = JObject.Parse(DataJson);
                string mailnguoigui = json["data1"]["nguoilapkpi_email"].ToString();
                string mailphotonggiamdoc = json["data1"]["photongxemxetkpi_email"].ToString();
                string namkpi = json["data1"]["nam"].ToString().Trim(); //"2018";
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetKPICompanyL1/?makpi=";

                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 5px; padding: 0; width: 100%; font-size: 1em;color:black;'>"); //margin: 0 auto;  de canh giua
                sb.Append("<table cellpadding='0' cellspacing='0' width='100%' >");
                sb.Append("<tr>");

                sb.Append("<td>");
                sb.Append("<div style='width:400px; text-align:center;font-weight: bold;float:left;line-height:45px'>");
                sb.Append("<p style= 'width:400px;text-align:center;font-size:18px;font-weight:bold;line-height:45px;padding-left:80px;float:left;'>KPI CẤP CÔNG TY</p>");
                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</body>");

                sb.Append("<hr style=border: 1px solid #000; width: 100% />");

                sb.Append("<table style='width:100%; font-size:13px;'>");

                sb.Append("<tr><td rowspan='4' colspan='2' style='border: 1px solid #000;font-size: 16pt;color:#f3332e;text-align: center;'>KPI NĂM <br /> <span style='font-size: 29pt;color:#8e2bf1;text-align: center;'>" + namkpi + "</span></td>");

              //<th rowspan="4" colspan="2" class="boxEdit" style="font-size: 16pt;color:#f3332e;padding-top: 35px">NĂM KPI<textarea style="width:100%; font-size: 29pt; color:#8e2bf1; border: 0px; text-align: center; resize:none;" rows="1" placeholder="" id="txtnam" name="nam">@Model.nam</textarea></th>

                sb.Append("<td colspan='12' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP CÔNG TY - KPI NĂM " + namkpi + "</td>");
                sb.Append("</tr>");

                sb.Append("<tr><td colspan='12' style='text-align: left;border: 1px solid #000;'>Công ty Cổ Phần Đầu Tư Xây Dựng Ricons:</td></tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='8' style='text-align: left;border: 1px solid #000;'>Bộ phận soạn thảo: " + json["data1"]["bophansoanthao"].ToString().Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Biểu mẫu: QP04FM12</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày ban hành: " + json["data1"]["ngaybanhanh"].ToString().Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày cập nhật: " + json["data1"]["ngaycapnhat"].ToString().Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Lần cập nhật: " + json["data1"]["lancapnhat"].ToString().Trim() + " </td>");
                sb.Append("</tr>");

                sb.Append("<tr style='margin: 0 auto;'>");
                sb.Append("<td style='border: 1px solid #000;width:3%;text-align:center'>Stt</td>");
                sb.Append("<td style='border: 1px solid #000;width:17%;text-align:center'>Mục tiêu</td>");
                sb.Append("<td style='border: 1px solid #000;width:5%;text-align:center'>Trọng số</td>");
                sb.Append("<td style='border: 1px solid #000;width:10%;text-align:center'>Tiêu chí đánh giá</td>");
                sb.Append("<td style='border: 1px solid #000;width:10%;text-align:center'>Cách tính</td>");
                sb.Append("<td style='border: 1px solid #000;width:8%;text-align:center'>Thời điểm ghi nhận kết quả</td>");
                sb.Append("<td style='border: 1px solid #000;width:9%;text-align:center'>BP/Cá nhân nhận mục tiêu</td>");
                sb.Append("<td style='border: 1px solid #000;width:9%;text-align:center'>Nguồn chứng minh</td>");
                sb.Append("<td style='border: 1px solid #000;width:5%;text-align:center'>Đơn vị tính</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>KPIs kế hoạch</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>KPIs thực hiện</td>");
                sb.Append("<td style='border: 1px solid #000;width:7%;text-align:center'>Tỉ lệ % hoàn thành KPIs</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>Kết quả</td>");
                sb.Append("<td style='border: 1px solid #000;width:8%;text-align:center'>Ghi chú</td>");
                sb.Append("</tr>");

                //string mact = ""; string kpichinhsuact = "";
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "A")
                        sb.Append("<tr style='background-color:#f9fd0d;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "B")
                        sb.Append("<tr style='background-color:#99f1b8;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "C")
                        sb.Append("<tr style='background-color:#5edcf1 ;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "D")
                        sb.Append("<tr style='background-color:#90abe8 ;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0;text-align:center'>");
                    else sb.Append("<tr>");

                    string nguoncm = json_tiendo_giatri[i]["nguonchungminh"].ToString().Trim();
                    string nguonchungminh = "";
                    if (nguoncm.Trim() != "")
                    {
                        string[] chuoicm = nguoncm.Split(',');
                        for (int m = 0; m < chuoicm.Length; m++)
                        {
                            if (lstResult_phongban.Where(p => p.maphongban == chuoicm[m].ToString()).ToList().Count() > 0)
                            {
                                if (m < chuoicm.Length - 1)
                                    nguonchungminh = nguonchungminh + lstResult_phongban.Where(p => p.maphongban == chuoicm[m].ToString()).ToList()[0].tenphongban.ToString() + "<br/>";
                                else
                                    nguonchungminh = nguonchungminh + lstResult_phongban.Where(p => p.maphongban == chuoicm[m].ToString()).ToList()[0].tenphongban.ToString();
                            }
                        }
                            
                    }

                    string nguoncm1 = json_tiendo_giatri[i]["nguonchungminh1"].ToString().Trim();
                    string nguonchungminh1 = "";
                    if (nguoncm1.Trim() != "")
                    {
                        string[] chuoicm1 = nguoncm1.Split(',');
                        for (int m = 0; m < chuoicm1.Length; m++)
                        {
                            if (lstResult_phongban.Where(p => p.maphongban == chuoicm1[m].ToString()).ToList().Count() > 0)
                            {
                                if (m < chuoicm1.Length - 1)
                                    nguonchungminh1 = nguonchungminh1 + lstResult_phongban.Where(p => p.maphongban == chuoicm1[m].ToString()).ToList()[0].tenphongban.ToString() + "<br/>";
                                else
                                    nguonchungminh1 = nguonchungminh1 + lstResult_phongban.Where(p => p.maphongban == chuoicm1[m].ToString()).ToList()[0].tenphongban.ToString();
                            }
                        }

                    }

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["stt"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["muctieu"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["trongso"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["tieuchidanhgia"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["cachtinh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ngayghinhanketqua"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");


                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + nguonchungminh + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + nguonchungminh1 + "</td>");

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["donvitinh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["kehoach"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["thuchien"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["tilehoanthanh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ketqua"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ghichu"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }
                string strEncryptCode = linkname.Trim() + makpicongty + "0070pXQSeNsQRuzoCmUYfuX" + "&mailnguoigui=" + mailnguoigui + "&capdoduyet=1";// + "&kpichinhsua=" + kpichinhsua + "&mact=" + mact + "sQRu17zoCm5687AVoiGXX" + "&kpichinhsuact=" + kpichinhsuact;
                
                sb.Append("</table>");

                string chuoidy = "width:200px;height: 40px;background-color:#0090d9;border-radius:15px;text-align:center;color:#fff; margin: 0 8px 0 0;text-decoration: none; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);";
                string chuoikdy = "width:200px;height: 40px;background-color:red;border-radius:15px;text-align:center;color:#fff; margin: 0 8px 0 0;text-decoration: none; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);";

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:0px; font-size:22px; height :40px; line-height:51px; padding-top:5px;'><a href='" + strEncryptCode + "&dy=1' style='" + chuoidy + "'>&nbsp;&nbsp;Đồng ý&nbsp;&nbsp;</a>&nbsp;&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2' style='" + chuoikdy + "'>&nbsp;&nbsp;Không đồng ý&nbsp;&nbsp;</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, mailphotonggiamdoc);
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Gửi KPI năm", System.Text.Encoding.UTF8);
                message.Subject = "Gửi KPI năm";
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
        public JsonResult Save_guimail(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiLevelCompanyServices services = new KpiLevelCompanyServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_KpiLevelCompany(DataJson, nguoitao, "1", "0", "0");
            if (iresult != "-1")
            {
                string kq = send_Mail(iresult, DataJson);
                if (kq == "1")
                    return Json(new { success = true, makpicongty = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                else
                {
                    services.Save_KpiLevelCompany_nguoiguihoso(iresult);
                    return Json(new { success = 1, makpicongty = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                } 
            }
            else
            {
                return Json(new { success = false, makpicongty = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ExportLicensing()
        {
            string makpicongty = this.Request.Url.Segments[3];
            string avvvaa = "xslt/Fileinhangthang/";
            string filepacth = Server.MapPath(avvvaa).Replace("KpiCompany\\ExportLicensing\\", "");

            List<KpiLevelCompanyModels> lstaddstaff = new List<KpiLevelCompanyModels>();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            lstaddstaff = service.SelectRows_KpiLevelCompany_hieuchinh(makpicongty);
            List<SetCellValue> list = new List<SetCellValue>();
            if (lstaddstaff.Count > 0)
            {
                list.Add(new SetCellValue { RowIndex = 3, ColumnIndex = 2, Value = "Ngày ban hành: " + lstaddstaff[0].ngaybanhanh });
                list.Add(new SetCellValue { RowIndex = 3, ColumnIndex = 6, Value = "Ngày cập nhật: " + lstaddstaff[0].ngaycapnhat });
                list.Add(new SetCellValue { RowIndex = 3, ColumnIndex = 10, Value = "Lần cập nhật: " + lstaddstaff[0].lancapnhat });
            }
            List<KpiLevelCompanyDetailModels> lstResult = service.SelectRows_KpiLevelCompanyDetail_hieuchinh(makpicongty);
            DanhmucServices servicedm = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = servicedm.SelectRows(parampb);
            //var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            DataTable tblPrint = new DataTable();
            tblPrint.Columns.Add("stt");
            tblPrint.Columns.Add("muctieu");
            tblPrint.Columns.Add("trongso");
            tblPrint.Columns.Add("tieuchidanhgia");
            tblPrint.Columns.Add("cachtinh");
            tblPrint.Columns.Add("ngayghinhanketqua");
            tblPrint.Columns.Add("nguonchungminh");
            // tblPrint.Columns.Add("nguonchungminh1");
            tblPrint.Columns.Add("donvitinh");
            tblPrint.Columns.Add("kehoach");
            tblPrint.Columns.Add("thuchien");
            tblPrint.Columns.Add("tilehoanthanh");
            tblPrint.Columns.Add("ketqua");
            tblPrint.Columns.Add("ghichu");
            foreach (var item in lstResult)
            {
                DataRow tmp = tblPrint.NewRow();
                tblPrint.Rows.Add(tmp);
                tmp["stt"] = item.stt.Trim();
                tmp["muctieu"] = item.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                tmp["trongso"] = item.trongso.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                tmp["tieuchidanhgia"] = item.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                tmp["cachtinh"] = item.cachtinh;
                tmp["ngayghinhanketqua"] = item.ngayghinhanketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                string nguonchungminh = "";
                if (item.nguonchungminh != "")
                {
                    string[] ncm = item.nguonchungminh.Split(',');
                    for (int i = 0; i < ncm.Length; i++)
                    {
                        var pbcm = lstResult_phongban.Where(p => p.maphongban == ncm[0].ToString()).ToList();
                        if (pbcm.Count > 0 && i < ncm.Length - 1)
                            nguonchungminh = nguonchungminh + pbcm[0].tenphongban + ";";
                        else if (pbcm.Count > 0 && i == ncm.Length - 1)
                            nguonchungminh = nguonchungminh + pbcm[0].tenphongban;
                    }
                }
                tmp["nguonchungminh"] = nguonchungminh;
                //tmp["nguonchungminh1"] = item.nguonchungminh1;
                tmp["donvitinh"] = item.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                tmp["kehoach"] = item.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                tmp["thuchien"] = item.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                tmp["tilehoanthanh"] = item.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                tmp["ketqua"] = item.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                tmp["ghichu"] = item.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&");
            }

            string tenfile = "KPICONGTY.xls";
            string filedownload = DateTime.Now.ToString("ddMMyyyyHHmmss") + "KPICONGTY.xls";
            string fileName = filepacth + filedownload;
            ExportToExcel export = new ExportToExcel(Functions.GetTemplateFileName(tenfile, filepacth));
            export.TemplateExportKPICTY(tblPrint, 5, 0, list, fileName);

            var FileVirtualPath = "~/xslt/Fileinhangthang/" + filedownload;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));

        }


        #endregion

        #region TRANG CHỦ KPI CẤP PHÒNG BAN

        public ActionResult Index_Department()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            lstResult_phongban = new List<PhongBanModels>();
            lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            string pb ="";
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim().ToLower() == "1")
            {
                pb = "<option value=0>Chọn phòng ban</option>";
                foreach (var item in lstResult_phongban.Where(p=>p.phongban_congtruong=="0"))
                {
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p=>p.maphongban==maphongban))
                {
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }
                
            ViewBag.sbphongban = pb.ToString();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            lstphongban[0].xoa = DateTime.Now.Year.ToString("0000");
            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiLevelDepartment(KpiLevelDepartmentModels model, int curentPage)
        {
            KpiLevelDepartmentModels param = new KpiLevelDepartmentModels();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();

            string grouptk =  Session["grouptk"].ToString().Trim();
            string Sessionid = Session["userid"].ToString().Trim();
            param.maphongban = model.maphongban;
            param.tinhtrangduyet = model.tinhtrangduyet;
            param.timkiemnam = model.timkiemnam;

            int tongsodong = service.CountRows_KpiLevelDepartment(param, grouptk, Sessionid);
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

            List<KpiLevelDepartmentModels> lstResult = new List<KpiLevelDepartmentModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_KpiLevelDepartment(param, trangbd, trangkt, grouptk, Sessionid);
            }
            else if (curentPage != 1 && curentPage > sotrang) curentPage = curentPage - 1;
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            int tongdong = 0;
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = trangbd;
                string bankiemsoatduyet = "";
                foreach (var item in lstResult)
                {
                    strSTT = i.ToString();

                    if (item.bankiemsoat_duyet.Trim() == "1")
                        bankiemsoatduyet = "Ban KPI đã chuyển";
                    else if (item.bankiemsoat_duyet.Trim() == "2")
                        bankiemsoatduyet = "Ban KPI trả lại";
                    else bankiemsoatduyet = "Chờ Ban KPI xem xét";

                    if (item.nguoilapkpi_dagui.Trim() == "1")
                        item.nguoilapkpi_dagui_text = "Đã gửi HS/" + bankiemsoatduyet;
                    else if (item.nguoilapkpi_dagui.Trim() == "0")
                        item.nguoilapkpi_dagui_text = "Chưa gửi HS";

                    else if (item.nguoilapkpi_dagui.Trim() == "2")
                        item.nguoilapkpi_dagui_text = "Y/C chỉnh sữa KPI";

                    if (item.photongxemxetkpi_duyet.Trim() == "1")
                        item.photongxemxetkpi_duyet_text = "Đã Duyệt";
                    else if (item.photongxemxetkpi_duyet.Trim() == "2")
                        item.photongxemxetkpi_duyet_text = "Không Duyệt";
                    else if (item.photongxemxetkpi_duyet.Trim() == "0")
                        item.photongxemxetkpi_duyet_text = "Chờ Duyệt";

                    if (item.tonggiamdocxemxetkpi_duyet.Trim() == "1")
                        item.tonggiamdocxemxetkpi_duyet_text = "Đã Duyệt";
                    else if (item.tonggiamdocxemxetkpi_duyet.Trim() == "2")
                        item.tonggiamdocxemxetkpi_duyet_text = "Không Duyệt";
                    else if (item.tonggiamdocxemxetkpi_duyet.Trim() == "0")
                        item.tonggiamdocxemxetkpi_duyet_text = "Chờ Duyệt";

                    sbRows.Append(PrepareDataJson_KpiLevelDepartment(item, strSTT));
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

        private StringBuilder PrepareDataJson_KpiLevelDepartment(KpiLevelDepartmentModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.makpiphongban.ToString(), function.ReadXMLGetKeyEncrypt());
            string style = "";
            if (model.photongxemxetkpi_duyet == "2" || model.bankiemsoat_duyet == "2" || model.tonggiamdocxemxetkpi_duyet == "2") { style = "background-color:#ff6b6b;"; }
            else if (model.photongxemxetkpi_duyet == "0" || model.bankiemsoat_duyet == "0" || model.tonggiamdocxemxetkpi_duyet == "0") { style = "background-color:#71ff71;"; }
            else if (model.tonggiamdocxemxetkpi_duyet == "1") { style = "background-color:#cacbd0;"; }

            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' makpiphongban='" + strEncryptCode + "' phongban_congtruong='" + 1 + "'/>", model.makpiphongban);
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
               
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Department_Edit", "KpiCompany", new { makpiphongban = model.makpiphongban.ToString() }) + "'title='QP04FM13'>" + "QP04FM13" + "</a>\"");
                sbResult.Append("},");

                // Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Department_Edit", "KpiCompany", new { makpiphongban = model.makpiphongban.ToString() }) + "'title='" + model.tenphongban + "'>" + model.tenphongban + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Department_Edit", "KpiCompany", new { makpiphongban = model.makpiphongban.ToString() }) + "'title='" + model.ngaybanhanh + "'>" + model.ngaybanhanh + "</a>\"");
                sbResult.Append("},");

                //Tên đon vi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Department_Edit", "KpiCompany", new { makpiphongban = model.makpiphongban.ToString() }) + "'title='" + model.ngaycapnhat + "'>" + model.ngaycapnhat + "</a>\"");
                sbResult.Append("},");


                //Trình trạng hồ sơ
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col13\",");
                sbResult.Append("\"col_id\":\"13\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.nguoilapkpi_dagui_text + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");

                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi_ngay + "\"");
                sbResult.Append("},");


                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col11\",");
                sbResult.Append("\"col_id\":\"11\",");

                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");


                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col12\",");
                sbResult.Append("\"col_id\":\"12\",");
                //sbResult.Append("\"title\":\"" + model.ngayhoc + "\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi_ngay + "\"");
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

        // THÊM MỚI KPI CẤP PHÒNG BAN.
        public ActionResult Kpi_Level_Department_New()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            string pb = ""; string captrentructiep = ""; string captrentructiep_email = "";
           
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                var lstcaptrentt = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                if (lstcaptrentt.Count>0)
                {
                    pb = "<option value=" + lstcaptrentt[0].maphongban + ">" + lstcaptrentt[0].tenphongban + "</option>";
                    captrentructiep = lstcaptrentt[0].hotenquanly;
                    captrentructiep_email = lstcaptrentt[0].ghichu;
                }
                foreach (var item in lstResult_phongban.Where(p => p.maphongban != maphongban && p.phongban_congtruong=="0"))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
                {
                    captrentructiep = item.hotenquanly;
                    captrentructiep_email = item.ghichu;
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }

            ViewBag.sbphongban = pb.ToString();
            StringBuilder sbNguoiThucHien = new StringBuilder();
            foreach (var item in lstResult_phongban.Where(p => p.phongban_congtruong == "0"))
            {
                sbNguoiThucHien.Append(string.Format("<option value={0}>{1}</option>", item.maphongban, item.tenphongban));
            }
            ViewBag.nguoiThucHiens = sbNguoiThucHien.ToString();

            List<thongtinnhanvienModels> tblthongtinnhanvien = new List<thongtinnhanvienModels>();
            tblthongtinnhanvien.Add(new thongtinnhanvienModels() { hoten = Session["fullname"].ToString(), mailnhanvien = Session["thudientu"].ToString().Trim(),  captrentructiep = captrentructiep, captrentructiep_email = captrentructiep_email });
            return View(tblthongtinnhanvien[0]);

            //return View(lstResult_phongban[0]);
        }

        //Load du lieu kpi cap công ty (có trực thuộc phòng ban nào) xuống phòng ban đó.
        [HttpPost]
        public JsonResult SelectRows_LoadKPIfromCompanyDetail_theophongban(KpiLevelCompanyDetailModels model)
        {
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            string maphongban = Session["maphongban"].ToString().Trim();
            List<KpiLevelCompanyDetailModels> lstResult = service.SelectRows_LoadKPIfromCompanyDetail_theophongban(maphongban);
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                //lstResult_xinnghiphep_manv.Add(new TableModels { ngayxinnghitu = ngaynghitu.ToString("dd/MM/yyyy"), nghiphep = "TS/2" });
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    item1.congty = "1";
                    sbRows.Append(PrepareDataJson_LoadKPIfromCompanyDetail_theophongban(item1, strSTT));
                    if (item1.stt == "A" || item1.stt == "B" || item1.stt == "C" || item1.stt == "D")
                    {
                        if (lstResult.Where(p => p.thuoccap.Trim() == item1.stt).Count() == 1)
                        {
                            item1.stt = "1"; item1.muctieu = ""; item1.trongso = ""; item1.tieuchidanhgia = ""; item1.cachtinh = "";
                            item1.ngayghinhanketqua = ""; item1.nguonchungminh = ""; item1.donvitinh = ""; item1.kehoach = "";
                            item1.thuchien = ""; item1.tilehoanthanh = ""; item1.ketqua = ""; item1.ghichu = ""; item1.congty = "0";
                            sbRows.Append(PrepareDataJson_LoadKPIfromCompanyDetail_theophongban(item1, strSTT));
                        }
                    }
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

        private StringBuilder PrepareDataJson_LoadKPIfromCompanyDetail_theophongban(KpiLevelCompanyDetailModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style = "";

                if (model.stt.ToUpper() == "A") { group = "1"; kpicha = "1"; style = "background-color:#f9fd0d;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "B") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "C") { group = "3"; kpicha = "1"; style = "background-color:#5edcf1;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "D") { group = "4"; kpicha = "1"; style = "background-color:#90abe8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "") { style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "A") group = "1";
                else if (model.thuoccap.ToUpper() == "B") group = "2";
                else if (model.thuoccap.ToUpper() == "C") group = "3";
                else if (model.thuoccap.ToUpper() == "D") group = "4";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpicongty_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpicongty + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpiphongban_chitiet" + "\", \"value\":\"" + 0 + "\"},{\"name\":\"" + "makpiphongban" + "\", \"value\":\"" + 0 + "\"},{\"name\":\"" + "makpicongty_chitiet" + "\", \"value\":\"" + model.makpicongty_chitiet + "\"},{\"name\":\"" + "makpicongty" + "\", \"value\":\"" + model.makpicongty + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"},{\"name\":\"" + "congty" + "\", \"value\":\"" + model.congty + "\"},{\"name\":\"" + "sttcon" + "\", \"value\":\"" + model.stt + "\"},{\"name\":\"" + "sttcha" + "\", \"value\":\"" + 1 + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                sbResult.Append("},");

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt == "")
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + model.muctieu + "\"");
                    sbResult.Append("},");

                    //trong so
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='0%' ></input>" + "\"");
                    sbResult.Append("},");

                    //Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    // Don vi tinh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");


                    //phong ban
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='0%' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("}");

                    #endregion
                }
                else
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    if(model.muctieu.ToString()!="")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='true' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else
                        sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");

                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tieuchidanhgia + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.cachtinh + "' ></input>" + "\"");
                   // sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh + "</textarea>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ngayghinhanketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    //sbResult.Append("\"col_value\":\"" + "<select  id='filter01' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Người thực hiện' class='chosen-select-nguoithuchien'  name='nguonchungminh' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' ></textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add-child'>AddChild</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                    sbResult.Append("}");

                    #endregion
                }

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
        public JsonResult Save_Department(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiLevelCompanyServices services = new KpiLevelCompanyServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_KpiLevelDepartment(DataJson, nguoitao, "0", "0", "0");
            if (iresult != "0")
            {
                return Json(new { success = true, makpiphongban = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                JObject json = JObject.Parse(DataJson);
                iresult = json["data1"]["makpiphongban"].ToString();
                return Json(new { success = false, makpiphongban = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
        }


        //Gui mail cho ban kiểm soát KPI trước
        public string send_MailBanKiemSoatKPI(string makpiphongban, string DataJson)
        {
            _logger.Start("send_Mail");
            try
            {
                JObject json = JObject.Parse(DataJson);
                string mailnguoigui = json["data1"]["nguoilapkpi_email"].ToString();
                string mailbankiemsoat = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user_bkskpi"));//json["data1"]["bankiemsoat_email"].ToString();
                string namkpi = json["data1"]["nam"].ToString().Trim(); //"2018";
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetKPIDepartmentL1/?makpi=";
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 5px; padding: 0; width: 100%; font-size: 1em;color:black;'>"); //margin: 0 auto;  de canh giua
                sb.Append("<table cellpadding='0' cellspacing='0' width='100%' >");
                sb.Append("<tr>");

                sb.Append("<td>");
                sb.Append("<div style='width:400px; text-align:center;font-weight: bold;float:left;line-height:45px'>");
                sb.Append("<p style= 'width:400px;text-align:center;font-size:18px;font-weight:bold;line-height:45px;padding-left:80px;float:left;'>KPI CẤP PHÒNG/BAN</p>");
                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</body>");

                sb.Append("<hr style=border: 1px solid #000; width: 100% />");

                sb.Append("<table style='width:100%; font-size:13px;'>");

                sb.Append("<tr><td rowspan='4' colspan='2' style='border: 1px solid #000;font-size: 16pt;color:#f3332e;text-align: center;'>NĂM KPI <br /> <span style='font-size: 29pt;color:#8e2bf1;text-align: center;'>" + namkpi + "</span></td>");

                //<th rowspan="4" colspan="2" class="boxEdit" style="font-size: 16pt;color:#f3332e;padding-top: 35px">NĂM KPI<textarea style="width:100%; font-size: 29pt; color:#8e2bf1; border: 0px; text-align: center; resize:none;" rows="1" placeholder="" id="txtnam" name="nam">@Model.nam</textarea></th>

                sb.Append("<td colspan='11' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP PHÒNG/BAN - KPI NĂM " + namkpi + "</td>");
                sb.Append("</tr>");

                sb.Append("<tr><td colspan='11' style='text-align: left;border: 1px solid #000;'>Công ty Cổ Phần Đầu Tư Xây Dựng Ricons:</td></tr>");

                var phongban = lstResult_phongban.Where(p => p.maphongban == json["data1"]["maphongban"].ToString().Trim()).ToList();
                string tenphongban = "";
                if (phongban.Count > 0)
                {
                    tenphongban = phongban[0].tenphongban;
                }


                sb.Append("<tr>");
                sb.Append("<td colspan='8' style='text-align: left;border: 1px solid #000;'>Phòng Ban: " + tenphongban + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Biểu mẫu: QP04FM13</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày ban hành: " + json["data1"]["ngaybanhanh"].ToString().Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày cập nhật: " + json["data1"]["ngaycapnhat"].ToString().Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Lần cập nhật: 00/01 </td>");
                sb.Append("</tr>");

                sb.Append("<tr style='margin: 0 auto;'>");
                sb.Append("<td style='border: 1px solid #000;width:3%;text-align:center'>Stt</td>");
                sb.Append("<td style='border: 1px solid #000;width:17%;text-align:center'>Mục tiêu</td>");
                sb.Append("<td style='border: 1px solid #000;width:5%;text-align:center'>Trọng số</td>");
                sb.Append("<td style='border: 1px solid #000;width:10%;text-align:center'>Tiêu chí đánh giá</td>");
                sb.Append("<td style='border: 1px solid #000;width:10%;text-align:center'>Cách tính</td>");
                sb.Append("<td style='border: 1px solid #000;width:8%;text-align:center'>Thời điểm ghi nhận kết quả</td>");
                sb.Append("<td style='border: 1px solid #000;width:9%;text-align:center'>Nguồn chứng minh</td>");
                sb.Append("<td style='border: 1px solid #000;width:5%;text-align:center'>Đơn vị tính</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>KPIs kế hoạch</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>KPIs thực hiện</td>");
                sb.Append("<td style='border: 1px solid #000;width:7%;text-align:center'>Tỉ lệ % hoàn thành KPIs</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>Kết quả</td>");
                sb.Append("<td style='border: 1px solid #000;width:8%;text-align:center'>Ghi chú</td>");
                sb.Append("</tr>");

                //string mact = ""; string kpichinhsuact = "";
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "A")
                        sb.Append("<tr style='background-color:#f9fd0d;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "B")
                        sb.Append("<tr style='background-color:#99f1b8;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "C")
                        sb.Append("<tr style='background-color:#5edcf1 ;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "D")
                        sb.Append("<tr style='background-color:#90abe8 ;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0;text-align:center'>");
                    else sb.Append("<tr>");

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["stt"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["muctieu"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["trongso"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["tieuchidanhgia"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["cachtinh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ngayghinhanketqua"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["nguonchungminh"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["donvitinh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["kehoach"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["thuchien"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["tilehoanthanh"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ketqua"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ghichu"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }
                string strEncryptCode = linkname.Trim() + makpiphongban + "0070pXQSeNsQRuzoCmUYfuX" + "&mailnguoigui=" + mailnguoigui + "&capdoduyet=1";// + "&kpichinhsua=" + kpichinhsua + "&mact=" + mact + "sQRu17zoCm5687AVoiGXX" + "&kpichinhsuact=" + kpichinhsuact;
                sb.Append("</table>");

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý Duyệt KPI</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, mailbankiemsoat);
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Gửi KPI Cấp Phòng", System.Text.Encoding.UTF8);
                message.Subject = "Gửi KPI Cấp Phòng";
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
        public JsonResult Save_Department_guimail(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiLevelCompanyServices services = new KpiLevelCompanyServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_KpiLevelDepartment(DataJson, nguoitao, "1", "0", "0");
            if (iresult != "-1")
            {
                string kq = send_MailBanKiemSoatKPI(iresult, DataJson);
                if (kq == "1")
                    return Json(new { success = true, makpicongty = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                else
                {
                    services.Save_KpiLevelDepartment_nguoiguihoso(iresult);
                    JObject json = JObject.Parse(DataJson);
                    iresult = json["data1"]["makpiphongban"].ToString();
                    return Json(new { success = false, makpiphongban = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                JObject json = JObject.Parse(DataJson);
                iresult = json["data1"]["makpiphongban"].ToString();
                return Json(new { success = false, makpiphongban = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Kpi_Level_Department_Edit(string makpiphongban)
        {
            if (!IsLogged())
                return BackToLogin();

            DanhmucServices servicedm = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = servicedm.SelectRows(parampb);

            List<KpiLevelDepartmentModels> lstKpidepartment = new List<KpiLevelDepartmentModels>();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            lstKpidepartment = service.SelectRows_KpiLevelDepartment_hieuchinh(makpiphongban);

            StringBuilder sbphongban = new StringBuilder();
            //string maphongban = Session["maphongban"].ToString().Trim();
            string pb = "";
            //if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim().ToLower() == "1")
            //{
            //    pb = "<option value=0>Chọn phòng ban</option>";
            //    foreach (var item in lstResult_phongban)
            //    {
            //        pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            //    }
            //}
            //else
            //{
            //    foreach (var item in lstResult_phongban.Where(p => p.maphongban == lstKpidepartment[0].maphongban.Trim()))
            //    {
            //        pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            //    }
            //}
            foreach (var item in lstResult_phongban.Where(p => p.maphongban == lstKpidepartment[0].maphongban.Trim()))
            {
                pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }

            ViewBag.sbphongban = pb.ToString();
            return View(lstKpidepartment[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiLevelDepartmentDetail_hieuchinh(KpiLevelDepartmentDetailModels model)
        {
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            List<KpiLevelDepartmentDetailModels> lstResult = service.SelectRows_KpiLevelDepartmentDetail_hieuchinh(model.makpiphongban.ToString());
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_KpiLevelDepartmentDetail_hieuchinh(item1, strSTT));
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

        private StringBuilder PrepareDataJson_KpiLevelDepartmentDetail_hieuchinh(KpiLevelDepartmentDetailModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style = "";

                if (model.stt.ToUpper() == "A") { group = "1"; kpicha = "1"; style = "background-color:#f9fd0d;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "B") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "C") { group = "3"; kpicha = "1"; style = "background-color:#5edcf1;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "D") { group = "4"; kpicha = "1"; style = "background-color:#90abe8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "") { style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "A") group = "1";
                else if (model.thuoccap.ToUpper() == "B") group = "2";
                else if (model.thuoccap.ToUpper() == "C") group = "3";
                else if (model.thuoccap.ToUpper() == "D") group = "4";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpiphongban_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpiphongban + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpiphongban_chitiet" + "\", \"value\":\"" + model.makpiphongban_chitiet + "\"},{\"name\":\"" + "makpiphongban" + "\", \"value\":\"" + model.makpiphongban + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "sttcon" + "\", \"value\":\"" + model.sttcon + "\"},{\"name\":\"" + "sttcha" + "\", \"value\":\"" + model.sttcha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                sbResult.Append("},");

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt == "")
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + model.muctieu + "\"");
                    sbResult.Append("},");

                    //trong so
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    // Don vi tinh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");


                    //phong ban
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    //string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("}");

                    #endregion
                }
                else
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");

                    if(model.congty=="1")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='true' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else
                        sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");

                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tieuchidanhgia + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.cachtinh + "' ></input>" + "\"");
                    // sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh + "</textarea>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");

                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ngayghinhanketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Nguồn chứng minh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    //sbResult.Append("\"col_value\":\"" + "<select  id='filter01' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Người thực hiện' class='chosen-select-nguoithuchien'  name='nguonchungminh' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' >" + model.nguonchungminh + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                    sbResult.Append("}");

                    #endregion
                }



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

        #endregion


        #region  TRANG CHỦ KPI CẤP CÁ NHÂN PHÓ TỔNG GIÁM ĐỐC - GIÁM ĐỐC KHỐI, GIÁM ĐỐC DỰ ÁN, TRƯỞNG PHÒNG BAN
        public ActionResult Index_Du_Director()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            lstResult_phongban = new List<PhongBanModels>();
            lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            string machucdanhkpi = Session["chucdanhkpi"].ToString().Trim();
            string pb = "";
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                pb = "<option value=0>Chọn phòng ban</option>";
                foreach (var item in lstResult_phongban)
                {
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p=>p.maphongban==maphongban))
                {
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }
            ViewBag.sbphongban = pb.ToString();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            lstphongban[0].xoa = machucdanhkpi;
            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiLevelDuDirectorTp(KpiLevelDuDirectorTpModels model, int curentPage)
        {
            KpiLevelDuDirectorTpModels param = new KpiLevelDuDirectorTpModels();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            param.nguoitao = int.Parse(Session["userid"].ToString());

            string grouptk = Session["grouptk"].ToString().Trim();
            string Sessionid = Session["userid"].ToString().Trim();
            param.maphongban = model.maphongban;
            param.thongtintimkiem = model.thongtintimkiem;
            int tongsodong = service.CountRows_KpiLevelDuDirectorTp(param, grouptk, Sessionid);
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
            if (curentPage != 1 && curentPage != 0 && curentPage <= sotrang)
            {
                trangbd = (trangkt * (curentPage - 1)) + 1;
                trangkt = trangkt * curentPage;
            }

            List<KpiLevelDuDirectorTpModels> lstResult = new List<KpiLevelDuDirectorTpModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_KpiLevelDuDirectorTp(param, trangbd, trangkt, grouptk, Sessionid);
            }
            else if (curentPage != 1 && curentPage > sotrang) curentPage = curentPage - 1;
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            int tongdong = 0;
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = trangbd;
                string bankiemsoatduyet = "";
                foreach (var item in lstResult)
                {
                    strSTT = i.ToString();
                    if (item.bankiemsoat_duyet.Trim() == "1")
                        bankiemsoatduyet = "Ban KPI đã duyệt";
                    else if (item.bankiemsoat_duyet.Trim() == "2")
                        bankiemsoatduyet = "Ban KPI trả lại";
                    else bankiemsoatduyet = "Chờ Ban KPI duyệt";

                    if (item.nguoilapkpi_dagui.Trim() == "1")
                        item.nguoilapkpi_dagui_text = "Đã gửi HS/" + bankiemsoatduyet;
                    else if (item.nguoilapkpi_dagui.Trim() == "0")
                        item.nguoilapkpi_dagui_text = "Chưa gửi HS";

                    if (item.chucdanh.Trim() == "6" || item.chucdanh.Trim() == "7" || item.chucdanh.Trim() == "8" || item.maphongban.Trim() == "67")
                    {
                        item.photongxemxetkpi = "x";
                        item.photongxemxetkpi_duyet_text = "x";
                        item.photongxemxetkpi_ngay = "x";
                    }
                    else
                    {
                        if (item.photongxemxetkpi_duyet.Trim() == "1")
                            item.photongxemxetkpi_duyet_text = "Đã Duyệt";
                        else if (item.photongxemxetkpi_duyet.Trim() == "2")
                            item.photongxemxetkpi_duyet_text = "Không Duyệt";
                        else if (item.photongxemxetkpi_duyet.Trim() == "0")
                            item.photongxemxetkpi_duyet_text = "Chờ Duyệt";
                    }

                    if (item.tonggiamdocxemxetkpi_duyet.Trim() == "1")
                        item.tonggiamdocxemxetkpi_duyet_text = "Đã Duyệt";
                    else if (item.tonggiamdocxemxetkpi_duyet.Trim() == "2")
                        item.tonggiamdocxemxetkpi_duyet_text = "Không Duyệt";
                    else if (item.tonggiamdocxemxetkpi_duyet.Trim() == "0")
                        item.tonggiamdocxemxetkpi_duyet_text = "Chờ Duyệt";

                    sbRows.Append(PrepareDataJson_KpiLevelDuDirectorTp(item, strSTT));
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

        private StringBuilder PrepareDataJson_KpiLevelDuDirectorTp(KpiLevelDuDirectorTpModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.makpicanhancap.ToString(), function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' makpicanhancap='" + strEncryptCode + "' phongban_congtruong='" + 1 + "'/>", model.makpicanhancap);
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

                ////Mã đơn vị
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col3\",");
                //sbResult.Append("\"col_id\":\"3\",");
                //string machucdanhkpi ="";
                //if (model.chucdanh == "2") machucdanhkpi = "QP04FM16"; else machucdanhkpi = "QP04FM15";
                //sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Du_Director_Edit", "KpiCompany", new { makpicanhancap = model.makpicanhancap.ToString() }) + "'title=" + machucdanhkpi + ">" + machucdanhkpi + "</a>\"");
                //sbResult.Append("},");

                // Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Du_Director_Edit", "KpiCompany", new { makpicanhancap = model.makpicanhancap.ToString() }) + "'title='" + model.tenphongban + "'>" + model.tenphongban + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Du_Director_Edit", "KpiCompany", new { makpicanhancap = model.makpicanhancap.ToString() }) + "'title='" + model.manhanvien + "'>" + model.manhanvien + "</a>\"");
                sbResult.Append("},");

                // Họ và tên nhân viên soạn kpi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Du_Director_Edit", "KpiCompany", new { makpicanhancap = model.makpicanhancap.ToString() }) + "'title='" + model.hovaten + "'>" + model.hovaten + "</a>\"");
                sbResult.Append("},");


                //Tên chức danh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"title\":\"" + model.chucdanh + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Du_Director_Edit", "KpiCompany", new { makpicanhancap = model.makpicanhancap.ToString() }) + "'title='" + model.tenchucdanh + "'>" + model.tenchucdanh + "</a>\"");
                sbResult.Append("},");

                //Ngày đăng ký
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Du_Director_Edit", "KpiCompany", new { makpicanhancap = model.makpicanhancap.ToString() }) + "'title='" + model.ngaydangky + "'>" + model.ngaydangky + "</a>\"");
                sbResult.Append("},");

                // Ngày đánh giá
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + model.ngaydanhgia + "\"");
                sbResult.Append("},");

                //Tình trạng HOSo
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                sbResult.Append("\"col_value\":\"" + model.nguoilapkpi_dagui_text + "\"");
                sbResult.Append("},");

                // Họ tên phó giám đốc
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col11\",");
                sbResult.Append("\"col_id\":\"11\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi + "\"");
                sbResult.Append("},");

                //Đã duyệt hay chưa duyệt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col12\",");
                sbResult.Append("\"col_id\":\"12\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");

                //Đã duyệt hay chưa duyệt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col13\",");
                sbResult.Append("\"col_id\":\"13\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi_ngay + "\"");
                sbResult.Append("},");

                //Đã duyệt hay chưa duyệt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col14\",");
                sbResult.Append("\"col_id\":\"14\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi + "\"");
                sbResult.Append("},");

                //Đã duyệt hay chưa duyệt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col15\",");
                sbResult.Append("\"col_id\":\"15\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");

                //Ngày duyệt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col16\",");
                sbResult.Append("\"col_id\":\"16\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi_ngay + "\"");
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

        public ActionResult Kpi_Level_Du_Director_New()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb); //chucdanhkpi
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            string machucdanhkpi = Session["chucdanhkpi"].ToString().Trim();
            string mailcanhan = Session["thudientu"].ToString().Trim();
            string pb = ""; string captrentructiep = ""; string captrentructiep_email = "";
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                var lstcaptrentt = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                if (lstcaptrentt.Count>0)
                {
                    pb = "<option value=" + lstcaptrentt[0].maphongban + ">" + lstcaptrentt[0].tenphongban + "</option>";
                    captrentructiep = lstcaptrentt[0].hotenquanly;
                    captrentructiep_email = lstcaptrentt[0].ghichu;
                }
                foreach (var item in lstResult_phongban.Where(p => p.maphongban != maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
                {
                    captrentructiep = item.hotenquanly;
                    captrentructiep_email = item.ghichu;
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }

            if (machucdanhkpi == "6" || machucdanhkpi == "7" || machucdanhkpi == "8" || maphongban == "67")
            { captrentructiep = "Lê Miên Thụy"; captrentructiep_email = "thuy.lemien@ricons.vn"; }


            ViewBag.sbphongban = pb.ToString();
            ChucDanhModels param = new ChucDanhModels();
            List<ChucDanhModels> lstchucdanh = service.SelectRows_chucvu(param);
           
            string chucdanhs = ""; string tenchucdanhkpi = "";
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                var lstchucdanhht = lstchucdanh.Where(p => p.machucdanh.ToString().Trim() == machucdanhkpi).ToList();
                if (lstchucdanhht.Count > 0)
                {
                    tenchucdanhkpi = lstchucdanhht[0].tenchucdanh.ToUpper();
                    chucdanhs = "<option value=" + lstchucdanhht[0].machucdanh + ">" + lstchucdanhht[0].tenchucdanh + "</option>";
                }
                foreach (var item in lstchucdanh.Where(p => p.machucdanh.ToString().Trim() != machucdanhkpi))
                {
                    chucdanhs = chucdanhs + "<option value=" + item.machucdanh + "> " + item.tenchucdanh + " </option>";
                }
            }
            else
            {
                foreach (var item in lstchucdanh.Where(p => p.machucdanh.ToString() == machucdanhkpi))
                {
                    tenchucdanhkpi = item.tenchucdanh.ToUpper();
                    chucdanhs = chucdanhs + "<option value=" + item.machucdanh + "> " + item.tenchucdanh + " </option>";
                }
            }
            ViewBag.sbchucdanh = chucdanhs.ToString();
            List<thongtinnhanvienModels> tblthongtinnhanvien = new List<thongtinnhanvienModels>();
            tblthongtinnhanvien.Add(new thongtinnhanvienModels() { manhanvien = Session["manhansu"].ToString(), hoten = Session["fullname"].ToString(), mailnhanvien=mailcanhan, maphongban = maphongban, chucdanhkpi = machucdanhkpi, tenchucdanhkpi = tenchucdanhkpi, captrentructiep = captrentructiep, captrentructiep_email = captrentructiep_email });
            return View(tblthongtinnhanvien[0]);
        }
        //Load du lieu kpi cấp công ty (có trực thuộc ban giám đốc xuống) Lấy dữ liệu từ cấp cty.
        [HttpPost]
        public JsonResult SelectRows_LoadKPIfromCompanyDetail_photong_gdkhoi_gdduan_tp(KpiLevelCompanyDetailModels model)
        {
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            string maphongban = Session["maphongban"].ToString().Trim();
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            string machucdanhkpi = Session["chucdanhkpi"].ToString().Trim();
            if (machucdanhkpi == "6" || machucdanhkpi == "7" || machucdanhkpi == "8")
            {
                List<KpiLevelCompanyDetailModels> lstResult = new List<KpiLevelCompanyDetailModels>();
                lstResult = service.SelectRows_LoadKPIfromCompanyDetail_theophongban(maphongban);
                if (lstResult.Count > 0)
                {
                    string strSTT = "";
                    int i = 1;
                    foreach (var item1 in lstResult) // lay du lieu cap cong ty
                    {
                        strSTT = i.ToString();
                        sbRows.Append(PrepareDataJson_LoadKPIfromCompanyDetail__photong_gdkhoi_gdduan_tp(item1, strSTT));
                        if (item1.stt == "A" || item1.stt == "B" || item1.stt == "C" || item1.stt == "D")
                        {
                            if (lstResult.Where(p => p.thuoccap.Trim() == item1.stt).Count() == 1)
                            {
                                item1.stt = "1"; item1.muctieu = ""; item1.trongso = ""; item1.tieuchidanhgia = ""; item1.cachtinh = "";
                                item1.ngayghinhanketqua = ""; item1.nguonchungminh = ""; item1.donvitinh = ""; item1.kehoach = "";
                                item1.thuchien = ""; item1.tilehoanthanh = ""; item1.ketqua = ""; item1.ghichu = "";
                                sbRows.Append(PrepareDataJson_LoadKPIfromCompanyDetail__photong_gdkhoi_gdduan_tp(item1, strSTT));
                            }
                        }
                        i++;
                    }
                    if (sbRows.Length > 0)
                        sbRows.Remove(sbRows.Length - 1, 1);
                }
            }
            else if (machucdanhkpi == "2" || maphongban=="67") // Lấy dữ liệu phòng
            {
                List<KpiLevelDepartmentDetailModels> lstResult = new List<KpiLevelDepartmentDetailModels>();
                lstResult.Add(new KpiLevelDepartmentDetailModels() { makpiphongban = 0, makpiphongban_chitiet = 0, stt = "A", muctieu = "TÀI CHÍNH", trongso = "0%", thuoccap="A" });
                lstResult.Add(new KpiLevelDepartmentDetailModels() { makpiphongban = 0, makpiphongban_chitiet = 0, stt = "B", muctieu = "KHÁCH HÀNG", trongso = "0%", thuoccap = "B" });
                lstResult.Add(new KpiLevelDepartmentDetailModels() { makpiphongban = 0, makpiphongban_chitiet = 0, stt = "C", muctieu = "VẬN HÀNH", trongso = "0%", thuoccap = "C" });
                lstResult.Add(new KpiLevelDepartmentDetailModels() { makpiphongban = 0, makpiphongban_chitiet = 0, stt = "D", muctieu = "PHÁT TRIỂN ĐỔI MỚI", trongso = "0%", thuoccap = "D" });
                lstResult.Add(new KpiLevelDepartmentDetailModels() { makpiphongban = 0, makpiphongban_chitiet = 0, stt = "", muctieu = "TỔNG CỘNG", trongso = "0%", thuoccap = "" });
                if (lstResult.Count > 0)
                {
                    string strSTT = "";
                    int i = 1;
                    foreach (var item1 in lstResult)
                    {
                        strSTT = i.ToString();
                        sbRows.Append(PrepareDataJson_LoadKPIfromCompanyDetail_truongphong(item1, strSTT));
                        if (item1.stt == "A" || item1.stt == "B" || item1.stt == "C" || item1.stt == "D")
                        {
                            item1.stt = "1"; item1.muctieu = ""; item1.trongso = ""; item1.tieuchidanhgia = ""; item1.cachtinh = "";
                            item1.ngayghinhanketqua = ""; item1.nguonchungminh = ""; item1.donvitinh = ""; item1.kehoach = "";
                            item1.thuchien = ""; item1.tilehoanthanh = ""; item1.ketqua = ""; item1.ghichu = "";
                            sbRows.Append(PrepareDataJson_LoadKPIfromCompanyDetail_truongphong(item1, strSTT));
                        }
                        i++;
                    }
                    if (sbRows.Length > 0)
                        sbRows.Remove(sbRows.Length - 1, 1);
                }
            }
            
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            sbResult.Append("\"Pages\":\"" + "212" + "\",");
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");
            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_LoadKPIfromCompanyDetail__photong_gdkhoi_gdduan_tp(KpiLevelCompanyDetailModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style = "";

                if (model.stt.ToUpper() == "A") { group = "1"; kpicha = "1"; style = "background-color:#f9fd0d;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "B") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "C") { group = "3"; kpicha = "1"; style = "background-color:#5edcf1;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "D") { group = "4"; kpicha = "1"; style = "background-color:#90abe8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "") { style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "A") group = "1";
                else if (model.thuoccap.ToUpper() == "B") group = "2";
                else if (model.thuoccap.ToUpper() == "C") group = "3";
                else if (model.thuoccap.ToUpper() == "D") group = "4";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpicongty_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpicongty + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpicanhancap_chitiet" + "\", \"value\":\"" + 0 + "\"},{\"name\":\"" + "makpicanhancap" + "\", \"value\":\"" + 0 + "\"},{\"name\":\"" + "makpicongty_chitiet" + "\", \"value\":\"" + model.makpicongty_chitiet + "\"},{\"name\":\"" + "makpicongty" + "\", \"value\":\"" + model.makpicongty + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                sbResult.Append("},");

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt == "")
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + model.muctieu + "\"");
                    sbResult.Append("},");

                    //trong so
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='0%' ></input>" + "\"");
                    sbResult.Append("},");

                    //Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    // Don vi tinh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");


                    //phong ban
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='0%' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("}");

                    #endregion
                }
                else
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");

                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tieuchidanhgia + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.cachtinh + "' ></input>" + "\"");
                    // sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh + "</textarea>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");

                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ngayghinhanketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //lstResult_phongban
                    //string nguoncm = model.nguonchungminh; string tenphongban = "";
                    //for (int i = 0; i < nguoncm.Split(',').Length; i++)
                    //{
                    //    var phongban = lstResult_phongban.Where(p => p.maphongban == nguoncm[i].ToString()).ToList();
                    //    if (phongban.Count > 0)
                    //        tenphongban = tenphongban + phongban[0].tenphongban;

                    //    nguoncm = "Ban giám đốc";
                    //}

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    //sbResult.Append("\"col_value\":\"" + "<select  id='filter01' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Người thực hiện' class='chosen-select-nguoithuchien'  name='nguonchungminh' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' ></textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                    sbResult.Append("}");

                    #endregion
                }

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

        private StringBuilder PrepareDataJson_LoadKPIfromCompanyDetail_truongphong(KpiLevelDepartmentDetailModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style = "";

                if (model.stt.ToUpper() == "A") { group = "1"; kpicha = "1"; style = "background-color:#f9fd0d;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "B") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "C") { group = "3"; kpicha = "1"; style = "background-color:#5edcf1;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "D") { group = "4"; kpicha = "1"; style = "background-color:#90abe8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "") { style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "A") group = "1";
                else if (model.thuoccap.ToUpper() == "B") group = "2";
                else if (model.thuoccap.ToUpper() == "C") group = "3";
                else if (model.thuoccap.ToUpper() == "D") group = "4";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpiphongban_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpiphongban + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpicanhancap_chitiet" + "\", \"value\":\"" + 0 + "\"},{\"name\":\"" + "makpicanhancap" + "\", \"value\":\"" + 0 + "\"},{\"name\":\"" + "makpiphongban_chitiet" + "\", \"value\":\"" + model.makpiphongban_chitiet + "\"},{\"name\":\"" + "makpiphongban" + "\", \"value\":\"" + model.makpiphongban + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                sbResult.Append("},");

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt == "")
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + model.muctieu + "\"");
                    sbResult.Append("},");

                    //trong so
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='0%' ></input>" + "\"");
                    sbResult.Append("},");

                    //Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    // Don vi tinh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");


                    //phong ban
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='0%' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("}");

                    #endregion
                }
                else
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' ></textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a> <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-Search'>Xem</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                    sbResult.Append("}");

                    #endregion
                }

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
        
        public ActionResult Kpi_Level_Du_Director_Edit(string makpicanhancap)
        {
            if (!IsLogged())
                return BackToLogin();
            List<KpiLevelDuDirectorTpModels> KpiLevelDuDirectorTp = new List<KpiLevelDuDirectorTpModels>();
            KpiLevelCompanyServices servicetp = new KpiLevelCompanyServices();
            KpiLevelDuDirectorTp = servicetp.SelectRows_KpiLevelDuDirectorTp_hieuchinh(makpicanhancap);
            
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb); //chucdanhkpi
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = KpiLevelDuDirectorTp[0].maphongban.Trim();//Session["maphongban"].ToString().Trim();
            string pb = "";
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                var lstcaptrentt = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                if (lstcaptrentt.Count > 0)
                    pb = "<option value=" + lstcaptrentt[0].maphongban + ">" + lstcaptrentt[0].tenphongban + "</option>";
                foreach (var item in lstResult_phongban.Where(p => p.maphongban != maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            ViewBag.sbphongban = pb.ToString();
            ChucDanhModels param = new ChucDanhModels();
            List<ChucDanhModels> lstchucdanh = service.SelectRows_chucvu(param);
            string machucdanhkpi = KpiLevelDuDirectorTp[0].chucdanh.Trim();
            string chucdanhs = ""; 
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                var lstchucdanhht = lstchucdanh.Where(p => p.machucdanh.ToString().Trim() == machucdanhkpi).ToList();
                if (lstchucdanhht.Count > 0)
                {
                    chucdanhs = "<option value=" + lstchucdanhht[0].machucdanh + ">" + lstchucdanhht[0].tenchucdanh + "</option>";
                }
                foreach (var item in lstchucdanh.Where(p => p.machucdanh.ToString().Trim() != machucdanhkpi))
                {
                    chucdanhs = chucdanhs + "<option value=" + item.machucdanh + "> " + item.tenchucdanh + " </option>";
                }
            }
            else
            {
                foreach (var item in lstchucdanh.Where(p => p.machucdanh.ToString() == machucdanhkpi))
                {
                    chucdanhs = chucdanhs + "<option value=" + item.machucdanh + "> " + item.tenchucdanh + " </option>";
                }
            }
            if (Session["grouptk"].ToString().Trim() == "1")
            {
                KpiLevelDuDirectorTp[0].xoa = "0";
            }
            else
            {
                KpiLevelDuDirectorTp[0].xoa = "";
            }

            ViewBag.sbchucdanh = chucdanhs.ToString();
            KpiLevelDuDirectorTp[0].namkpi = KpiLevelDuDirectorTp[0].ngaydangky.Split('/')[2].ToString();
            return View(KpiLevelDuDirectorTp[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiLevelDuDirectorTpDetail_hieuchinh(KpiLevelDuDirectorTpDetailModels model, string chucdanh)
        {
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            List<KpiLevelDuDirectorTpDetailModels> lstResult = service.SelectRows_KpiLevelDuDirectorTpDetail_hieuchinh(model.makpicanhancap.ToString());
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_KpiLevelDuDirectorTpDetail_hieuchinh(item1, strSTT, chucdanh));
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

        private StringBuilder PrepareDataJson_KpiLevelDuDirectorTpDetail_hieuchinh(KpiLevelDuDirectorTpDetailModels model, string couter,string chucdanh)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style = "";

                if (model.stt.ToUpper() == "A") { group = "1"; kpicha = "1"; style = "background-color:#f9fd0d;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "B") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "C") { group = "3"; kpicha = "1"; style = "background-color:#5edcf1;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "D") { group = "4"; kpicha = "1"; style = "background-color:#90abe8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "") { style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "A") group = "1";
                else if (model.thuoccap.ToUpper() == "B") group = "2";
                else if (model.thuoccap.ToUpper() == "C") group = "3";
                else if (model.thuoccap.ToUpper() == "D") group = "4";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpicanhancap_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpicanhancap + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpicanhancap_chitiet" + "\", \"value\":\"" + model.makpicanhancap_chitiet + "\"},{\"name\":\"" + "makpicanhancap" + "\", \"value\":\"" + model.makpicanhancap + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                sbResult.Append("},");

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt == "")
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + model.muctieu + "\"");
                    sbResult.Append("},");

                    //trong so
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    // Don vi tinh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");


                    //phong ban
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    //string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("}");

                    #endregion
                }
                else
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");

                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tieuchidanhgia + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.cachtinh + "' ></input>" + "\"");
                    // sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh + "</textarea>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");

                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ngayghinhanketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Nguồn chứng minh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    //sbResult.Append("\"col_value\":\"" + "<select  id='filter01' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Người thực hiện' class='chosen-select-nguoithuchien'  name='nguonchungminh' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' >" + model.nguonchungminh + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "";
                    if (chucdanh=="2")
                        strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a> <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-Search'>Xem</a>";
                    else
                        strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                    sbResult.Append("}");

                    #endregion
                }

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
        public JsonResult Save_Du_Director_Tp(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiLevelCompanyServices services = new KpiLevelCompanyServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_Du_Director_Tp(DataJson, nguoitao, "0", "0", "0");
            if (iresult != "0")
            {
                return Json(new { success = true, makpicanhancap = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpicanhancap = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        //Gui mail cho ban kiểm soát KPI trước
        public string send_MailBanKiemSoatKPI_Canhancaptruongphong(string makpicanhancap, string DataJson)
        {
            _logger.Start("send_Mail");
            try
            {
                JObject json = JObject.Parse(DataJson);
                //string chucdanh = json["data1"]["nguoilapkpi_email"].ToString().Trim();
                string mailnguoigui = json["data1"]["nguoilapkpi_email"].ToString().Trim();
                string namkpi = json["data1"]["ngaydangky"].ToString().Split('/')[2];//lstResult[0].ngaydangky.Trim().Split('/')[2];
                string tenchucdanhkpi = "";

                string chucdanh = json["data1"]["chucdanh"].ToString().Trim();
                ChucDanhModels param = new ChucDanhModels();
                DanhmucServices service = new DanhmucServices();
                List<ChucDanhModels> lstchucdanh = service.SelectRows_chucvu(param);
                var lstchucdanhht = lstchucdanh.Where(p => p.machucdanh.ToString().Trim() == chucdanh).ToList();
                 if (lstchucdanhht.Count > 0)
                 {
                     tenchucdanhkpi = lstchucdanhht[0].tenchucdanh.ToUpper();
                 }

                string mailbankiemsoat = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user_bkskpi"));//json["data1"]["bankiemsoat_email"].ToString();
                //string namkpi = json["data1"]["nam"].ToString().Trim(); //"2018";
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetKPIDu_DirectorL1/?makpi=";
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 5px; padding: 0; width: 100%; font-size: 1em;color:black;'>"); //margin: 0 auto;  de canh giua
                sb.Append("<table cellpadding='0' cellspacing='0' width='100%' >");
                sb.Append("<tr>");

                sb.Append("<td>");
                sb.Append("<div style='width:400px; text-align:center;font-weight: bold;float:left;line-height:45px'>");
                sb.Append("<p style= 'width:400px;text-align:center;font-size:18px;font-weight:bold;line-height:45px;padding-left:80px;float:left;'>KPI CẤP CÁ NHÂN</p>");
                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</body>");

                sb.Append("<hr style=border: 1px solid #000; width: 100% />");

                sb.Append("<table style='width:100%; font-size:13px;'>");

                sb.Append("<tr><td rowspan='3' colspan='2' style='border: 1px solid #000;font-size: 16pt;color:#f3332e;text-align: center;'></td>"); //NĂM KPI <br /> <span style='font-size: 29pt;color:#8e2bf1;text-align: center;'>" + namkpi + "</span>
                //<th rowspan="4" colspan="2" class="boxEdit" style="font-size: 16pt;color:#f3332e;padding-top: 35px">NĂM KPI<textarea style="width:100%; font-size: 29pt; color:#8e2bf1; border: 0px; text-align: center; resize:none;" rows="1" placeholder="" id="txtnam" name="nam">@Model.nam</textarea></th>
                sb.Append("<td colspan='11' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP CÁ NHÂN " + tenchucdanhkpi.ToUpper() + " - KPI NĂM " + namkpi + "</td>");
                sb.Append("</tr>");

                //sb.Append("<tr><td colspan='11' style='text-align: left;border: 1px solid #000;'>Công ty Cổ Phần Đầu Tư Xây Dựng Ricons:</td></tr>");

                var phongban = lstResult_phongban.Where(p => p.maphongban == json["data1"]["maphongban"].ToString().Trim()).ToList();
                string tenphongban = "";
                if (phongban.Count > 0)
                {
                    tenphongban = phongban[0].tenphongban;
                }


                sb.Append("<tr>");
                sb.Append("<td colspan='8' style='text-align: left;border: 1px solid #000;'>Phòng Ban: " + tenphongban + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Biểu mẫu: QP04FM13</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày ban hành: " + json["data1"]["ngaybanhanh"].ToString().Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày cập nhật: " + json["data1"]["ngaycapnhat"].ToString().Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Lần cập nhật: 00/01 </td>");
                sb.Append("</tr>");

                sb.Append("<tr style='margin: 0 auto;'>");
                sb.Append("<td style='border: 1px solid #000;width:3%;text-align:center'>Stt</td>");
                sb.Append("<td style='border: 1px solid #000;width:17%;text-align:center'>Mục tiêu</td>");
                sb.Append("<td style='border: 1px solid #000;width:5%;text-align:center'>Trọng số</td>");
                sb.Append("<td style='border: 1px solid #000;width:10%;text-align:center'>Tiêu chí đánh giá</td>");
                sb.Append("<td style='border: 1px solid #000;width:10%;text-align:center'>Cách tính</td>");
                sb.Append("<td style='border: 1px solid #000;width:8%;text-align:center'>Thời điểm ghi nhận kết quả</td>");
                sb.Append("<td style='border: 1px solid #000;width:9%;text-align:center'>Nguồn chứng minh</td>");
                sb.Append("<td style='border: 1px solid #000;width:5%;text-align:center'>Đơn vị tính</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>KPIs kế hoạch</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>KPIs thực hiện</td>");
                sb.Append("<td style='border: 1px solid #000;width:7%;text-align:center'>Tỉ lệ % hoàn thành KPIs</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>Kết quả</td>");
                sb.Append("<td style='border: 1px solid #000;width:8%;text-align:center'>Ghi chú</td>");
                sb.Append("</tr>");

                //string mact = ""; string kpichinhsuact = "";
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "A")
                        sb.Append("<tr style='background-color:#f9fd0d;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "B")
                        sb.Append("<tr style='background-color:#99f1b8;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "C")
                        sb.Append("<tr style='background-color:#5edcf1 ;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "D")
                        sb.Append("<tr style='background-color:#90abe8 ;text-align:center'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0;text-align:center'>");
                    else sb.Append("<tr>");

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["stt"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["muctieu"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["trongso"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["tieuchidanhgia"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["cachtinh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ngayghinhanketqua"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["nguonchungminh"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["donvitinh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["kehoach"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["thuchien"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["tilehoanthanh"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ketqua"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ghichu"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }
                string strEncryptCode = linkname.Trim() + makpicanhancap + "0070pXQSeNsQRuzoCmUYfuX" + "&mailnguoigui=" + mailnguoigui + "&capdoduyet=1";// + "&kpichinhsua=" + kpichinhsua + "&mact=" + mact + "sQRu17zoCm5687AVoiGXX" + "&kpichinhsuact=" + kpichinhsuact;
                sb.Append("</table>");

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý Duyệt KPI</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, mailbankiemsoat);
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Gửi KPI Cấp cá nhân", System.Text.Encoding.UTF8);
                message.Subject = "Gửi KPI Cấp cá nhân";
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
        public JsonResult Save_Du_Director_Tp_Guimail(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiLevelCompanyServices services = new KpiLevelCompanyServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_Du_Director_Tp(DataJson, nguoitao, "1", "0", "0");
            if (iresult != "0")
            {
                string kq = send_MailBanKiemSoatKPI_Canhancaptruongphong(iresult, DataJson);
                if (kq == "1")
                    return Json(new { success = true, makpicanhancap = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                else
                {
                    services.Save_KpiLevelDepartment_nguoiguihoso(iresult);
                    return Json(new { success = 1, makpicanhancap = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, makpicanhancap = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region TRANG CHỦ KPI CẤP CÔNG TRƯỜNG.

        public ActionResult Index_Construction()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            lstResult_phongban = new List<PhongBanModels>();
            lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            string pb = "";
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim().ToLower() == "1")
            {
                pb = "<option value=0>Chọn phòng ban</option>";
                foreach (var item in lstResult_phongban.Where(p=>p.phongban_congtruong=="1"))
                {
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
                {
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }
            ViewBag.sbphongban = pb.ToString();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            lstphongban[0].xoa = DateTime.Now.Year.ToString("0000");
            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiLevelConstruction(KpiLevelDepartmentModels model, int curentPage)
        {
            KpiLevelDepartmentModels param = new KpiLevelDepartmentModels();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();

            string grouptk = Session["grouptk"].ToString().Trim();
            string Sessionid = Session["userid"].ToString().Trim();
            param.maphongban = model.maphongban;
            param.tinhtrangduyet = model.tinhtrangduyet;
            param.timkiemnam = model.timkiemnam;

            int tongsodong = service.CountRows_KpiLevelConstruction(param, grouptk, Sessionid);
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

            List<KpiLevelDepartmentModels> lstResult = new List<KpiLevelDepartmentModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_KpiLevelConstruction(param, trangbd, trangkt, grouptk, Sessionid);
            }
            else if (curentPage != 1 && curentPage > sotrang) curentPage = curentPage - 1;
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            int tongdong = 0;
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = trangbd;
                string bankiemsoatduyet = "";
                foreach (var item in lstResult)
                {
                    strSTT = i.ToString();

                    if (item.bankiemsoat_duyet.Trim() == "1")
                        bankiemsoatduyet = "Ban KPI đã chuyển";
                    else if (item.bankiemsoat_duyet.Trim() == "2")
                        bankiemsoatduyet = "Ban KPI trả lại";
                    else bankiemsoatduyet = "Chờ Ban KPI xem xét";

                    if (item.nguoilapkpi_dagui.Trim() == "1")
                        item.nguoilapkpi_dagui_text = "Đã gửi HS/" + bankiemsoatduyet;
                    else if (item.nguoilapkpi_dagui.Trim() == "0")
                        item.nguoilapkpi_dagui_text = "Chưa gửi HS";

                    else if (item.nguoilapkpi_dagui.Trim() == "2")
                        item.nguoilapkpi_dagui_text = "Y/C chỉnh sữa KPI";

                    if (item.photongxemxetkpi_duyet.Trim() == "1")
                        item.photongxemxetkpi_duyet_text = "Đã Duyệt";
                    else if (item.photongxemxetkpi_duyet.Trim() == "2")
                        item.photongxemxetkpi_duyet_text = "Không Duyệt";
                    else if (item.photongxemxetkpi_duyet.Trim() == "0")
                        item.photongxemxetkpi_duyet_text = "Chờ Duyệt";

                    if (item.tonggiamdocxemxetkpi_duyet.Trim() == "1")
                        item.tonggiamdocxemxetkpi_duyet_text = "Đã Duyệt";
                    else if (item.tonggiamdocxemxetkpi_duyet.Trim() == "2")
                        item.tonggiamdocxemxetkpi_duyet_text = "Không Duyệt";
                    else if (item.tonggiamdocxemxetkpi_duyet.Trim() == "0")
                        item.tonggiamdocxemxetkpi_duyet_text = "Chờ Duyệt";

                    sbRows.Append(PrepareDataJson_KpiLevelConstruction(item, strSTT));
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

        private StringBuilder PrepareDataJson_KpiLevelConstruction(KpiLevelDepartmentModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.makpiphongban.ToString(), function.ReadXMLGetKeyEncrypt());
            string style = "";
            if (model.photongxemxetkpi_duyet == "2" || model.bankiemsoat_duyet == "2" || model.tonggiamdocxemxetkpi_duyet == "2") { style = "background-color:#ff6b6b;"; }
            else if (model.photongxemxetkpi_duyet == "0" || model.bankiemsoat_duyet == "0" || model.tonggiamdocxemxetkpi_duyet == "0") { style = "background-color:#71ff71;"; }
            else if (model.tonggiamdocxemxetkpi_duyet == "1") { style = "background-color:#cacbd0;"; }

            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' makpiphongban='" + strEncryptCode + "' phongban_congtruong='" + 1 + "'/>", model.makpiphongban);
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

                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Construction_Edit", "KpiCompany", new { makpiphongban = model.makpiphongban.ToString() }) + "'title='QP04FM13'>" + "QP04FM13" + "</a>\"");
                sbResult.Append("},");

                // Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Construction_Edit", "KpiCompany", new { makpiphongban = model.makpiphongban.ToString() }) + "'title='" + model.tenphongban + "'>" + model.tenphongban + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Construction_Edit", "KpiCompany", new { makpiphongban = model.makpiphongban.ToString() }) + "'title='" + model.ngaybanhanh + "'>" + model.ngaybanhanh + "</a>\"");
                sbResult.Append("},");

                //Tên đon vi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Level_Construction_Edit", "KpiCompany", new { makpiphongban = model.makpiphongban.ToString() }) + "'title='" + model.ngaycapnhat + "'>" + model.ngaycapnhat + "</a>\"");
                sbResult.Append("},");


                //Trình trạng hồ sơ
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col13\",");
                sbResult.Append("\"col_id\":\"13\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.nguoilapkpi_dagui_text + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");

                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + model.photongxemxetkpi_ngay + "\"");
                sbResult.Append("},");


                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                //sbResult.Append("\"title\":\"" + model.sodienthoai + "\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col11\",");
                sbResult.Append("\"col_id\":\"11\",");

                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");


                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col12\",");
                sbResult.Append("\"col_id\":\"12\",");
                //sbResult.Append("\"title\":\"" + model.ngayhoc + "\",");
                sbResult.Append("\"col_value\":\"" + model.tonggiamdocxemxetkpi_ngay + "\"");
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

        // THÊM MỚI KPI CẤP CÔNG TRƯỜNG.
        public ActionResult Kpi_Level_Construction_New()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            string pb = ""; string captrentructiep = ""; string captrentructiep_email = "";

            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                var lstcaptrentt = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                if (lstcaptrentt.Count > 0)
                {
                    pb = "<option value=" + lstcaptrentt[0].maphongban + ">" + lstcaptrentt[0].tenphongban + "</option>";
                    captrentructiep = lstcaptrentt[0].hotenquanly;
                    captrentructiep_email = lstcaptrentt[0].ghichu;
                }
                foreach (var item in lstResult_phongban.Where(p => p.maphongban != maphongban && p.phongban_congtruong=="1"))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban && p.phongban_congtruong == "1"))
                {
                    captrentructiep = item.hotenquanly;
                    captrentructiep_email = item.ghichu;
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }

            ViewBag.sbphongban = pb.ToString();
            //StringBuilder sbNguoiThucHien = new StringBuilder();
            //foreach (var item in lstResult_phongban.Where(p => p.phongban_congtruong=="1"))
            //{
            //    sbNguoiThucHien.Append(string.Format("<option value={0}>{1}</option>", item.maphongban, item.tenphongban));
            //}
            //ViewBag.nguoiThucHiens = sbNguoiThucHien.ToString();

            List<thongtinnhanvienModels> tblthongtinnhanvien = new List<thongtinnhanvienModels>();
            tblthongtinnhanvien.Add(new thongtinnhanvienModels() { hoten = Session["fullname"].ToString(), mailnhanvien = Session["thudientu"].ToString().Trim(), captrentructiep = captrentructiep, captrentructiep_email = captrentructiep_email });
            return View(tblthongtinnhanvien[0]);

            //return View(lstResult_phongban[0]);
        }

        //Load danh mục lúc thêm mới
        [HttpPost]
        public JsonResult SelectRows_KpiLevelConstruction_Item(KpiLevelDepartmentDetailModels model)
        {
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            List<KpiLevelDepartmentDetailModels> lstResult = service.SelectRows_KpiLevelConstruction_Item();
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_KpiLevelConstruction_Item(item1, strSTT));
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

        private StringBuilder PrepareDataJson_KpiLevelConstruction_Item(KpiLevelDepartmentDetailModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style = "";

                if (model.stt.ToUpper() == "A") { group = "1"; kpicha = "1"; style = "background-color:#f9fd0d;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "B") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "C") { group = "3"; kpicha = "1"; style = "background-color:#5edcf1;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "D") { group = "4"; kpicha = "1"; style = "background-color:#90abe8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "E") { group = "5"; kpicha = "1"; style = "background-color:#c497ec;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "F") { group = "6"; kpicha = "1"; style = "background-color:#eaf35e;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "") { style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "A") group = "1";
                else if (model.thuoccap.ToUpper() == "B") group = "2";
                else if (model.thuoccap.ToUpper() == "C") group = "3";
                else if (model.thuoccap.ToUpper() == "D") group = "4";
                else if (model.thuoccap.ToUpper() == "E") group = "5";
                else if (model.thuoccap.ToUpper() == "F") group = "6";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpiphongban_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpiphongban + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpiphongban_chitiet" + "\", \"value\":\"" + model.makpiphongban_chitiet + "\"},{\"name\":\"" + "makpiphongban" + "\", \"value\":\"" + model.makpiphongban + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "sttcon" + "\", \"value\":\"" + model.sttcon + "\"},{\"name\":\"" + "sttcha" + "\", \"value\":\"" + model.sttcha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                sbResult.Append("},");

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt.ToUpper() == "E" || model.stt.ToUpper() == "F" || model.stt == "")
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + model.muctieu + "\"");
                    sbResult.Append("},");

                    //trong so
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    // Don vi tinh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");


                    //phong ban
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    //string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("}");

                    #endregion
                }
                else
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");

                    if (model.congty == "1")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='true' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else
                        sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");

                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tieuchidanhgia + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.cachtinh + "' ></input>" + "\"");
                    // sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh + "</textarea>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");

                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ngayghinhanketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Nguồn chứng minh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    //sbResult.Append("\"col_value\":\"" + "<select  id='filter01' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Người thực hiện' class='chosen-select-nguoithuchien'  name='nguonchungminh' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' >" + model.nguonchungminh + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 17px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 17px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                    sbResult.Append("}");

                    #endregion
                }
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
        public JsonResult Save_Construction(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiLevelCompanyServices services = new KpiLevelCompanyServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_KpiLevelConstruction(DataJson, nguoitao, "0", "0", "0");
            if (iresult != "0")
            {
                return Json(new { success = true, makpiphongban = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpiphongban = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save_Construction_guimail(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiLevelCompanyServices services = new KpiLevelCompanyServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_KpiLevelConstruction(DataJson, nguoitao, "1", "0", "0");
            if (iresult != "-1")
            {
                string kq = send_MailBanKiemSoatKPI(iresult, DataJson);
                if (kq == "1")
                    return Json(new { success = true, makpicongty = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                else
                {
                    services.Save_KpiLevelDepartment_nguoiguihoso(iresult);
                    return Json(new { success = 1, makpicongty = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, makpicongty = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Kpi_Level_Construction_Edit(string makpiphongban)
        {
            if (!IsLogged())
                return BackToLogin();

            DanhmucServices servicedm = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = servicedm.SelectRows(parampb);

            List<KpiLevelDepartmentModels> lstKpidepartment = new List<KpiLevelDepartmentModels>();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            lstKpidepartment = service.SelectRows_KpiLevelDepartment_hieuchinh(makpiphongban);

            StringBuilder sbphongban = new StringBuilder();
            //string maphongban = Session["maphongban"].ToString().Trim();
            string pb = "";
            //if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim().ToLower() == "1")
            //{
            //    pb = "<option value=0>Chọn phòng ban</option>";
            //    foreach (var item in lstResult_phongban)
            //    {
            //        pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            //    }
            //}
            //else
            //{
            //    foreach (var item in lstResult_phongban.Where(p => p.maphongban == lstKpidepartment[0].maphongban.Trim()))
            //    {
            //        pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            //    }
            //}
            foreach (var item in lstResult_phongban.Where(p => p.maphongban == lstKpidepartment[0].maphongban.Trim()))
            {
                pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }

            ViewBag.sbphongban = pb.ToString();
            return View(lstKpidepartment[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiLevelConstructionDetail_hieuchinh(KpiLevelDepartmentDetailModels model)
        {
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            List<KpiLevelDepartmentDetailModels> lstResult = service.SelectRows_KpiLevelDepartmentDetail_hieuchinh(model.makpiphongban.ToString());
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_KpiLevelConstructionDetail_hieuchinh(item1, strSTT));
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

        private StringBuilder PrepareDataJson_KpiLevelConstructionDetail_hieuchinh(KpiLevelDepartmentDetailModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style = "";

                if (model.stt.ToUpper() == "A") { group = "1"; kpicha = "1"; style = "background-color:#f9fd0d;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "B") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "C") { group = "3"; kpicha = "1"; style = "background-color:#5edcf1;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "D") { group = "4"; kpicha = "1"; style = "background-color:#90abe8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "E") { group = "5"; kpicha = "1"; style = "background-color:#c497ec;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "F") { group = "6"; kpicha = "1"; style = "background-color:#eaf35e;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "") { style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "A") group = "1";
                else if (model.thuoccap.ToUpper() == "B") group = "2";
                else if (model.thuoccap.ToUpper() == "C") group = "3";
                else if (model.thuoccap.ToUpper() == "D") group = "4";
                else if (model.thuoccap.ToUpper() == "E") group = "5";
                else if (model.thuoccap.ToUpper() == "F") group = "6";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpiphongban_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpiphongban + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpiphongban_chitiet" + "\", \"value\":\"" + model.makpiphongban_chitiet + "\"},{\"name\":\"" + "makpiphongban" + "\", \"value\":\"" + model.makpiphongban + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "sttcon" + "\", \"value\":\"" + model.sttcon + "\"},{\"name\":\"" + "sttcha" + "\", \"value\":\"" + model.sttcha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                sbResult.Append("\"col_id\":\"1\",");
                sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                sbResult.Append("},");

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt.ToUpper() == "E" || model.stt.ToUpper() == "F" || model.stt == "")
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");
                    sbResult.Append("\"col_value\":\"" + model.muctieu + "\"");
                    sbResult.Append("},");

                    //trong so
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    // Don vi tinh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");


                    //phong ban
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    //string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("}");

                    #endregion
                }
                else
                {
                    #region
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col2\",");
                    sbResult.Append("\"col_id\":\"2\",");

                    if (model.congty == "1")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='true' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else
                        sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");

                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tieuchidanhgia + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.cachtinh + "' ></input>" + "\"");
                    // sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh + "</textarea>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");

                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ngayghinhanketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Nguồn chứng minh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    //sbResult.Append("\"col_value\":\"" + "<select  id='filter01' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Người thực hiện' class='chosen-select-nguoithuchien'  name='nguonchungminh' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' >" + model.nguonchungminh + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col13\",");
                    sbResult.Append("\"col_id\":\"13\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ghichu + "' ></input>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col14\",");
                    sbResult.Append("\"col_id\":\"14\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Attachment + "\"");
                    sbResult.Append("}");

                    #endregion
                }



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

        #endregion



    }
}