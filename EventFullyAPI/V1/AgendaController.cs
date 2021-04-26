using EventFully.Models;
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
    public class AgendaController : Controller
    {
        private readonly IAgendaService _agendaService;
        /// <summary>
        /// Event Controller
        /// </summary>
        /// <param name="agendaService"></param>
        public AgendaController(
            IAgendaService agendaService
            )
        {
            _agendaService = agendaService;
        }

        /// <summary>
        /// Gets a list of agenda items.
        /// </summary>
        /// <returns>The requested agenda items.</returns>
        /// <response code="200">The agenda items were successfully retrieved.</response>
        /// <response code="404">The agenda items do not exist.</response>
        //[Authorize]
        [HttpGet("{eventId:int}/{userId}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAgendaItems(int eventId, string userId)
        {
            try
            {
                var items = await _agendaService.GetAgendaItems(eventId);

                return Ok(items.Select(i=> new
                {
                    i.Description,
                    i.EndDate,
                    i.EventId,
                    i.Id,
                    i.Location,
                    i.StartDate,
                    i.Title,
                    IsUserAgendaItem = i.UserAgendaItems.Any(x=>x.UserId == userId),
                    i.TrackAgendaItems
                }));
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets a list of agenda items.
        /// </summary>
        /// <returns>The requested agenda items.</returns>
        /// <response code="200">The agenda items were successfully retrieved.</response>
        /// <response code="404">The agenda items do not exist.</response>
        //[Authorize]
        [HttpGet("{eventId:int}/{userId}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetMyAgendaItems(int eventId, string userId)
        {
            try
            {
                var items = await _agendaService.GetMyAgendaItems(eventId, userId);

                return Ok(items.Select(i => new
                {
                    i.Description,
                    i.EndDate,
                    i.EventId,
                    i.Id,
                    i.Location,
                    i.StartDate,
                    i.Title,
                    IsUserAgendaItem = true,
                    i.TrackAgendaItems
                }));
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets a list of agenda items.
        /// </summary>
        /// <returns>The requested agenda items.</returns>
        /// <response code="200">The agenda items were successfully retrieved.</response>
        /// <response code="404">The agenda items do not exist.</response>
        //[Authorize]
        [HttpGet("{eventId:int}/{userId}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetMyAgendaReminders(int eventId, string userId)
        {
            try
            {
                var items = await _agendaService.GetMyAgendaItems(eventId, userId);

                return Ok(items.Select(i => new
                {
                    i.Description,
                    i.EndDate,
                    i.EventId,
                    i.Id,
                    i.Location,
                    i.StartDate,
                    i.Title,
                    IsUserAgendaItem = true,
                    TimeRemaining = $"{Math.Round(i.StartDate.Subtract(DateTime.Now).TotalMinutes,0)}m"
                }).Take(3));
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets an agenda items.
        /// </summary>
        /// <returns>The requested agenda item.</returns>
        /// <response code="200">The agenda item was successfully retrieved.</response>
        /// <response code="404">The agenda item does not exist.</response>
        //[Authorize]
        [HttpGet("{agendaItemId:int}/{userId}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAgendaItem(int agendaItemId, string userId = "#")
        {
            try
            {
                var item = await _agendaService.GetAgendaItem(agendaItemId);

                return Ok(new
                {
                    item.AgendaItemSpeakers,
                    item.Description,
                    item.EndDate,
                    item.EventId,
                    item.Id,
                    item.Location,
                    item.StartDate,
                    item.Title,
                    IsUserAgendaItem = item.UserAgendaItems.Any(x=>x.UserId == userId)
                });
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Adds a user agenda item.
        /// </summary>
        /// <returns>The requested agenda item.</returns>
        /// <response code="200">The agenda item was successfully retrieved.</response>
        /// <response code="404">The agenda item does not exist.</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddUserAgendaItem([FromBody] UserAgendaItem userAgendaitem)
        {
            try
            {
                var item = await _agendaService.AddUserAgendaItem(userAgendaitem);
                return Ok(new
                {
                    item.AgendaItemSpeakers,
                    item.Description,
                    item.EndDate,
                    item.EventId,
                    item.Id,
                    item.Location,
                    item.StartDate,
                    item.Title,
                    IsUserAgendaItem = true
                });
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Removes a user agenda item.
        /// </summary>
        /// <returns>The requested agenda item.</returns>
        /// <response code="200">The agenda item was successfully retrieved.</response>
        /// <response code="404">The agenda item does not exist.</response>
        [HttpPost("{agendaItemId:int}/{userId}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RemoveUserAgendaItem(int agendaItemId, string userId)
        {
            try
            {
                var item = await _agendaService.RemoveUserAgendaItem(agendaItemId, userId);
                return Ok(new
                {
                    item.AgendaItemSpeakers,
                    item.Description,
                    item.EndDate,
                    item.EventId,
                    item.Id,
                    item.Location,
                    item.StartDate,
                    item.Title,
                    IsUserAgendaItem = item.UserAgendaItems.Any(x => x.UserId == userId)
                });
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets a list of tracks.
        /// </summary>
        /// <returns>The requested tracks.</returns>
        /// <response code="200">The tracks were successfully retrieved.</response>
        /// <response code="404">The tracks do not exist.</response>
        //[Authorize]
        [HttpGet("{eventId:int}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetTracks(int eventId)
        {
            try
            {
                var items = new List<Track>();
                items.Add(new Track() {
                    EventId = eventId,
                    Id = 0,
                    TrackName = "All Events"
                });
                items.AddRange(await _agendaService.GetTracks(eventId));

                return Ok(items);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
