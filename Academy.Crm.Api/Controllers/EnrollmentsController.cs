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
[Route("")]
[Authorize]
public class EnrollmentsController(IUnitOfWork uow, IMapper mapper) : ControllerBase
{
    [HttpGet("enrollments")]
    public async Task<ActionResult<PagedResult<EnrollmentDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sort = null)
    {
        var query = uow.Enrollments.Query();
        query = query.ApplyFilters(Request.Query).ApplySort(sort);
        var (paged, total) = query.ApplyPaging(page, pageSize);
        var items = await paged.ProjectTo<EnrollmentDto>(mapper.ConfigurationProvider).ToListAsync();
        return Ok(new PagedResult<EnrollmentDto>(items, new PagingDto(page, Math.Min(pageSize, 100), total)));
    }

    // Admin create enrollment
    [HttpPost("enrollments")]
    public async Task<ActionResult<EnrollmentDto>> Create([FromBody] EnrollmentDto dto, CancellationToken ct)
    {
        if (await uow.Enrollments.AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId, ct))
            return Conflict("Enrollment already exists for this student and course");

        var entity = mapper.Map<Enrollment>(dto);
        entity.RegisteredAt ??= DateTime.UtcNow;
        entity.Status ??= EnrollmentStatus.Pending;
        await uow.Enrollments.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, mapper.Map<EnrollmentDto>(entity));
    }

    [HttpGet("enrollments/{id:int}")]
    public async Task<ActionResult<EnrollmentDto>> Get([FromRoute] int id, CancellationToken ct)
    {
        var entity = await uow.Enrollments.Query().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return NotFound();
        return Ok(mapper.Map<EnrollmentDto>(entity));
    }

    // Student self-enroll
    [HttpPost("courses/{courseId:int}/enroll")]
    public async Task<ActionResult<EnrollmentDto>> SelfEnroll([FromRoute] int courseId, CancellationToken ct)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "studentId");
        if (claim == null || !int.TryParse(claim.Value, out var studentId)) return Forbid();

        if (await uow.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct))
            return Conflict("Already enrolled");

        var entity = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            Status = EnrollmentStatus.Pending,
            RegisteredAt = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Unpaid
        };
        await uow.Enrollments.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Ok(mapper.Map<EnrollmentDto>(entity));
    }
}

