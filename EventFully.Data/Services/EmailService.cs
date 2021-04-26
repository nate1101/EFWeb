using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using EventFully.Services.Interfaces;
using EventFully.Repositories.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;

namespace EventFully.Services
{
    public class ConfirmEmail
    {
        public string Link { get; set; }
    }

    public class EmailService : IEmailService
    {
        //public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        //{
        //    Options = optionsAccessor.Value;
        //}

        //public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public async Task<bool> SendEmailConfirmationAsync(string email, string subject, string callbackUrl)
        {
            var client = new SendGridClient("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");
            var msg = new SendGridMessage();
            msg.SetFrom("support@eventbx.com");
            msg.SetTemplateId("d-356ddf7ecf54406d81a42ed7ab4835f4");
            msg.AddTo(email);

            msg.Personalizations[0].TemplateData = new { link = callbackUrl };
            var response = await client.SendEmailAsync(msg);

            return true;
        }

        public async Task<bool> SendPasswordResetAsync(string email, string subject, string callbackUrl)
        {
            var client = new SendGridClient("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");
            var msg = new SendGridMessage();
            msg.SetFrom("support@eventbx.com");
            msg.SetTemplateId("d-d292494feec04dc0a105ac273c49af4c");
            msg.AddTo(email);

            msg.Personalizations[0].TemplateData = new { link = callbackUrl };
            var response = await client.SendEmailAsync(msg);

            return true;
        }

        public async Task<bool> SendEventInvitationAsync(string email, string subject, string callbackUrl, string eventName)
        {
            var client = new SendGridClient("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");
            var msg = new SendGridMessage();
            msg.SetFrom("support@eventbx.com");
            msg.SetTemplateId("d-67a9cb2cf0224a4392e512d235eb5775");
            msg.AddTo(email);

            msg.Personalizations[0].TemplateData = new { eventName = eventName, link = callbackUrl };
            var response = await client.SendEmailAsync(msg);

            return true;
        }
    }


}
