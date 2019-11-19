using RICONS.Core.Functions;
using RICONS.DataServices.DataClass;
using RICONS.Logger;
using RICONS.Web.Data.Services;
using RICONS.Web.Models;
using Nop.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace RICONS.Web.Controllers
{
    public class Danhmuc2Controller : BaseController
    {
        public static string maphongban = "";
        string ngaytao = "GETDATE()";
        public static List<thongtingiamdocModels> lstthongtingiamdoc = new List<thongtingiamdocModels>();
        #region Fields
        Log4Net _logger = new Log4Net(typeof(DanhmucController));
        #endregion
        //
        // GET: /Danhmuc phong ban/
        public ActionResult Index()
        {
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            thongtingiamdocModels param = new thongtingiamdocModels();
            lstthongtingiamdoc = service.SelectRows_thongtingiamdoc(param);
            StringBuilder sbTendonvi = new StringBuilder();
            foreach (var item in lstthongtingiamdoc)
            {
                sbTendonvi.Append(string.Format("<option value='{0}'>{1}</option>", item.mathongtin, item.hovaten));
            }

            ViewBag.keHoachs = sbTendonvi.ToString();
            return View();
        }
        [HttpPost]
        public JsonResult SelectRows(PhongBanModels model)
        {
            PhongBanModels param = new PhongBanModels();
            DanhmucServices service = new DanhmucServices();
            List<PhongBanModels> lstResult = service.SelectRows(param);
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;

                foreach (var item in lstResult)
                {
                    strSTT = i.ToString();

                    if (item.madonvi == "1")
                        item.tendonvi = "Công ty cổ phẩn Ricons";
                    sbRows.Append(PrepareDataJson(item, strSTT));
                    i++;
                }
                if (sbRows.Length > 0)
                    sbRows.Remove(sbRows.Length - 1, 1);
            }
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            sbResult.Append("\"currentRow\":\"" + lstResult.Count + "\",");
            sbResult.Append("\"Pages\":\"" + lstResult.Count + "\",");
            if (model.maphongban != "0")
            {
                sbResult.Append("\"SubRow\":\"" + "true" + "\",");
                sbResult.Append("\"RowID\":\"" + model.maphongban + "\",");
            }
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");

            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson(PhongBanModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.maphongban, function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + AES.EncryptText(model.maphongban, function.ReadXMLGetKeyEncrypt()) + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' mpb='" + strEncryptCode + "'/>", model.maphongban);
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
                sbResult.Append("\"col_value\":\"" + model.maphongban.Trim() + "\"");
                sbResult.Append("},");

                //Mã phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + model.tenphongban.Trim() + "\"");
                sbResult.Append("},");

                //ho va ten
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + model.hovaten.Trim() + "\"");
                sbResult.Append("},");

                //Email
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + model.email + "\"");
                sbResult.Append("},");
                //so dien thoai luu thanh email cap quan ly
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                sbResult.Append("\"col_value\":\"" + model.sodienthoai.Trim() + "\"");
                sbResult.Append("},");



                //thuoc quan ly
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"title\":\"" + model.thuocquanly.Trim() + "\",");
                sbResult.Append("\"col_value\":\"" + model.hotenquanly + "\"");
                sbResult.Append("},");

                //Ghi chú
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + model.ghichu.Trim() + "\"");
                sbResult.Append("},");


                //thuoc quan ly1
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col11\",");
                sbResult.Append("\"col_id\":\"11\",");
                sbResult.Append("\"title\":\"" + model.thuocquanly1.Trim() + "\",");
                sbResult.Append("\"col_value\":\"" + model.hotenquanly1 + "\"");
                sbResult.Append("},");

                //Ghi chú1
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col12\",");
                sbResult.Append("\"col_id\":\"12\",");
                sbResult.Append("\"col_value\":\"" + model.ghichu1.Trim() + "\"");
                sbResult.Append("},");


                //thuoc quan ly2
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col13\",");
                sbResult.Append("\"col_id\":\"13\",");
                sbResult.Append("\"title\":\"" + model.thuocquanly2.Trim() + "\",");
                sbResult.Append("\"col_value\":\"" + model.hotenquanly2 + "\"");
                sbResult.Append("},");

                //Ghi chú2
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col14\",");
                sbResult.Append("\"col_id\":\"14\",");
                sbResult.Append("\"col_value\":\"" + model.ghichu2.Trim() + "\"");
                sbResult.Append("},");






                //dinh kem tap tin
                string strHTML_Attachment = "";
                #region
                //string link = Url.Action("Edit", "Account", new { id = EncDec.EncodeCrypto(model.mataikhoan) });
                strHTML_Attachment = "<a href='#' class='edit' ><i class='fa fa-pencil-square-o' ></i></a>&nbsp;<a href='#' class='del'><i class='fa fa-trash-o' ></i></a>";
                #endregion
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
        public ActionResult Updatephongban(string act, string ID, PhongBanModels model)
        {
            if (!IsLogged())
                return BackToLogin();
            if (act == "create")
            {
                PhongBanModels paramphongban = new PhongBanModels();
                paramphongban.maphongban = "";
                paramphongban.madonvi = "1";
                paramphongban.tenphongban = model.tenphongban;
                paramphongban.sodienthoai = model.sodienthoai;

                paramphongban.email = model.email;
                paramphongban.hovaten = model.hovaten;


                var lstcaptrentt = lstthongtingiamdoc.Where(p => p.mathongtin == model.thuocquanly).ToList();
                if (lstcaptrentt.Count > 0)
                {
                    paramphongban.thuocquanly = model.thuocquanly;
                    paramphongban.hotenquanly = lstcaptrentt[0].hovaten;
                    paramphongban.ghichu = model.ghichu;
                }

                var lstcaptrentt1 = lstthongtingiamdoc.Where(p => p.mathongtin == model.thuocquanly1).ToList();
                if (lstcaptrentt1.Count > 0)
                {
                    paramphongban.thuocquanly1 = model.thuocquanly1;
                    paramphongban.hotenquanly1 = lstcaptrentt1[0].hovaten;
                    paramphongban.ghichu1 = model.ghichu1;
                }

                var lstcaptrentt2 = lstthongtingiamdoc.Where(p => p.mathongtin == model.thuocquanly2).ToList();
                if (lstcaptrentt2.Count > 0)
                {
                    paramphongban.thuocquanly2 = model.thuocquanly2;
                    paramphongban.hotenquanly2 = lstcaptrentt1[0].hovaten;
                    paramphongban.ghichu2 = model.ghichu2;
                }


                paramphongban.xoa = "0";
                paramphongban.nguoitao = int.Parse(Session["userid"].ToString());
                paramphongban.ngaytao = ngaytao;
                paramphongban.phongban_congtruong = "0";
                DanhmucServices services = new DanhmucServices();
                string result = services.Insert_phongban(paramphongban);
                maphongban = result;
            }
            else if (act == "update")
            {
                PhongBanModels paramphongban = new PhongBanModels();
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                paramphongban.maphongban = AES.DecryptText(ID, function.ReadXMLGetKeyEncrypt());
                paramphongban.madonvi = "1";
                paramphongban.tenphongban = model.tenphongban;
                paramphongban.sodienthoai = model.sodienthoai;

                paramphongban.email = model.email;
                paramphongban.hovaten = model.hovaten;

                var lstcaptrentt = lstthongtingiamdoc.Where(p => p.mathongtin == model.thuocquanly).ToList();
                if (lstcaptrentt.Count > 0)
                {
                    paramphongban.thuocquanly = model.thuocquanly;
                    paramphongban.hotenquanly = lstcaptrentt[0].hovaten;
                    paramphongban.ghichu = model.ghichu;
                }

                var lstcaptrentt1 = lstthongtingiamdoc.Where(p => p.mathongtin == model.thuocquanly1).ToList();
                if (lstcaptrentt1.Count > 0)
                {
                    paramphongban.thuocquanly1 = model.thuocquanly1;
                    paramphongban.hotenquanly1 = lstcaptrentt1[0].hovaten;
                    paramphongban.ghichu1 = model.ghichu1;
                }

                var lstcaptrentt2 = lstthongtingiamdoc.Where(p => p.mathongtin == model.thuocquanly2).ToList();
                if (lstcaptrentt2.Count > 0)
                {
                    paramphongban.thuocquanly2 = model.thuocquanly2;
                    paramphongban.hotenquanly2 = lstcaptrentt1[0].hovaten;
                    paramphongban.ghichu2 = model.ghichu2;
                }

                paramphongban.xoa = "0";
                paramphongban.nguoitao = int.Parse(Session["userid"].ToString());
                paramphongban.ngaytao = ngaytao;
                DanhmucServices services = new DanhmucServices();
                string result = services.Insert_phongban(paramphongban);
                maphongban = result;
            }
            else if (act == "delete")
            {
                PhongBanModels paramphongban = new PhongBanModels();
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                paramphongban.maphongban = AES.DecryptText(ID, function.ReadXMLGetKeyEncrypt());
                paramphongban.nguoihieuchinh = int.Parse(Session["userid"].ToString());
                DanhmucServices services = new DanhmucServices();
                string result = services.Deleted_phongban(paramphongban.maphongban.ToString(), paramphongban.nguoihieuchinh.ToString());
                maphongban = result;
            }
            return RedirectToAction("Index", "Danhmuc");
        }





        /// //Danh muc chuc danh chuc vu
        public ActionResult Indexchucdanh()
        {
            if (!IsLogged())
                return BackToLogin();
            return View();
        }

        [HttpPost]
        public JsonResult SelectRows_chucvu(ChucDanhModels model)
        {
            ChucDanhModels param = new ChucDanhModels();
            DanhmucServices service = new DanhmucServices();
            List<ChucDanhModels> lstResult = service.SelectRows_chucvu(param);
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbRows = new StringBuilder();
            if (lstResult.Count > 0)
            {
                string strSTT = "";
                int i = 1;

                foreach (var item in lstResult)
                {
                    strSTT = i.ToString();
                    sbRows.Append(PrepareDataJson_chucvu(item, strSTT));
                    i++;
                }
                if (sbRows.Length > 0)
                    sbRows.Remove(sbRows.Length - 1, 1);
            }
            sbResult.Append("{");
            sbResult.Append("\"isHeader\":\"" + "111" + "\",");
            sbResult.Append("\"currentRow\":\"" + lstResult.Count + "\",");
            sbResult.Append("\"Pages\":\"" + lstResult.Count + "\",");
            if (model.machucdanh.ToString() != "0")
            {
                sbResult.Append("\"SubRow\":\"" + "true" + "\",");
                sbResult.Append("\"RowID\":\"" + model.machucdanh + "\",");
            }
            sbResult.Append("\"data\":[" + sbRows.ToString() + "]");
            sbResult.Append("}");

            return Json(sbResult.ToString(), JsonRequestBehavior.AllowGet);
        }

        private StringBuilder PrepareDataJson_chucvu(ChucDanhModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.machucdanh.ToString(), function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + AES.EncryptText(model.machucdanh.ToString(), function.ReadXMLGetKeyEncrypt()) + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' mpb='" + strEncryptCode + "'/>", model.machucdanh);
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
                sbResult.Append("\"col_value\":\"" + model.machucdanh + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + model.tenchucdanh + "<a href='" + Url.Action("Indexchucdanh", "Danhmuc", new { mapb = strEncryptCode }) + "' title='" + model.tenchucdanh + "'></a>" + "\"");
                sbResult.Append("},");

                //Mã phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + model.tengiaodich + "\"");
                sbResult.Append("},");

                //// Số diện thoại
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col6\",");
                //sbResult.Append("\"col_id\":\"6\",");
                //sbResult.Append("\"col_value\":\"" + model.tenphongban + "\"");
                //sbResult.Append("},");

                // Số diện thoại
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + model.ghichu + "\"");
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
        public ActionResult Updatechucvu(string act, string ID, ChucDanhModels model)
        {
            if (!IsLogged())
                return BackToLogin();
            if (act == "create")
            {
                ChucDanhModels param = new ChucDanhModels();
                param.machucdanh = model.machucdanh;
                param.tenchucdanh = model.tenchucdanh;
                param.tengiaodich = model.tengiaodich;
                param.ghichu = model.ghichu;
                param.xoa = "0";
                param.nguoitao = int.Parse(Session["userid"].ToString());
                param.ngaytao = ngaytao;
                DanhmucServices services = new DanhmucServices();
                string result = services.InsertRow_chucdanh(param);
            }
            else if (act == "update")
            {
                ChucDanhModels param = new ChucDanhModels();
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                param.machucdanh = int.Parse(AES.DecryptText(ID, function.ReadXMLGetKeyEncrypt()));
                param.tenchucdanh = model.tenchucdanh;
                param.tengiaodich = model.tengiaodich;
                param.ghichu = model.ghichu;
                param.xoa = "0";
                param.nguoitao = int.Parse(Session["userid"].ToString());
                param.ngaytao = ngaytao;
                DanhmucServices services = new DanhmucServices();
                bool result = services.UpdateRow_chucdanh(param);
            }
            else if (act == "delete")
            {
                ChucDanhModels param = new ChucDanhModels();
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                param.machucdanh = int.Parse(AES.DecryptText(ID, function.ReadXMLGetKeyEncrypt()));

                param.nguoihieuchinh = int.Parse(Session["userid"].ToString());
                DanhmucServices services = new DanhmucServices();

                bool result = services.DeleteRows_chucdanh(param);

            }
            return RedirectToAction("Indexchucdanh", "Danhmuc");
        }

        /// Danh mục giáo viên

        public ActionResult Indexgiaovien()
        {
            if (!IsLogged())
                return BackToLogin();
            StringBuilder sbTendonvi = new StringBuilder();
            sbTendonvi.Append(string.Format("<option value='{0}'>{1}</option>", "1", "Nội bộ"));
            sbTendonvi.Append(string.Format("<option value='{0}'>{1}</option>", "2", "Bên ngoài"));
            ViewBag.keHoachs = sbTendonvi.ToString();
            return View();
        }





    }
}