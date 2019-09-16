using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using RICONS.Core.Functions;

namespace RICONS.Web.Models
{
    public class MailObj
    {
        public string To { get; set; }
        public string SenderName { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Attachs { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string Subject { get; set; }
    }
    public class EmailComponent
    {
        public bool Send(MailObj email)
        {
            //var message = "";
            var status = 0;

            //string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
            //string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));

            MailMessage mailMessage = new MailMessage();
            //mailMessage.To.Add(email.To);
            //mailMessage.From = new MailAddress("thson.spys@gmail.com", email.SenderName, Encoding.UTF8);
            //mailMessage.Subject = email.Subject;
            //mailMessage.Body = email.Body;
            //mailMessage.IsBodyHtml = true;
            //mailMessage.SubjectEncoding = Encoding.UTF8;

            //try
            //{
            //    using (SmtpClient smtpClient = new SmtpClient())
            //    {
            //        smtpClient.EnableSsl = true;
            //        smtpClient.Host = "smtp.gmail.com";
            //        smtpClient.Port = 25;
            //        smtpClient.UseDefaultCredentials = true;
            //        smtpClient.Credentials = new NetworkCredential("thson.spys@gmail.com", "hongson123");
            //        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //        smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            //        smtpClient.Send(mailMessage);
            //    }

            //    return true;
            //}
            //catch (Exception e)
            //{
            //    return false;
            //}

            string smtp_host = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_host"));
            string smtp_user_mailgui = string.Format(Functiontring.ReturnStringFormatthongtincauhinhmail("smtp_user"));
            MailMessage message = new System.Net.Mail.MailMessage(smtp_user_mailgui, email.To);
            message.From = new MailAddress(smtp_user_mailgui.Trim(), "ĐĂNG KÝ Đi CÔNG TÁC", System.Text.Encoding.UTF8);
            message.Subject = email.Subject;
            message.Body = email.Body.ToString();
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient(smtp_host);
            client.UseDefaultCredentials = true;
            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}", ex.ToString());
                return false;
            }
        }
    }
}