using EventFully.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.API.V1.Controllers
{
    /// <summary>
    /// Represents a RESTful service of events.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class ExhibitorController : Controller
    {
        private readonly IExhibitorService _exhibitorService;
        /// <summary>
        /// Exhibitor Controller
        /// </summary>
        /// <param name="exhibitorService"></param>
        public ExhibitorController(
            IExhibitorService exhibitorService
            )
        {
            _exhibitorService = exhibitorService;
        }

        /// <summary>
        /// Gets exhibitor by id.
        /// </summary>
        /// <returns>The requested exhibitor.</returns>
        /// <response code="200">The exhibitor was successfully retrieved.</response>
        /// <response code="404">The exhibitor does not exist.</response>
        //[Authorize]
        [HttpGet("{exhibitorId:int}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetExhibitorById(int exhibitorId)
        {
            try
            {
                var item = await _exhibitorService.GetExhibitorById(exhibitorId);

                return Ok(item);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets a list of exhibitors.
        /// </summary>
        /// <returns>The requested exhibitors.</returns>
        /// <response code="200">The exhibitors were successfully retrieved.</response>
        /// <response code="404">The exhibitors do not exist.</response>
        //[Authorize]
        [HttpGet("{eventId:int}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetExhibitorsByEventId(int eventId)
        {
            try
            {
                var exhibitors = await _exhibitorService.GetExhibitorsByEventId(eventId);

                return Ok(exhibitors);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
