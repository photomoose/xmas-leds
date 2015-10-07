using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace XmasLeds.WebApi.Controllers
{
    public class SmsController : Controller
    {
        private MessagingFactory _factory;

        public SmsController()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            _factory = MessagingFactory.CreateFromConnectionString(connectionString);            
        }

        [HttpPost]
        public ActionResult Index(string from, string body)
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

                var topicClient = _factory.CreateTopicClient("Commands");

                var json = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(colourRequest)));
                var brokeredMessage = new BrokeredMessage(json);

                topicClient.Send(brokeredMessage);

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