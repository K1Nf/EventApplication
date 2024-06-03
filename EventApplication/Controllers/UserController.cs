using EventApplication.Data;
using EventApplication.Models;
using EventApplication.ViewModels;
using User = EventApplication.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSharpFunctionalExtensions.ValueTasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;
using EventApplication.Models.RequestFolder;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventApplication.Controllers
{
    //[ApiController]
    [Microsoft.AspNetCore.Mvc.Route("Users")]
    public class UserController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("{id:guid}")]
        [ActionName("Account")]
        public async Task<ActionResult<User>> Account(Guid id)
        {
            var user = await _context.Users.Include(u=>u.Tags).FirstOrDefaultAsync(u=>u.Id == id);
            if (user == null) return BadRequest();

            List<Event> events = await _context.Events.Where(eve => eve.UserId == id).ToListAsync();

            
            events.ForEach(Event.ChangeStatus);


            AccEventVM AccEv = new()
            {
                User = user,
                Events = events
            };


            return View("Account", AccEv);
        }




        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("create")]
        public async Task<ActionResult<User>> CreateNewUser([FromForm] UserRequest userRequest)
        {
            var user = Models.User.CreateUser(userRequest);

            if (user.IsSuccess)
            {
                await _context.Users.AddAsync(user.Value);
                await _context.SaveChangesAsync();

                return View();
                //return Created();
            }

            return View();
            //return BadRequest();
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ActionName("GiveRating")]

        public async Task<ActionResult> GiveRating([FromBody] Guid id, [FromForm] int mark)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return View();
                //return BadRequest("User was not found");


            user.ChangeRating(mark);


            return View(user);
            //return Ok();
        }




        //[HttpGet]
        //[Microsoft.AspNetCore.Mvc.Route("usertags")]
        //public async Task<ActionResult> GetUsersWithTheirTags([AsParameters] int id)
        //{
        //
        //    var user = await _context.Users.Include(u => u.Tags).FirstOrDefaultAsync(u => u.Id == id);
        //    var tag = await _context.Tags.FirstOrDefaultAsync(u => u.Id == 5);
        //
        //    if (user == null)
        //        return BadRequest();
        //    return View("userInfo", user);
        //}


        
        [Microsoft.AspNetCore.Mvc.Route("Edit/{id:guid}")]
        [ActionName("Edit")]
        [HttpGet]
        public async Task<ActionResult<User>> EditAsync(Guid Id)
        {
            IEnumerable<SelectListItem> tagsList = _context.Tags.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            ViewBag.Tags = tagsList;


            var user = await _context.Users.FindAsync(Id);

            if (user == null)
                return BadRequest();


            return View("Edit", user);
        }
        


        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("EditUser")]
        [ActionName("EditUser")]
        public async Task<ActionResult> EditUser([FromForm] User user)
        {
            await _context.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(x => x
            .SetProperty(ev => ev.FirstName, user.FirstName)
            .SetProperty(ev => ev.LastName, user.LastName)
            .SetProperty(ev => ev.SecondName, user.SecondName)
            .SetProperty(ev => ev.PhoneNumber, user.PhoneNumber)
            .SetProperty(ev => ev.Email, user.Email)
            .SetProperty(ev => ev.City, user.City)
            .SetProperty(ev => ev.Country, user.Country)
            .SetProperty(ev => ev.Information, user.Information)
            .SetProperty(ev => ev.BirthDate, user.BirthDate)
            .SetProperty(ev => ev.Image_link, user.Image_link));
            await _context.SaveChangesAsync();


            List<int> tagsId = [.. user.TagsIds];
            List<Tag> tagsFromDb = [];

            foreach (var tagId in tagsId)
            {
                var tagFromDb = await _context.Tags.FindAsync(tagId);
                if (tagFromDb == null)
                    return BadRequest();
                tagsFromDb.Add(tagFromDb);
            }


            var userFromDb = await _context.Users.Include(ev => ev.Tags).FirstOrDefaultAsync(ev => ev.Id == user.Id);
            if (userFromDb == null) return BadRequest();


            userFromDb.Tags = tagsFromDb;
            _context.Users.Update(userFromDb);
            await _context.SaveChangesAsync();
            return Redirect($"/Users/{userFromDb.Id}");
        }
    }
}
