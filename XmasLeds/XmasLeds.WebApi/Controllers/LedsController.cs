using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Rumr.DurryLights.Domain;
using Rumr.DurryLights.ServiceBus;

namespace XmasLeds.WebApi.Controllers
{
    public class LedsController : ApiController
    {
        private readonly IBusPublisher _busPublisher;

        public LedsController()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            _busPublisher = new BusPublisher(connectionString);
        }

        public async Task<IHttpActionResult> Post([FromBody]LightDisplay lightDisplay)
        {
            if (lightDisplay == null)
            {
                return BadRequest("Invalid colour request.");
            }

            await _busPublisher.PublishAsync(lightDisplay);

            return Ok();
        }
    }
}
