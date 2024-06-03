using CSharpFunctionalExtensions;
using EventApplication.Data;
using EventApplication.Models;
using EventApplication.Models.RequestFolder;
using EventApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace EventApplication.Controllers
{
    [Route("[controller]")]
    public class EventsController(ILogger<EventsController> logger, ApplicationDbContext context) : Controller
    {
        private readonly ILogger<EventsController> _logger = logger;
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetEventById")] 
        public async Task<ActionResult<Event>> GetEventById([FromRoute] Guid id)
        {
            var @event = await _context.Events.Include(ev => ev.User).Include(u => u.Tags).FirstOrDefaultAsync(ev => ev.Id == id);
            if(@event == null) return NotFound();


            Event.ChangeStatus(@event);


            return View("EventPage", @event);
        }



        [HttpGet]
        [Route("")]
        [ActionName("GetEvents")]
        public async Task<ActionResult<List<Event>>> GetEvents()
        {
            var allEvents = await _context.Events
                .Include(ev => ev.User)
                .Include(u => u.Tags)
                .OrderByDescending(ev=>ev.Created)
                //.OrderByDescending(ev => ev.Date)
                //.ThenBy(ev => ev.TimeStart)
                .ToListAsync();


            allEvents.ForEach(Event.ChangeStatus);


            var events = allEvents.Where(ev => ev.Status == "Запланировано").ToList();
            return View("index", events);
        }



        [HttpGet]
        [Route("Create")]
        [ActionName("Create")]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> tagsList = _context.Tags.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            ViewBag.Tags = tagsList;


            return View();
        }



        [HttpPost]
        public async Task<ActionResult> CreatePost(EventRequest eventRequest)
        {
            var eventResult = Event.CreateEvent(eventRequest);

            if (eventResult.IsSuccess)
            {
                Event @event = eventResult.Value;
                foreach (var tagId in eventRequest.TagesIds)
                {
                    var tagFromDb = await _context.Tags.FindAsync(tagId);
                    if (tagFromDb == null)
                        return BadRequest();
                    @event.Tags.Add(tagFromDb);
                }


                await _context.Events.AddAsync(@event);
                await _context.SaveChangesAsync();


                return Redirect($"/Users/{@event.UserId}");

            }
            else return View("Create");
        }



        [HttpGet]
        [ActionName("Update")]
        [Route("update/{id:guid}")]
        public async Task<ActionResult<Event>> UpdateAsync(Guid id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(ev => ev.Id == id);


            IEnumerable<SelectListItem> tagsList = _context.Tags.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            ViewBag.Tags = tagsList;


            if (@event == null) return NotFound();
            return View(@event);
        }



        [HttpPost]
        [ActionName("UpdateEventAsync")]
        [Route("UpdateEventAsync")]
        public async Task<ActionResult> UpdateEventAsync([FromForm] Event _event)  // протестить навигацию после обновления
        {
            await _context.Events.Where(ev => ev.Id == _event.Id).ExecuteUpdateAsync(x => x
            .SetProperty(ev => ev.Name, _event.Name)
            .SetProperty(ev => ev.Place, _event.Place)
            .SetProperty(ev => ev.Max_Ghost, _event.Max_Ghost)
            .SetProperty(ev => ev.TimeStart, _event.TimeStart)
            .SetProperty(ev => ev.TimeEnd, _event.TimeEnd)
            .SetProperty(ev => ev.Date, _event.Date)
            .SetProperty(ev => ev.Description, _event.Description));
            await _context.SaveChangesAsync();


            List<int> tagsId = [.. _event.TagesIds];
            List<Tag> tagsFromDb = [];

            foreach (var tagId in tagsId)
            {
                var tagFromDb = await _context.Tags.FindAsync(tagId);
                if (tagFromDb == null)
                    return BadRequest();
                tagsFromDb.Add(tagFromDb);
            }


            var eventFromDb = await _context.Events.Include(ev => ev.Tags).Include(ev => ev.User).FirstOrDefaultAsync(ev => ev.Id == _event.Id);
            if (eventFromDb == null) return BadRequest();


            eventFromDb.Tags = tagsFromDb;
            _context.Events.Update(eventFromDb);
            await _context.SaveChangesAsync();
            return View("EventPage", eventFromDb);
        }

        

        [HttpGet]
        [ActionName("Delete")]
        [Route("Delete/{id:guid}")]
        public async Task<ActionResult<Event>> DeleteAsync(Guid id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(ev => ev.Id == id);
            if (@event == null) return NotFound();


            return View(@event);
        }



        [HttpPost]
        [ActionName("DeleteEventAsync")]
        [Route("DeleteEventAsync")]
        public async Task<ActionResult<Event>> DeleteEventAsync(Guid id) // протестить навигацию после обновления
        {
            await _context.Events.Where(ev => ev.Id == id).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
            
            
            return Redirect("/Events");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
