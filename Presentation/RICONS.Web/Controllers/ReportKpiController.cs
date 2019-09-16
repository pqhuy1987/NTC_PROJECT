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
using System.Text.RegularExpressions;

namespace RICONS.Web.Controllers
{
    public class ReportKpiController : BaseController
    {
        #region Fields
        Log4Net _logger = new Log4Net(typeof(ReportKpiController));
        public static List<PhongBanModels> lstResult_phongban = new List<PhongBanModels>();
        public static List<ChucDanhModels> lstResult_chucdanh = new List<ChucDanhModels>();
        #endregion

        public ActionResult Index_month()
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
                var lstpban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                if (lstpban.Count() > 0)
                {
                    pb = "<option value="+0+">" + "Lựa chọn phòng ban" + "</option>";
                    pb = pb + "<option value=" + lstpban[0].maphongban + ">" + lstpban[0].tenphongban + "</option>";
                }
                    
                foreach (var item in lstResult_phongban.Where(p => p.maphongban != maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            ViewBag.sbphongban = pb.ToString();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            lstphongban[0].xoa = machucdanhkpi;
            lstphongban[0].sodienthoai = DateTime.Now.ToString("MM/yyyy");
            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiReport_Month(KpiReport_MonthModels model, int curentPage)
        {
            KpiReport_MonthModels param = new KpiReport_MonthModels();
            ReportKpiServices service = new ReportKpiServices();
            param.nguoitao = int.Parse(Session["userid"].ToString());
            param.chucdanh = Session["chucdanhkpi"].ToString().Trim();
            param.hovaten = model.hovaten;
            param.maphongban = model.maphongban;
            param.thangnam = model.thangnam;
            param.kiemtra = model.kiemtra;
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                param.xoa = "1";
            }
            int tongsodong = service.CountRows_ReportKpi_Month(param);
            int sotrang = 1;
            if (tongsodong > 21)
            {
                if (tongsodong % 21 > 0)
                {
                    sotrang = (tongsodong / 21) + 1;
                }
                else
                {
                    sotrang = (tongsodong / 21);
                }
            }
            int trangbd = 1; int trangkt = 21;
            if (curentPage != 1 && curentPage <= sotrang)
            {
                trangbd = (trangkt * (curentPage - 1)) + 1;
                trangkt = trangkt * curentPage;
            }
            List<KpiReport_MonthModels> lstResult = new List<KpiReport_MonthModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_ReportKpi_Month(param, trangbd, trangkt);
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
                    sbRows.Append(PrepareDataJson_KpiReport_Month(item, strSTT));
                    if (item.groupcha != "1")
                        i++;
                    else i=1;
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

        private StringBuilder PrepareDataJson_KpiReport_Month(KpiReport_MonthModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.matonghopketqua.ToString(), function.ReadXMLGetKeyEncrypt());
            string style = "";
            if (model.groupcha.ToString().Trim() == "1") { style = "background-color:#719bef;font-weight: bold;font-size: 13px;"; }
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"},{\"name\":\"" + "groupcha" + "\", \"value\":\"" + model.groupcha + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell

                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' matonghopketqua='" + model.matonghopketqua + "' phongban_congtruong='" + 1 + "'/>", model.matonghopketqua);
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1\",");
                sbResult.Append("\"col_id\":\"1\",");
                if (model.groupcha == "1")
                    sbResult.Append("\"col_value\":\"" +""+ "\"");
                else sbResult.Append("\"col_value\":\"" + strHTML_Checkbox + "\"");
                sbResult.Append("},");
                //stt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col2 stt\",");
                sbResult.Append("\"col_id\":\"2\",");
                if(model.groupcha.Trim() =="1")
                    sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                else sbResult.Append("\"col_value\":\"" + couter + "\"");
                sbResult.Append("},");

                //Mã đơn vị
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col3\",");
                sbResult.Append("\"col_id\":\"3\",");
                
                sbResult.Append("\"col_value\":\"" + "<a href='" +"" + "'title=" + model.manhanvien + ">" + model.manhanvien + "</a>\"");
                sbResult.Append("},");

                // Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.hovaten + "'>" + model.hovaten + "</a>\"");
                sbResult.Append("},");

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.tenchucdanhns + "'>" + model.tenchucdanhns + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.hanhvithaido + "'>" + model.hanhvithaido + "</a>\"");
                sbResult.Append("},");

                // Họ và tên nhân viên soạn kpi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.giaiquyetcongviec + "'>" + model.giaiquyetcongviec + "</a>\"");
                sbResult.Append("},");


                //Tên chức danh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"title\":\"" + model.chucdanh + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.ketquakpi + "'>" + model.ketquakpi + "</a>\"");
                sbResult.Append("},");

                //Ngày đăng ký
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                sbResult.Append("\"title\":\"" + model.maphanloaiketqua + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.phanloaiketqua + "'>" + model.phanloaiketqua + "</a>\"");
                sbResult.Append("},");

                // Ngày đánh giá
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                if (model.groupcha.Trim() == "1")
                    sbResult.Append("\"col_value\":\"" +""+ "\"");
                else
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='2' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu + "</textarea>" + "\"");
                sbResult.Append("}");

               

                //Ngày duyệt
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col13\",");
                //sbResult.Append("\"col_id\":\"13\",");
                //sbResult.Append("\"col_value\":\"" + model.truongphongxemxetkpi_ngay + "\"");
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

        [HttpPost]
        public JsonResult Save_Kpi_Month(string DataJson)
        {
            JsonResult Data = new JsonResult();
            ReportKpiServices services = new ReportKpiServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_Kpi_Month(DataJson, nguoitao);
            if (iresult != "1")
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Index_EmployeeYear()
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
                var lstpban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                if (lstpban.Count() > 0)
                {
                    pb = "<option value=" + 0 + ">" + "Lựa chọn phòng ban" + "</option>";
                    pb = pb + "<option value=" + lstpban[0].maphongban + ">" + lstpban[0].tenphongban + "</option>";
                }

                foreach (var item in lstResult_phongban.Where(p => p.maphongban != maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            ViewBag.sbphongban = pb.ToString();
            var lstphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
            lstphongban[0].xoa = machucdanhkpi;
            lstphongban[0].sodienthoai = DateTime.Now.ToString("MM/yyyy");
            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiReport_Year(KpiReport_MonthModels model, int curentPage)
        {
            KpiReport_MonthModels param = new KpiReport_MonthModels();
            ReportKpiServices service = new ReportKpiServices();
            param.nguoitao = int.Parse(Session["userid"].ToString());
            param.chucdanh = Session["chucdanhkpi"].ToString().Trim();
            param.hovaten = model.hovaten;
            param.maphongban = model.maphongban;
            param.thangnam = model.thangnam;
            param.kiemtra = model.kiemtra;
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                param.xoa = "1";
            }
            int tongsodong = service.CountRows_ReportKpi_Year(param);
            int sotrang = 1;
            if (tongsodong > 21)
            {
                if (tongsodong % 21 > 0)
                {
                    sotrang = (tongsodong / 21) + 1;
                }
                else
                {
                    sotrang = (tongsodong / 21);
                }
            }
            int trangbd = 1; int trangkt = 21;
            if (curentPage != 1 && curentPage <= sotrang)
            {
                trangbd = (trangkt * (curentPage - 1)) + 1;
                trangkt = trangkt * curentPage;
            }
            List<KpiReport_MonthModels> lstResult = new List<KpiReport_MonthModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_ReportKpi_Month(param, trangbd, trangkt);
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
                    sbRows.Append(PrepareDataJson_KpiReport_Year(item, strSTT));
                    if (item.groupcha != "1")
                        i++;
                    else i = 1;
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

        private StringBuilder PrepareDataJson_KpiReport_Year(KpiReport_MonthModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.matonghopketqua.ToString(), function.ReadXMLGetKeyEncrypt());
            string style = "";
            if (model.groupcha.ToString().Trim() == "1") { style = "background-color:#719bef;font-weight: bold;font-size: 13px;"; }
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"},{\"name\":\"" + "groupcha" + "\", \"value\":\"" + model.groupcha + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell

                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' matonghopketqua='" + model.matonghopketqua + "' phongban_congtruong='" + 1 + "'/>", model.matonghopketqua);
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col1\",");
                sbResult.Append("\"col_id\":\"1\",");
                if (model.groupcha == "1")
                    sbResult.Append("\"col_value\":\"" + "" + "\"");
                else sbResult.Append("\"col_value\":\"" + strHTML_Checkbox + "\"");
                sbResult.Append("},");
                //stt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col2 stt\",");
                sbResult.Append("\"col_id\":\"2\",");
                if (model.groupcha.Trim() == "1")
                    sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                else sbResult.Append("\"col_value\":\"" + couter + "\"");
                sbResult.Append("},");

                //Mã đơn vị
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col3\",");
                sbResult.Append("\"col_id\":\"3\",");

                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title=" + model.manhanvien + ">" + model.manhanvien + "</a>\"");
                sbResult.Append("},");

                // Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.hovaten + "'>" + model.hovaten + "</a>\"");
                sbResult.Append("},");

                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col10\",");
                sbResult.Append("\"col_id\":\"10\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.tenchucdanhns + "'>" + model.tenchucdanhns + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.hanhvithaido + "'>" + model.hanhvithaido + "</a>\"");
                sbResult.Append("},");

                // Họ và tên nhân viên soạn kpi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.giaiquyetcongviec + "'>" + model.giaiquyetcongviec + "</a>\"");
                sbResult.Append("},");


                //Tên chức danh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"title\":\"" + model.chucdanh + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.ketquakpi + "'>" + model.ketquakpi + "</a>\"");
                sbResult.Append("},");

                //Ngày đăng ký
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                sbResult.Append("\"title\":\"" + model.maphanloaiketqua + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + "" + "'title='" + model.phanloaiketqua + "'>" + model.phanloaiketqua + "</a>\"");
                sbResult.Append("},");

                // Ngày đánh giá
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                if (model.groupcha.Trim() == "1")
                    sbResult.Append("\"col_value\":\"" + "" + "\"");
                else
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='2' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu + "</textarea>" + "\"");
                sbResult.Append("}");



                //Ngày duyệt
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col13\",");
                //sbResult.Append("\"col_id\":\"13\",");
                //sbResult.Append("\"col_value\":\"" + model.truongphongxemxetkpi_ngay + "\"");
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


    }
}