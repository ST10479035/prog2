using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organiser)
                .Select(e => new EventDto(e.EventId, e.OrganiserId, e.CategoryId, e.Title, e.Description, e.EventDate, e.Location, e.Category!.CategoryName, $"{e.Organiser!.FirstName} {e.Organiser.LastName}"))
                .ToListAsync();

            return Ok(events);
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organiser)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound(new { message = "Event ID does not exist." });

            return Ok(new EventDto(ev.EventId, ev.OrganiserId, ev.CategoryId, ev.Title, ev.Description, ev.EventDate, ev.Location, ev.Category!.CategoryName, $"{ev.Organiser!.FirstName} {ev.Organiser.LastName}"));
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var newEvent = new Event
            {
                OrganiserId = organiserId,
                CategoryId = dto.CategoryId,
                Title = dto.Title,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.EventId }, newEvent);
        }


        [HttpPost("{id}/enrol")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> EnrollInEvent(int id)
        {
            var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!await _context.Events.AnyAsync(e => e.EventId == id))
                return NotFound(new { message = "Event not found." });

            if (await _context.EventEnrolments.AnyAsync(e => e.EventId == id && e.ParticipantId == participantId))
                return Conflict(new { message = "Already enrolled in event." });

            var enrolment = new EventEnrolment
            {
                EventId = id,
                ParticipantId = participantId,
                Status = "Registered"
            };

            _context.EventEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Created(string.Empty, new EnrolmentDto(enrolment.EnrolmentId, enrolment.EventId, enrolment.ParticipantId, enrolment.EnrolmentDate, enrolment.Status, null, null));
        }


        [HttpGet("{id}/enrolments")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetEventEnrolments(int id)
        {
            var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var ev = await _context.Events.FindAsync(id);

            if (ev == null) return NotFound(new { message = "Event not found." });
            if (ev.OrganiserId != organiserId) return StatusCode(403, new { message = "Forbidden - Not creator of event." });

            var enrolments = await _context.EventEnrolments
                .Include(e => e.Participant)
                .Where(e => e.EventId == id)
                .Select(e => new EnrolmentDto(e.EnrolmentId, e.EventId, e.ParticipantId, e.EnrolmentDate, e.Status, $"{e.Participant!.FirstName} {e.Participant.LastName}", ev.Title))
                .ToListAsync();

            return Ok(enrolments);
        }
        [HttpPost("{id}/results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> SubmitResult(int id, [FromBody] CreateResultDto dto)
        {
            var enrolment = await _context.EventEnrolments.FindAsync(dto.EnrolmentId);
            if (enrolment == null || enrolment.EventId != id)
                return BadRequest(new { message = "Invalid enrolment record for this event." });

            if (await _context.Results.AnyAsync(r => r.EnrolmentId == dto.EnrolmentId))
                return Conflict(new { message = "Result already recorded for this enrolment." });

            var result = new Result
            {
                EnrolmentId = dto.EnrolmentId,
                FinishTimeSeconds = dto.FinishTimeSeconds,
                Position = dto.Position,
                Status = dto.Status
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Created(string.Empty, new ResultDto(result.ResultId, result.EnrolmentId, result.FinishTimeSeconds, result.Position, result.Status, null));
        }
        [HttpGet("{id}/results")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventResults(int id)
        {
            if (!await _context.Events.AnyAsync(e => e.EventId == id))
                return NotFound(new { message = "Event not found." });

            var results = await _context.Results
                .Include(r => r.Enrolment)
                .ThenInclude(e => e!.Participant)
                .Where(r => r.Enrolment!.EventId == id)
                .OrderBy(r => r.Position)
                .Select(r => new ResultDto(
                    r.ResultId,
                    r.EnrolmentId,
                    r.FinishTimeSeconds,
                    r.Position,
                    r.Status,
                    $"{r.Enrolment!.Participant!.FirstName} {r.Enrolment.Participant.LastName}"
                ))
                .ToListAsync();

            return Ok(results);
        }
    }
}