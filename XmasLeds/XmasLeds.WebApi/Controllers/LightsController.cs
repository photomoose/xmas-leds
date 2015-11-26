using System.Threading.Tasks;
using System.Web.Http;
using Rumr.DurryLights.Domain.Models;
using Rumr.DurryLights.Domain.Services;

namespace XmasLeds.WebApi.Controllers
{
    public class LightsController : ApiController
    {
        private readonly ILightsService _lightsService;

        public LightsController(ILightsService lightsService)
        {
            _lightsService = lightsService;
        }

        public async Task<IHttpActionResult> Post([FromBody] LightsRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _lightsService.HandleRequestAsync(request);

            return Ok(result);
        }
    }
}
