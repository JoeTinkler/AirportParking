using System.Collections.Generic;
using System.Linq;
using AirportParking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AirportParking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingSpaceController : ControllerBase
    {
        private readonly ILogger<ParkingSpaceController> _logger;
        private readonly IDataHelper _dataHelper;

        public ParkingSpaceController(ILogger<ParkingSpaceController> logger, IDataHelper dataHelper)
        {
            _logger = logger;
            _dataHelper = dataHelper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ParkingSpace>> Get()
        {
            return Ok(_dataHelper.GetParkingSpaces());
        }
        
        [HttpGet]
        [Route("{id?}")]
        public ActionResult<ParkingSpace> Get([FromRoute] int id)
        {
            return Ok(_dataHelper.GetParkingSpace(id));
        }
        
        [HttpPost]
        public ActionResult Post([FromBody] ParkingSpace space)
        {
            _dataHelper.AddOrUpdateParkingSpace(space);
            return Ok();
        }
        
        [HttpDelete]
        [Route("{id?}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _dataHelper.RemoveParkingSpace(id);
            return Ok();
        }
    }
}