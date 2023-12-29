using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace mail.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SendMail(string username, string receiver)
        {
            
            string title = "My Portfolio";
            string url = "https://zuhair.is-a.dev/";
            string description = "Here is my Portfolio";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string body = PopulateBody(username, title, url, description);
            SendHtmlFormattedEmail(receiver, "My Portfolio published!", body);
            return Content("Email sent successfully.");
        }

        private string PopulateBody(string userName, string title, string url, string description)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Template/EmailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", userName);
            body = body.Replace("{Title}", title);
            body = body.Replace("{Url}", url);
            body = body.Replace("{Description}", description);
            return body;
        }

        private void SendHtmlFormattedEmail(string recepientEmail, string subject, string body)
        {
            SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            using (MailMessage mm = new MailMessage(smtpSection.From, recepientEmail))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = smtpSection.Network.Host;
                    smtp.EnableSsl = smtpSection.Network.EnableSsl;
                    smtp.UseDefaultCredentials = true;
                    NetworkCredential networkCred = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                    smtp.Credentials = networkCred;    
                    smtp.Port = smtpSection.Network.Port;
                    smtp.Send(mm);
                }
            }
        }


    }
}