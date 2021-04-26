using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventFully.EMS.Models;
using Microsoft.AspNetCore.Authorization;
using EventFully.Services.Interfaces;
using System.Linq.Expressions;
using DataTables.AspNet.Core;
using EventFully.Models;
using DataTables.AspNet.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EventFully.Constant;
using Stripe;
using System.Text.Encodings.Web;
using ExcelDataReader;
using System.Globalization;
using Stripe.Checkout;

namespace EventFully.EMS.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IEmailService _emailService;
        private readonly ISpeakerService _speakerService;
        private readonly IAgendaService _agendaService;
        private readonly IExhibitorService _exhibitorService;
        private readonly IGenericSearchService<Sponsor> _sponsorGenericSearchService;
        private readonly IGenericSearchService<UserEventRoleView> _userGenericSearchService;
        private readonly IGenericSearchService<SpeakerListView> _speakerGenericSearchService;
        private readonly IGenericSearchService<TrackListView> _trackGenericSearchService;
        private readonly IGenericSearchService<Exhibitor> _exhibitorGenericSearchService;
        private readonly IGenericSearchService<AgendaItem> _agendaItemGenericSearchService;
        private readonly IGenericSearchService<AgendaItemView> _agendaItemViewGenericSearchService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CloudSettings _cloudSettings;
        private readonly ICloudService _cloudService;
        private readonly IAuthorizationService _authorizationService;
        private readonly StripeSettings _stripeSettings;
        public EventController(
            IEmailService emailService,
            IEventService eventService,
            ISpeakerService speakerService,
            IAgendaService agendaService,
            IExhibitorService exhibitorService,
            IGenericSearchService<UserEventRoleView> userGenericSearchService,
            IGenericSearchService<Sponsor> sponsorGenericSearchService,
            IGenericSearchService<SpeakerListView> speakerGenericSearchService,
            IGenericSearchService<TrackListView> trackGenericSearchService,
            IGenericSearchService<Exhibitor> exhibitorGenericSearchService,
            IGenericSearchService<AgendaItem> agendaItemGenericSearchService,
            IGenericSearchService<AgendaItemView> agendaItemViewGenericSearchService,
            UserManager<ApplicationUser> userManager,
            IOptions<CloudSettings> cloudSettings,
            IOptions<StripeSettings> stripeSettings,
            ICloudService cloudService,
            IAuthorizationService authorizationService
            )
        {
            _emailService = emailService;
            _eventService = eventService;
            _speakerService = speakerService;
            _agendaService = agendaService;
            _exhibitorService = exhibitorService;
            _userGenericSearchService = userGenericSearchService;
            _sponsorGenericSearchService = sponsorGenericSearchService;
            _speakerGenericSearchService = speakerGenericSearchService;
            _trackGenericSearchService = trackGenericSearchService;
            _exhibitorGenericSearchService = exhibitorGenericSearchService;
            _agendaItemGenericSearchService = agendaItemGenericSearchService;
            _agendaItemViewGenericSearchService = agendaItemViewGenericSearchService;
            _userManager = userManager;
            _cloudSettings = cloudSettings.Value;
            _stripeSettings = stripeSettings.Value;
            _cloudService = cloudService;
            _authorizationService = authorizationService;
        }

        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> Index(int eventId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId),
                SpeakerCount = await _speakerService.GetSpeakerCountByEventId(eventId),
                ExhibitorCount = await _exhibitorService.GetExhibitorCountByEventId(eventId),
                ScheduleCount = await _agendaService.GetAgendaItemCountByEventId(eventId),
                TrackCount = await _agendaService.GetTrackCountByEventId(eventId),
                SponsorCount = await _eventService.GetSponsorCountByEventId(eventId)
            };
            return View(vm);
        }

        [HttpGet("event/editevent/{eventId:int}")]
        public async Task<IActionResult> EditEvent(int eventId)
        {
            ViewBag.StateList = GetStateList();
            if (eventId > 0)
            {
                var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
                if (!authResult.Succeeded)
                    return RedirectToAction("Index", "Home");

                var vm = new EventViewModel()
                {
                    Event = await _eventService.GetEventById(eventId)
                };

                ViewBag.AddressList = new List<SelectListItem>(){new SelectListItem()
                {
                    Text = vm.Event.AddressLabel,
                    Value = vm.Event.AddressLabelValue
                } };

                return View(vm);
            }
            else
            {
                var vm = new EventViewModel()
                {
                    Event = new EventFully.Models.Event() { StartDate = DateTime.Now, EndDate = DateTime.Now, EventBanner = "https://assets.eventbx.com/header-bkg.jpg", EventThumb = "https://assets.eventbx.com/header-bkg.jpg"} 
                    };
                return View(vm);
            }
            
            
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(EventViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isNewEvent = false;
                    if (vm.Event.Id == 0)
                    {
                        vm.Event.CreatedDate = DateTime.Now;
                        vm.Event.CreatedByUserId = _userManager.GetUserId(User);
                        isNewEvent = true;
                    }
                    vm.Event.ModifiedDate = DateTime.Now;
                    vm.Event.ModifiedByUserId = _userManager.GetUserId(User);

                    // save event
                    var eventItem = await _eventService.SaveEvent(vm.Event);

                    var jsonString = HttpContext.Request.Form["slimBanner[]"];
                    if (!String.IsNullOrEmpty(jsonString.ToString()))
                    {
                        SlimImage imageAttributes = JsonConvert.DeserializeObject<SlimImage>(jsonString.ToString());
                        //var container = await _cloudService.GetContainer(_cloudSettings.EventsPhotosContainer);
                        var blobName = String.Format("events/{0}_640x320{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(imageAttributes.Output.Name));

                        var fileUrl = await _cloudService.UploadFile(imageAttributes.Output.Image, blobName);
                        eventItem.EventBanner = fileUrl;
                        await _eventService.SaveEvent(eventItem);
                    }

                    jsonString = HttpContext.Request.Form["slimBadge[]"];
                    if (!String.IsNullOrEmpty(jsonString.ToString()))
                    {
                        SlimImage imageAttributes = JsonConvert.DeserializeObject<SlimImage>(jsonString.ToString());
                        //var container = await _cloudService.GetContainer(_cloudSettings.EventsPhotosContainer);
                        var blobName = String.Format("events/{0}_200x200{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(imageAttributes.Output.Name));

                        var fileUrl = await _cloudService.UploadFile(imageAttributes.Output.Image, blobName);
                        eventItem.EventThumb = fileUrl;
                        await _eventService.SaveEvent(eventItem);
                    }

                    // add the user as an admin
                    if (isNewEvent)
                    {
                        await _eventService.SaveUserEventRole(new UserEventRole()
                        {
                            EventId = eventItem.Id,
                            UserId = eventItem.CreatedByUserId,
                            RoleId = SecurityRole.Administrator
                        });
                    }
                    return Json(new
                    {
                        success = true,
                        returnToHomeUrl = $"/event/{eventItem.Id}",
                        reloadUrl = $"/event/editevent/{eventItem.Id}"
                    });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { success = false });
            }
        }

        [HttpGet("event/speakers/{eventId:int}")]
        public async Task<IActionResult> Speakers(int eventId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId),
                //SpeakerCount = await _speakerService.GetSpeakerCountByEventId(eventId),
            };
            return View(vm);
        }

        [HttpGet("event/speakers/EditSpeaker/{eventId:int}/{speakerId:int}")]
        public async Task<IActionResult> Speaker(int eventId, int speakerId = 0)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var vm = new SpeakerViewModel();
            if (speakerId > 0)
                vm.Speaker = await _speakerService.GetSpeakerById(speakerId);
            else
            {
                vm.Speaker = new Speaker() { EventId = eventId, Event = await _eventService.GetEventById(eventId) };
            }
                

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSpeaker(SpeakerViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.Speaker.Id == 0)
                    {
                        vm.Speaker.CreatedDate = DateTime.Now;
                        vm.Speaker.CreatedByUserId = _userManager.GetUserId(User);
                    }
                    vm.Speaker.ModifiedDate = DateTime.Now;
                    vm.Speaker.ModifiedByUserId = _userManager.GetUserId(User);
                    vm.Speaker.FullName = String.Format("{0} {1}", vm.Speaker.FirstName, vm.Speaker.LastName);

                    // save speaker
                    var speaker = await _speakerService.SaveSpeaker(vm.Speaker);

                    var jsonString = HttpContext.Request.Form["slimProfile[]"];
                    if (!String.IsNullOrEmpty(jsonString.ToString()))
                    {
                        SlimImage imageAttributes = JsonConvert.DeserializeObject<SlimImage>(jsonString.ToString());
                        //var container = await _cloudService.GetContainer(_cloudSettings.SpeakerPhotosContainer);
                        var blobName = String.Format("speaker/{0}_200x200{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(imageAttributes.Output.Name));

                        var fileUrl = await _cloudService.UploadFile(imageAttributes.Output.Image, blobName);
                        speaker.ProfilePic = fileUrl;
                        await _speakerService.SaveSpeaker(speaker);
                    }

                    return Json(new {
                        success = true,
                        speakerId = speaker.Id,
                        addAnotherRedirectUrl =$"/event/speakers/editspeaker/{vm.Speaker.EventId}/0",
                        returnToListUrl =$"/event/speakers/{vm.Speaker.EventId}",
                        reloadUrl = $"/event/speakers/editspeaker/{vm.Speaker.EventId}/{speaker.Id}"
                    });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAgendaItem(int agendaItemId)
        {
            try
            {
                await _agendaService.RemoveAgendaItem(agendaItemId);
                await _agendaService.RemoveAgendaItemTracks(agendaItemId);
                await _agendaService.RemoveUserAgendaItems(agendaItemId);

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAgendaItem(AgendaViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.AgendaItem.Id == 0)
                    {
                        vm.AgendaItem.CreatedDate = DateTime.Now;
                        vm.AgendaItem.CreatedByUserId = _userManager.GetUserId(User);
                    }
                    vm.AgendaItem.ModifiedDate = DateTime.Now;
                    vm.AgendaItem.ModifiedByUserId = _userManager.GetUserId(User);
                    vm.AgendaItem.StartDate = Convert.ToDateTime($"{vm.AgendaItem.StartDate.ToShortDateString()} {vm.AgendaItem.StartTime}");
                    vm.AgendaItem.EndDate = Convert.ToDateTime($"{vm.AgendaItem.EndDate.ToShortDateString()} {vm.AgendaItem.EndTime}");
                    // save agenda item
                    var item = await _agendaService.SaveAgendaItem(vm.AgendaItem);

                    // remove existing speakers
                    if (vm.AgendaItem.Id > 0)
                    {
                        await _speakerService.RemoveAgendaItemSpeakers(vm.AgendaItem.Id);
                        await _agendaService.RemoveAgendaItemTracks(vm.AgendaItem.Id);
                    }
                    // add speakers
                    List<AgendaItemSpeaker> selectedSpeaker = new List<AgendaItemSpeaker>();
                    if (vm.SelectedSpeakers.Count() > 0)
                    {
                        foreach (var speaker in vm.SelectedSpeakers)
                            selectedSpeaker.Add(new AgendaItemSpeaker() { AgendaItemId = item.Id, SpeakerId = Convert.ToInt32(speaker) });

                        await _speakerService.AddAgendaItemSpeakers(selectedSpeaker);
                    }

                    //add tracks
                    List<TrackAgendaItem> selectedTracks = new List<TrackAgendaItem>();
                    if (vm.SelectedTracks.Count() > 0)
                    {
                        foreach (var track in vm.SelectedTracks)
                            selectedTracks.Add(new TrackAgendaItem() { AgendaItemId = item.Id, TrackId = Convert.ToInt32(track) });

                        await _agendaService.AddAgendaItemTracks(selectedTracks);
                    }

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> AddAgendaItemToSpeaker(int agendaItemId, int speakerId)
        {
            try
            {
                await _speakerService.AddAgendaItemToSpeaker(new AgendaItemSpeaker() { AgendaItemId = agendaItemId, SpeakerId = speakerId});

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> RemoveAgendaItemToSpeaker(int agendaItemId, int speakerId)
        {
            try
            {
                var success = await _speakerService.RemoveAgendaItemToSpeaker(agendaItemId, speakerId );

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> DeleteSpeaker(int speakerId)
        {
            try
            {
                var success = await _speakerService.DeleteSpeaker(speakerId);

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false });
            }
        }

        [HttpGet("event/AgendaItemsOld/{eventId:int}")]
        public async Task<IActionResult> AgendaItemsOld(int eventId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var agendaItems = await _agendaService.GetAgendaItems(eventId);
            var minStartDate = "08:00";

            if (agendaItems.Count > 0)
                minStartDate = agendaItems.Min(i => i.StartDate.ToString("HH:mm"));

            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId),
                MinStartDate = minStartDate
            };
            return View(vm);
        }

        [HttpGet("event/AgendaItems/{eventId:int}")]
        public async Task<IActionResult> AgendaItems(int eventId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var agendaItems = await _agendaService.GetAgendaItems(eventId);
            var eventItem = await _eventService.GetEventById(eventId);

            var minStartDate = "08:00";

            List<DateTime> dates = new List<DateTime>();

            if (agendaItems.Count > 0)
            {
                minStartDate = agendaItems.Min(i => i.StartDate.ToString("HH:mm"));

                var maxAgendaDate = agendaItems.Max(i => i.StartDate);
                var minAgendaDate = agendaItems.Min(i => i.StartDate);
                var days = maxAgendaDate.Subtract(minAgendaDate).TotalDays;
                
                for (var i = 0; i <= days; i++)
                {
                    dates.Add(minAgendaDate.AddDays(i));
                }
            }
            else
            {
                minStartDate = eventItem.StartDate.ToString("HH:mm");

                var maxAgendaDate = eventItem.EndDate;
                var minAgendaDate = eventItem.StartDate;
                var days = maxAgendaDate.Subtract(minAgendaDate).TotalDays;

                for (var i = 0; i <= days; i++)
                {
                    dates.Add(minAgendaDate.AddDays(i));
                }
            }

            var vm = new EventViewModel()
            {
                Event = eventItem,
                MinStartDate = minStartDate,
                AgendaDates = dates
            };
            return View(vm);
        }

        //[HttpGet("event/GetAgendaItemMinTime/{eventId:int}")]
        public async Task<IActionResult> GetAgendaItemMinTime(int eventId)
        {
            var agendaItems = await _agendaService.GetAgendaItems(eventId);
            var minStartDate = "08:00";

            if (agendaItems.Count > 0)
                minStartDate = agendaItems.Min(i => i.StartDate.ToString("HH:mm"));

            return Json(new { minTime = minStartDate });
        }
        public async Task<IActionResult> GetAgendaItemListOld(int eventId)
        {
            var events = await _agendaService.GetAgendaItems(eventId);

            return Ok(events.Select(i => new AgendaItemCalendarEvent()
            {
                Id = i.Id,
                EventId = i.EventId,
                Title = i.Title,
                Start = i.StartDate.ToString(),
                End = i.EndDate.ToString(),
                AllDay = false,
                TrackAgendaItems = i.TrackAgendaItems
            }));
        }

        public async Task<IActionResult> GetAgendaItemList(int eventId, string agendaDate)
        {
            var events = await _agendaService.GetAgendaItems(eventId);
            events = events.Where(i => i.StartDate.ToShortDateString() == agendaDate).ToList();
            return Ok(events.Select(i => new AgendaItemCalendarEvent()
            {
                Id = i.Id,
                EventId = i.EventId,
                Title = i.Title,
                Start = i.StartDate.ToString(),
                End = i.EndDate.ToString(),
                AllDay = false,
                TrackAgendaItems = i.TrackAgendaItems
            }));
        }

        [HttpPost]
        public async Task<IActionResult> AddTrack(int eventId, string hexColor, string trackName)
        {
            try
            {
                var track = await _agendaService.SaveTrack(new Track()
                {
                    EventId = eventId,
                    HexColor = hexColor,
                    TrackName = trackName
                });

                return Json(new { success = true, trackId = track.Id });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [HttpPost("event/GetAgendaItemsByEvent")]
        public IActionResult GetAgendaItemsByEvent(IDataTablesRequest request, int eventId)
        {
            // set initial filter
            Expression<Func<AgendaItemView, bool>> filterExpression = c => c.EventId == eventId;

            // init search expression
            Expression<Func<AgendaItemView, bool>> searchExpression = null;
            // create default sort
            //ParameterExpression param = Expression.Parameter(typeof(AgendaItemView), "x");
            //var body = Expression.Convert(Expression.Property(param, "StartDate"), typeof(object));
            //var orderBy = Expression.Lambda<Func<AgendaItemView, object>>(body, param);
            Expression<Func<AgendaItemView, object>> orderBy = c => c.ScheduledDate;
            //dynamic orderBy = orderByDate;

            // get the sorting information
            string sortDirection = "Ascending";
            var sortColumn = request.Columns.Where(c => c.Sort != null).FirstOrDefault();

            if (sortColumn != null)
            {
                // set the sort direction
                switch (sortColumn.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        sortDirection = "Descending";
                        break;
                    case SortDirection.Descending:
                        sortDirection = "Ascending";
                        break;
                    default:
                        sortDirection = "Ascending";
                        break;
                }

                //set the sort field
                switch (sortColumn.Field.ToUpper())
                {
                    case "SCHEDULEDDATE":
                        orderBy = c => c.ScheduledDate;
                        break;
                    case "TITLE":
                        orderBy = c => c.Title;
                        break;
                    default:
                        orderBy = c => c.ScheduledDate;
                        break;
                }
            }

            // handle search
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                string search = request.Search.Value;
                searchExpression = s => s.Title.Contains(search);
            }
            // get the datatable results
            SearchResponse<AgendaItemView> resultx = _agendaItemViewGenericSearchService.Search(filterExpression, searchExpression, orderBy, request.Length + request.Start, request.Start, sortDirection);

            // create the datatable response
            var result = DataTablesResponse.Create(request, resultx.iTotalRecords, resultx.iTotalDisplayRecords, resultx.Results);

            // return the result
            return new DataTablesJsonResult(result, true);
        }

        [HttpPost("event/GetSpeakerListByEvent")]
        public IActionResult GetSpeakerListByEvent(IDataTablesRequest request, int eventId)
        {
            // set initial filter
            Expression<Func<SpeakerListView, bool>> filterExpression = c => c.EventId == eventId;

            // init search expression
            Expression<Func<SpeakerListView, bool>> searchExpression = null;
            // create default sort
            Expression<Func<SpeakerListView, object>> orderBy = c => c.LastName;

            // get the sorting information
            string sortDirection = "Ascending";
            var sortColumn = request.Columns.Where(c => c.Sort != null).FirstOrDefault();

            if (sortColumn != null)
            {
                // set the sort direction
                switch (sortColumn.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        sortDirection = "Descending";
                        break;
                    case SortDirection.Descending:
                        sortDirection = "Ascending";
                        break;
                    default:
                        sortDirection = "Ascending";
                        break;
                }

                // set the sort field
                switch (sortColumn.Field.ToUpper())
                {
                    case "FULLNAME":
                        orderBy = c => c.LastName;
                        break;
                    case "COMPANYNAME":
                        orderBy = c => c.CompanyName;
                        break;
                    default:
                        orderBy = c => c.LastName;
                        break;
                }
            }

            // handle search
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                string search = request.Search.Value;
                searchExpression = s => s.FullName.Contains(search) || s.CompanyName.Contains(search);
            }
            // get the datatable results
            SearchResponse<SpeakerListView> resultx = _speakerGenericSearchService.Search(filterExpression, searchExpression, orderBy, request.Length + request.Start, request.Start, sortDirection);

            // create the datatable response
            var result = DataTablesResponse.Create(request, resultx.iTotalRecords, resultx.iTotalDisplayRecords, resultx.Results);

            // return the result
            return new DataTablesJsonResult(result, true);
        }

        [HttpPost("event/GetSpeakerAgendaItems")]
        public IActionResult GetSpeakerAgendaItems(IDataTablesRequest request, int eventId, int speakerId)
        {
            // set initial filter
            Expression<Func<AgendaItem, bool>> filterExpression = c => c.EventId == eventId;

            // init search expression
            Expression<Func<AgendaItem, bool>> searchExpression = null;
            // create default sort
            Expression<Func<AgendaItem, object>> orderBy = c => c.Title;

            // get the sorting information
            string sortDirection = "Ascending";
            var sortColumn = request.Columns.Where(c => c.Sort != null).FirstOrDefault();

            if (sortColumn != null)
            {
                // set the sort direction
                switch (sortColumn.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        sortDirection = "Descending";
                        break;
                    case SortDirection.Descending:
                        sortDirection = "Ascending";
                        break;
                    default:
                        sortDirection = "Ascending";
                        break;
                }

                // set the sort field
                switch (sortColumn.Field.ToUpper())
                {
                    case "TITLE":
                        orderBy = c => c.Title;
                        break;
                    case "STARTDATE":
                        orderBy = c => c.StartDate;
                        break;
                    default:
                        orderBy = c => c.Title;
                        break;
                }
            }

            // handle search
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                string search = request.Search.Value;
                searchExpression = s => s.Title.Contains(search);
            }
            // get the datatable results
            SearchResponse<AgendaItem> resultx = _agendaItemGenericSearchService.Search(filterExpression, searchExpression, orderBy, request.Length + request.Start, request.Start, sortDirection);

            resultx.Results.ForEach(i => i.CurrentSpeakerAssigned = i.AgendaItemSpeakers.Any(x => x.SpeakerId == speakerId));
            
            // create the datatable response
            var result = DataTablesResponse.Create(request, resultx.iTotalRecords, resultx.iTotalDisplayRecords, resultx.Results);

            // return the result
            return new DataTablesJsonResult(result, true);
        }

        public async Task<IActionResult> EditAgendaItem(int id)
        {
            AgendaViewModel vm = new AgendaViewModel();

            vm.AgendaItem = await _agendaService.GetAgendaItem(id);
            vm.AgendaItem.StartTime = vm.AgendaItem.StartDate.ToShortTimeString();
            vm.AgendaItem.EndTime = vm.AgendaItem.EndDate.ToShortTimeString();
            vm.Tracks = await GetTrackList(vm.AgendaItem.EventId, vm.AgendaItem.TrackAgendaItems);

            ViewBag.EventTimes = GetAgendaTimes();
            ViewBag.SpeakerList = await GetSpeakerList(vm.AgendaItem.EventId, vm.AgendaItem.AgendaItemSpeakers);

            return PartialView("_ModalAgendaItem", vm);
        }

        public async Task<IActionResult> AddAgendaItem(int eventId, DateTime startDate)
        {
            AgendaViewModel vm = new AgendaViewModel();

            vm.AgendaItem = new AgendaItem();
            vm.AgendaItem.EventId = eventId;
            vm.AgendaItem.StartDate = startDate;
            vm.AgendaItem.EndDate = startDate;
            vm.AgendaItem.StartTime = vm.AgendaItem.StartDate.ToShortTimeString();
            vm.AgendaItem.EndTime = vm.AgendaItem.EndDate.ToShortTimeString();
            vm.Tracks = await GetTrackList(vm.AgendaItem.EventId, vm.AgendaItem.TrackAgendaItems);

            ViewBag.EventTimes = GetAgendaTimes();
            ViewBag.SpeakerList = await GetSpeakerList(vm.AgendaItem.EventId, vm.AgendaItem.AgendaItemSpeakers);

            return PartialView("_ModalAgendaItem", vm);
        }

        private List<SelectListItem> GetAgendaTimes()
        {
            List<String> times = new List<string>();
            var agendaDate = new DateTime(DateTime.Today.Year, 1, 1, 0, 0, 0);
            for (var i = 0;i< 96;i++)
            {
                times.Add(agendaDate.ToShortTimeString());
                agendaDate = agendaDate.AddMinutes(15);
            }

            return times.Select(i => new SelectListItem() { Value = i, Text = i }).ToList();
        }

        private async Task<List<SelectListItem>> GetSpeakerList(int eventId, ICollection<AgendaItemSpeaker> selectedSpeakers)
        {
            var speakers = await _speakerService.GetSpeakersByEventId(eventId);
            List<SelectListItem> speakerList = new List<SelectListItem>();
            foreach(var speaker in speakers)
            {
                if (selectedSpeakers != null)
                    speakerList.Add(new SelectListItem() { Value = speaker.Id.ToString(), Text = speaker.FullName, Selected = selectedSpeakers.Any(i => i.SpeakerId == speaker.Id)});
                else
                    speakerList.Add(new SelectListItem() { Value = speaker.Id.ToString(), Text = speaker.FullName });
            }
            return speakerList;
        }

        private async Task<List<Track>> GetTrackList(int eventId, ICollection<TrackAgendaItem> selectedTracks)
        {
            var tracks = await _agendaService.GetTracks(eventId);
            List<Track> trackList = new List<Track>();
            foreach (var track in tracks)
            {
                if (selectedTracks != null)
                    track.Selected = selectedTracks.Any(i => i.TrackId == track.Id);

                trackList.Add(track);
            }
            return trackList;
        }

        [HttpGet("event/exhibitors/{eventId:int}")]
        public async Task<IActionResult> Exhibitors(int eventId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId),
            };
            return View(vm);
        }

        [HttpGet("event/exhibitors/EditExhibitor/{eventId:int}/{exhibitorId:int}")]
        public async Task<IActionResult> Exhibitor(int eventId, int exhibitorId = 0)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            ViewBag.StateList = GetStateList();
            var eventItem = await _eventService.GetEventById(eventId);
            var vm = new ExhibitorViewModel();
            if (exhibitorId > 0)
            {
                vm.Exhibitor = await _exhibitorService.GetExhibitorById(exhibitorId);
                vm.Exhibitor.EventName = eventItem.EventName;
            }
            else
            {
                vm.Exhibitor = new Exhibitor() { EventId = eventId, EventName = eventItem.EventName };
            }


            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SaveExhibitor(ExhibitorViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.Exhibitor.Id == 0)
                    {
                        vm.Exhibitor.CreatedDate = DateTime.Now;
                        vm.Exhibitor.CreatedByUserId = _userManager.GetUserId(User);
                    }
                    vm.Exhibitor.ModifiedDate = DateTime.Now;
                    vm.Exhibitor.ModifiedByUserId = _userManager.GetUserId(User);
                    if (!String.IsNullOrEmpty(vm.Exhibitor.ContactPhone))
                        vm.Exhibitor.ContactPhone = vm.Exhibitor.ContactPhone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-","");

                    // save exhibitor
                    var exhibitor = await _exhibitorService.SaveExhibitor(vm.Exhibitor);

                    var jsonString = HttpContext.Request.Form["slimProfile[]"];
                    if (!String.IsNullOrEmpty(jsonString.ToString()))
                    {
                        SlimImage imageAttributes = JsonConvert.DeserializeObject<SlimImage>(jsonString.ToString());
                        //var container = await _cloudService.GetContainer(_cloudSettings.ExhibitorPhotosContainer);
                        var blobName = String.Format("exhibitor/{0}_200x200{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(imageAttributes.Output.Name));

                        //var fileUrl = await _cloudService.UploadBlob(imageAttributes.Output.Image, imageAttributes.Output.Type, container, blobName);
                        var fileUrl = await _cloudService.UploadFile(imageAttributes.Output.Image, blobName);
                        exhibitor.ProfilePic = fileUrl;
                        await _exhibitorService.SaveExhibitor(exhibitor);
                    }

                    return Json(new
                    {
                        success = true,
                        addAnotherRedirectUrl = $"/event/exhibitors/editexhibitor/{vm.Exhibitor.EventId}/0",
                        returnToListUrl = $"/event/exhibitors/{vm.Exhibitor.EventId}",
                        reloadUrl = $"/event/exhibitors/editexhibitor/{vm.Exhibitor.EventId}/{exhibitor.Id}"
                    });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> DeleteExhibitor(int exhibitorId)
        {
            try
            {
                var success = await _exhibitorService.DeleteExhibitor(exhibitorId);

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false });
            }
        }

        [HttpPost("event/GetExhibitorListByEvent")]
        public IActionResult GetExhibitorListByEvent(IDataTablesRequest request, int eventId)
        {
            // set initial filter
            Expression<Func<Exhibitor, bool>> filterExpression = c => c.EventId == eventId;

            // init search expression
            Expression<Func<Exhibitor, bool>> searchExpression = null;
            // create default sort
            Expression<Func<Exhibitor, object>> orderBy = c => c.ExhibitorName;

            // get the sorting information
            string sortDirection = "Ascending";
            var sortColumn = request.Columns.Where(c => c.Sort != null).FirstOrDefault();

            if (sortColumn != null)
            {
                // set the sort direction
                switch (sortColumn.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        sortDirection = "Descending";
                        break;
                    case SortDirection.Descending:
                        sortDirection = "Ascending";
                        break;
                    default:
                        sortDirection = "Ascending";
                        break;
                }

                // set the sort field
                switch (sortColumn.Field.ToUpper())
                {
                    case "EXHIBITORNAME":
                        orderBy = c => c.ExhibitorName;
                        break;
                    default:
                        orderBy = c => c.ExhibitorName;
                        break;
                }
            }

            // handle search
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                string search = request.Search.Value;
                searchExpression = s => s.ExhibitorName.Contains(search) || s.ContactName.Contains(search);
            }
            // get the datatable results
            SearchResponse<Exhibitor> resultx = _exhibitorGenericSearchService.Search(filterExpression, searchExpression, orderBy, request.Length + request.Start, request.Start, sortDirection);

            // create the datatable response
            var result = DataTablesResponse.Create(request, resultx.iTotalRecords, resultx.iTotalDisplayRecords, resultx.Results);

            // return the result
            return new DataTablesJsonResult(result, true);
        }

        [HttpGet("event/tracks/{eventId:int}")]
        public async Task<IActionResult> Tracks(int eventId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId),
            };
            return View(vm);
        }

        [HttpPost("event/GetTrackListByEvent")]
        public IActionResult GetTrackListByEvent(IDataTablesRequest request, int eventId)
        {
            // set initial filter
            Expression<Func<TrackListView, bool>> filterExpression = c => c.EventId == eventId;

            // init search expression
            Expression<Func<TrackListView, bool>> searchExpression = null;
            // create default sort
            Expression<Func<TrackListView, object>> orderBy = c => c.TrackName;

            // get the sorting information
            string sortDirection = "Ascending";
            var sortColumn = request.Columns.Where(c => c.Sort != null).FirstOrDefault();

            if (sortColumn != null)
            {
                // set the sort direction
                switch (sortColumn.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        sortDirection = "Descending";
                        break;
                    case SortDirection.Descending:
                        sortDirection = "Ascending";
                        break;
                    default:
                        sortDirection = "Ascending";
                        break;
                }

                // set the sort field
                switch (sortColumn.Field.ToUpper())
                {
                    case "TRACKNAME":
                        orderBy = c => c.TrackName;
                        break;
                    default:
                        orderBy = c => c.TrackName;
                        break;
                }
            }

            // handle search
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                string search = request.Search.Value;
                searchExpression = s => s.TrackName.Contains(search);
            }
            // get the datatable results
            SearchResponse<TrackListView> resultx = _trackGenericSearchService.Search(filterExpression, searchExpression, orderBy, request.Length + request.Start, request.Start, sortDirection);

            // create the datatable response
            var result = DataTablesResponse.Create(request, resultx.iTotalRecords, resultx.iTotalDisplayRecords, resultx.Results);

            // return the result
            return new DataTablesJsonResult(result, true);
        }

        public async Task<IActionResult> EditTrack(int id)
        {
            TrackViewModel vm = new TrackViewModel();

            vm.Track = await _agendaService.GetTrackById(id);

            return PartialView("_ModalTrack", vm);
        }

        public async Task<IActionResult> AddTrack(int eventId)
        {
            TrackViewModel vm = new TrackViewModel();
            vm.Track = new Track()
            {
                EventId = eventId
            };

            return PartialView("_ModalTrack", vm);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTrack(TrackViewModel vm)
        {
            try
            {
                if(vm.Track.Id == 0)
                {
                    vm.Track.CreatedByUserId = _userManager.GetUserId(User);
                    vm.Track.CreatedDate = DateTime.Now;
                }
                vm.Track.ModifiedByUserId = _userManager.GetUserId(User);
                vm.Track.ModifiedDate = DateTime.Now;

                var track = await _agendaService.SaveTrack(vm.Track);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> DeleteTrack(int trackId)
        {
            try
            {
                var success = await _agendaService.DeleteTrack(trackId);

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false });
            }
        }

        [HttpGet("event/sponsors/{eventId:int}")]
        public async Task<IActionResult> Sponsors(int eventId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId),
            };
            return View(vm);
        }

        [HttpGet("event/users/{eventId:int}")]
        public async Task<IActionResult> Users(int eventId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, new RequirementType() { Type = Constant.RequirementTypes.Event, Id = eventId }, "AdministrateEvent");
            if (!authResult.Succeeded)
                return RedirectToAction("Index", "Home");

            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId),
            };
            return View(vm);
        }

        public async Task<IActionResult> EditSponsor(int id)
        {
            SponsorViewModel vm = new SponsorViewModel();

            vm.Sponsor = await _eventService.GetSponsorById(id);

            ViewBag.StateList = GetStateList();

            return PartialView("_ModalSponsor", vm);
        }

        public async Task<IActionResult> AddSponsor(int eventId)
        {
            SponsorViewModel vm = new SponsorViewModel();

            vm.Sponsor = new Sponsor() { EventId = eventId };

            ViewBag.StateList = GetStateList();

            return PartialView("_ModalSponsor", vm);
        }

        [HttpPost("event/GetSponsorListByEvent")]
        public IActionResult GetSponsorListByEvent(IDataTablesRequest request, int eventId)
        {
            // set initial filter
            Expression<Func<Sponsor, bool>> filterExpression = c => c.EventId == eventId;

            // init search expression
            Expression<Func<Sponsor, bool>> searchExpression = null;
            // create default sort
            Expression<Func<Sponsor, object>> orderBy = c => c.SponsorName;

            // get the sorting information
            string sortDirection = "Ascending";
            var sortColumn = request.Columns.Where(c => c.Sort != null).FirstOrDefault();

            if (sortColumn != null)
            {
                // set the sort direction
                switch (sortColumn.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        sortDirection = "Descending";
                        break;
                    case SortDirection.Descending:
                        sortDirection = "Ascending";
                        break;
                    default:
                        sortDirection = "Ascending";
                        break;
                }

                // set the sort field
                switch (sortColumn.Field.ToUpper())
                {
                    case "SPONSORNAME":
                        orderBy = c => c.SponsorName;
                        break;
                    default:
                        orderBy = c => c.SponsorName;
                        break;
                }
            }

            // handle search
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                string search = request.Search.Value;
                searchExpression = s => s.SponsorName.Contains(search);
            }
            // get the datatable results
            SearchResponse<Sponsor> resultx = _sponsorGenericSearchService.Search(filterExpression, searchExpression, orderBy, request.Length + request.Start, request.Start, sortDirection);

            // create the datatable response
            var result = DataTablesResponse.Create(request, resultx.iTotalRecords, resultx.iTotalDisplayRecords, resultx.Results);

            // return the result
            return new DataTablesJsonResult(result, true);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSponsor(SponsorViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.Sponsor.Id == 0)
                    {
                        vm.Sponsor.CreatedDate = DateTime.Now;
                        vm.Sponsor.CreatedByUserId = _userManager.GetUserId(User);
                    }
                    vm.Sponsor.ModifiedDate = DateTime.Now;
                    vm.Sponsor.ModifiedByUserId = _userManager.GetUserId(User);
                    if (!String.IsNullOrEmpty(vm.Sponsor.ContactPhone))
                        vm.Sponsor.ContactPhone = vm.Sponsor.ContactPhone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-","");

                    // save sponsor
                    var sponsor = await _eventService.SaveSponsor(vm.Sponsor);

                    var jsonString = HttpContext.Request.Form["slimProfile[]"];
                    if (!String.IsNullOrEmpty(jsonString.ToString()))
                    {
                        SlimImage imageAttributes = JsonConvert.DeserializeObject<SlimImage>(jsonString.ToString());
                        //var container = await _cloudService.GetContainer(_cloudSettings.SponsorPhotosContainer);
                        var blobName = String.Format("sponsor/{0}_200x200{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(imageAttributes.Output.Name));

                        var fileUrl = await _cloudService.UploadFile(imageAttributes.Output.Image, blobName);
                        sponsor.ProfilePic = fileUrl;
                        await _eventService.SaveSponsor(sponsor);
                    }

                    jsonString = HttpContext.Request.Form["slimBanner[]"];
                    if (!String.IsNullOrEmpty(jsonString.ToString()))
                    {
                        SlimImage imageAttributes = JsonConvert.DeserializeObject<SlimImage>(jsonString.ToString());
                        //var container = await _cloudService.GetContainer(_cloudSettings.SponsorPhotosContainer);
                        var blobName = String.Format("sponsor/{0}_640x100{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(imageAttributes.Output.Name));

                        var fileUrl = await _cloudService.UploadFile(imageAttributes.Output.Image, blobName);
                        sponsor.BannerImage = fileUrl;
                        await _eventService.SaveSponsor(sponsor);
                    }

                    return Json(new
                    {
                        success = true,
                        addAnotherRedirectUrl = $"/event/sponsors/editexhibitor/{vm.Sponsor.EventId}/0",
                        returnToListUrl = $"/event/sponsors/{vm.Sponsor.EventId}",
                        reloadUrl = $"/event/sponsors/editsponsor/{vm.Sponsor.EventId}/{sponsor.Id}"
                    });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> DeleteSponsor(int sponsorId)
        {
            try
            {
                var success = await _eventService.DeleteSponsor(sponsorId);

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false });
            }
        }

        [HttpGet("event/publish/{eventId:int}")]
        public async Task<IActionResult> Publish(int eventId, string message, bool paid)
        {
            ViewData["ErrorMessage"] = "";
            if (!String.IsNullOrEmpty(message))
            {
                ViewData["ErrorMessage"] = "An error occurred while saving your order.";
                if (paid)
                    ViewData["ErrorMessage"] += " You card was charged and our Support Staff has been notified and will contact you shortly.";
                else
                    ViewData["ErrorMessage"] += " You card was not charged and our Support Staff has been notified of the issue.";
            }

            //// create stripe checkout session
            //StripeConfiguration.SetApiKey(_stripeSettings.SecretKey);
            //var options = new SessionCreateOptions
            //{
            //    PaymentMethodTypes = new List<string> { "card", },
            //    BillingAddressCollection = "required",
            //    LineItems = new List<SessionLineItemOptions> {
            //        new SessionLineItemOptions {
            //            Name = "Single Event",
            //            Description = "EventBx - Single Event Purchase (Valid For 1 year)",
            //            Amount = 24999,
            //            Currency = "usd",
            //            Quantity = 1,
            //        },
            //    },
            //    SuccessUrl = $"https://localhost:44356/Event/Publish/{eventId}",
            //    CancelUrl = $"https://localhost:44356/Event/Publish/{eventId}",
            //};

            //var service = new SessionService();
            //Session session = service.Create(options);

            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId),
                Order = await _eventService.GetOrderByEventId(eventId)
                //StripeSessionId = session.Id
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Publish(EventViewModel model)
        {
            var eventItem = await _eventService.GetEventById(model.Event.Id);
            eventItem.Published = true;
            eventItem.PublishedByUserId = _userManager.GetUserId(User);
            eventItem.PublishedDate = DateTime.Now;
            await _eventService.SaveEvent(eventItem);

            return RedirectToAction("Publish", new { eventId = model.Event.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Unpublish(EventViewModel model)
        {
            var eventItem = await _eventService.GetEventById(model.Event.Id);
            eventItem.Published = false;
            eventItem.PublishedByUserId = _userManager.GetUserId(User);
            eventItem.PublishedDate = DateTime.Now;
            await _eventService.SaveEvent(eventItem);

            return RedirectToAction("Publish", new { eventId = model.Event.Id });
        }

        [HttpPost]
        public async Task<IActionResult> PayForEvent(
            string stripeEmail,
            string stripeToken,
            string stripeBillingName,
            string stripeBillingAddressLine1,
            string stripeBillingAddressApt,
            string stripeBillingAddressZip,
            string stripeBillingAddressCity,
            string stripeBillingAddressState,
            string stripeBillingAddressCountry,
            string stripeBillingAddressCountryCode,
            EventViewModel vm)
        {
            var charges = new ChargeService();

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = 24900,
                Description = "EventBx - Single Event Purchase (Valid For 1 year)",
                Currency = "usd",
                SourceId = stripeToken,
                StatementDescriptor = "EventBx",
                Capture = false
            });

            Charge captureCharge = new Charge();
            if (charge.Status == "succeeded")
            {
                try
                {
                    var order = new EventFully.Models.Order()
                    {
                        BillingAddressApt = stripeBillingAddressApt,
                        BillingAddressCity = stripeBillingAddressCity,
                        BillingAddressCountry = stripeBillingAddressCountry,
                        BillingAddressCountryCode = stripeBillingAddressCountryCode,
                        BillingAddressLine1 = stripeBillingAddressLine1,
                        BillingAddressState = stripeBillingAddressState,
                        BillingAddressZip = stripeBillingAddressZip,
                        BillingName = stripeBillingName,
                        CreatedByUserId = _userManager.GetUserId(User),
                        CreatedDate = DateTime.Now,
                        EmailAddress = stripeEmail,
                        EventId = vm.Event.Id,
                        OrderDate = DateTime.Now,
                        OrderTotal = 249.00M,
                        StripePaymentId = charge.Id
                    };

                    var orderItems = new List<EventFully.Models.OrderItem>(){new EventFully.Models.OrderItem()
                    {
                        Description = "EventBx - Single Event Purchase (Valid For 1 year)",
                        ItemAmount = 249.00M,
                        ProcessingFees = 0.00M,
                        Quantity = 1
                    } };

                    //throw new Exception("Test Error");
                    await _eventService.SaveOrder(order, orderItems);
                    
                    captureCharge = charges.Capture(charge.Id, null);
                    return RedirectToAction("Publish", new { eventId = vm.Event.Id });
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Publish", new { eventId = vm.Event.Id, message="Error",paid=captureCharge.Captured == null ? false :captureCharge.Captured });
                }
            }
            else
            {
                return RedirectToAction("Publish", new { eventId = vm.Event.Id, message = "Error", paid = captureCharge.Captured == null ? false : captureCharge.Captured });
            }
        }

        [HttpPost("event/GetUserListByEvent")]
        public IActionResult GetUserListByEvent(IDataTablesRequest request, int eventId)
        {
            // set initial filter
            Expression<Func<UserEventRoleView, bool>> filterExpression = c => c.EventId == eventId;

            // init search expression
            Expression<Func<UserEventRoleView, bool>> searchExpression = null;
            // create default sort
            Expression<Func<UserEventRoleView, object>> orderBy = c => c.Name;

            // get the sorting information
            string sortDirection = "Ascending";
            var sortColumn = request.Columns.Where(c => c.Sort != null).FirstOrDefault();

            if (sortColumn != null)
            {
                // set the sort direction
                switch (sortColumn.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        sortDirection = "Descending";
                        break;
                    case SortDirection.Descending:
                        sortDirection = "Ascending";
                        break;
                    default:
                        sortDirection = "Ascending";
                        break;
                }

                // set the sort field
                switch (sortColumn.Field.ToUpper())
                {
                    case "NAME":
                        orderBy = c => c.Name;
                        break;
                    case "ROLE":
                        orderBy = c => c.Role;
                        break;
                    default:
                        orderBy = c => c.Name;
                        break;
                }
            }

            // handle search
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                string search = request.Search.Value;
                searchExpression = s => s.Name.Contains(search) || s.Role.Contains(search);
            }
            // get the datatable results
            SearchResponse<UserEventRoleView> resultx = _userGenericSearchService.Search(filterExpression, searchExpression, orderBy, request.Length + request.Start, request.Start, sortDirection);

            // create the datatable response
            var result = DataTablesResponse.Create(request, resultx.iTotalRecords, resultx.iTotalDisplayRecords, resultx.Results);

            // return the result
            return new DataTablesJsonResult(result, true);
        }

        public async Task<IActionResult> DeleteEventUser(int id)
        {
            try
            {
                // get the user event role
                var userEventRole = await _eventService.GetUserEventRoleById(id);
                // check that there is at least one admin for the event
                var adminsRemaining = await _eventService.AreThereAdminsRemaining(userEventRole.UserId, userEventRole.EventId);

                if (adminsRemaining)
                {
                    var success = await _eventService.DeleteUserEventRole(userEventRole);
                    if(success)
                        return Json(new { success = success });
                    else
                        return Json(new { success = false, message="An unexpected error occurred while removing the user from the event." });
                }
                else
                    return Json(new { success = false, message="This use cannot be removed from the event because there has to be at least one Administrator Role." });
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> AddEventUser(int eventId)
        {
            UserEventInvitationViewModel vm = new UserEventInvitationViewModel();

            vm.UserEventInvitation = new UserEventInvitation() { EventId = eventId };

            return PartialView("_ModalUser", vm);
        }

        public async Task<IActionResult> SendUserEventInvitation(string email, int eventId)
        {
            try
            {
                var existingInvite = await _eventService.GetUserEventInvitation(email, eventId);
                if (existingInvite != null)
                {
                    existingInvite.ModifiedDate = DateTime.Now;
                    existingInvite.ModifiedByUserId = _userManager.GetUserId(User);
                    existingInvite.Token = Guid.NewGuid().ToString();
                    existingInvite.TokenExpiration = DateTime.Now.AddDays(7);
                    existingInvite.RoleId = 1;

                    var invitation = await _eventService.SaveUserEventInvitation(existingInvite);
                    invitation.Event = await _eventService.GetEventById(invitation.EventId);

                    var callbackUrl = Url.Action(
                        "ConfirmEventInvitation",
                        "Account",
                        new { email = invitation.EmailAddress, token = invitation.Token },
                        Url.ActionContext.HttpContext.Request.Scheme
                        );

                    await _emailService.SendEventInvitationAsync(invitation.EmailAddress, "Confirm your Invitation", callbackUrl, invitation.Event.EventName);

                    return Json(new
                    {
                        success = true
                    });
                }
                else
                {
                    return Json(new { success = false, message = "The original invitation cannot be found, therefore it cannot be resent." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while sending the invitation." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveUser(UserEventInvitationViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var invitation = new UserEventInvitation();
                    var existingInvite = await _eventService.GetUserEventInvitation(vm.UserEventInvitation.EmailAddress, vm.UserEventInvitation.EventId);
                    if (existingInvite != null)
                    {
                        existingInvite.ModifiedDate = DateTime.Now;
                        existingInvite.ModifiedByUserId = _userManager.GetUserId(User);
                        existingInvite.Token = Guid.NewGuid().ToString();
                        existingInvite.TokenExpiration = DateTime.Now.AddDays(7);
                        existingInvite.RoleId = 1;

                        invitation = await _eventService.SaveUserEventInvitation(existingInvite);
                    }
                    else
                    {
                        vm.UserEventInvitation.CreatedDate = DateTime.Now;
                        vm.UserEventInvitation.CreatedByUserId = _userManager.GetUserId(User);
                        vm.UserEventInvitation.ModifiedDate = DateTime.Now;
                        vm.UserEventInvitation.ModifiedByUserId = _userManager.GetUserId(User);
                        vm.UserEventInvitation.Token = Guid.NewGuid().ToString();
                        vm.UserEventInvitation.TokenExpiration = DateTime.Now.AddDays(7);
                        vm.UserEventInvitation.RoleId = 1;

                        invitation = await _eventService.SaveUserEventInvitation(vm.UserEventInvitation);
                    }

                    // save invitation
                    
                    invitation.Event = await _eventService.GetEventById(invitation.EventId);

                    var callbackUrl = Url.Action(
                        "ConfirmEventInvitation",
                        "Account",
                        new { email = invitation.EmailAddress, token = invitation.Token },
                        Url.ActionContext.HttpContext.Request.Scheme
                        );

                    await _emailService.SendEventInvitationAsync(vm.UserEventInvitation.EmailAddress, "Confirm your Invitation", callbackUrl, invitation.Event.EventName);

                    return Json(new
                    {
                        success = true
                    });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { success = false });
            }
        }

        [HttpGet("/event/ImportSchedule/{eventId:int}")]
        public async Task<IActionResult> ImportSchedule(int eventId)
        {
            var vm = new ImportScheduleViewModel();
            vm.Event = await _eventService.GetEventById(eventId);

            return View(vm);
        }

        [HttpPost("/event/UploadSchedule")]
        public async Task<IActionResult> UploadSchedule(ImportScheduleViewModel vm)
        {
            // get existing speakers
            var existingSpeakers = await _speakerService.GetSpeakersByEventId(vm.Event.Id);
            // get existing tracks
            var existingTracks = await _agendaService.GetTracks(vm.Event.Id);

            foreach (var ev in vm.ImportedScheduleItems)
            {
                var agendaItem = new AgendaItem()
                {
                    CreatedByUserId = _userManager.GetUserId(User),
                    CreatedDate = DateTime.Now,
                    Description = ev.SessionDescription,
                    EndDate = Convert.ToDateTime(ev.SessionDate.Value.ToShortDateString() + " " +  ev.SessionEndTime),
                    EventId = vm.Event.Id,
                    Location = ev.SessionLocation,
                    ModifiedByUserId = _userManager.GetUserId(User),
                    ModifiedDate = DateTime.Now,
                    StartDate = Convert.ToDateTime(ev.SessionDate.Value.ToShortDateString() + " " + ev.SessionStartTime),
                    Title = ev.SessionTitle
                };

                agendaItem = await _agendaService.SaveAgendaItem(agendaItem);

                // speakers
                if(!String.IsNullOrEmpty(ev.SessionSpeaker))
                {
                    var speakers = ev.SessionSpeaker.Split(',');
                    foreach(var speaker in speakers)
                    {
                        var speakerName = speaker.Trim();

                        // check existence
                        if(existingSpeakers.Any(i=>i.FullName == speakerName))
                        {
                            // add speaker
                            AgendaItemSpeaker agendaItemSpeaker = new AgendaItemSpeaker()
                            {
                                AgendaItemId = agendaItem.Id,
                                SpeakerId = existingSpeakers.Where(i => i.FullName == speakerName).FirstOrDefault().Id
                            };

                            await _speakerService.AddAgendaItemToSpeaker(agendaItemSpeaker);
                        }
                        else
                        {
                            // add speaker
                            var newSpeaker = new Speaker()
                            {
                                CreatedByUserId = _userManager.GetUserId(User),
                                CreatedDate = DateTime.Now,
                                EventId = vm.Event.Id,
                                FirstName = speakerName.Substring(0,speakerName.IndexOf(" ")).Trim(),
                                LastName = speakerName.Substring(speakerName.IndexOf(" ")).Trim(),
                                FullName = speakerName,
                                ModifiedByUserId = _userManager.GetUserId(User),
                                ModifiedDate = DateTime.Now
                            };

                            newSpeaker = await _speakerService.SaveSpeaker(newSpeaker);
                            existingSpeakers.Add(newSpeaker);
                            AgendaItemSpeaker agendaItemSpeaker = new AgendaItemSpeaker()
                            {
                                AgendaItemId = agendaItem.Id,
                                SpeakerId = newSpeaker.Id
                            };
                            await _speakerService.AddAgendaItemToSpeaker(agendaItemSpeaker);
                        }
                    }
                }

                // tracks
                if (!String.IsNullOrEmpty(ev.SessionTrack))
                {
                    var tracks = ev.SessionTrack.Split(',');
                    foreach (var track in tracks)
                    {
                        // check existence
                        if (existingTracks.Any(i => i.TrackName == track.Trim()))
                        {
                            // add track
                            TrackAgendaItem trackAgendaItem = new TrackAgendaItem()
                            {
                                AgendaItemId = agendaItem.Id,
                                TrackId = existingTracks.Where(i => i.TrackName == track.Trim()).FirstOrDefault().Id
                            };

                            await _agendaService.AddAgendaItemTracks(new List<TrackAgendaItem>()
                            {
                                trackAgendaItem
                            });
                        }
                        else
                        {
                            // add track
                            var newTrack = new Track()
                            {
                                CreatedByUserId = _userManager.GetUserId(User),
                                CreatedDate = DateTime.Now,
                                EventId = vm.Event.Id,
                                ModifiedByUserId = _userManager.GetUserId(User),
                                ModifiedDate = DateTime.Now,
                                HexColor = "#FFFFFF",
                                TrackName = track.Trim()
                            };

                            newTrack = await _agendaService.SaveTrack(newTrack);
                            existingTracks.Add(newTrack);
                            TrackAgendaItem trackAgendaItem = new TrackAgendaItem()
                            {
                                AgendaItemId = agendaItem.Id,
                                TrackId = newTrack.Id
                            };

                            await _agendaService.AddAgendaItemTracks(new List<TrackAgendaItem>()
                            {
                                trackAgendaItem
                            });
                        }
                    }
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ImportSchedule(ImportScheduleViewModel vm)
        {
            var importedItems = new List<ImportedScheduleItem>();
            if (Request != null)
            {
                try
                {
                    var file = Request.Form.Files["importFile"];
                    if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = file.FileName;

                        var data = file.OpenReadStream();

                        IExcelDataReader excelReader = null;
                        if (fileName.EndsWith("xls"))
                            excelReader = ExcelReaderFactory.CreateBinaryReader(data);

                        if (fileName.EndsWith("xlsx"))
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(data);

                        if(excelReader != null)
                        {
                            int skipRows = 4;
                            int sessionDateCol = 0;
                            int sessionStartTimeCol = 1;
                            int sessionEndTimeCol = 2;
                            int sessionTitleCol = 3;
                            int sessionDescriptionCol = 4;
                            int sessionLocationCol = 5;
                            int sessionTrackCol = 6;
                            int sessionSpeakerCol = 7;

                            for (var i = 0; i <= skipRows; i++)
                            {
                                excelReader.Read();
                            }

                            while (excelReader.Read())
                            {
                                //var validated = true;
                                DateTime? sessionDate = null;
                                var sessionStartTime = "";
                                var sessionEndTime = "";
                                var sessionTitle = "";
                                var sessionDescription = "";
                                var sessionLocation = "";
                                var sessionTrack = "";
                                var sessionSpeaker = "";
                                var validationMessage = "";

                                var line = 1;
                                // validate session date
                                try
                                {
                                    sessionDate = excelReader.GetDateTime(sessionDateCol);
                                    //if (excelReader.GetDateTime(sessionDateCol).Year == 1)
                                    //    validationMessage = $"<li>Invalid Date. Column {sessionDateCol}</li>";
                                }
                                catch
                                {
                                    validationMessage = $"<li>Invalid Date. Column {sessionDateCol}</li>";
                                }

                                // validate start time
                                try
                                {
                                    //if (!String.IsNullOrEmpty(excelReader.GetString(sessionStartTimeCol)))
                                    //{
                                        // check for datetime first
                                        try
                                        {
                                            var eventDateTime = excelReader.GetDateTime(sessionStartTimeCol);
                                            sessionStartTime = eventDateTime.ToShortTimeString();
                                        }
                                        catch
                                        {
                                            DateTime dateTime = DateTime.ParseExact(excelReader.GetString(sessionStartTimeCol), "hh:mm tt", CultureInfo.InvariantCulture);
                                            TimeSpan span = dateTime.TimeOfDay;
                                            sessionStartTime = dateTime.ToShortTimeString();
                                        }
                                    //}
                                    //else
                                    //    validationMessage += $"<li>Invalid Start Time. Column {sessionStartTimeCol}</li>";
                                }
                                catch
                                {
                                    validationMessage += $"<li>Invalid Start Time. Column {sessionStartTimeCol}</li>";
                                }

                                // validate end time
                                try
                                {
                                    //if (!String.IsNullOrEmpty(excelReader.GetString(sessionEndTimeCol)))
                                    //{
                                        // check for datetime first
                                        try
                                        {
                                            var eventDateTime = excelReader.GetDateTime(sessionEndTimeCol);
                                            sessionEndTime = eventDateTime.ToShortTimeString();
                                        }
                                        catch
                                        {
                                            DateTime dateTime = DateTime.ParseExact(excelReader.GetString(sessionEndTimeCol), "hh:mm tt", CultureInfo.InvariantCulture);
                                            TimeSpan span = dateTime.TimeOfDay;
                                            sessionEndTime = dateTime.ToShortTimeString();
                                        }
                                    //}
                                    //else
                                    //    validationMessage += $"<li>Invalid End Time. Column {sessionEndTimeCol}</li>";
                                }
                                catch
                                {
                                    validationMessage += $"<li>Invalid End Time. Column {sessionEndTimeCol}</li>";
                                }

                                // validate title
                                try
                                {
                                    if (!String.IsNullOrEmpty(excelReader.GetString(sessionTitleCol)))
                                    {
                                        sessionTitle = excelReader.GetString(sessionTitleCol).Trim();
                                        if (String.IsNullOrEmpty(sessionTitle))
                                            validationMessage += $"<li>Invalid Title. Column {sessionTitleCol}</li>";
                                        else if(sessionTitle.Length > 255)
                                            validationMessage += $"<li>Invalid Title. Column {sessionTitleCol}. Length must be less than 255 characters</li>";
                                    }
                                    else
                                        validationMessage += $"<li>Invalid Title. Column {sessionTitleCol}</li>";
                                }
                                catch
                                {
                                    validationMessage += $"<li>Invalid Title. Column {sessionTitleCol}</li>";
                                }

                                // validate description
                                try
                                {
                                    if (!String.IsNullOrEmpty(excelReader.GetString(sessionDescriptionCol)))
                                    {
                                        sessionDescription = excelReader.GetString(sessionDescriptionCol).Trim();

                                        if (sessionDescription.Length > 8000)
                                            validationMessage += $"<li>Invalid Description. Column {sessionDescriptionCol}. Length must be less than 8000 characters.</li>";
                                    }
                                }
                                catch
                                {
                                    validationMessage += $"<li>Invalid Title. Column {sessionDescriptionCol}</li>";
                                }

                                // validate location
                                try
                                {
                                    if (!String.IsNullOrEmpty(excelReader.GetString(sessionLocationCol)))
                                    {
                                        sessionLocation = excelReader.GetString(sessionLocationCol).Trim();

                                        if (sessionLocation.Length > 255)
                                            validationMessage += $"<li>Invalid Location. Column {sessionLocationCol}. Length must be less than 255 characters.</li>";
                                    }
                                }
                                catch
                                {
                                    validationMessage += $"<li>Invalid Title. Column {sessionLocationCol}</li>";
                                }

                                // validate tracks
                                try
                                {
                                    if (!String.IsNullOrEmpty(excelReader.GetString(sessionTrackCol)))
                                    {
                                        sessionTrack = excelReader.GetString(sessionTrackCol).Trim();
                                        var tracks = sessionTrack.Split(',');
                                        foreach(var track in tracks)
                                        {
                                            if(track.Trim().Length > 255)
                                                validationMessage += $"<li>Invalid Track. Column {sessionTrackCol}. Length of each track must be less than 255 characters.</li>";
                                        }
                                    }
                                }
                                catch
                                {
                                    validationMessage += $"<li>Invalid Track. Column {sessionTrackCol}</li>";
                                }

                                // validate speakers
                                try
                                {
                                    if (!String.IsNullOrEmpty(excelReader.GetString(sessionSpeakerCol)))
                                    {
                                        sessionSpeaker = excelReader.GetString(sessionSpeakerCol).Trim();
                                        var speakers = sessionSpeaker.Split(',');
                                        foreach (var speaker in speakers)
                                        {
                                            if (speaker.Trim().Length > 101)
                                                validationMessage += $"<li>Invalid Speaker. Column {sessionSpeakerCol}. Length of each speaker name must be less than 101 characters.</li>";
                                        }
                                    }
                                }
                                catch
                                {
                                    validationMessage += $"<li>Invalid Speaker. Column {sessionSpeakerCol}</li>";
                                }

                                importedItems.Add(new ImportedScheduleItem()
                                {
                                    Id = line,
                                    ErrorMessage = validationMessage,
                                    SessionDate = sessionDate, //excelReader.GetDateTime(sessionDateCol),
                                    SessionDescription = sessionDescription,
                                    SessionEndTime = sessionEndTime,
                                    SessionLocation = sessionLocation,
                                    SessionSpeaker = sessionSpeaker,
                                    SessionStartTime = sessionStartTime,
                                    SessionTitle = sessionTitle,
                                    SessionTrack = sessionTrack
                                });
                                line++;
                            }
                        }
                    }
                    //return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            vm.Event = await _eventService.GetEventById(vm.Event.Id);
            vm.ImportedScheduleItems = importedItems;

            return View("UploadSchedule", vm);
        }
        public static List<SelectListItem> GetStateList()
        {
            List<String> States = new List<string>();

            States.Add("AK");
            States.Add("AL");
            States.Add("AR");
            States.Add("AZ");
            States.Add("CA");
            States.Add("CO");
            States.Add("CT");
            States.Add("DC");
            States.Add("DE");
            States.Add("FL");
            States.Add("GA");
            States.Add("HI");
            States.Add("IA");
            States.Add("ID");
            States.Add("IL");
            States.Add("IN");
            States.Add("KS");
            States.Add("KY");
            States.Add("LA");
            States.Add("MA");
            States.Add("MD");
            States.Add("ME");
            States.Add("MI");
            States.Add("MN");
            States.Add("MO");
            States.Add("MS");
            States.Add("MT");
            States.Add("NC");
            States.Add("ND");
            States.Add("NE");
            States.Add("NH");
            States.Add("NJ");
            States.Add("NM");
            States.Add("NV");
            States.Add("NY");
            States.Add("OH");
            States.Add("OK");
            States.Add("OR");
            States.Add("PA");
            States.Add("RI");
            States.Add("SC");
            States.Add("SD");
            States.Add("TN");
            States.Add("TX");
            States.Add("UT");
            States.Add("VA");
            States.Add("VT");
            States.Add("WA");
            States.Add("WI");
            States.Add("WV");
            States.Add("WY");


            return States.Select(t => new SelectListItem { Text = t, Value = t }).ToList();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public static class LambdaExpressionExtensions
    {
        //public static Expression<Func<TInput, object>> ToUntypedPropertyExpression<TInput, TOutput>(this Expression<Func<TInput, TOutput>> expression)
        //{
        //    var memberName = ((MemberExpression)expression.Body).Member.Name;

        //    var param = Expression.Parameter(typeof(TInput));
        //    var field = Expression.Property(param, memberName);
        //    return Expression.Lambda<Func<TInput, object>>(field, param);
        //}

        public static Expression<Func<TModel, TToProperty>> Cast<TModel, TFromProperty, TToProperty>(Expression<Func<TModel, TFromProperty>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(TToProperty));

            return Expression.Lambda<Func<TModel, TToProperty>>(converted, expression.Parameters);
        }
    }
}
