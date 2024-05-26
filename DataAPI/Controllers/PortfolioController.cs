using DataAPI.Models;
using DataAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Mail;


namespace DataAPI.Controllers
{
    public class PortfolioController : BaseApiController
    {
        public readonly PortfolioServices portfolioServices;

        public PortfolioController(PortfolioServices portfolio)
        {
            portfolioServices = portfolio;
        }

        [HttpGet(Name = "GetContacts")]
        public async Task<List<contact>> GetContacts()
        {
            return await portfolioServices.getContactsAsync();
        }

        [HttpGet("GetContact/{id:length(24)}")]
        public async Task<ActionResult<contact>> getContactById(string Id)
        {
            var contact = await portfolioServices.GetContact(Id);

            if (contact is null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPost]
        public async Task<ActionResult<contact>> savenewuser(contact obj)
        {
            if (obj is null)
            {
                return NoContent();
            }
            await portfolioServices.createUser(obj);

            return Ok(obj);
        }

        [HttpPost("/downloads")]
        public async Task<ActionResult<ResumeDownload>> downloadResume(contact contact)
        {
            ResumeDownload obj = new ResumeDownload();
            obj.contact = contact;
            if (obj.contact is null)
            {
                return NoContent();
            }

            var x = await portfolioServices.getResume(obj);
            if (x.guid_generated is not null)
            {
                // Path to the static file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resume-Soumit-Mondal.pdf");
                // Ensure the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Return the file for download
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Position = 0;

                    return File(memory, "application/pdf", "Resume-Soumit-Mondal.pdf");
                }
                else
                {
                    // Handle file not found scenario
                    return NotFound();
                }

            }

            //return Ok(x);
            return Ok();
        }

        [HttpPost("/downloadsResume")]
        public async Task<ActionResult<ResumeDownload>> downloadResumeone(contact contact)
        {
            Helper.successMessage = string.Empty;
            Helper.FailureMessage = string.Empty;

            ResumeDownload obj = new ResumeDownload();
            obj.contact = contact;
            if (obj.contact is null)
            {
                return NoContent();
            }

            var x = await portfolioServices.getResume(obj);
            if (x.guid_generated is null)
            {
                // failure case
                string email = x.contact.Email;
                if (email != null)
                {
                    Helper.FailureMessage += "Hi " + x.contact.Name + "," +"<br/>Your request for the resume download has been declined.<br/><br/>Thank you for visiting the website, Everytype of suggestions are welcome.<br/><br/>For more information you can use any of the follwing resources, Soumit will immediately respond.<br/>"+ "<br/>Phone:  " + Helper.masterPhoneWork+",<br/>Email:  "+Helper.masterWorkEmail+ ",<br/>LinkedIn:  "+Helper.masterLinkedIn+"<br/>";
                    sendMessage(email, Helper.FailureMessage);
                }
            }
            else
            { 
                // success case
                Helper.successMessage += $"Hi {x.contact.Name},<br/>Your request for the resume download has been successfully processed.<br/><br/>Thank you for visiting the website, Everytype of suggestions are welcome.<br/><br/>For more information you can use any of the follwing resources, Soumit will immediately respond.<br/>" + "<br/>Phone:  " + Helper.masterPhoneWork + ",<br/>Email:  " + Helper.masterWorkEmail + ",<br/>LinkedIn:  " + Helper.masterLinkedIn + "<br/>";
                sendMessage(x.contact.Email, Helper.successMessage);
            }
            return Ok(x);
        }

        //google-pass-key="huhrbnbqhegjscmz"
        public static void sendMessage(string toEmail,string messages)
        {
            string fromEmail = Helper.masterEmail;
            string fromPassKey = Helper.masterPasskey;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.Subject = "New Email Alert from Soumit Mondal";
            message.To.Add(new MailAddress(toEmail));
            message.Body = $"<html><body>{messages}</body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassKey),
                EnableSsl = true,
            };
            smtpClient.Send(message);
        }
    }
}
