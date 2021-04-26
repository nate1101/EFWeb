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
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        /// <summary>
        /// Event Controller
        /// </summary>
        /// <param name="eventService"></param>
        public EventsController(
            IEventService eventService
            )
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Gets a list of current and future events.
        /// </summary>
        /// <returns>The requested events.</returns>
        /// <response code="200">The events were successfully retrieved.</response>
        /// <response code="404">The events do not exist.</response>
        //[Authorize]
        [HttpGet]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCurrentEvents()
        {
            try
            {
                var events = await _eventService.GetCurrentEvents();

                return Ok(events.Select(i=>new {
                    i.City,
                    i.Description,
                    i.EndDate,
                    i.EventBanner,
                    i.EventName,
                    i.EventThumb,
                    i.Id,
                    i.PostalCode,
                    i.StartDate,
                    i.State,
                    i.Street,
                    i.VenueName
                }));
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets event by id.
        /// </summary>
        /// <returns>The requested event.</returns>
        /// <response code="200">The event was successfully retrieved.</response>
        /// <response code="404">The event does not exist.</response>
        //[Authorize]
        [HttpGet("{eventId:int}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            try
            {
                var eventItem = await _eventService.GetEventById(eventId);

                return Ok(new
                {
                    eventItem.City,
                    eventItem.Description,
                    eventItem.EndDate,
                    eventItem.EventBanner,
                    eventItem.EventName,
                    eventItem.EventThumb,
                    eventItem.Id,
                    eventItem.PostalCode,
                    eventItem.StartDate,
                    eventItem.State,
                    eventItem.Street,
                    eventItem.VenueName,
                    eventItem.Longitude,
                    eventItem.Latitude,
                    eventItem.AddressLabel
                }
                );
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets a list of sponsors.
        /// </summary>
        /// <returns>The requested sponsors.</returns>
        /// <response code="200">The sponsors were successfully retrieved.</response>
        /// <response code="404">The sponsors do not exist.</response>
        //[Authorize]
        [HttpGet("{eventId:int}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetSponsorsByEventId(int eventId)
        {
            try
            {
                var sponsors = await _eventService.GetSponsorsByEvent(eventId);

                return Ok(sponsors);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets sponsor by id.
        /// </summary>
        /// <returns>The requested sponsor.</returns>
        /// <response code="200">The sponsor was successfully retrieved.</response>
        /// <response code="404">The sponsor does not exist.</response>
        //[Authorize]
        [HttpGet("{sponsorId:int}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetSponsorById(int sponsorId)
        {
            try
            {
                var sponsor = await _eventService.GetSponsorById(sponsorId);

                return Ok(sponsor);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
