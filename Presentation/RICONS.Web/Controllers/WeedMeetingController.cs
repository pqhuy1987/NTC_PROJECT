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
    public class WeedMeetingController : BaseController
    {
        Log4Net _logger = new Log4Net(typeof(WeedMeetingController));

        public ActionResult Index()
        {
            //check login
            if (!IsLogged())
                return BackToLogin();
            DanhmucServices service = new DanhmucServices();
            PhongBanModels parampb = new PhongBanModels();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            string pb = "";
            string thudientu = Session["thudientu"].ToString().Trim();
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                //var lstpban = lstResult_phongban.Where(p => p.maphongban == maphongban).ToList();
                //if (lstpban.Count() > 0)
                //    pb = "<option value=" + lstpban[0].maphongban + ">" + lstpban[0].tenphongban + "</option>";
                pb = pb + "<option value=0>Chọn phòng ban</option>";
                foreach (var item in lstResult_phongban)
                    pb = pb + "<option value=" + item.maphongban + "> " + item.tenphongban + " </option>";
            }
            else
            {
                foreach (var item in lstResult_phongban.Where(p => p.email == thudientu || p.ghichu == thudientu))
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
            //param.nguoitao = int.Parse(Session["userid"].ToString());
            param.maphongban = model.maphongban;
            if (Session["loginid"].ToString().Trim().ToLower() == "admin" || Session["grouptk"].ToString().Trim() == "1")
            {
                param.nguoitao = 0;
            }

            int tongsodong = service.CountRows_WeedMeeting(param);
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
                lstResult = service.SelectRows_WeedMeeting(param, trangbd, trangkt);
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

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"col_value\":\"" + model.lydobuoihop + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");

                sbResult.Append("\"col_value\":\"" + model.thanhphanthamdu + "\"");
                sbResult.Append("},");

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
            return View();
        }

        [HttpPost]
        public JsonResult Save(string DataJson)
        {
            JsonResult Data = new JsonResult();

            JObject json = JObject.Parse(DataJson);
            string macuochop = json["macuochop"].ToString();

            DaotaoServices servicevpp = new DaotaoServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = servicevpp.Save_WeedMeeting(DataJson, nguoitao);

            if (iresult != "-1")
             {
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
                string chuoinoidungs="";
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
                     var directoryPath = Server.MapPath("~/FileUpload/") + macuochop.Replace("/",".");
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
    }
}