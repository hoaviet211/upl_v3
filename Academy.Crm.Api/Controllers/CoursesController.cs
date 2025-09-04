using Academy.Crm.Api.DTOs;
using Academy.Crm.Api.Utils;
using Academy.Crm.Core.Entities;
using Academy.Crm.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academy.Crm.Api.Controllers;

[ApiController]
[Route("courses")]
[Authorize]
public class CoursesController(IUnitOfWork uow, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<CourseDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sort = null, [FromQuery] string? q = null)
    {
        var query = uow.Courses.Query();
        query = query.ApplyFilters(Request.Query);
        query = query.ApplySearch(q, nameof(Course.CourseCode), nameof(Course.CourseName));
        query = query.ApplySort(sort);
        var (paged, total) = query.ApplyPaging(page, pageSize);
        var items = await paged.ProjectTo<CourseDto>(mapper.ConfigurationProvider).ToListAsync();
        return Ok(new PagedResult<CourseDto>(items, new PagingDto(page, Math.Min(pageSize, 100), total)));
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CourseDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.CourseCode) || string.IsNullOrWhiteSpace(dto.CourseName))
            return UnprocessableEntity("Missing required fields");

        var entity = mapper.Map<Course>(dto);
        await uow.Courses.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        var result = mapper.Map<CourseDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDto>> GetById([FromRoute] int id, CancellationToken ct)
    {
        var entity = await uow.Courses.Query().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return NotFound();
        return Ok(mapper.Map<CourseDto>(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CourseDto>> Update([FromRoute] int id, [FromBody] CourseDto dto, CancellationToken ct)
    {
        var entity = await uow.Courses.Query().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return NotFound();
        entity.CourseCode = dto.CourseCode;
        entity.CourseName = dto.CourseName;
        entity.ProgrammeId = dto.ProgrammeId;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        uow.Courses.Update(entity);
        await uow.SaveChangesAsync(ct);
        return Ok(mapper.Map<CourseDto>(entity));
    }
}

