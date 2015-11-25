using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Rumr.DurryLights.Domain.Models;
using Rumr.DurryLights.Domain.Services;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace XmasLeds.WebApi.Controllers
{
    public class SmsController : Controller
    {
        private readonly ILightsService _lightsService;

        public SmsController(ILightsService lightsService)
        {
            _lightsService = lightsService;
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

            await _lightsService.HandleRequestAsync(request);

            var response = new TwilioResponse();

            return new TwiMLResult(response);
        }
    }
}