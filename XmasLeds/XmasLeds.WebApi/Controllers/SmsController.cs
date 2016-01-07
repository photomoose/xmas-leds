using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Rumr.DurryLights.Domain.Models;
using Rumr.DurryLights.Domain.Services;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;
using XmasLeds.WebApi.Configuration;

namespace XmasLeds.WebApi.Controllers
{
    public class SmsController : Controller
    {
        private readonly ILightsService _lightsService;
        private readonly ISmsSettings _smsSettings;

        public SmsController(ILightsService lightsService, ISmsSettings smsSettings)
        {
            _lightsService = lightsService;
            _smsSettings = smsSettings;
        }

        [HttpPost]
        public async Task<ActionResult> Index(string from, string body)
        {
            var request = new LightsRequest
            {
                Source = "twilio",
                From = from,
                Text = body
            };

            var lightsResponse = await _lightsService.HandleRequestAsync(request);

            var response = new TwilioResponse();

            if (_smsSettings.AllowResponses && lightsResponse.IsSuccess)
            {
                if (lightsResponse.IsScheduled && lightsResponse.ScheduledForUtc.HasValue)
                {
                    response.Sms(string.Format("Ooops, you're in a queue! Don't worry, your lights have been scheduled for {0}. Merry Christmas from #157!",
                        lightsResponse.ScheduledForUtc.Value.ToString("HH:mm")));
                }
                else
                {
                    response.Sms("Thanks! Your lights will be shown shortly. Merry Christmas from #157!");                    
                }
            }

            return new TwiMLResult(response);
        }
    }
}