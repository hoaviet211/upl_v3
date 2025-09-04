using Academy.Crm.Api.DTOs;
using Academy.Crm.Api.Utils;
using Academy.Crm.Core.Entities;
using Academy.Crm.Core.Enums;
using Academy.Crm.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academy.Crm.Api.Controllers;

[ApiController]
[Route("attendance")]
[Authorize]
public class AttendanceController(IUnitOfWork uow, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<AttendanceDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sort = null)
    {
        var query = uow.Attendances.Query();
        query = query.ApplyFilters(Request.Query).ApplySort(sort);
        var (paged, total) = query.ApplyPaging(page, pageSize);
        var items = await paged.ProjectTo<AttendanceDto>(mapper.ConfigurationProvider).ToListAsync();
        return Ok(new PagedResult<AttendanceDto>(items, new PagingDto(page, Math.Min(pageSize, 100), total)));
    }

    public record CheckInRequest(int EnrollmentId, int ClassSessionId, AttendanceStatus Status, AttendanceMethod? Method, string? Note);

    [HttpPost("checkin")]
    public async Task<ActionResult<AttendanceDto>> CheckIn([FromBody] CheckInRequest req, CancellationToken ct)
    {
        if (await uow.Attendances.AnyAsync(a => a.EnrollmentId == req.EnrollmentId && a.ClassSessionId == req.ClassSessionId, ct))
            return Conflict("Attendance already exists (EnrollmentId, ClassSessionId)");

        var attendance = new Attendance
        {
            EnrollmentId = req.EnrollmentId,
            ClassSessionId = req.ClassSessionId,
            Status = req.Status,
            Method = req.Method,
            CheckInTime = DateTime.UtcNow,
            Note = req.Note
        };
        await uow.Attendances.AddAsync(attendance, ct);
        await uow.SaveChangesAsync(ct);
        return Ok(mapper.Map<AttendanceDto>(attendance));
    }
}

