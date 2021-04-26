using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailConfirmationAsync(string email, string subject, string callbackUrl);
        Task<bool> SendPasswordResetAsync(string email, string subject, string callbackUrl);
        Task<bool> SendEventInvitationAsync(string email, string subject, string callbackUrl, string eventName);
    }
}
