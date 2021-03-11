using System;
using AirportParking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AirportParking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IAppointmentProcessor _appointmentProcessor;

        public BookingController(ILogger<BookingController> logger, IAppointmentProcessor appointmentProcessor)
        {
            _logger = logger;
            _appointmentProcessor = appointmentProcessor;
        }

        [HttpPost]
        [Route("recommend")]
        public ActionResult<int> GetRecommendation([FromBody]RecommendRequest request)
        {
            try
            {
                return Ok(_appointmentProcessor.RecommendSpace(request));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("book")]
        public ActionResult<BookingResponse> BookAppointment([FromBody]BookingRequest request)
        {
            try
            {
                return Ok(_appointmentProcessor.BookAppointment(request));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e.Message);
            }
        }
    }
}