using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventFully.EMS.Models;
using Microsoft.AspNetCore.Authorization;
using EventFully.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using EventFully.Models;

namespace EventFully.EMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEventService _eventService;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(
            IEventService eventService,
            UserManager<ApplicationUser> userManager
            )
        {
            _eventService = eventService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Landing()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var events = await _eventService.GetUserEvents(await _userManager.GetUserIdAsync(user));
            events.ForEach(i => i.EventOrder = _eventService.GetOrderByEventId(i.Id).Result);
            
            return View(events);
        }

        public async Task<IActionResult> Event(int eventId)
        {
            var vm = new EventViewModel()
            {
                Event = await _eventService.GetEventById(eventId)
            };
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
