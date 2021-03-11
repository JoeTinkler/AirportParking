using System.Collections.Generic;
using System.Linq;
using AirportParking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AirportParking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AirplaneTypeController : ControllerBase
    {
        private readonly ILogger<AirplaneTypeController> _logger;
        private readonly IDataHelper _dataHelper;

        public AirplaneTypeController(ILogger<AirplaneTypeController> logger, IDataHelper dataHelper)
        {
            _logger = logger;
            _dataHelper = dataHelper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AircraftType>> Get()
        {
            return Ok(_dataHelper.GetAircraftTypes());
        }
        
        [HttpGet]
        [Route("{id?}")]
        public ActionResult<AircraftType> Get([FromRoute] string id)
        {
            return Ok(_dataHelper.GetAircraftType(id));
        }

        [HttpPost]
        public ActionResult Post([FromBody] AircraftType type)
        {
            _dataHelper.AddOrUpdateAircraftType(type);
            return Ok();
        }
        
        [HttpDelete]
        [Route("{id?}")]
        public ActionResult Delete([FromRoute] string id)
        {
            _dataHelper.RemoveAircraftType(id);
            return Ok();
        }
    }
}