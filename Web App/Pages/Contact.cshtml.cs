using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;

namespace Web_App.Pages
{
    public class ContactModel : PageModel
    {
        public string isSend { get; set; }
        public void OnGet()
        {
        }

        // Getting data from the HTML form after the button is pressed 
        public void OnPost() 
        {
            var name  = Request.Form["name"];
            var email = Request.Form["emailaddress"];
            var message = Request.Form["message"];
            
            SendMail(name, email, message);
        }

        // Sending the form via email using the website's email server 
        // Code adapted from ZetBit, 2022
        public bool SendMail(string name, string email, string userMessage)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            message.From = new MailAddress("noreply@chester.ac.uk");
            message.To.Add("admin@chester.ac.uk");
            message.Subject = "Website contact form";
            message.IsBodyHtml = true;
            message.Body = "<p>Name: " + name + "</p>" + "<p>Email: " + email + "</p>" + "<p>Message: " + userMessage + "</p>";

            smtpClient.Port = 25;
            smtpClient.Host = "2214148.win.studentwebserver.co.uk";
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("WebApp-2214148", "P@55word");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                // smtpClient.Send(message); // Commented out as Plesk doesn't support outgoing emails 
                isSend = "success";
            }
            catch (Exception)
            {
                isSend = "fail";
            }
            return true;
        }

    }
}
