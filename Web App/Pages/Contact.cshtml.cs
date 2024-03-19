using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;

namespace Web_App.Pages
{
    public class ContactModel : PageModel
    {
        public bool messageIsSent;

        public void OnGet()
        {
        }

        // Getting the information from the contact form 
        public IActionResult OnPost() 
        {
            var name  = Request.Form["name"];
            var email = Request.Form["emailaddress"];
            var message = Request.Form["message"];
            
            messageIsSent = SendMail(name, email, message);

            if (messageIsSent)
            {
                return RedirectToPage("/ContactSuccess");
            }
            else
            {
                return RedirectToPage("/ContactFail");
            }    
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

            // After submitting the form information the user is redirected to the success or fail page 
            try
            {
                // smtpClient.Send(message); // Commented out as Plesk doesn't support outgoing emails 
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
