using System;
using System.IO;
using System.Web.Mvc;
using Nop.Admin.Controllers;
using RICONS.Core.Functions;
using RICONS.DataServices.Executes.Collaborates;
using RICONS.DataServices.Executes.CongTruong;
using RICONS.Services.Variables;
using RICONS.Web.Models;

namespace RICONS.Web.Controllers
{
    public class CollaborateController : BaseController
    {
        // GET: Collaborate
        public ActionResult Index()
        {
            if (!IsLogged())
                return BackToLogin();
            return View();
        }

        public ActionResult CollaborateList(SearchCollaborateModel model, OptionResult option)
        {
            if (Session["loginid"].ToString() != "admin")
            {
                model.NguoiDi = Int32.Parse(Session["userid"].ToString());
            }
            var result = _dataService.CollaborateMany(model, option);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CollaborateEdit(int? id)
        {
            var mpb = Session["maphongban"] as string;
            var pb = _dataService.PhongBanOne(mpb);
            var model = new CollaborateViewModel()
            {
                TuNgay = DateTime.Now,
                DenNgay = DateTime.Now,
                EmailNguoiDuyet = pb.email
            };
            if (id.HasValue)
            {
                model = _dataService.CollaborateOne(id.Value);
            }

            
            ViewData["ListCongTruong"] = _dataService.GetListCongTruong(new SearchCongTruongModel());
            return PartialView("~/Views/Collaborate/Partials/CollaborateEdit.cshtml", model);
        }
        [HttpPost]
        public ActionResult CollaborateEdit(CollaborateEditModel model)
        {
            model.NguoiDi = int.Parse(Session["userid"].ToString());
            var collaborate = _dataService.CollaborateCommand(model);
            if (model.SendMail)
            {
                var sModel = new SendMailModel();
                var domain = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("domain"));

                sModel.TieuDe = model.TieuDe;
                sModel.TuNgay = model.TuNgay;
                sModel.DenNgay = model.DenNgay;
                sModel.MoTa = model.MoTa;
                string chuoicongtruong = "";
                string[] macongtruong = model.NoiCongTac.Split(',');
                for (int i=0;i< macongtruong.Length;i++)
                {
                    var congtruong = _dataService.GetListAllPhongbanCongtruongbyID(macongtruong[i].ToString());
                    if (congtruong.Count > 0 && i<(macongtruong.Length-1))
                        chuoicongtruong = chuoicongtruong + congtruong[0].tenphongban+", ";
                    if (congtruong.Count > 0 && i == (macongtruong.Length - 1))
                        chuoicongtruong = chuoicongtruong + congtruong[0].tenphongban;
                }
                sModel.phongbancongtruongten = chuoicongtruong;







                // Call Array.Sort method.


                sModel.AgreeLink = domain +
                                   "/Collaborate/ApprovalCollaborate/?" +
                                   "id=" + collaborate.Id +
                                   "&status=" + 2;
                sModel.DisagreeLink = domain +
                                      "/Collaborate/ApprovalCollaborate/?" +
                                      "id=" + collaborate.Id +
                                      "&status=" + 3;

                string content = RenderPartialToString("~/Views/Collaborate/Emails/MailCongTac.cshtml", sModel);


                var mail = new EmailComponent();
                var result = mail.Send(new MailObj()
                {
                    To = collaborate.EmailNguoiDuyet.Trim(),
                    Subject = "ĐĂNG KÝ Đi CÔNG TÁC",
                    Body = content
                });

                _dataService.ApprovalCollaborate(collaborate.Id, 1);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _dataService.DeleteCollaborate(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovalCollaborate(int id, int status)
        {
            _dataService.ApprovalCollaborate(id, status);
            ViewData["Status"] = status;
            return View();
        }
        public string RenderPartialToString(string controlName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, controlName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                    ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}