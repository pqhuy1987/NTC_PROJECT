using RICONS.Logger;
using Nop.Admin.Controllers;
using RICONS.Web.Data.Services;
using RICONS.Web.Models;
using RICONS.Core.Functions;
using ClosedXML.Excel;
using ExportHelper.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace RICONS.Web.Controllers
{
    public class KpiEmployeeController : BaseController
    {
        #region Fields
        Log4Net _logger = new Log4Net(typeof(KpiEmployeeController));
        public static List<PhongBanModels> lstResult_phongban = new List<PhongBanModels>();
        public static List<ChucDanhModels> lstResult_chucdanh = new List<ChucDanhModels>();
        #endregion

        #region  TRANG CHỦ KPI NHÂN VIÊN CÔNG TY RICONS

        public ActionResult Index_Employee()
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
                    pb = "<option value=" + lstpban[0].maphongban + ">" + lstpban[0].tenphongban + "</option>";
                pb = pb + "<option value=0>Chọn phòng ban</option>";
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
            return View(lstphongban[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiEmployee(KpiEmployeeModels model, int curentPage)
        {
            KpiEmployeeModels param = new KpiEmployeeModels();
            KpiEmployeeServices service = new KpiEmployeeServices();
            param.nguoitao = int.Parse(Session["userid"].ToString());
            param.chucdanh = Session["chucdanhkpi"].ToString().Trim();
            param.hovaten = model.hovaten;
            param.maphongban = model.maphongban;
            param.trinhtranghoso = model.trinhtranghoso;
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                param.xoa = "1";
            }
            int tongsodong = service.CountRows_KpiEmployee(param);
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

            List<KpiEmployeeModels> lstResult = new List<KpiEmployeeModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRow_KpiEmployee(param, trangbd, trangkt);
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

                    string chinhsua = "";
                    if (item.kpichinhsua.Trim() == "6") chinhsua = " (Đã duyệt chỉnh sữa)";
                    else if (item.kpichinhsua.Trim() == "2" || item.kpichinhsua.Trim() == "3" || item.kpichinhsua.Trim() == "4" || item.kpichinhsua.Trim() == "5") chinhsua = " (Đang sữa chờ duyệt)";
                    if (item.nguoilapkpi_dagui.Trim() == "1")
                        item.nguoilapkpi_dagui_text = "Đã gửi HS";
                    else if (item.nguoilapkpi_dagui.Trim() == "0")
                        item.nguoilapkpi_dagui_text = "Chưa gửi HS";

                    if (item.truongphongxemxetkpi_duyet.Trim() == "1" || item.truongphongxemxetkpi_duyet.Trim() == "5")
                        item.truongphongxemxetkpi_duyet_text = "Đã Duyệt" +chinhsua;
                    else if (item.truongphongxemxetkpi_duyet.Trim() == "2")
                        item.truongphongxemxetkpi_duyet_text = "Không Duyệt";
                    else if (item.truongphongxemxetkpi_duyet.Trim() == "0")
                        item.truongphongxemxetkpi_duyet_text = "Chờ Duyệt";

                    sbRows.Append(PrepareDataJson_Employee(item, strSTT));
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

        private StringBuilder PrepareDataJson_Employee(KpiEmployeeModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.makpinhanvien.ToString(), function.ReadXMLGetKeyEncrypt());

            string style = "";
            if (model.truongphongxemxetkpi_duyet == "2") { style = "background-color:#ff6b6b;"; }
            else if (model.truongphongxemxetkpi_duyet == "0") { style = "background-color:#71ff71;"; }
            else if (model.truongphongxemxetkpi_duyet == "1") { style = "background-color:#cacbd0;"; }


            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + strEncryptCode + "\"},{\"name\":\"" + "nguoilapkpi_dagui" + "\", \"value\":\"" + model.nguoilapkpi_dagui + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' nguoilapkpi_dagui='" + model.nguoilapkpi_dagui + "' makpinhanvien='" + strEncryptCode + "' phongban_congtruong='" + 1 + "'/>", model.makpinhanvien);
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

                string bmkpi = "QP04 FM18";
                //Mã đơn vị
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col3\",");
                sbResult.Append("\"col_id\":\"3\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Employee_Edit", "KpiEmployee", new { makpinhanvien = model.makpinhanvien.ToString() }) + "'title=" + bmkpi + ">" + bmkpi + "</a>\"");
                sbResult.Append("},");

                // Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Employee_Edit", "KpiEmployee", new { makpinhanvien = model.makpinhanvien.ToString() }) + "'title='" + model.tenphongban + "'>" + model.tenphongban + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Employee_Edit", "KpiEmployee", new { makpinhanvien = model.makpinhanvien.ToString() }) + "'title='" + model.manhanvien + "'>" + model.manhanvien + "</a>\"");
                sbResult.Append("},");

                // Họ và tên nhân viên soạn kpi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Employee_Edit", "KpiEmployee", new { makpinhanvien = model.makpinhanvien.ToString() }) + "'title='" + model.hovaten + "'>" + model.hovaten + "</a>\"");
                sbResult.Append("},");


                //Tên chức danh
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"title\":\"" + model.chucdanh + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Employee_Edit", "KpiEmployee", new { makpinhanvien = model.makpinhanvien.ToString() }) + "'title='" + model.tenchucdanh + "'>" + model.tenchucdanh + "</a>\"");
                sbResult.Append("},");

                //Ngày đăng ký
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Kpi_Employee_Edit", "KpiCompany", new { makpinhanvien = model.makpinhanvien.ToString() }) + "'title='" + model.ngaydangky + "'>" + model.ngaydangky + "</a>\"");
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
                sbResult.Append("\"col_value\":\"" + model.truongphongxemxetkpi + "\"");
                sbResult.Append("},");

                //Đã duyệt hay chưa duyệt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col12\",");
                sbResult.Append("\"col_id\":\"12\",");
                sbResult.Append("\"col_value\":\"" + model.truongphongxemxetkpi_duyet_text + "\"");
                sbResult.Append("},");

                //Ngày duyệt
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col13\",");
                sbResult.Append("\"col_id\":\"13\",");
                sbResult.Append("\"col_value\":\"" + model.truongphongxemxetkpi_ngay + "\"");
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


        #region THÊM MỚI KPI NHÂN VIÊN.

        public ActionResult Kpi_Employee_New()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            lstResult_phongban = new List<PhongBanModels>();
            lstResult_phongban = service.SelectRows(parampb); //chucdanhkpi
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = Session["maphongban"].ToString().Trim();
            string pb = ""; string captrentructiep = ""; string mailtp = "";
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                var lstcaptrentt = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                if (lstcaptrentt.Count>0)
                {
                    pb = "<option value=" + lstcaptrentt[0].maphongban + ">" + lstcaptrentt[0].tenphongban + "</option>";
                    captrentructiep = lstcaptrentt[0].hovaten;
                    mailtp = lstcaptrentt[0].email;
                }
                foreach (var item in lstResult_phongban.Where(p => p.maphongban != maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
                {
                    captrentructiep = item.hovaten;
                    mailtp = item.email;
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
                }
            }
            ViewBag.sbphongban = pb.ToString();
            ChucDanhModels param = new ChucDanhModels();
            List<ChucDanhModels> lstchucdanh = service.SelectRows_chucvu(param);
            string machucdanhkpi = Session["chucdanhkpi"].ToString().Trim();
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
            tblthongtinnhanvien.Add(new thongtinnhanvienModels() { manhanvien = Session["manhansu"].ToString(), hoten = Session["fullname"].ToString(), chucdanhkpi = machucdanhkpi, tenchucdanhkpi = tenchucdanhkpi, captrentructiep = captrentructiep, mailnhanvien = Session["thudientu"].ToString(),mailtruongphong=mailtp });
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
            else if (machucdanhkpi == "2") // Lấy dữ liệu phòng
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
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu + "</textarea>" + "\"");
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
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttieuchidanhgia' name='tieuchidanhgia' >" + model.tieuchidanhgia + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.cachtinh + "' ></input>" + "\"");
                    // sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh + "</textarea>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtcachtinh' name='cachtinh' >" + model.cachtinh + "</textarea>" + "\"");

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
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.kehoach + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.thuchien + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.tilehoanthanh + "' ></input>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh + "</textarea>" + "\"");
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
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtghichu' name='ghichu' >" + model.ghichu + "</textarea>" + "\"");
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
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
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
        #endregion

        public ActionResult Kpi_Employee_Edit(string makpinhanvien)
        {
            if (!IsLogged())
                return BackToLogin();
            List<KpiEmployeeModels> KpiEmployee = new List<KpiEmployeeModels>();
            KpiEmployeeServices servicetp = new KpiEmployeeServices();
            KpiEmployee = servicetp.SelectRows_KpiEmployee_hieuchinh(makpinhanvien);
            
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            lstResult_phongban = new List<PhongBanModels>();
            lstResult_phongban = service.SelectRows(parampb); //chucdanhkpi
            StringBuilder sbphongban = new StringBuilder();
            string maphongban = KpiEmployee[0].maphongban.Trim();//Session["maphongban"].ToString().Trim();
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
            string machucdanhkpi = KpiEmployee[0].chucdanh.Trim();
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
            ViewBag.sbchucdanh = chucdanhs.ToString();
            KpiEmployee[0].namkpi = KpiEmployee[0].ngaydangky.Split('/')[2].ToString();
            KpiEmployee[0].thangkpi = KpiEmployee[0].ngaydangky.Split('/')[1].ToString();
            KpiEmployee[0].truongphongchucdanh = Session["chucdanhkpi"].ToString().Trim();
            return View(KpiEmployee[0]);
        }

        [HttpPost]
        public JsonResult SelectRows_KpiEmployeeDetail_hieuchinh(KpiEmployeeDetailModels model, string chucdanh)
        {
            KpiEmployeeServices service = new KpiEmployeeServices();
            List<KpiEmployeeDetailModels> lstResult = service.SelectRows_KpiEmployeeDetail_hieuchinh(model.makpinhanvien.ToString());
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;
                foreach (var item1 in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_Employee_hieuchinh(item1, strSTT, chucdanh));
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

        private StringBuilder PrepareDataJson_Employee_hieuchinh(KpiEmployeeDetailModels model, string couter, string chucdanh)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            try
            {
                string group = ""; string kpicha = ""; string style = "";

                if (model.stt.ToUpper() == "I") { group = "1"; kpicha = "1"; style = "background-color:#719bef;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.ToUpper() == "II") { group = "2"; kpicha = "1"; style = "background-color:#99f1b8;font-weight: bold;font-size: 13px;"; }
                else if (model.stt.Trim() == "") { style = "background-color:#e2e0e0;font-weight: bold;font-size: 13px;"; }

                if (model.thuoccap.ToUpper() == "I") group = "1";
                else if (model.thuoccap.ToUpper() == "II") group = "2";

                if (model.kpichinhsua == "2" || model.kpichinhsua == "3" || model.kpichinhsua == "4" || model.kpichinhsua == "5") style = "background-color:rgba(236, 255, 11, 0.34);";

                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + model.makpinhanvien_chitiet + "\",");
                sbResult.Append("\"col_id_mabosungnhansu\":\"" + model.makpinhanvien + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpinhanvien_chitiet" + "\", \"value\":\"" + model.makpinhanvien_chitiet + "\"},{\"name\":\"" + "makpinhanvien" + "\", \"value\":\"" + model.makpinhanvien + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"},{\"name\":\"" + "kpichinhsua" + "\", \"value\":\"" + model.kpichinhsua + "\"}],");
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
                if (model.stt.ToUpper() == "I" || model.stt.ToUpper() == "II" || model.stt == "")
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
                    if(model.kpichinhsua=="1")
                        sbResult.Append("\"col_value\":\"" + "<input readonly='readonly' style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.trongso + "' ></input>" + "\"");
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
                    if (model.kpichinhsua == "1")
                        sbResult.Append("\"col_value\":\"" + "<input readonly='readonly' style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"\"");
                    sbResult.Append("},");

                    //string strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a>";
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
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
                    if (model.kpichinhsua == "1" || model.kpichinhsua == "3" || model.kpichinhsua == "4" || model.kpichinhsua == "5" || model.kpichinhsua == "6")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='readonly' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='Nhập mục tiêu kpi' id='txtmuctieu' name='muctieu' >" + model.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    string abc = model.muctieu.Replace("daunhaydoi", "'");
                    // Trọng số
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"type\":\"hidden\",");
                    sbResult.Append("\"col_class\":\"ovh col3\",");
                    sbResult.Append("\"col_id\":\"3\",");
                    if (model.kpichinhsua == "1" || model.kpichinhsua == "3" || model.kpichinhsua == "4" || model.kpichinhsua == "5" || model.kpichinhsua == "6")
                        sbResult.Append("\"col_value\":\"" + "<textarea  readonly='readonly' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttrongso' name='trongso' >" + model.trongso + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Tiêu chí đánh giá
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col4\",");
                    sbResult.Append("\"col_id\":\"4\",");
                    if (model.kpichinhsua == "1" || model.kpichinhsua == "3" || model.kpichinhsua == "4" || model.kpichinhsua == "5" || model.kpichinhsua == "6")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='readonly' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtphuongphapdo' name='phuongphapdo' >" + model.phuongphapdo.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtphuongphapdo' name='phuongphapdo' >" + model.phuongphapdo.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    // Cách tính
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col5\",");
                    sbResult.Append("\"col_id\":\"5\",");
                    if (model.kpichinhsua == "1" || model.kpichinhsua == "3" || model.kpichinhsua == "4" || model.kpichinhsua == "5" || model.kpichinhsua == "6")
                        sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' >" + model.nguonchungminh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' >" + model.nguonchungminh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");


                    //Ngày ghi nhận kết quả
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col6\",");
                    sbResult.Append("\"col_id\":\"6\",");
                    if (model.kpichinhsua == "1" || model.kpichinhsua == "3" || model.kpichinhsua == "4" || model.kpichinhsua == "5" || model.kpichinhsua == "6")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='readonly' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");



                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    if (model.kpichinhsua == "1" || model.kpichinhsua == "3" || model.kpichinhsua == "4" || model.kpichinhsua == "5" || model.kpichinhsua == "6")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='readonly' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkehoach' name='kehoach' >" + model.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    if (model.khoaketqua == "1")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='readonly' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtthuchien' name='thuchien' >" + model.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col9\",");
                    sbResult.Append("\"col_id\":\"9\",");
                    if (model.khoaketqua == "1")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='readonly' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txttilehoanthanh' name='tilehoanthanh' >" + model.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col10\",");
                    sbResult.Append("\"col_id\":\"10\",");
                    if (model.khoaketqua == "1")
                        sbResult.Append("\"col_value\":\"" + "<textarea readonly='readonly' style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    else sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtketqua' name='ketqua' >" + model.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");


                    //cai nay khong khoá de nod thong tin
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col11\",");
                    sbResult.Append("\"col_id\":\"11\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtkyxacnhan' name='kyxacnhan' >" + model.kyxacnhan.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    sbResult.Append("},");

                    string strHTML_Attachment = "";
                    if (model.kpichinhsua == "2" || model.kpichinhsua == "3" || model.kpichinhsua == "4" || model.kpichinhsua == "5" || model.kpichinhsua == "6")
                        strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a><br/> <b>Y/CCS </b><input type='checkbox' checked='checked' onclick='Select(this);' class='chkCheck'/> <br/><a href='#login-box' style='text-align: center;font-weight: bold;font-size: 13px;' class='login-window'>KPI Phòng</a>";
                    else strHTML_Attachment = "<a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-add'>Add</a> | <a href='#' style='text-align: center;font-weight: bold;font-size: 13px;' class='button-del'>Del</a><br/> <b>Y/CCS </b><input type='checkbox' onclick='Select(this);' class='chkCheck'/> <br/><a href='#login-box' style='text-align: center;font-weight: bold;font-size: 13px;' class='login-window'>KPI Phòng</a>";
                    //
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col12\",");
                    sbResult.Append("\"col_id\":\"12\",");
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
        public JsonResult Save_KpiEmployee(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiEmployeeServices services = new KpiEmployeeServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_KpiEmployee(DataJson, nguoitao, "0", "0");
            if (iresult != "0")
            {
                return Json(new { success = true, makpinhanvien = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpinhanvien = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save_KpiEmployee_guimail(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiEmployeeServices services = new KpiEmployeeServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_KpiEmployee(DataJson, nguoitao, "1", "0");
            if (iresult != "0")
            {
                string kq=send_Mail(iresult, DataJson);
                if (kq == "1")
                    return Json(new { success = true, makpinhanvien = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                else
                {
                    services.Save_KpiEmployee_nguoiguihoso(iresult);
                    return Json(new { success = 1, makpinhanvien = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
                } 
            }
            else
            {
                return Json(new { success = false, makpinhanvien = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public string send_Mail(string makpinhanvien, string DataJson)
        {
            _logger.Start("send_Mail");
            try
            {
                JObject json = JObject.Parse(DataJson);
                string mailemployee = json["data1"]["email"].ToString();

                string hovaten = json["data1"]["hovaten"].ToString();
                string captrentructiep = json["data1"]["captrentructiep"].ToString();

                string truongphongemail = json["data1"]["truongphongemail"].ToString();

                string thangkpi = json["data1"]["ngaydangky"].ToString().Trim().Split('/')[1];//"03";
                string namkpi = json["data1"]["ngaydangky"].ToString().Trim().Split('/')[2]; //"2018";
                string kpichinhsua = json["data1"]["kpichinhsua"].ToString().Trim();
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetEmployee/?makpi=";

                
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                string maphongban=json["data1"]["maphongban"].ToString();
                var lstlayphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                if (lstlayphongban.Count > 0)
                {
                    maphongban = lstlayphongban[0].tenphongban;
                }
                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 20px; padding: 0; width: 100%; font-size: 1em;color:black;'>"); //margin: 0 auto;  de canh giua
                sb.Append("<table cellpadding='0' cellspacing='0' width='100%' >");
                sb.Append("<tr>");

                //sb.Append("<td>");
                //sb.Append("<div style='width:150px;float:left;height :45px; line-height:45px; padding-top:10px;'>");
                //sb.Append("<img src='http://i.imgur.com/yKqNNy2.png'  alt='ddd' style='width:100px; height:45px;'/>");
                //sb.Append("</div>");
                //sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<div style='width:400px; text-align:center;font-weight: bold;float:left;line-height:45px'>");
                sb.Append("<p style= 'width:400px;text-align:center;font-size:18px;font-weight:bold;line-height:45px;padding-left:80px;float:left;'>ĐĂNG KÝ KPI CẤP CÁ NHÂN</p>");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</body>");
                sb.Append("<hr style=border: 1px solid #000; width: 100% />");

                sb.Append("<table style='width:100%; font-size:16px;'>");
                sb.Append("<tr style='width:100%; font-size:15px;'><td style='padding-left:10px;'><p><strong><em><u>Kính gửi:</u></em>&nbsp;&nbsp; Anh/Chị " + captrentructiep + " - Trưởng Phòng " + maphongban + "</strong></p></td></tr>");
                sb.Append("<tr style='width:100%; font-size:13px;'><td style='padding-left:10px;'>Căn cứ <b>quy định thực hiện theo KPI cấp cá nhân đã được ban hành</b>. Dựa theo phạm vi công việc và phân công nhiệm vụ của vị trí đảm nhận trong tháng này</td></tr>");
                sb.Append("<tr style='width:100%; font-size:13px;'><td style='padding-left:10px;'>Kính đề nghị Trưởng Bộ Phận xem xét và phê duyệt nội dung đăng ký<b> KPI - Cấp Cá nhân Tháng "+thangkpi+"/"+namkpi+" </b>với các nội dung như sau:</td></tr>");
                sb.Append("<tr style='width:100%; font-size:13px;'><td style='padding-left:10px;'></td></tr>");
                sb.Append("</table>");

                sb.Append("<table style='width:100%; font-size:13px;'>");
                sb.Append("<tr><td rowspan='3' colspan='2' style='border: 1px solid #000'></td>");
                sb.Append("<td colspan='9' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP CÁ NHÂN - ĐĂNG KÝ KPI THÁNG " + thangkpi + "  - NĂM " + namkpi + "</td>");
                sb.Append("</tr>");
                
                sb.Append("<tr><td colspan='2' style='text-align: left;border: 1px solid #000;'>Phòng Ban:</td>");
                sb.Append("<td colspan='5' style='border: 1px solid #000;'>" + maphongban + "</td>");
                sb.Append("<td colspan='2' style='border: 1px solid #000;text-align: right;'>Biễu mẫu: QP04 FM18</td></tr>");
                sb.Append("<tr><td colspan='2' style='text-align: left;border: 1px solid #000;'>Ngày ban hành</td>");
                sb.Append("<td colspan='2' style='border: 1px solid #000;'>" + json["data1"]["ngaybanhanh"].ToString().Trim() + "</td>");
                sb.Append("<td colspan='5' style='text-align: right;border: 1px solid #000;'>Lần cập nhật: 00/01</td></tr>");
                sb.Append("<tr style='box-shadow: none;'>");
                sb.Append("<td colspan='5' style='text-align: left;border: 1px solid #000'>");
                sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Họ và tên: &nbsp;&nbsp;<b>" + json["data1"]["hovaten"].ToString().Trim() + "</b></div>");
                sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Chức danh: &nbsp;&nbsp;<b>" + "Nhân viên" + "</b></div>");
                sb.Append("<div class='col-md-1' style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Cấp trên trực tiếp: &nbsp;&nbsp;<b>" + json["data1"]["truongphongxemxetkpi"].ToString().Trim() + "</b></div>");
                sb.Append("</td>");
                sb.Append("<td colspan='6' style='text-align: left;border: 1px solid #000'>");
                sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Mã nhân viên: &nbsp;&nbsp;<b>" + json["data1"]["manhanvien"].ToString().Trim() + "</b></div>");
                sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Ngày đăng ký: &nbsp;&nbsp;<b>" + json["data1"]["ngaydangky"].ToString().Trim() + "</b></div>");
                sb.Append("<div class='col-md-1' style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Ngày đánh giá: &nbsp;&nbsp;<b>" + json["data1"]["ngaydanhgia"].ToString().Trim() + "</b></div>");
                sb.Append("</td>");
                sb.Append("<tr>");
                sb.Append("<tr>");
                sb.Append("<td style='border: 1px solid #000;width:4%;text-align:center'>Stt</td>");
                sb.Append("<td style='border: 1px solid #000;width:18%;text-align:center'>Tiêu chí/ Mục tiêu đánh giá</td>");
                sb.Append("<td style='border: 1px solid #000;width:6%;text-align:center'>Trọng số</td>");
                sb.Append("<td style='border: 1px solid #000;width:11%;text-align:center'>Phương pháp đo</td>");
                sb.Append("<td style='border: 1px solid #000;width:11%;text-align:center'>Nguồn dữ liệu chứng minh</td>");
                sb.Append("<td style='border: 1px solid #000;width:9%;text-align:center'>Đơn vị tính</td>");
                sb.Append("<td style='border: 1px solid #000;width:7%;text-align:center'>Kế hoạch</td>");
                sb.Append("<td style='border: 1px solid #000;width:7%;text-align:center'>Thực hiện</td>");
                sb.Append("<td style='border: 1px solid #000;width:7%;text-align:center'>Tỉ lệ % hoàn thành</td>");
                sb.Append("<td style='border: 1px solid #000;width:7%;text-align:center'>Kết quả</td>");
                sb.Append("<td style='border: 1px solid #000;width:8%;text-align:center'>Ký xác nhận</td>");
                sb.Append("</tr>");
                string mact = ""; string kpichinhsuact = "";
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    if (json_tiendo_giatri[i]["stt"].ToString().Trim()=="I")
                        sb.Append("<tr style='background-color:#f9fd0d'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "II")
                        sb.Append("<tr style='background-color:#99f1b8'>");
                    else if (json_tiendo_giatri[i]["stt"].ToString().Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0'>");
                    else sb.Append("<tr>");

                    if (i < (json_tiendo_giatri.Count() - 1))
                    {
                        mact = mact + json_tiendo_giatri[i]["makpinhanvien_chitiet"].ToString().Trim() + ",";
                        kpichinhsuact = kpichinhsuact + json_tiendo_giatri[i]["kpichinhsua"].ToString().Trim() + ",";
                    }
                       
                    else 
                    {
                        mact = mact + json_tiendo_giatri[i]["makpinhanvien_chitiet"].ToString().Trim();
                        kpichinhsuact = kpichinhsuact + json_tiendo_giatri[i]["kpichinhsua"].ToString().Trim();
                    }

                    sb.Append("<td style='font-size:12px; line-height:24px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["stt"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["muctieu"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["trongso"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["phuongphapdo"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["nguonchungminh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["donvitinh"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["kehoach"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["thuchien"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["tilehoanthanh"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["ketqua"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["kyxacnhan"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }

                string strEncryptCode = linkname.Trim() + makpinhanvien + "0070pXQSeNsQRuzoCmUYfuX" + "&mailemployee=" + mailemployee + "&capdoduyet=1" + "&kpichinhsua=" + kpichinhsua + "&mact=" + mact + "sQRu17zoCm5687AVoiGXX" + "&kpichinhsuact=" + kpichinhsuact;

                sb.Append("</table>");

                string chuoidy = "width:200px;height: 40px;background-color:#0090d9;border-radius:15px;text-align:center;color:#fff; margin: 0 8px 0 0;text-decoration: none; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);";
                string chuoikdy = "width:200px;height: 40px;background-color:red;border-radius:15px;text-align:center;color:#fff; margin: 0 8px 0 0;text-decoration: none; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);";

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:0px; font-size:22px; height :40px; line-height:51px; padding-top:5px;'><a href='" + strEncryptCode + "&dy=1' style='" + chuoidy + "'>&nbsp;&nbsp;Đồng ý&nbsp;&nbsp;</a>&nbsp;&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2' style='" + chuoikdy + "'>&nbsp;&nbsp;Không đồng ý&nbsp;&nbsp;</a></td></tr>");
                sb.Append("</table>");

                //sb.Append("<table style='width:850px;'>");
                //sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý Duyệt KPI</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                //sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, truongphongemail.Trim());
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "", System.Text.Encoding.UTF8);
                message.Subject = "ĐỀ NGHỊ PHÊ DUYỆT KPI CẤP CÁ NHÂN - NHÂN SỰ " + hovaten.ToUpper()+" THÁNG "+thangkpi+"/"+namkpi;
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

        public string send_Mail_yeucauchinhsua_chitiet(string DataJson)
        {
            _logger.Start("send_Mail");
            try
            {

                JObject json = JObject.Parse(DataJson);
                JArray json_tiendo_giatri = (JArray)json["data1"];

                string makpinhanvien_chitiet = json_tiendo_giatri[0]["makpinhanvien_chitiet"].ToString();
                string makpinhanvien = json_tiendo_giatri[0]["makpinhanvien"].ToString()+"QA741xwz78i035760byasdfux";
                string mailemployee = json_tiendo_giatri[0]["email"].ToString();
                string truongphongemail = json_tiendo_giatri[0]["truongphongemail"].ToString();

                string thangkpi = json_tiendo_giatri[0]["ngaydangky"].ToString().Trim().Split('/')[1];//"03";
                string namkpi = json_tiendo_giatri[0]["ngaydangky"].ToString().Trim().Split('/')[2]; //"2018";

                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetEmployee_Yeucauchinhsua/?makpict=";

               
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                //string maphongban = json["data1"]["maphongban"].ToString();
                //var lstlayphongban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                //if (lstlayphongban.Count > 0)
                //{
                //    maphongban = lstlayphongban[0].tenphongban;
                //}
                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 20px; padding: 0; width: 100%; font-size: 1em;color:black;'>"); //margin: 0 auto;  de canh giua
                sb.Append("<table cellpadding='0' cellspacing='0' width='100%' >");
                sb.Append("<tr>");

                //sb.Append("<td>");
                //sb.Append("<div style='width:150px;float:left;height :45px; line-height:45px; padding-top:10px;'>");
                //sb.Append("<img src='http://i.imgur.com/yKqNNy2.png'  alt='ddd' style='width:100px; height:45px;'/>");
                //sb.Append("</div>");
                //sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<div style='width:400px; text-align:center;font-weight: bold;float:left;line-height:45px'>");
                sb.Append("<p style= 'width:400px;text-align:center;font-size:18px;font-weight:bold;line-height:45px;padding-left:80px;float:left;'>YÊU CẦU CHỈNH SỮA KPI CÁ NHÂN " + thangkpi + "  - NĂM " + namkpi + " VỚI NỘI DUNG NHƯ SAU</p>");
                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</body>");

                sb.Append("<hr style=border: 1px solid #000; width: 100% />");

                sb.Append("<table style='width:100%; font-size:13px;'>");
                //sb.Append("<tr><td rowspan='3' colspan='2' style='border: 1px solid #000'></td>");
                //sb.Append("<td colspan='9' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>YÊU CẦU CHỈNH SỮA KPI " + thangkpi + "  - NĂM " + namkpi + " VỚI NỘI DUNG NHƯ SAU</td>");
                //sb.Append("</tr>");
                //sb.Append("<tr><td colspan='2' style='text-align: left;border: 1px solid #000;'>Phòng Ban:</td>");
                //sb.Append("<td colspan='5' style='border: 1px solid #000;'>" + maphongban + "</td>");
                //sb.Append("<td colspan='2' style='border: 1px solid #000;text-align: right;'>Biễu mẫu: QP04 FM18</td></tr>");
                //sb.Append("<tr><td colspan='2' style='text-align: left;border: 1px solid #000;'>Ngày ban hành</td>");
                //sb.Append("<td colspan='2' style='border: 1px solid #000;'>" + json["data1"]["ngaybanhanh"].ToString().Trim() + "</td>");
                //sb.Append("<td colspan='5' style='text-align: right;border: 1px solid #000;'>Lần cập nhật: 00/01</td></tr>");

                sb.Append("<tr style='box-shadow: none;'>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000'>");
                sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Mã nhân viên: &nbsp;&nbsp;<b>" + json_tiendo_giatri[0]["manhanvien"].ToString().Trim() + "</b></div>");
                sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Họ và tên: &nbsp;&nbsp;<b>" + json_tiendo_giatri[0]["hovaten"].ToString().Trim() + "</b></div>");
                sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Chức danh: &nbsp;&nbsp;<b>" + "Nhân viên" + "</b></div>");
                //sb.Append("<div class='col-md-1' style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Cấp trên trực tiếp: &nbsp;&nbsp;<b>" + json["data1"]["truongphongxemxetkpi"].ToString().Trim() + "</b></div>");
                sb.Append("</td>");
                //sb.Append("<td colspan='6' style='text-align: left;border: 1px solid #000'>");
                //sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Mã nhân viên: &nbsp;&nbsp;<b>" + json["data1"]["manhanvien"].ToString().Trim() + "</b></div>");
                //sb.Append("<div style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Ngày đăng ký: &nbsp;&nbsp;<b>" + json["data1"]["ngaydangky"].ToString().Trim() + "</b></div>");
                //sb.Append("<div class='col-md-1' style='min-width: 150px; padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp; Ngày đánh giá: &nbsp;&nbsp;<b>" + json["data1"]["ngaydanhgia"].ToString().Trim() + "</b></div>");
                //sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr style='background-color:#f9fd0d'>");
                sb.Append("<td style='border: 1px solid #000;width:5%;text-align:center'>Stt</td>");
                sb.Append("<td style='border: 1px solid #000;width:40%;text-align:center'>Tiêu chí/ Mục tiêu đánh giá</td>");
                sb.Append("<td style='border: 1px solid #000;width:10%;text-align:center'>Trọng số</td>");
                sb.Append("<td style='border: 1px solid #000;width:45%;text-align:center'>Tiêu chí/ Mục tiêu thay đổi</td>");
                sb.Append("</tr>");

                //JArray json_tiendo_giatri = (JArray)json["data2"];
                string mact = "";
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    sb.Append("<tr>");

                    if (i < (json_tiendo_giatri.Count() - 1))
                        mact = mact + json_tiendo_giatri[i]["makpinhanvien_chitiet"].ToString().Trim() + ",";
                    else mact = mact + json_tiendo_giatri[i]["makpinhanvien_chitiet"].ToString().Trim();

                    sb.Append("<td style='font-size:12px; line-height:24px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["stt"].ToString().Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + json_tiendo_giatri[i]["muctieu"].ToString().Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + json_tiendo_giatri[i]["trongso"].ToString().Trim() + "</td>");

                    string chuoinoidung = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["kyxacnhan"].ToString().Trim(), @"\\n", " \n ").Replace("daunhaydon", "'").Replace("daukytuva", "&");
                    //string chuoicat = "\n";
                    //string[] elements = Regex.Split(chuoinoidung, chuoicat);
                   
                    //sb.Append("<td>");
                    //foreach (var element in elements)
                    //{
                    //    sb.Append("<td style='float:left; padding-left:10px;font-size:17px;'>" + element + "</td>");
                    //}
                    //sb.Append("</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + chuoinoidung + "</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</table>");

                string strEncryptCode = linkname.Trim() + mact + "0070pXQSeNsQRuzoCmUYfuX" + "&mailemployee=" + mailemployee + "&capdoduyet=1" + "&makpi=" + makpinhanvien;

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý Chỉnh sữa KPI</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");


                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, truongphongemail.Trim());
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Yêu cầu chỉnh sữa KPI", System.Text.Encoding.UTF8);
                message.Subject = "Yêu cầu chỉnh sữa KPI";
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
        public JsonResult Save_Yeucauchinhsua_chitiet(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiEmployeeServices servicevpp = new KpiEmployeeServices();
            bool iresult = servicevpp.Save_Yeucauchinhsua_chitiet(DataJson);
            if (iresult == true)
            {
                string kq = send_Mail_yeucauchinhsua_chitiet(DataJson);
                if (kq == "1")
                    return Json(new { success = true}, JsonRequestBehavior.AllowGet);
                else
                {
                    //servicevpp.Save_KpiEmployee_nguoiguihoso(iresult);
                    return Json(new { success = 1}, JsonRequestBehavior.AllowGet);
                } 
                //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save_Ketquakpi(string DataJson)
        {
            JsonResult Data = new JsonResult();
            KpiEmployeeServices services = new KpiEmployeeServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = services.Save_Ketquakpi(DataJson, nguoitao);
            if (iresult != "0")
            {
                return Json(new { success = true, makpinhanvien = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpinhanvien = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult Khoamuctieukpithang(string DataJson)
        {
            JObject json = JObject.Parse(DataJson);
            //string mailemployee = json["email"].ToString();
            string strEncryptCode = json["makpinhanvien"].ToString();
            //string dy = json["dongy"].ToString();
            KpiEmployeeServices service = new KpiEmployeeServices();
            bool kq = service.UpdateRow_Employee_Khoaketquakpi(strEncryptCode);
            //if (kq && dy == "1")
            //{
            //    send_Mail(mailemployee, "KPI được duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI");
            //}
            //else if (kq && dy == "2")
            //{
            //    send_Mail(mailemployee, "KPI không được duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI");
            //}
            if (kq)
            {
                return Json(new { success = true, makpinhanvien = int.Parse(strEncryptCode) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpinhanvien = strEncryptCode }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult dlgView()
        {
            if (!IsLogged())
                return BackToLogin();
            return View();
        }

        [HttpPost]
        public JsonResult SelectRows_KpiLevelDepartmentDetail_hieuchinh(string maphongban,string nam)
        {
            KpiEmployeeServices service = new KpiEmployeeServices();
            List<KpiLevelDepartmentDetailModels> lstResult = service.SelectRows_KpiLevelDepartmentDetail_hieuchinh(maphongban,nam);
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
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "makpiphongban_chitiet" + "\", \"value\":\"" + model.makpiphongban_chitiet + "\"},{\"name\":\"" + "makpiphongban" + "\", \"value\":\"" + model.makpiphongban + "\"},{\"name\":\"" + "group" + "\", \"value\":\"" + group + "\"},{\"name\":\"" + "kpicha" + "\", \"value\":\"" + kpicha + "\"},{\"name\":\"" + "style" + "\", \"value\":\"" + style + "\"}],");
                sbResult.Append("\"col_value\":[");

                #region Data cell
                //colum checkbox

              

                // Muc tiêu
                if (model.stt.ToUpper() == "A" || model.stt.ToUpper() == "B" || model.stt.ToUpper() == "C" || model.stt.ToUpper() == "D" || model.stt == "")
                {
                    #region

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col1\",");
                    sbResult.Append("\"col_id\":\"1\",");
                    sbResult.Append("\"col_value\":\"" +""+ "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                    sbResult.Append("\"col_id\":\"1\",");
                    sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                    sbResult.Append("},");

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
                    sbResult.Append("}");
                    #endregion
                }
                else
                {
                    #region

                    string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' makpiphongban_chitiet='" + model.makpiphongban_chitiet + "' phongban_congtruong='" + 1 + "'/>", model.makpiphongban_chitiet);

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col1\",");
                    sbResult.Append("\"col_id\":\"1\",");
                    sbResult.Append("\"col_value\":\"" + strHTML_Checkbox + "\"");
                    sbResult.Append("},");


                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col1 stt\",");
                    sbResult.Append("\"col_id\":\"1\",");
                    sbResult.Append("\"col_value\":\"" + model.stt + "\"");
                    sbResult.Append("},");


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
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtngayghinhanketqua' name='ngayghinhanketqua' >" + model.ngayghinhanketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: center; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.ngayghinhanketqua + "' ></input>" + "\"");
                    sbResult.Append("},");

                    //Nguồn chứng minh
                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col7 boxEdit\",");
                    sbResult.Append("\"col_id\":\"7\",");
                    //sbResult.Append("\"col_value\":\"" + "<select  id='filter01' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' data-placeholder='Người thực hiện' class='chosen-select-nguoithuchien'  name='nguonchungminh' multiple='multiple'> '" + nguoncm + "'   </select>" + "\"");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtnguonchungminh' name='nguonchungminh' >" + model.nguonchungminh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<select id='filter02' style='width:100%;padding-left: 2px;border: 1px solid #aaa;  border-radius: 3px;background-color: #fff;height: 22px;' class='chosen-select-loaikehoach'  name='phongban'>'" + pb + "</select>" + "\"");
                    sbResult.Append("},");

                    sbResult.Append("{");
                    sbResult.Append("\"colspan\":\"1\",");
                    sbResult.Append("\"col_class\":\"ovh col8\",");
                    sbResult.Append("\"col_id\":\"8\",");
                    sbResult.Append("\"col_value\":\"" + "<textarea style='width:100%; resize:none; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' rows='4' placeholder='' id='txtdonvitinh' name='donvitinh' >" + model.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</textarea>" + "\"");
                    //sbResult.Append("\"col_value\":\"" + "<input style='width:100%;padding-left: 5px; text-align: left; height:27px; border-radius: 3px; border: 1px solid #bbb;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset; transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;' value='" + model.donvitinh + "' ></input>" + "\"");
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

        public ActionResult ExportLicensing()
        {
            if (!IsLogged())
                return BackToLogin();

            string makpinhanvien = this.Request.Url.Segments[3];
            string avvvaa = "xslt/Fileinhangthang/";
            string filepacth = Server.MapPath(avvvaa).Replace("KpiEmployee\\ExportLicensing\\", "");

            List<KpiEmployeeModels> KpiEmployee = new List<KpiEmployeeModels>();
            KpiEmployeeServices servicetp = new KpiEmployeeServices();
            KpiEmployee = servicetp.SelectRows_KpiEmployee_hieuchinh(makpinhanvien);
            string hotennv = ""; string hotentruongphong = "";
            List<SetCellValue> list = new List<SetCellValue>();
            if (KpiEmployee.Count > 0)
            {
                DanhmucServices servicedm = new DanhmucServices();
                PhongBanModels parampb = new PhongBanModels();
                List<PhongBanModels> lstResult_phongban = servicedm.SelectRows(parampb);
                var lstphongban = lstResult_phongban.Where(p => p.maphongban == KpiEmployee[0].maphongban).ToList();
                string tenphongbans = "";
                if (lstphongban.Count > 0)
                {
                    tenphongbans = lstphongban[0].tenphongban;
                }
                string thangdangky=""; string namdangky="";
                if(KpiEmployee[0].ngaydangky.ToString()!="")
                {
                    thangdangky=KpiEmployee[0].ngaydangky.Substring(3,2);
                    namdangky = KpiEmployee[0].ngaydangky.Substring(6, 4);
                }
                hotennv = KpiEmployee[0].nguoilapkpi; hotentruongphong = KpiEmployee[0].captrentructiep;
                list.Add(new SetCellValue { RowIndex = 15, ColumnIndex = 1, Value = KpiEmployee[0].captrentructiep });

                list.Add(new SetCellValue { RowIndex = 0, ColumnIndex = 2, Value = "KPI CÁ NHÂN - ĐĂNG KÝ KPI THÁNG " + thangdangky + " - NĂM "+namdangky });

                list.Add(new SetCellValue { RowIndex = 1, ColumnIndex = 2, Value = "Phòng Ban: " + tenphongbans });
                list.Add(new SetCellValue { RowIndex = 2, ColumnIndex = 2, Value = "Ngày ban hành: " + KpiEmployee[0].ngaybanhanh });
                list.Add(new SetCellValue { RowIndex = 2, ColumnIndex = 8, Value = "Lần cập nhật: " + KpiEmployee[0].lancapnhat });

                list.Add(new SetCellValue { RowIndex = 3, ColumnIndex = 1, Value = "Họ và tên: " + KpiEmployee[0].hovaten });
                list.Add(new SetCellValue { RowIndex = 4, ColumnIndex = 1, Value = "Chức danh: " + KpiEmployee[0].tenchucdanh });
                list.Add(new SetCellValue { RowIndex = 5, ColumnIndex = 1, Value = "Cấp trên trực tiếp: " + KpiEmployee[0].captrentructiep });

                list.Add(new SetCellValue { RowIndex = 3, ColumnIndex = 5, Value = "Mã nhân viên: " + KpiEmployee[0].manhanvien });
                list.Add(new SetCellValue { RowIndex = 4, ColumnIndex = 5, Value = "Ngày đăng ký: " + KpiEmployee[0].ngaydangky });
                list.Add(new SetCellValue { RowIndex = 5, ColumnIndex = 5, Value = "Ngày đánh giá: " + KpiEmployee[0].ngaydanhgia });

            }

            List<KpiEmployeeDetailModels> lstResult = servicetp.SelectRows_KpiEmployeeDetail_hieuchinh(makpinhanvien);

            DataTable tblPrint = new DataTable();
            tblPrint.Columns.Add("stt");
            tblPrint.Columns.Add("muctieu");
            tblPrint.Columns.Add("trongso");
            tblPrint.Columns.Add("phuongphapdo");
            tblPrint.Columns.Add("nguonchungminh");
            tblPrint.Columns.Add("donvitinh");
            tblPrint.Columns.Add("kehoach");
            tblPrint.Columns.Add("thuchien");
            tblPrint.Columns.Add("tilehoanthanh");
            tblPrint.Columns.Add("ketqua");
            tblPrint.Columns.Add("kyxacnhan");

            foreach (var item in lstResult)
            {
                DataRow tmp = tblPrint.NewRow();
                tblPrint.Rows.Add(tmp);
                tmp["stt"] = item.stt.Trim();
                tmp["muctieu"] = item.muctieu.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n"," ");
                tmp["trongso"] = item.trongso.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
                tmp["phuongphapdo"] = item.phuongphapdo.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
                tmp["nguonchungminh"] = item.nguonchungminh.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
                tmp["donvitinh"] = item.donvitinh.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
                tmp["kehoach"] = item.kehoach.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
                tmp["thuchien"] = item.thuchien.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
                tmp["tilehoanthanh"] = item.tilehoanthanh.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
                tmp["ketqua"] = item.ketqua.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
                tmp["kyxacnhan"] = item.kyxacnhan.Replace("daunhaydon", "'").Replace("daukytuva", "&").Replace("\n", " ");
            }
            string tenfile = "KPICANHANVANPHONG.xls";
            string filedownload = DateTime.Now.ToString("ddMMyyyyHHmmss") + "KPICANHANVANPHONG.xls";
            string fileName = filepacth + filedownload;
            ExportToExcel export = new ExportToExcel(Functions.GetTemplateFileName(tenfile, filepacth));
            export.TemplateExportKPICANHANVANPHONG(tblPrint, 7, 0, list, fileName, hotennv, hotentruongphong);

            var FileVirtualPath = "~/xslt/Fileinhangthang/" + filedownload;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));

        }

        public ActionResult ExportDanhSachTong()
        {
            //check login
            if (!IsLogged())
                return BackToLogin();

            string maphongban = this.Request.Url.Segments[3];
            List<KpiEmployeeModels> KpiEmployee = new List<KpiEmployeeModels>();
            KpiEmployeeServices servicetp = new KpiEmployeeServices();
            //string chucdanhkpi = Session["chucdanhkpi"].ToString().Trim();
            //string chucdanhkpi =Session["loginid"].ToString().Trim().ToLower();
            KpiEmployee = servicetp.SelectRow_KpiEmployee_XuatExcel(maphongban);
            DataTable dt = new DataTable();
            dt.TableName = "Employee";
            dt.Columns.Add("stt");
            dt.Columns.Add("manhanvien");
            dt.Columns.Add("hovaten");
            dt.Columns.Add("tenphongban");
            dt.Columns.Add("tenchucdanh");
            dt.Columns.Add("tinhtranghoso");
            int i = 0;
            foreach (var item in KpiEmployee)
            {
                DataRow row = dt.NewRow();
                dt.Rows.Add(row);
                row["stt"] = i++;
                row["manhanvien"] = item.manhanvien;
                row["hovaten"] = item.hovaten;
                row["tenphongban"] = item.tenphongban;
                row["tenchucdanh"] = item.tenchucdanh;
                row["tinhtranghoso"] = item.truongphongxemxetkpi_duyet_text;
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
            return RedirectToAction("Index_Employee");
        }

        public ActionResult Deleted(string makpinhanvien)
        {
            if (!IsLogged())
                return BackToLogin();
            if (makpinhanvien != null)
            {
                //FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                //makpinhanvien = AES.DecryptText(makpinhanvien, function.ReadXMLGetKeyEncrypt());
                KpiEmployeeServices services = new KpiEmployeeServices();
                bool kq = services.DeletedRow_KpiEmployee(makpinhanvien, Session["userid"].ToString());
                if (kq)
                {

                }
            }
            return RedirectToAction("Index_Employee");
        }

        public ActionResult Save_KpiEmployee_Xoadongdulieu(string DataJson)
        {
            if (!IsLogged())
                return BackToLogin();
            if (DataJson != "0")
            {
                //FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                //makpinhanvien = AES.DecryptText(makpinhanvien, function.ReadXMLGetKeyEncrypt());
                KpiEmployeeServices services = new KpiEmployeeServices();
                bool kq = services.Save_KpiEmployee_Xoadongdulieu(DataJson, Session["userid"].ToString());
                if (kq)
                {

                }
            }
            return RedirectToAction("Index_Employee");
        }



        #endregion

    }
}