using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler_BackEnd.Models;

namespace Scheduler_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private IScheduleService scheduleService { get; set; }

        public MainController(IScheduleService service) { 
            scheduleService = service;  
        }



        [HttpPost("users")]
        public IActionResult AddUser(User newUser)
        {
            if (ModelState.IsValid)
            {
                DataStorage.Users.Add(newUser);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("meetings")]
        public IActionResult FindMatchingTimeSlot(MeetingRequirements requirements)
        {
            var res = scheduleService.FindMatchingTimeSlot(requirements);
            if (res == null)
            {
                return NotFound("No matching slot");
            }
            return Ok(res);
        }

        [HttpPost("meetings/add")]
        public IActionResult AddMeeting(Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                DataStorage.Meetings.Add(meeting);
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("users/{userId:int}/meetings")]
        public IActionResult GetAllUserMeetings(int userId)
        {
            var result = DataStorage.Meetings
                .FindAll(m => m.Participants.Contains(userId));
            if (result.Count > 0)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
