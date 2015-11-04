using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Rumr.DurryLights.Domain;
using Rumr.DurryLights.ServiceBus;
using Rumr.DurryLights.Sql;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;
using Colour = Rumr.DurryLights.Domain.Colour;

namespace XmasLeds.WebApi.Controllers
{
    public class SmsController : Controller
    {
        private readonly IBusPublisher _busPublisher;
        private readonly LightDisplayParser _lightDisplayParser;

        public SmsController()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            _busPublisher = new BusPublisher(connectionString);
            _lightDisplayParser = new LightDisplayParser(new ColourRepository());
        }

        [HttpPost]
        public async Task<ActionResult> Index(string from, string body)
        {
            var response = new TwilioResponse();

            var lightDisplay = await _lightDisplayParser.ParseAsync(body);

            if (lightDisplay != null)
            {
                await _busPublisher.PublishAsync(lightDisplay);

                //response.Message(string.Format("Hello {0}. Your colour request was successful.", from));
            }

            return new TwiMLResult(response);
        }
    }
}