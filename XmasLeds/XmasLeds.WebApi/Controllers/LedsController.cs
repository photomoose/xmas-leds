using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace XmasLeds.WebApi.Controllers
{
    public class LedsController : ApiController
    {
        private MessagingFactory _factory;

        public LedsController()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            _factory = MessagingFactory.CreateFromConnectionString(connectionString);
        }

        public IHttpActionResult Post([FromBody]ColourRequest colourRequest)
        {
            if (colourRequest == null)
            {
                return BadRequest("Invalid colour request.");
            }

            var topicClient = _factory.CreateTopicClient("Commands");

            var json = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(colourRequest)));
            var brokeredMessage = new BrokeredMessage(json);

            topicClient.Send(brokeredMessage);

            return Ok();
        }
    }

    public class ColourRequest
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }
}
