using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Controllers;
using RICONS.DataServices.Context;
using RICONS.DataServices.Executes.CongTruong;
using RICONS.DataServices.Executes.ConstructionMeetings;

namespace RICONS.Web.Controllers
{
    public class ConstructionMeetingController : BaseController
    {
        // GET: ConstructionMeeting
        public ActionResult Index()
        {
            if (!IsLogged())
                return BackToLogin();
            var lstGiamDoc = _dataService.GetListGiamDocDuAn();
            //List<m_donvi_thongtingiamdoc> abc = new List<m_donvi_thongtingiamdoc>();
            //if (Session["grouptk"].ToString().Trim() == "3")
            //{
            //    string mail = Session["thudientu"].ToString().Trim();
            //    //foreach (var row in congtruong)
            //    //{
            //    //    lstGiamDoc = lstGiamDoc.Where(x => x.mathongtin == row.thuocquanly.Value || x.mathongtin == row.thuocquanly1.Value || x.mathongtin == row.thuocquanly2.Value).ToList();
            //    //    if (lstGiamDoc.Count > 0)
            //    //        abc.Add(new m_donvi_thongtingiamdoc { mathongtin = lstGiamDoc[0].mathongtin, hovaten = lstGiamDoc[0].hovaten });
            //    //}
            //}
            try
            {
                if (Session["grouptk"].ToString().Trim() == "3")
                {
                    var ct = Session["maphongban"] as string;
                    var congtruong = _dataService.GetListCongTruong(new SearchCongTruongModel()
                    {
                        CongTruong = ct,
                        EmailThuKy = Session["thudientu"].ToString().Trim()
                    }).FirstOrDefault();
                    ViewData["CongTruong"] = congtruong;
                }
                ViewData["LstGiamDoc"] = lstGiamDoc;
            }
            catch (Exception) {
                lstGiamDoc = new List<m_donvi_thongtingiamdoc>();
                lstGiamDoc.Add(new m_donvi_thongtingiamdoc { mathongtin = 0, hovaten = "" });
                ViewData["LstGiamDoc"] = lstGiamDoc;
            }
            return View();
        }
        public ActionResult Index2()
        {
            if (!IsLogged())
                return BackToLogin();
            return View();
        }

        public ActionResult ConstructionMeetingEdit(int? id, int giamdoc, int congtruong, DateTime date)
        {
            var model = new ConstructionMeetingViewModel()
            {
                GiamDoc = giamdoc,
                CongTruong = congtruong,
                Date = date
            };
            if (id.HasValue)
            {
                model = _dataService.ConstructionMeetingOne(id.Value);
            }
            return PartialView("~/Views/ConstructionMeeting/Partials/ConstructionMeetingEdit.cshtml", model);
        }
        [HttpPost]
        public ActionResult ConstructionMeetingEdit(ConstructionMeetingEditModel model)
        {
            if (model.FileDinhKemPostFileBase != null && model.FileDinhKemPostFileBase.FileName != "")
            {
                var ext = Path.GetExtension(model.FileDinhKemPostFileBase.FileName);
                var path = DateFolder("/assets/files/", null) + "/" + Guid.NewGuid() + ext;
                var fp = Server.MapPath(path);
                model.FileDinhKemPostFileBase.SaveAs(fp);
                model.FileDinhKem = path;
            }
            model.Date = DateTime.ParseExact(model.DateStr,"MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var result = _dataService.ConstructionMeetingCommand(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteConstructionMeeting(int id)
        {
            var result = _dataService.DeleteConstructionMeeting(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListCongTruong(SearchCongTruongModel model)
        {
            var result = _dataService.GetListCongTruong(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListConstructionMeeting(SearchConstructionMeetingModel model)
        {
            if (Session["grouptk"].ToString() == "3") model.CongTruong = int.Parse(Session["maphongban"].ToString());
            var result = _dataService.ConstructionMeetingMany(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string DateFolder(string path, DateTime? date)
        {

            var now = date ?? DateTime.Now;
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            path += "/" + now.Year;
            var fp = Server.MapPath(path);

            if (!Directory.Exists(fp))
            {
                Directory.CreateDirectory(fp);
            }
            path += "/" + now.Month;
            fp = Server.MapPath(path);
            if (!Directory.Exists(fp))
            {
                Directory.CreateDirectory(fp);
            }
            path += "/" + now.Day;
            fp = Server.MapPath(path);
            if (!Directory.Exists(fp))
            {
                Directory.CreateDirectory(fp);
            }
            return path;
        }
    }
}