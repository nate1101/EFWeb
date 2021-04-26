using EventFully.Models;
using EventFully.Services.Interfaces;
using Floxdc.ExponentServerSdk;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.API.V1.Controllers
{
    /// <summary>
    /// Represents a RESTful service of events.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class AppController : Controller
    {
        private readonly INotificationService _notificationService;
        /// <summary>
        /// App Controller
        /// </summary>
        /// <param name="notificationService"></param>
        public AppController(
            INotificationService notificationService
            )
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gets a auth token.
        /// </summary>
        /// <returns>The requested token.</returns>
        /// <response code="200">The token was successfully retrieved.</response>
        /// <response code="404">The token does not exist.</response>
        [HttpPost("{token}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreatePushSubscription(string token)
        {
            try
            {
                var push = new PushSubscription()
                {
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Token = token,
                    ActiveFlag = true
                };
                await _notificationService.SavePushSubscription(push);

                return Json(new { success=true});
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        /// <summary>
        /// Send Push Reminders
        /// </summary>
        /// <returns>The sent count.</returns>
        /// <response code="200">The reminders were successfully sent.</response>
        /// <response code="404">The reminders were not successfully sent.</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SendPushReminders()
        {
            try
            {
                var reminders = await _notificationService.GetActivePushReminders();
                if (reminders.Count > 0)
                {
                    var client = new PushClient();

                    var notificationList = new List<PushMessage>();

                    foreach (var reminder in reminders)
                    {
                        notificationList.Add(new PushMessage(reminder.Token, title: "Reminder", body: reminder.Message));
                    }
                    try
                    {
                        var response = await client.PublishMultiple(notificationList);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return Json(new { sent = reminders.Count, error = ex.Message });
                    }
                }

                return Json(new { sent = reminders.Count, error = "" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        /// <summary>
        /// Get active status of a push subscription
        /// </summary>
        /// <returns>bool</returns>
        /// <response code="200">The status was received.</response>
        /// <response code="404">The status was not received.</response>
        [HttpGet("{token}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> IsNotificationActive(string token)
        {
            try
            {
                var result = await _notificationService.IsNotificationActive(token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        /// <summary>
        /// update push subscription
        /// </summary>
        /// <returns>bool</returns>
        /// <response code="200">The status was received.</response>
        /// <response code="404">The status was not received.</response>
        [HttpGet("{token}/{active}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdatePushSubscription(string token, bool active)
        {
            try
            {
                var result = await _notificationService.GetPushSubscription(token);
                if(result != null)
                {
                    result.ModifiedDate = DateTime.Now;
                    result.ActiveFlag = active;

                    var updateRes = await _notificationService.SavePushSubscription(result);

                    return Json(new { success = true });
                }
                else
                {
                    return await CreatePushSubscription(token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }
    }
}
