using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using EventFully.Functions.Models;
using EventFully.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventFully.Functions
{
    public static class PushReminders
    {
        private static HttpClient httpClient = new HttpClient();

        public class PushReminderResponse
        {
            public int Sent { get; set; }
            public string Error { get; set; }
        }

        [FunctionName("SendPushReminders")]
        public static async void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            using (HttpResponseMessage responseMessage = await httpClient.GetAsync("https://XXXXXXX/api/v1/App/SendPushReminders"))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonResult = responseMessage.Content.ReadAsStringAsync().Result;

                    PushReminderResponse result = JsonConvert.DeserializeObject<PushReminderResponse>(jsonResult);
                    log.LogInformation("SENT: " + result.Sent.ToString());
                    if(!String.IsNullOrEmpty(result.Error))
                        log.LogError("ERROR: " + result.Error);
                }
            }
                    
        }
    }
}
