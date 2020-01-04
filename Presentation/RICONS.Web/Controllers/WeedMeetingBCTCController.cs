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
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Mime;

namespace RICONS.Web.Controllers
{
    public class WeedMeetingBCTCController : BaseController
    {
        Log4Net _logger = new Log4Net(typeof(WeedMeeting2Controller));

        public ActionResult Index()
        {
            //check login
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRowsBCTC(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string pb = "";
            string thudientu = Session["thudientu"].ToString().Trim();
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                //var lstpban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                //if (lstpban.Count() > 0)
                //    pb = "<option value=" + lstpban[0].maphongban + ">" + lstpban[0].tenphongban + "</option>";
                //pb = pb + "<option value=0>Chọn phòng ban</option>";
                foreach (var item in lstResult_phongban)
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.email == thudientu || p.sodienthoai == thudientu || p.ghichu == thudientu
                    || p.ghichu1 == thudientu || p.ghichu2 == thudientu
                    || p.cv_thietbi == thudientu || p.gs_thietbi == thudientu
                    || p.cv_hsse == thudientu || p.gs_hsse == thudientu
                    || p.cv_qaqc == thudientu || p.gs_qaqc == thudientu
                    || p.cv_mep == thudientu || p.gs_mep == thudientu))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            ViewBag.sbphongban = pb.ToString();


            //foreach (var item in lstResult_phongban.Where(p=>p.phongban_congtruong=="0"))
            //{
            //    sbphongban.Append(string.Format("<option value='{0}'>{1}</option>", item.maphongban, item.tenphongban));
            //}
            // ViewBag.sbphongban = sbphongban.ToString();
            return View();
        }

        [HttpPost]
        public JsonResult SelectRows_WeedMeeting(WeedMeetingModels model, int curentPage)
        {
            WeedMeetingModels param = new WeedMeetingModels();
            DaotaoServices service = new DaotaoServices();
            DanhmucServices service_danhmuc = new DanhmucServices();
            //param.nguoitao = int.Parse(Session["userid"].ToString());
            param.maphongban = model.maphongban;
            param.loaibaocao = model.loaibaocao;
            //if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            //{
            param.nguoitao = 0;
            //}
            string thudientu = Session["thudientu"].ToString().Trim();



            List<PhongBanModels> y = null;
            var lstcaptrentt = y;

            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service_danhmuc.SelectRowsBCTC(parampb);
            lstcaptrentt = lstResult_phongban.Where(p => p.maphongban == model.maphongban).ToList();

            //StringBuilder sbloaibaocao = new StringBuilder();
            //sbloaibaocao.Append(string.Format("<option value={0}>{1}</option>", "1", "Báo cáo tuần CHT/TPB"));
            if (lstcaptrentt[0].email == thudientu || lstcaptrentt[0].ghichu == thudientu || lstcaptrentt[0].sodienthoai == thudientu
                || lstcaptrentt[0].ghichu1 == thudientu || lstcaptrentt[0].ghichu2 == thudientu || Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                param.loaibaocao = param.loaibaocao;
            }
            else if (model.loaibaocao == 2 && (lstcaptrentt[0].cv_thietbi == thudientu || lstcaptrentt[0].gs_thietbi == thudientu))
            {
                param.loaibaocao = 2;
            }
            else if (model.loaibaocao == 3 && (lstcaptrentt[0].cv_hsse == thudientu || lstcaptrentt[0].gs_hsse == thudientu))
            {
                param.loaibaocao = 3;
            }

            else if (model.loaibaocao == 4 && (lstcaptrentt[0].cv_qaqc == thudientu || lstcaptrentt[0].gs_qaqc == thudientu))
            {
                param.loaibaocao = 4;
            }

            else if (model.loaibaocao == 5 && (lstcaptrentt[0].cv_mep == thudientu || lstcaptrentt[0].gs_mep == thudientu))
            {
                param.loaibaocao = 5;
            }
            else
            {
                param.loaibaocao = 6;
            }

            int tongsodong = service.CountRows_WeedMeeting2(param);
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

            List<WeedMeetingModels> lstResult = new List<WeedMeetingModels>();
            if (curentPage <= sotrang)
            {
                lstResult = service.SelectRows_WeedMeetingBCTC(param, trangbd, trangkt);
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
                    if (item.loaicuochop == "1") item.tenloaicuochop = "Họp tuần phòng";
                    else if (item.loaicuochop == "2") item.tenloaicuochop = "Họp tuần BGĐ";
                    else if (item.loaicuochop == "3") item.tenloaicuochop = "Họp khác";
                    else item.tenloaicuochop = "";

                    if (item.phonghop == "1") item.tenphonghop = "Tầng 3 - Phòng họp 1";
                    else if (item.phonghop == "2") item.tenphonghop = "Tầng 3 - Phòng họp 2";
                    else if (item.phonghop == "3") item.tenphonghop = "Tầng 3 - Phòng họp lớn";
                    else if (item.phonghop == "4") item.tenphonghop = "Tầng 3A - Phòng họp nhỏ";
                    else if (item.phonghop == "5") item.tenphonghop = "Tầng 3A - Phòng họp lớn";
                    else if (item.phonghop == "6") item.tenphonghop = "Khác";
                    else item.tenphonghop = "";

                    sbRows.Append(PrepareDataJson_WeedMeeting(item, strSTT));
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

        private StringBuilder PrepareDataJson_WeedMeeting(WeedMeetingModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.macuochop.ToString(), function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + AES.EncryptText(model.macuochop.ToString(), function.ReadXMLGetKeyEncrypt()) + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' macuochop='" + strEncryptCode + "'/>", model.macuochop);
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
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Edit", "WeedMeeting", new { macuochop = strEncryptCode }) + "'title='" + model.matuan + "'>" + model.tentuan + "</a>\"");
                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Edit", "WeedMeeting", new { macuochop = strEncryptCode }) + "'title='" + model.ngayhop + "'>" + model.ngayhop + "</a>\"");
                //sbResult.Append("\"col_value\":\"" + model.tenkhoahoc + "\"");
                sbResult.Append("},");

                //Họ và tên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Edit", "WeedMeeting", new { macuochop = strEncryptCode }) + "'title='" + model.loaicuochop + "'>" + model.tenloaicuochop + "</a>\"");
                sbResult.Append("},");


                //Tên đon vi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                //sbResult.Append("\"title\":\"" + model.madonvi + "\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Edit", "WeedMeeting", new { macuochop = strEncryptCode }) + "'title='" + model.maphongban + "'>" + model.tenphongban + "</a>\"");
                sbResult.Append("},");

                ////Tên phòng ban
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col7\",");
                //sbResult.Append("\"col_id\":\"7\",");
                //sbResult.Append("\"col_value\":\"" + model.lydobuoihop + "\"");
                //sbResult.Append("},");

                ////Tên phòng ban
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col8\",");
                //sbResult.Append("\"col_id\":\"8\",");

                //sbResult.Append("\"col_value\":\"" + model.thanhphanthamdu + "\"");
                //sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"<a href='" + Url.Action("DownloadFile", "WeedMeeting", new { tenfile = model.tenfile, idcode = model.uploadfile }) + "'>" + model.tenfile + "</a>\"");
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

        ///Thêm mới lịch họp tuần

        public ActionResult Create()
        {
            if (!IsLogged())
                return BackToLogin();
            Item_weedModels param = new Item_weedModels();
            DanhmucServices service = new DanhmucServices();
            List<Item_weedModels> lstResult_Tuan = service.SelectRows_giaovien(param);
            StringBuilder sbtuan = new StringBuilder();

            foreach (var item in lstResult_Tuan)
            {
                item.tentuan = item.tentuan + "   " + item.tungay + "  " + item.denngay;
                sbtuan.Append(string.Format("<option value='{0}'>{1}</option>", item.matuan, item.tentuan));
            }
            ViewBag.sbtuan = sbtuan.ToString();

            //phong ban
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows2(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string pb = "";
            string maphongban = Session["maphongban"].ToString().Trim();
            string macongtruong = Session["macongtruong"].ToString().Trim();
            int loaicuochop = (int)Session["loaicuochop"];
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                pb = pb + "<option value=0>Chọn phòng ban</option>";
                foreach (var item in lstResult_phongban)
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == macongtruong))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            ViewBag.sbphongban = pb.ToString();

            StringBuilder sbphonghop = new StringBuilder();
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "0", "Lựa chọn Phòng họp"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "1", "Tầng 3 - Phòng họp 1"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "2", "Tầng 3 - Phòng họp 2"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "3", "Tầng 3 - Phòng họp lớn"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "4", "Tầng 3A - Phòng họp nhỏ"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "5", "Tầng 3A - Phòng họp lớn"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "6", "Khác"));
            ViewBag.sbphonghop = sbphonghop.ToString();

            StringBuilder sbloaibaocao = new StringBuilder();
            if (loaicuochop == 1)
                sbloaibaocao.Append(string.Format("<option value={0}>{1}</option>", "1", "Báo cáo tuần CHT/TPB"));
            else if (loaicuochop == 2)
                sbloaibaocao.Append(string.Format("<option value={0}>{1}</option>", "2", "Báo cáo tuần Thiết Bị"));
            else if (loaicuochop == 3)
                sbloaibaocao.Append(string.Format("<option value={0}>{1}</option>", "3", "Báo cáo tuần HSSE"));
            else if (loaicuochop == 4)
                sbloaibaocao.Append(string.Format("<option value={0}>{1}</option>", "4", "Báo cáo tuần QAQC"));
            else if (loaicuochop == 5)
                sbloaibaocao.Append(string.Format("<option value={0}>{1}</option>", "5", "Báo cáo tuần MEP"));
            else
                sbloaibaocao.Append(string.Format("<option value={0}>{1}</option>", "6", "Chọn loại báo cáo"));

            ViewBag.sbloaibaocao = sbloaibaocao.ToString();

            return View();
        }

        [HttpPost]
        public JsonResult Save(string DataJson)
        {
            JsonResult Data = new JsonResult();

            JObject json = JObject.Parse(DataJson);
            string macuochop = json["uploadfile"].ToString();
            string filename = json["tenfile"].ToString();
            string maphongban = json["maphongban"].ToString();
            int loaibaocao = (int)json["loaibaocao"];
            int phongban_congtruong = (int)json["phongban_congtruong"];

            DaotaoServices servicevpp = new DaotaoServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = servicevpp.Save_WeedMeeting(DataJson, nguoitao);

            var directoryPath = Server.MapPath("~/FileUpload/") + macuochop.Replace("/", ".");
            if (!System.IO.Directory.Exists(directoryPath))
                System.IO.Directory.CreateDirectory(directoryPath);
            string path = Path.Combine(directoryPath, filename);

            if (iresult != "-1")
            {
                iresult = "1";
                if (loaibaocao == 1)
                    MailLich("TEST", path, filename, maphongban, phongban_congtruong);
                return Json(new { success = true, macuochop = int.Parse(iresult) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, macuochop = int.Parse(macuochop) }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(string macuochop)
        {
            if (!IsLogged())
                return BackToLogin();
            Item_weedModels param = new Item_weedModels();
            DanhmucServices service = new DanhmucServices();
            List<Item_weedModels> lstResult_Tuan = service.SelectRows_giaovien(param);
            StringBuilder sbtuan = new StringBuilder();
            foreach (var item in lstResult_Tuan)
            {
                item.tentuan = item.tentuan + "   " + item.tungay + " - " + item.denngay;
                sbtuan.Append(string.Format("<option value='{0}'>{1}</option>", item.matuan, item.tentuan));
            }
            ViewBag.sbtuan = sbtuan.ToString();

            //phong ban
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string pb = "";
            string maphongban = Session["maphongban"].ToString().Trim();
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                pb = pb + "<option value=0>Chọn phòng ban</option>";
                foreach (var item in lstResult_phongban)
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.maphongban == maphongban))
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            ViewBag.sbphongban = pb.ToString();

            StringBuilder sbphonghop = new StringBuilder();
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "0", "Lựa chọn Phòng họp"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "1", "Tầng 3 - Phòng họp 1"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "2", "Tầng 3 - Phòng họp 2"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "3", "Tầng 3 - Phòng họp lớn"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "4", "Tầng 3A - Phòng họp nhỏ"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "5", "Tầng 3A - Phòng họp lớn"));
            sbphonghop.Append(string.Format("<option value={0}>{1}</option>", "6", "Khác"));
            ViewBag.sbphonghop = sbphonghop.ToString();

            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            macuochop = AES.DecryptText(macuochop, function.ReadXMLGetKeyEncrypt());
            DaotaoServices servicevpp = new DaotaoServices();
            List<WeedMeetingModels> lstResult = new List<WeedMeetingModels>();
            lstResult = servicevpp.SelectRows_WeedMeeting_hieuchinh(macuochop);
            if (lstResult.Count > 0)
            {
                //replace(/'/g, "daunhaydon").replace(/"/g, '').replace(/&/g, 'daukytuva') + "',";
                lstResult[0].lydobuoihop = lstResult[0].lydobuoihop.Replace("daunhaydon", "'").Replace("daukytuva", "&");
                lstResult[0].thanhphanthamdu = lstResult[0].thanhphanthamdu.Replace("daunhaydon", "'").Replace("daukytuva", "&");

                //System.Text.RegularExpressions.Regex.Replace(json["noidungcuochop"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                string chuoinoidungs = "";
                string chuoinoidung = System.Text.RegularExpressions.Regex.Replace(lstResult[0].noidungcuochop, @"\\n", "\n");
                string[] noisungcuochop = chuoinoidung.Split(new char[] { '\n' });
                for (int i = 0; i < noisungcuochop.Length; i++)
                    chuoinoidungs = chuoinoidungs + noisungcuochop[i].Replace("daunhaydon", "'").Replace("daukytuva", "&") + Environment.NewLine;
                lstResult[0].noidungcuochop = chuoinoidungs;
                return View(lstResult[0]);
            }

            return View();
        }

        public ActionResult Deleted(string madangky)
        {
            if (!IsLogged())
                return BackToLogin();
            if (madangky != null)
            {
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                madangky = AES.DecryptText(madangky, function.ReadXMLGetKeyEncrypt());
                DaotaoServices service = new DaotaoServices();
                bool kq = service.DeletedRow_Dangky_vpp(madangky, Session["userid"].ToString());
                if (kq)
                {

                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Upload(string macuochop)
        {
            //FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            //string strEncryptCode = AES.DecryptText(macuochop, function.ReadXMLGetKeyEncrypt());
            if (macuochop != "0" && macuochop != "" && macuochop != null)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    var directoryPath = Server.MapPath("~/FileUpload/") + macuochop.Replace("/", ".");
                    if (!System.IO.Directory.Exists(directoryPath))
                        System.IO.Directory.CreateDirectory(directoryPath);
                    var path = Path.Combine(directoryPath, fileName);
                    file.SaveAs(path);
                    //DaotaoServices daotaosr = new DaotaoServices();
                    //daotaosr.Save_Capnhatfileupload(strEncryptCode, fileName);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DownloadFile(string tenfile, string idcode)
        {
            var FileVirtualPath = "~/FileUpload/" + idcode + "/" + tenfile;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        public void MailLich(string NoiDung, string path, string filename, string maphongban, int phongban_congtruong)
        {
            string sMailGui = System.Configuration.ConfigurationManager.AppSettings["MailSend"];
            string sPass = System.Configuration.ConfigurationManager.AppSettings["MailPass"];
            string sHost = System.Configuration.ConfigurationManager.AppSettings["MailHost"];
            string sPort = System.Configuration.ConfigurationManager.AppSettings["MailPort"];
            string sTieuDe = System.Configuration.ConfigurationManager.AppSettings["ScheduleSubject"];
            string sMailTo = System.Configuration.ConfigurationManager.AppSettings["schedulerman"];

            // gui mail

            var fromAddress = new MailAddress(sMailGui);

            string fromPassword = sPass;
            string subject = sTieuDe;
            string body = NoiDung;

            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();

            List<PhongBanModels> lstResult_phongban;
            List<PhongBanModels> y = null;
            var lstcaptrentt = y;

            if (phongban_congtruong == 0)
            {
                lstResult_phongban = service.SelectRows(parampb);
                lstcaptrentt = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                sMailTo = lstcaptrentt[0].email;
            }
            else
            {
                lstResult_phongban = service.SelectRows2(parampb);
                lstcaptrentt = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                sMailTo = lstcaptrentt[0].email;
            }

            var toAddress = new MailAddress(sMailTo);

            var smtp = new SmtpClient
            {
                Host = sHost,
                Port = int.Parse(sPort),
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            //var smtp = new SmtpClient();

            subject = "NEWTECONS PROJECT REPORT: " + lstcaptrentt[0].tenphongban;
            body = "<p>Kính gửi anh chị,</p><p>Đính kèm là báo cáo tuần gửi từ hệ thống của: " + lstcaptrentt[0].tenphongban + "</p>" + "<p>Trân trọng,</p>";

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    Attachment data = new Attachment(path, MediaTypeNames.Application.Octet);
                    message.Attachments.Add(data);
                    if (!string.IsNullOrEmpty(lstcaptrentt[0].sodienthoai))
                        message.CC.Add(lstcaptrentt[0].sodienthoai);

                    if (!string.IsNullOrEmpty(lstcaptrentt[0].ghichu))
                        message.CC.Add(lstcaptrentt[0].ghichu);

                    if (!string.IsNullOrEmpty(lstcaptrentt[0].ghichu1))
                        message.CC.Add(lstcaptrentt[0].ghichu1);

                    if (!string.IsNullOrEmpty(lstcaptrentt[0].ghichu2))
                        message.CC.Add(lstcaptrentt[0].ghichu2);
                    smtp.Send(message);
                    smtp.Dispose();
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                        {
                            // Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                            System.Threading.Thread.Sleep(5000);
                            smtp.Send(message);
                        }
                        else
                        {
                            //  Console.WriteLine("Failed to deliver message to {0}", ex.InnerExceptions[i].FailedRecipient);
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //  Console.WriteLine("Exception caught in RetryIfBusy(): {0}",ex.ToString());
                    throw ex;
                }
                finally
                {
                    smtp.Dispose();
                }
            }

        }

        protected override void Dispose(bool disposing)
        {
            //taskService.Dispose();
            base.Dispose(disposing);
        }
    }
}