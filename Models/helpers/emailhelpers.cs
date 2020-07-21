using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DataSystem.Models.emailhelpers
{
    public class emailhelpers
    {
        public static bool SendEmail(string Email, string Message, string Period, string CCmails, string DateFrom, string DateTo, string implementerName,string supplyname)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential("scmunicef@gmail.com", "$cm@123456");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("scmunicef@gmail.com"),
                    Subject = implementerName + "-Comments period/quarter :" + Period,
                    Body = "<h3> Request type: " + Period + "</h3>" + "<h3> Date From: " + DateFrom + "</h3>" + "<h3> Date To :" + DateTo + "</h3>" +"<h3> Supply item: "+supplyname+"</h3>"+ Message
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(Email));
                foreach (var address in CCmails.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(new MailAddress(address));
                }
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials,
                    Timeout = int.MaxValue
                };

                client.Send(mail);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
    }
}
