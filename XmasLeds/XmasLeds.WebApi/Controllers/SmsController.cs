using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Rumr.DurryLights.Domain;
using Rumr.DurryLights.ServiceBus;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace XmasLeds.WebApi.Controllers
{
    public class SmsController : Controller
    {
        private readonly IBusPublisher _busPublisher;

        public SmsController()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            _busPublisher = new BusPublisher(connectionString);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string from, string body)
        {
            var matches = Regex.Match(body, "^(\\d{1,3}),(\\d{1,3}),(\\d{1,3})$", RegexOptions.Compiled);

            if (matches.Success)
            {
                var colourRequest = new ColourRequest
                {
                    Red = int.Parse(matches.Groups[1].Value),
                    Green = int.Parse(matches.Groups[2].Value),
                    Blue = int.Parse(matches.Groups[3].Value)
                };

                await _busPublisher.PublishAsync(colourRequest);

                var response = new TwilioResponse();
                response.Message(string.Format("Hello {0}. Your colour request was successful.", from));

                return new TwiMLResult(response);
            }
            else
            {
                var response = new TwilioResponse();
                response.Message(string.Format("Hello {0}. Your msg was {1}. Have a nice day!", from, body));

                return new TwiMLResult(response);
            }
        }
    }
}