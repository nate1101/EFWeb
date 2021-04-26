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
    public class SpeakersController : Controller
    {
        private readonly ISpeakerService _speakerService;
        /// <summary>
        /// Speaker Controller
        /// </summary>
        /// <param name="speakerService"></param>
        public SpeakersController(
            ISpeakerService speakerService
            )
        {
            _speakerService = speakerService;
        }

        /// <summary>
        /// Gets speaker by id.
        /// </summary>
        /// <returns>The requested speaker.</returns>
        /// <response code="200">The speaker was successfully retrieved.</response>
        /// <response code="404">The speaker does not exist.</response>
        //[Authorize]
        [HttpGet("{speakerId:int}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetSpeakerById(int speakerId)
        {
            try
            {
                var item = await _speakerService.GetSpeakerById(speakerId);

                return Ok(new {
                    item.Bio,
                    item.CompanyName,
                    item.EventId,
                    item.FirstName,
                    item.FullName,
                    item.Id,
                    item.LastName,
                    item.ProfilePic
                });
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets a list of speakers.
        /// </summary>
        /// <returns>The requested speakers.</returns>
        /// <response code="200">The speakers were successfully retrieved.</response>
        /// <response code="404">The speakers do not exist.</response>
        //[Authorize]
        [HttpGet("{eventId:int}")]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetSpeakersByEventId(int eventId)
        {
            try
            {
                var speakers = await _speakerService.GetSpeakersByEventId(eventId);

                return Ok(speakers.Select(i=> new
                {
                    i.Bio,
                    i.CompanyName,
                    i.EventId,
                    i.FirstName,
                    i.FullName,
                    i.Id,
                    i.LastName,
                    i.ProfilePic
                }));
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
