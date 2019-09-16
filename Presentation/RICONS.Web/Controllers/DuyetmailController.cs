using Newtonsoft.Json.Linq;
using RICONS.Core.Functions;
using RICONS.Web.Data.Services;
using RICONS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RICONS.Web.Controllers
{
    public class DuyetmailController : Controller
    {
        //
        // GET: /Duyetmail/
        public ActionResult Index()
        {
            return View();
        }


        public string send_Mail(string mailguitoi, string dongy, string subject)
        {
            try
            {
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user, mailguitoi.Trim());
                message.From = new MailAddress(smtp_user.Trim(), subject, System.Text.Encoding.UTF8);
                message.Subject = subject;
                message.Body = dongy;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;
                SmtpClient client = new SmtpClient(smtp_host);
                client.UseDefaultCredentials = true;
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in CreateTestMessage2(): {0}", ex.ToString());
                }
                return "Successfull!";
            }
            catch (Exception ms)
            {
                return ms.Message;
            }
        }

        #region
        public string DuyetEmployee(string makpi, string dy, string mailemployee, string capdoduyet, string kpichinhsua, string mact, string kpichinhsuact) 
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");

            string strmact = mact.Replace("sQRu17zoCm5687AVoiGXX", "");

            KpiEmployeeServices service = new KpiEmployeeServices();
            bool kq = true;
            if (capdoduyet.Trim() == "1")
                kq = service.UpdateRow_Employee_duyet_khongduyet(strEncryptCode, dy, kpichinhsua, strmact, kpichinhsuact);
            if (kq && dy == "1")
            {
                send_Mail(mailemployee, "KPI được duyệt. Yêu cầu xem chi tiết trên phần mềm","Đồng ý duyệt KPI");
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailemployee, "KPI không được duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.","Không đồng ý duyệt KPI");
                Duyetphep = "KPI không được duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }
       
        [HttpPost]
        public JsonResult DuyetEmployee_html(string DataJson, string email, string kpichinhsua)
        {
            JObject json = JObject.Parse(DataJson);
            string mailemployee = json["email"].ToString();
            string strEncryptCode = json["makpinhanvien"].ToString();
            string dy = json["dongy"].ToString();
            KpiEmployeeServices service = new KpiEmployeeServices();
            bool kq = service.UpdateRow_Employee_duyet_khongduyet_html(strEncryptCode, dy);
            if (kq && dy == "1")
            {
                send_Mail(mailemployee, "KPI được duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI");
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailemployee, "KPI không được duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI");
            }
            if (kq)
            {
                return Json(new { success = true, makpinhanvien = int.Parse(strEncryptCode) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpinhanvien = strEncryptCode }, JsonRequestBehavior.AllowGet);
            }
        }

        public string DuyetEmployee_Yeucauchinhsua(string makpict, string dy, string mailemployee, string capdoduyet, string makpi)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpict.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
            string makpinhanvien = makpi.Replace("QA741xwz78i035760byasdfux", "");
            KpiEmployeeServices service = new KpiEmployeeServices();
            bool kq = true;
            if (capdoduyet.Trim() == "1")
                kq = service.UpdateRow_Employee_duyet_Yeucauchinhsua(strEncryptCode, dy, makpinhanvien);
            if (kq && dy == "1")
            {
                send_Mail(mailemployee, "Yêu cầu chỉnh sữa KPI được duyệt. Xem chi tiết trên phần mềm những mục yêu cầu để chỉnh sữa", "Yêu cầu chỉnh sữa KPI");
                Duyetphep = "Yêu cầu chỉnh sữa KPI được duyệt";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailemployee, "Yêu cầu chỉnh sữa KPI không được duyệt. Bạn kiểm tra lại nội dung yêu cầu.", "Yêu cầu chỉnh sữa KPI");
                Duyetphep = "Yêu cầu chỉnh sữa KPI không được duyệt.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        public string writedata(string DataJson)
        {
            JObject json = JObject.Parse(DataJson);
            string mailemployee = json["email"].ToString();
            string strEncryptCode = json["makpinhanvien"].ToString();
            string dy = json["dongy"].ToString();
            KpiEmployeeServices service = new KpiEmployeeServices();
            string ma = "1";
            return ma;
        }




        #endregion

        #region  KPI CẤP CÔNG TY
        public string send_MailLan2(string makpicongty)
        {
            try
            {
                //string DataJson = "";
                makpicongty = makpicongty.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
                KpiLevelCompanyServices service = new KpiLevelCompanyServices();
                List<KpiLevelCompanyModels> lstResult = service.SelectRows_KpiLevelCompany_hieuchinh(makpicongty);
                List<KpiLevelCompanyDetailModels> lstResult_chitiet = service.SelectRows_KpiLevelCompanyDetail_hieuchinh(makpicongty);

                //JObject json = JObject.Parse(DataJson);

                DanhmucServices servicess = new DanhmucServices();
                PhongBanModels parampb = new PhongBanModels();
                List<PhongBanModels> lstResult_phongban = servicess.SelectRows(parampb);

                string mailnguoigui = lstResult[0].nguoilapkpi_email.Trim();
                string mailnguoigui1 = lstResult[0].photongxemxetkpi_email.Trim();
                string mailtonggiamdoc = lstResult[0].tonggiamdocxemxetkpi_email.Trim(); 
                string namkpi = lstResult[0].nam.Trim();
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetKPICompanyL2/?makpi=";
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 1px; padding: 0; width: 100%; font-size: 1em;color:black;'>");
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

                sb.Append("<tr><td rowspan='4' colspan='2' style='border: 1px solid #000;font-size: 16pt;color:#f3332e;text-align: center;'>NĂM KPI <br /> <span style='font-size: 29pt;color:#8e2bf1;text-align: center;'>" + namkpi + "</span></td>");

                sb.Append("<td colspan='12' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP CÔNG TY - KPI NĂM " + namkpi + "</td>");
                sb.Append("</tr>");

                sb.Append("<tr><td colspan='12' style='text-align: left;border: 1px solid #000;'>Công Ty Cổ Phần Đầu Tư Xây Dựng Ricons:</td></tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='8' style='text-align: left;border: 1px solid #000;'>Bộ phận soạn thảo: " + lstResult[0].bophansoanthao.Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Biểu mẫu: QP04FM12</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày ban hành: " + lstResult[0].ngaybanhanh.Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày cập nhật: " + lstResult[0].ngaycapnhat.Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Lần cập nhật: " + lstResult[0].lancapnhat.Trim() + " </td>");
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

                for (int i = 0; i < lstResult_chitiet.Count(); i++)
                {
                    if (lstResult_chitiet[i].stt.Trim() == "A")
                        sb.Append("<tr style='background-color:#f9fd0d;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "B")
                        sb.Append("<tr style='background-color:#99f1b8;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "C")
                        sb.Append("<tr style='background-color:#5edcf1 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "D")
                        sb.Append("<tr style='background-color:#90abe8 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0;text-align:center'>");
                    else sb.Append("<tr>");

                    string nguoncm = lstResult_chitiet[i].nguonchungminh.Trim();
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

                    string nguoncm1 = lstResult_chitiet[i].nguonchungminh1.Trim();
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

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].stt.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].muctieu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].trongso.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].tieuchidanhgia.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].cachtinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ngayghinhanketqua.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + nguonchungminh + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + nguonchungminh1 + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].donvitinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].kehoach.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].thuchien.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].tilehoanthanh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ketqua.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ghichu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }
                string strEncryptCode = linkname.Trim() + makpicongty + "0070pXQSeNsQRuzoCmUYfuX" + "&mailnguoigui=" + mailnguoigui + "&capdoduyet=2" + "&mailnguoigui1=" + mailnguoigui1;// +"&mact=" + mact + "sQRu17zoCm5687AVoiGXX" + "&kpichinhsuact=" + kpichinhsuact;
                sb.Append("</table>");

                string chuoidy = "width:200px;height: 40px;background-color:#0090d9;border-radius:15px;text-align:center;color:#fff; margin: 0 8px 0 0;text-decoration: none; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);";
                string chuoikdy = "width:200px;height: 40px;background-color:red;border-radius:15px;text-align:center;color:#fff; margin: 0 8px 0 0;text-decoration: none; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);";

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:0px; font-size:22px; height :40px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1' style='" + chuoidy + "'>&nbsp;&nbsp;Đồng ý&nbsp;&nbsp;</a>&nbsp;&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2' style='" + chuoikdy + "'>&nbsp;&nbsp;Không đồng ý&nbsp;&nbsp;</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, mailtonggiamdoc);
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
                
                return "-1";
            }
        }

        public string DuyetKPICompanyL1(string makpi, string dy, string mailnguoigui, string capdoduyet)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");

            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.UpdateRow_KPICompany_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet);
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI năm được Phó Tổng Giám Đốc duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm");
                send_MailLan2(makpi);
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI năm không được Phó Tổng Giám Đốc duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm");
                Duyetphep = "KPI năm không được duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        public string DuyetKPICompanyL2(string makpi, string dy, string mailnguoigui, string capdoduyet, string mailnguoigui1)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");

            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.UpdateRow_KPICompany_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet);
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI năm được Tổng Giám Đốc duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm");
                send_Mail(mailnguoigui1, "KPI năm được Tổng Giám Đốc duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm");
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI năm không được Tổng Giám Đốc duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm");
                send_Mail(mailnguoigui1, "KPI năm không được Tổng Giám Đốc duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm");
                Duyetphep = "KPI năm không được duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        #endregion


        #region KPI CẤP PHÒNG BAN

        // GỬI MAIL CHO CẤP TRÊN TRỰC TIẾP
        public string send_MailKPIDepartment1(string makpi)
        {
            try
            {
                //string DataJson = "";
                makpi = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
                KpiLevelCompanyServices service = new KpiLevelCompanyServices();
                List<KpiLevelDepartmentModels> lstResult = service.SelectRows_KpiLevelDepartment_hieuchinh(makpi);
                List<KpiLevelDepartmentDetailModels> lstResult_chitiet = service.SelectRows_KpiLevelDepartmentDetail_hieuchinh(makpi);

                //JObject json = JObject.Parse(DataJson);

                string mailnguoigui = lstResult[0].nguoilapkpi_email.Trim();
                string mailbankiemsoatkpi = lstResult[0].bankiemsoat_email.Trim();
                string mailcaptrentructiep = lstResult[0].photongxemxetkpi_email.Trim();
                string mailtonggiamdoc = lstResult[0].tonggiamdocxemxetkpi_email.Trim();

                string namkpi = lstResult[0].nam.Trim();
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetKPIDepartmentL2/?makpi=";
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 1px; padding: 0; width: 100%; font-size: 1em;color:black;'>");
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

                sb.Append("<td colspan='11' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP PHÒNG/BAN - KPI NĂM " + namkpi + "</td>");
                sb.Append("</tr>");

                sb.Append("<tr><td colspan='11' style='text-align: left;border: 1px solid #000;'>Công Ty Cổ Phần Đầu Tư Xây Dựng Ricons:</td></tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='8' style='text-align: left;border: 1px solid #000;'>Phòng Ban: " + lstResult[0].tenphongban.Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Biểu mẫu: QP04FM13</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày ban hành: " + lstResult[0].ngaybanhanh.Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày cập nhật: " + lstResult[0].ngaycapnhat.Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Lần cập nhật: " + lstResult[0].lancapnhat.Trim() + " </td>");
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


                for (int i = 0; i < lstResult_chitiet.Count(); i++)
                {
                    if (lstResult_chitiet[i].stt.Trim() == "A")
                        sb.Append("<tr style='background-color:#f9fd0d;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "B")
                        sb.Append("<tr style='background-color:#99f1b8;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "C")
                        sb.Append("<tr style='background-color:#5edcf1 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "D")
                        sb.Append("<tr style='background-color:#90abe8 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0;text-align:center'>");
                    else sb.Append("<tr>");

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].stt.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].muctieu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].trongso.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].tieuchidanhgia.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].cachtinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ngayghinhanketqua.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].nguonchungminh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].donvitinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].kehoach.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].thuchien.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].tilehoanthanh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ketqua.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ghichu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }
                string strEncryptCode = linkname.Trim() + makpi + "0070pXQSeNsQRuzoCmUYfuX" + "&mailnguoigui=" + mailnguoigui + "&capdoduyet=2" + "&mailbankiemsoatkpi=" + mailbankiemsoatkpi;// +"&mact=" + mact + "sQRu17zoCm5687AVoiGXX" + "&kpichinhsuact=" + kpichinhsuact;
                sb.Append("</table>");

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý Duyệt KPI</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, mailcaptrentructiep);
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Gửi KPI cấp Phòng năm", System.Text.Encoding.UTF8);
                message.Subject = "Gửi KPI cấp Phòng năm";
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

                return "-1";
            }
        }

        //GUI MAIL CHO TỔNG GIÁM ĐỐC DUYỆT
        public string send_MailKPIDepartment2(string makpi)
        {
            try
            {
                //string DataJson = "";
                makpi = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
                KpiLevelCompanyServices service = new KpiLevelCompanyServices();
                List<KpiLevelDepartmentModels> lstResult = service.SelectRows_KpiLevelDepartment_hieuchinh(makpi);
                List<KpiLevelDepartmentDetailModels> lstResult_chitiet = service.SelectRows_KpiLevelDepartmentDetail_hieuchinh(makpi);

                //JObject json = JObject.Parse(DataJson);

                string mailnguoigui = lstResult[0].nguoilapkpi_email.Trim();
                string mailbankiemsoatkpi = lstResult[0].bankiemsoat_email.Trim();
                string mailcaptrentructiep = lstResult[0].photongxemxetkpi_email.Trim();
                string mailtonggiamdoc = lstResult[0].tonggiamdocxemxetkpi_email.Trim();

                string namkpi = lstResult[0].nam.Trim();
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetKPIDepartmentL3/?makpi=";
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 1px; padding: 0; width: 100%; font-size: 1em;color:black;'>");
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

                sb.Append("<td colspan='11' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP PHÒNG/BAN - KPI NĂM " + namkpi + "</td>");
                sb.Append("</tr>");

                sb.Append("<tr><td colspan='11' style='text-align: left;border: 1px solid #000;'>Công Ty Cổ Phần Đầu Tư Xây Dựng Ricons:</td></tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='8' style='text-align: left;border: 1px solid #000;'>Phòng Ban: " + lstResult[0].tenphongban.Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Biểu mẫu: QP04FM13</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày ban hành: " + lstResult[0].ngaybanhanh.Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày cập nhật: " + lstResult[0].ngaycapnhat.Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Lần cập nhật: " + lstResult[0].lancapnhat.Trim() + " </td>");
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


                for (int i = 0; i < lstResult_chitiet.Count(); i++)
                {
                    if (lstResult_chitiet[i].stt.Trim() == "A")
                        sb.Append("<tr style='background-color:#f9fd0d;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "B")
                        sb.Append("<tr style='background-color:#99f1b8;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "C")
                        sb.Append("<tr style='background-color:#5edcf1 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "D")
                        sb.Append("<tr style='background-color:#90abe8 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0;text-align:center'>");
                    else sb.Append("<tr>");

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].stt.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].muctieu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].trongso.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].tieuchidanhgia.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].cachtinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ngayghinhanketqua.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].nguonchungminh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].donvitinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].kehoach.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].thuchien.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].tilehoanthanh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ketqua.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ghichu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }
                string strEncryptCode = linkname.Trim() + makpi + "0070pXQSeNsQRuzoCmUYfuX" + "&mailnguoigui=" + mailnguoigui + "&capdoduyet=3" + "&mailbankiemsoatkpi=" + mailbankiemsoatkpi;// +"&mact=" + mact + "sQRu17zoCm5687AVoiGXX" + "&kpichinhsuact=" + kpichinhsuact;
                sb.Append("</table>");

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý Duyệt KPI</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, mailtonggiamdoc);
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Gửi KPI cấp Phòng năm", System.Text.Encoding.UTF8);
                message.Subject = "Gửi KPI cấp Phòng năm";
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

                return "-1";
            }
        }


        // DUYỆT MAIL CỦA BAN KIEM SOAT KPI
        public string DuyetKPIDepartmentL1(string makpi, string dy, string mailnguoigui, string capdoduyet)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.Save_KpiLevelDepartment_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet);
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Phòng được Ban kiểm soát KPI duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm cấp Phòng");
                send_MailKPIDepartment1(makpi);
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Phòng không được duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm cấp Phòng");
                Duyetphep = "KPI năm cấp Phòng không được duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        //DUYỆT MAIL CỦA CẤP TRÊN TRỰC TIẾP VÀ GỬI MAIL CHO TỔNG GIÁM ĐỐC
        public string DuyetKPIDepartmentL2(string makpi, string dy, string mailnguoigui, string mailbankiemsoatkpi, string capdoduyet)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");

            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.Save_KpiLevelDepartment_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet);
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Phòng được cấp trên trực tiếp duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm cấp Phòng");
                send_Mail(mailbankiemsoatkpi, "KPI năm cấp Phòng được cấp trên trực tiếp duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm cấp Phòng");
                send_MailKPIDepartment2(makpi);
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Phòng không được cấp trên trực tiếp duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm cấp Phòng");
                send_Mail(mailbankiemsoatkpi, "KPI năm cấp Phòng không được cấp trên trực tiếp được duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm cấp Phòng");
                Duyetphep = "KPI năm cấp Phòng không được duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        //DUYỆT MAIL CỦA TỔNG GIÁM ĐỐC
        public string DuyetKPIDepartmentL3(string makpi, string dy, string mailnguoigui, string mailbankiemsoatkpi, string capdoduyet)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.Save_KpiLevelDepartment_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet);
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Phòng được được tổng Giám đốc duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm cấp Phòng");
                send_Mail(mailbankiemsoatkpi, "KPI năm cấp Phòng được tổng Giám đốc duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm cấp Phòng");
                //send_MailKPIDepartment2(makpi);
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Phòng không được tổng Giám đốc duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm cấp Phòng");
                send_Mail(mailbankiemsoatkpi, "KPI năm cấp Phòng không được tổng Giám đốc duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm cấp Phòng");
                Duyetphep = "KPI năm cấp Phòng không được tổng Giám đốc duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        [HttpPost]
        public JsonResult DuyetKPIDepartment_html(string DataJson)
        {
            JObject json = JObject.Parse(DataJson);
            string mailnguoigui = json["nguoilapkpi_email"].ToString();
            string strEncryptCode = json["makpiphongban"].ToString();
            string makpi = strEncryptCode + "0070pXQSeNsQRuzoCmUYfuX";
            string dy = json["dongy"].ToString();
            string capdoduyet = json["capdoduyet"].ToString();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.Save_KpiLevelDepartment_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet);
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Phòng được Ban kiểm soát KPI duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm cấp Phòng");
                send_MailKPIDepartment1(makpi);
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Phòng không được Ban kiểm soát KPI duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm cấp Phòng");
            }
            if (kq)
            {
                return Json(new { success = true, makpinhanvien = int.Parse(strEncryptCode) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpinhanvien = strEncryptCode }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion




        #region DUYỆT KPI CẤP CÁ NHÂN TỪ TRUONG PHONG TRO LEN - PHÓ TONG GIÁM ĐỐC

        // GỬI MAIL CHO CẤP TRÊN TRỰC TIẾP
        public string send_MailKPIDu_Director1(string makpi)
        {
            try
            {
                //string DataJson = "";
                makpi = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
                KpiLevelCompanyServices service = new KpiLevelCompanyServices();
                List<KpiLevelDuDirectorTpModels> lstResult = service.SelectRows_KpiLevelDuDirectorTp_hieuchinh(makpi);
                List<KpiLevelDuDirectorTpDetailModels> lstResult_chitiet = service.SelectRows_KpiLevelDuDirectorTpDetail_hieuchinh(makpi);

                string mailnguoigui = lstResult[0].nguoilapkpi_email.Trim();
                string mailbankiemsoatkpi = lstResult[0].bankiemsoat_email.Trim();
                string mailcaptrentructiep = lstResult[0].photongxemxetkpi_email.Trim();
                if (lstResult[0].chucdanh.Trim() == "6" || lstResult[0].chucdanh.Trim() == "7" || lstResult[0].chucdanh.Trim() == "8" || lstResult[0].maphongban.Trim() == "67")
                {
                    mailcaptrentructiep = lstResult[0].tonggiamdocxemxetkpi_email.Trim();
                }
                string mailtonggiamdoc = lstResult[0].tonggiamdocxemxetkpi_email.Trim();
                string tenchucdanh = lstResult[0].tenchucdanh.Trim().ToUpper();
                string chucdanh = lstResult[0].chucdanh.Trim().ToUpper();

                string namkpi = lstResult[0].ngaydangky.Trim().Split('/')[2];
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetKPIDu_DirectorL2/?makpi=";
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 1px; padding: 0; width: 100%; font-size: 1em;color:black;'>");
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

                sb.Append("<tr><td rowspan='3' colspan='2' style='border: 1px solid #000;font-size: 16pt;color:#f3332e;text-align: center;'></td>");
                //sb.Append("<tr><td rowspan='3' colspan='2' style='border: 1px solid #000;font-size: 16pt;color:#f3332e;text-align: center;'>NĂM KPI <br /> <span style='font-size: 29pt;color:#8e2bf1;text-align: center;'>" + namkpi + "</span></td>");

                sb.Append("<td colspan='11' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP CÁ NHÂN " + tenchucdanh + " - KPI NĂM " + namkpi + "</td>");
                sb.Append("</tr>");

                //sb.Append("<tr><td colspan='11' style='text-align: left;border: 1px solid #000;'>Công Ty Cổ Phần Đầu Tư Xây Dựng Ricons:</td></tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='8' style='text-align: left;border: 1px solid #000;'>Phòng Ban: " + lstResult[0].tenphongban.Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Biểu mẫu: QP04FM15</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày ban hành: " + lstResult[0].ngaybanhanh.Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày cập nhật: " + lstResult[0].ngaycapnhat.Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Lần cập nhật: " + lstResult[0].lancapnhat.Trim() + " </td>");
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


                for (int i = 0; i < lstResult_chitiet.Count(); i++)
                {
                    if (lstResult_chitiet[i].stt.Trim() == "A")
                        sb.Append("<tr style='background-color:#f9fd0d;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "B")
                        sb.Append("<tr style='background-color:#99f1b8;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "C")
                        sb.Append("<tr style='background-color:#5edcf1 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "D")
                        sb.Append("<tr style='background-color:#90abe8 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0;text-align:center'>");
                    else sb.Append("<tr>");

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].stt.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].muctieu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].trongso.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].tieuchidanhgia.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].cachtinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ngayghinhanketqua.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].nguonchungminh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].donvitinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].kehoach.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].thuchien.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].tilehoanthanh.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ketqua.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ghichu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }
                string strEncryptCode = linkname.Trim() + makpi + "0070pXQSeNsQRuzoCmUYfuX" + "&mailnguoigui=" + mailnguoigui + "&capdoduyet=2" + "&mailbankiemsoatkpi=" + mailbankiemsoatkpi + "&chucdanh=" + chucdanh + "&phongban=" + lstResult[0].maphongban.Trim();
                sb.Append("</table>");

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý Duyệt KPI</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, mailcaptrentructiep);
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Gửi KPI cấp Cá nhân", System.Text.Encoding.UTF8);
                message.Subject = "Gửi KPI cấp Cá nhân";
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

                return "-1";
            }
        }

        // GỬI MAIL CHO TỔNG GIÁM ĐỐC DUYỆT
        public string send_MailKPIDu_Director2(string makpi)
        {
            try
            {
                //string DataJson = "";
                makpi = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
                KpiLevelCompanyServices service = new KpiLevelCompanyServices();
                List<KpiLevelDuDirectorTpModels> lstResult = service.SelectRows_KpiLevelDuDirectorTp_hieuchinh(makpi);
                List<KpiLevelDuDirectorTpDetailModels> lstResult_chitiet = service.SelectRows_KpiLevelDuDirectorTpDetail_hieuchinh(makpi);

                string mailnguoigui = lstResult[0].nguoilapkpi_email.Trim();
                string mailbankiemsoatkpi = lstResult[0].bankiemsoat_email.Trim();
                string mailcaptrentructiep = lstResult[0].photongxemxetkpi_email.Trim();
                string mailtonggiamdoc = lstResult[0].tonggiamdocxemxetkpi_email.Trim();

                string tenchucdanh = lstResult[0].tenchucdanh.Trim().ToUpper();
                string chucdanh = lstResult[0].chucdanh.Trim().ToUpper();

                string namkpi = lstResult[0].ngaydangky.Trim().Split('/')[2];
                string linkname = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("linkname")) + "DuyetKPIDu_DirectorL3/?makpi=";
                string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
                string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

                #region

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head>");
                sb.Append("<link rel='stylesheet' type='text/css' href='theme.css' />");
                sb.Append("</head>");
                sb.Append("<body style='margin-top: 1px; padding: 0; width: 100%; font-size: 1em;color:black;'>");
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

                sb.Append("<tr><td rowspan='3' colspan='2' style='border: 1px solid #000;font-size: 16pt;color:#f3332e;text-align: center;'></td>");
                //sb.Append("<tr><td rowspan='3' colspan='2' style='border: 1px solid #000;font-size: 16pt;color:#f3332e;text-align: center;'>NĂM KPI <br /> <span style='font-size: 29pt;color:#8e2bf1;text-align: center;'>" + namkpi + "</span></td>");

                sb.Append("<td colspan='11' style='border: 1px solid #000;font-size: 18pt;background-color: azure;color: #8e2bf1;text-align:center'>KPI CẤP CÁ NHÂN " + tenchucdanh + " - KPI NĂM " + namkpi + "</td>");
                sb.Append("</tr>");

                //sb.Append("<tr><td colspan='11' style='text-align: left;border: 1px solid #000;'>Công Ty Cổ Phần Đầu Tư Xây Dựng Ricons:</td></tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='8' style='text-align: left;border: 1px solid #000;'>Phòng Ban: " + lstResult[0].tenphongban.Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Biểu mẫu: QP04FM13</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày ban hành: " + lstResult[0].ngaybanhanh.Trim() + " </td>");
                sb.Append("<td colspan='4' style='text-align: left;border: 1px solid #000;'>Ngày cập nhật: " + lstResult[0].ngaycapnhat.Trim() + " </td>");
                sb.Append("<td colspan='3' style='text-align: left;border: 1px solid #000;'>Lần cập nhật: " + lstResult[0].lancapnhat.Trim() + " </td>");
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


                for (int i = 0; i < lstResult_chitiet.Count(); i++)
                {
                    if (lstResult_chitiet[i].stt.Trim() == "A")
                        sb.Append("<tr style='background-color:#f9fd0d;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "B")
                        sb.Append("<tr style='background-color:#99f1b8;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "C")
                        sb.Append("<tr style='background-color:#5edcf1 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "D")
                        sb.Append("<tr style='background-color:#90abe8 ;text-align:center'>");
                    else if (lstResult_chitiet[i].stt.Trim() == "")
                        sb.Append("<tr style='background-color:#e2e0e0;text-align:center'>");
                    else sb.Append("<tr>");

                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].stt.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].muctieu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].trongso.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].tieuchidanhgia.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:left'>" + lstResult_chitiet[i].cachtinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ngayghinhanketqua.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].nguonchungminh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].donvitinh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].kehoach.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].thuchien.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].tilehoanthanh.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ketqua.Trim() + "</td>");
                    sb.Append("<td style='font-size:12px; border: 1px solid #000;text-align:center'>" + lstResult_chitiet[i].ghichu.Trim().Replace("daunhaydon", "'").Replace("daukytuva", "&") + "</td>");
                    sb.Append("</tr>");
                }
                string strEncryptCode = linkname.Trim() + makpi + "0070pXQSeNsQRuzoCmUYfuX" + "&mailnguoigui=" + mailnguoigui + "&capdoduyet=3" + "&mailbankiemsoatkpi=" + mailbankiemsoatkpi + "&chucdanh=" + chucdanh;// +"sQRu17zoCm5687AVoiGXX" + "&kpichinhsuact=" + kpichinhsuact;
                sb.Append("</table>");

                sb.Append("<table style='width:850px;'>");
                sb.Append("<tr><td style='float:left; padding-left:10px; font-size:22px; height :30px; background-color:0090d9; line-height:31px; padding-top:10px;'><a href='" + strEncryptCode + "&dy=1'> Đồng ý Duyệt KPI</a>&nbsp;&nbsp;<a href='" + strEncryptCode + "&dy=2'>Không đồng ý</a></td></tr>");
                sb.Append("</table>");

                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, mailtonggiamdoc);
                message.From = new MailAddress(smtp_user_mailgui.Trim(), "Gửi KPI cấp Phòng năm", System.Text.Encoding.UTF8);
                message.Subject = "Gửi KPI cấp Cá nhân";
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

                return "-1";
            }
        }

        // DUYỆT MAIL CỦA BAN KIEM SOAT KPI
        public string DuyetKPIDu_DirectorL1(string makpi, string dy, string mailnguoigui, string capdoduyet)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.Save_KpiLevelDuDirectorTp_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet,"","");
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Cá nhân được Ban kiểm soát KPI duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI năm cấp Cá nhân");
                send_MailKPIDu_Director1(makpi);
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Cá nhân không được duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm cấp Cá nhân");
                Duyetphep = "KPI năm cấp Cá nhân không được duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        //DUYỆT MAIL CỦA CẤP TRÊN TRỰC TIẾP VÀ GỬI MAIL CHO TỔNG GIÁM ĐỐC(NẾU PHO TONG GIÁM ĐỐC, GIÁM ĐỐC KHỐI, PHÒNG KE TOÁN LEN THANG TONG GIÁM ĐỐC)
        public string DuyetKPIDu_DirectorL2(string makpi, string dy, string mailnguoigui, string mailbankiemsoatkpi, string capdoduyet, string chucdanh,string phongban)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");

            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.Save_KpiLevelDuDirectorTp_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet,chucdanh,phongban);
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI cấp Cá nhân được cấp trên trực tiếp duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI cấp Cá nhân");
                send_Mail(mailbankiemsoatkpi, "KPI cấp Cá nhân được cấp trên trực tiếp duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI cấp Cá nhân");
                if (chucdanh.Trim() != "6" && chucdanh.Trim() != "7" && chucdanh.Trim() != "8")
                {
                    if (phongban.Trim() != "67") send_MailKPIDu_Director2(makpi);
                }
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI cấp Cá nhân không được cấp trên trực tiếp duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI cấp Cá nhân");
                send_Mail(mailbankiemsoatkpi, "KPI cấp Cá nhân không được cấp trên trực tiếp được duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI cấp Cá nhân");
                Duyetphep = "KPI cấp Cá nhân không được duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        //DUYỆT MAIL CỦA TỔNG GIÁM ĐỐC
        public string DuyetKPIDu_DirectorL3(string makpi, string dy, string mailnguoigui, string mailbankiemsoatkpi, string capdoduyet)
        {
            string Duyetphep = "Phản hồi thông tin thành công";
            string strEncryptCode = makpi.Replace("0070pXQSeNsQRuzoCmUYfuX", "");
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.Save_KpiLevelDuDirectorTp_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet, "", "");
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI cấp Cá nhân được được tổng Giám đốc duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI cấp Cá nhân");
                send_Mail(mailbankiemsoatkpi, "KPI cấp Cá nhân được tổng Giám đốc duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI cấp Cá nhân");
                //send_MailKPIDepartment2(makpi);
                Duyetphep = "Đồng ý duyệt KPI";
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI cấp Cá nhân không được tổng Giám đốc duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI năm cấp Phòng");
                send_Mail(mailbankiemsoatkpi, "KPI cấp Cá nhân không được tổng Giám đốc duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI cấp Cá nhân");
                Duyetphep = "KPI cấp Cá nhân không được tổng Giám đốc duyệt. Yêu cầu chỉnh sữa lại.";
            }
            else Duyetphep = "Lỗi xử lý. Liên hệ IT để hổ trợ.";
            return Duyetphep;
        }

        public JsonResult DuyetKPIDu_Director_tp_html(string DataJson)
        {
            JObject json = JObject.Parse(DataJson);
            string mailnguoigui = json["nguoilapkpi_email"].ToString();
            string strEncryptCode = json["makpicanhancap"].ToString();



            string makpi = strEncryptCode + "0070pXQSeNsQRuzoCmUYfuX";
            string dy = json["dongy"].ToString();
            string capdoduyet = json["capdoduyet"].ToString();
            KpiLevelCompanyServices service = new KpiLevelCompanyServices();
            bool kq = true;
            kq = service.Save_KpiLevelDuDirectorTp_Duyet_Khongduyet(strEncryptCode, dy, capdoduyet,"","");
            if (kq && dy == "1")
            {
                send_Mail(mailnguoigui, "KPI năm cấp Cá nhân được Ban kiểm soát KPI duyệt. Yêu cầu xem chi tiết trên phần mềm", "Đồng ý duyệt KPI cấp Cá nhân");
                send_MailKPIDu_Director1(makpi);
            }
            else if (kq && dy == "2")
            {
                send_Mail(mailnguoigui, "KPI Cấp Cá nhân không được duyệt. Yêu cầu chỉnh sữa lại. Yêu cầu xem chi tiết trên phần mềm.", "Không đồng ý duyệt KPI cấp Cá nhân");
            }
            if (kq)
            {
                return Json(new { success = true, makpicanhancap = int.Parse(strEncryptCode) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, makpicanhancap = strEncryptCode }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion




































    }
}