using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Academy.Crm.Core.Interfaces;

namespace Academy.Crm.Api.Controllers;

[ApiController]
[Route("reports")]
[Authorize]
public class ReportsController(IUnitOfWork uow) : ControllerBase
{
    public record CourseAttendanceReport(int CourseId, int TotalSessions, int TotalAttendances, double AttendancePercent);

    [HttpGet("course-attendance")]
    public async Task<ActionResult<CourseAttendanceReport>> CourseAttendance([FromQuery] int courseId, CancellationToken ct)
    {
        if (courseId <= 0) return BadRequest("courseId is required");
        var sessions = await uow.ClassSessions.Query().Where(s => s.CourseId == courseId).ToListAsync(ct);
        if (sessions.Count == 0) return NotFound("Course or sessions not found");
        var sessionIds = sessions.Select(s => s.Id).ToArray();
        var totalAttendances = await uow.Attendances.Query().CountAsync(a => sessionIds.Contains(a.ClassSessionId), ct);
        var totalSessions = sessions.Count;
        // percent ~ average attendance per session (simplified)
        var percent = totalSessions == 0 ? 0 : Math.Round((double)totalAttendances / totalSessions * 100.0, 2);
        return Ok(new CourseAttendanceReport(courseId, totalSessions, totalAttendances, percent));
    }
}

