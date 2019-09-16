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
    public class AftertrainingController : BaseController
    {
        Log4Net _logger = new Log4Net(typeof(AftertrainingController));

        public ActionResult Index()
        {
            //check login
            if (!IsLogged())
                return BackToLogin();
            daotao_taolopModels param = new daotao_taolopModels();
            List<daotao_taolopModels> lstResult_lop = new List<daotao_taolopModels>();
            SaudaotaoServices service_lop = new SaudaotaoServices();
            if (Session["thudientu"].ToString().Trim() == "1" || Session["loginid"].ToString().Trim() == "admin")
            {
                param.email = "";
            }
            else param.email = Session["thudientu"].ToString();

            lstResult_lop = service_lop.SelectRows_Laydslopdaotao(param);

            StringBuilder sblophoc = new StringBuilder();
            foreach (var item in lstResult_lop)
            {
                sblophoc.Append(string.Format("<option value='{0}'>{1}</option>", item.malop, item.tenlop));
            }
            ViewBag.sblophoc = sblophoc.ToString();
            return View();
        }

        [HttpPost]
        public JsonResult SelectRows_Danhsachdanhgiasaudaotao(daotao_ykiensaodaotaoModels model, int curentPage)
        {
            daotao_ykiensaodaotaoModels param = new daotao_ykiensaodaotaoModels();
            SaudaotaoServices service = new SaudaotaoServices();
            param.nguoitao = int.Parse(Session["userid"].ToString());
            param.malop = model.malop;
            int tongsodong = service.CountRows_Danhsachdanhgiasaudaotao(param);
            int sotrang = 1;
            if (tongsodong > 30)
            {
                if (tongsodong % 30 > 0)
                {
                    sotrang = (tongsodong / 30) + 1;
                }
                else
                {
                    sotrang = (tongsodong / 30);
                }
            }

            int trangbd = 1; int trangkt = 30;
            if (curentPage != 1 && curentPage <= sotrang)
            {
                trangbd = (trangkt * (curentPage - 1)) + 1;
                trangkt = trangkt * curentPage;
            }

            List<daotao_ykiensaodaotaoModels> lstResult = new List<daotao_ykiensaodaotaoModels>();
            if (curentPage <= sotrang)
            {
                
                lstResult = service.SelectRows_Danhsachdanhgiasaudaotao(param, trangbd, trangkt);
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
                    sbRows.Append(PrepareDataJson_Danhsachdanhgiasaudaotao(item, strSTT));
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

        private StringBuilder PrepareDataJson_Danhsachdanhgiasaudaotao(daotao_ykiensaodaotaoModels model, string couter)
        {
            //duong dan file encryption key
            FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
            StringBuilder sbResult = new StringBuilder();
            string strEncryptCode = AES.EncryptText(model.matiepnhan.ToString(), function.ReadXMLGetKeyEncrypt());
            string mahoa_lop = AES.EncryptText(model.malop.ToString(), function.ReadXMLGetKeyEncrypt());
            try
            {
                sbResult.Append("{");
                sbResult.Append("\"col_class\":\"rows-box\",");
                sbResult.Append("\"col_id\":\"" + strEncryptCode + "\",");
                sbResult.Append("\"col_attr\":[{\"name\":\"" + "subparent" + "\", \"value\":\"" + mahoa_lop + "\"}],");
                sbResult.Append("\"col_value\":[");
                #region Data cell
                //colum checkbox
                //string strHTML_Checkbox = string.Format("<input type='checkbox' onclick='Select(this);' class='chkCheck' codeid='{0}' malop='" + strEncryptCode + "'/>", model.malop);
                //sbResult.Append("{");
                //sbResult.Append("\"colspan\":\"1\",");
                //sbResult.Append("\"col_class\":\"ovh col1\",");
                //sbResult.Append("\"col_id\":\"1\",");
                //sbResult.Append("\"col_value\":\"" + strHTML_Checkbox + "\"");
                //sbResult.Append("},");
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

                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Createtrainning", "Aftertraining", new { idmalophc = model.malop }) + "'title='" + model.tenlop + "'>" + model.tenlop + "</a>\"");

                sbResult.Append("},");

                //Mã nhân viên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col4\",");
                sbResult.Append("\"col_id\":\"4\",");
                sbResult.Append("\"col_value\":\"" + "<a href='" + Url.Action("Createtrainning", "Classlist", new { idmalophc = model.malop }) + "'title='" + model.tieudekhoahoc + "'>" + model.tieudekhoahoc + "</a>\"");
                sbResult.Append("},");

                //Họ và tên
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col5\",");
                sbResult.Append("\"col_id\":\"5\",");
                sbResult.Append("\"col_value\":\"" + model.manv + "\"");
                sbResult.Append("},");


                //Tên đon vi
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col6\",");
                sbResult.Append("\"col_id\":\"6\",");
                sbResult.Append("\"col_value\":\"" + model.hovaten + "\"");
                sbResult.Append("},");


                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col7\",");
                sbResult.Append("\"col_id\":\"7\",");
                sbResult.Append("\"col_value\":\"" + model.email + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col8\",");
                sbResult.Append("\"col_id\":\"8\",");
                sbResult.Append("\"title\":\"" + model.maphongban + "\",");
                sbResult.Append("\"col_value\":\"" + model.tenphongban + "\"");
                sbResult.Append("},");

                //Tên phòng ban
                sbResult.Append("{");
                sbResult.Append("\"colspan\":\"1\",");
                sbResult.Append("\"col_class\":\"ovh col9\",");
                sbResult.Append("\"col_id\":\"9\",");
                sbResult.Append("\"col_value\":\"" + model.ngaydaotao + "\"");
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



        public static string maphongbanexcel = "0";

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

        public ActionResult Createtrainning(string idmalophc)
        {
            if (!IsLogged())
                return BackToLogin();

            daotao_taolopModels param = new daotao_taolopModels();
            List<daotao_taolopModels> lstResult_lop = new List<daotao_taolopModels>();
            SaudaotaoServices service_lop = new SaudaotaoServices();
            param.email = Session["thudientu"].ToString();
            lstResult_lop = service_lop.SelectRows_Laydslophoctheo_nhanvien(param);

            if (idmalophc == null)
            {
                StringBuilder sblophoc = new StringBuilder();
                sblophoc.Append(string.Format("<option value='{0}'>{1}</option>", "0", "Chọn tên lớp học"));
                foreach (var item in lstResult_lop)
                {
                    sblophoc.Append(string.Format("<option value='{0}'>{1}</option>", item.malop, item.tenlop));
                }
                ViewBag.sblophoc = sblophoc.ToString();
            }
            else
            {
                StringBuilder sblophoc = new StringBuilder();
                foreach (var item in lstResult_lop.Where(p=>p.malop==int.Parse(idmalophc.Trim())))
                {
                    sblophoc.Append(string.Format("<option value='{0}'>{1}</option>", item.malop, item.tenlop));
                }
                ViewBag.sblophoc = sblophoc.ToString();
            }

            //phong ban
            PhongBanModels parampb = new PhongBanModels();
            DanhmucServices service = new DanhmucServices();
            List<PhongBanModels> lstResult_phongban = service.SelectRows(parampb);
            StringBuilder sbphongban = new StringBuilder();
            foreach (var item in lstResult_phongban)
            {
                sbphongban.Append(string.Format("<option value={0}>{1}</option>", item.maphongban, item.tenphongban));
            }
            ViewBag.sbphongban = sbphongban.ToString();



            return View();
        }

      

        public static string matiepnhan = "";

        [HttpPost]
        public JsonResult Save(string DataJson)
        {
            JsonResult Data = new JsonResult();
            SaudaotaoServices servicevpp = new SaudaotaoServices();
            string nguoitao = Session["userid"].ToString();
            string iresult = servicevpp.Save_ykiensaodautao(DataJson, nguoitao, matiepnhan);
            if (iresult != "-1")
            {
                matiepnhan = iresult;
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}