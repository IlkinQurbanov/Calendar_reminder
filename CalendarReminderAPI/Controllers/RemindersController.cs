using CalendarReminderAPI.Models;
using CalendarReminderAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.OData;

namespace CalendarReminderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public RemindersController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet("current-watch-time")]
        public IActionResult GetCurrentWatchTime()
        {
            var currentTime = DateTime.Now;
            var watchTime = currentTime.ToString("HH:mm:ss");
            return Ok(new { Message = "Current watch time retrieved successfully.", Time = watchTime });
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Reminder>>> GetReminders()
        {
            try
            {
                var reminders = await _reminderService.GetRemindersAsync();
                return Ok(new { Message = "Reminders retrieved successfully.", Data = reminders });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving reminders.", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reminder>> GetReminder(int id)
        {
            try
            {
                var reminder = await _reminderService.GetReminderByIdAsync(id);
                if (reminder == null)
                    return NotFound(new { Message = $"Reminder with ID {id} not found." });

                return Ok(new { Message = "Reminder retrieved successfully.", Data = reminder });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving the reminder.", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Reminder>> CreateReminder(Reminder reminder)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Message = "Invalid reminder data.", Errors = ModelState.Values.SelectMany(v => v.Errors) });

                var createdReminder = await _reminderService.CreateReminderAsync(reminder);
                return CreatedAtAction(nameof(GetReminder), new { id = createdReminder.Id }, new { Message = "Reminder created successfully.", Data = createdReminder });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while creating the reminder.", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, Reminder reminder)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Message = "Invalid reminder data.", Errors = ModelState.Values.SelectMany(v => v.Errors) });

                var updatedReminder = await _reminderService.UpdateReminderAsync(id, reminder);
                if (updatedReminder == null)
                    return NotFound(new { Message = $"Reminder with ID {id} not found." });

                return Ok(new { Message = "Reminder updated successfully.", Data = updatedReminder });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while updating the reminder.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            try
            {
                var result = await _reminderService.DeleteReminderAsync(id);
                if (!result)
                    return NotFound(new { Message = $"Reminder with ID {id} not found." });

                return Ok(new { Message = "Reminder deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while deleting the reminder.", Error = ex.Message });
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportToCsv()
        {
            try
            {
                var csvData = await _reminderService.ExportToCsvAsync();
                return File(csvData, "text/csv", "reminders.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while exporting reminders to CSV.", Error = ex.Message });
            }
        }
    }
}