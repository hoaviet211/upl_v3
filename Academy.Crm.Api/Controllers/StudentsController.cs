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
[Route("students")]
[Authorize]
public class StudentsController(IUnitOfWork uow, IMapper mapper, IConfiguration cfg) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<StudentDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sort = null, [FromQuery] string? q = null)
    {
        var query = uow.Students.Query();
        query = query.ApplySearch(q, nameof(Student.FirstName), nameof(Student.LastName), nameof(Student.StudentCode), nameof(Student.PhoneNumber));
        query = query.ApplyFilters(Request.Query);
        query = query.ApplySort(sort);
        var (paged, total) = query.ApplyPaging(page, pageSize);

        var items = await paged.ProjectTo<StudentDto>(mapper.ConfigurationProvider).ToListAsync();
        return Ok(new PagedResult<StudentDto>(items, new PagingDto(page, Math.Min(pageSize, 100), total)));
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create([FromBody] StudentDto dto, CancellationToken ct)
    {
        // minimal validation
        if (string.IsNullOrWhiteSpace(dto.StudentCode) || string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) || string.IsNullOrWhiteSpace(dto.PhoneNumber))
            return UnprocessableEntity("Missing required fields");

        if (await uow.Students.AnyAsync(x => x.StudentCode == dto.StudentCode, ct))
            return Conflict("StudentCode already exists");

        var entity = mapper.Map<Student>(dto);
        entity.FullName = dto.FullName ?? ($"{dto.LastName} {dto.FirstName}".Trim());
        await uow.Students.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        var result = mapper.Map<StudentDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetById([FromRoute] int id, CancellationToken ct)
    {
        var student = await uow.Students.Query().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (student == null) return NotFound();
        return Ok(mapper.Map<StudentDto>(student));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StudentDto>> Update([FromRoute] int id, [FromBody] StudentDto dto, CancellationToken ct)
    {
        var student = await uow.Students.Query().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (student == null) return NotFound();
        // map allowed fields
        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.FullName = dto.FullName ?? ($"{dto.LastName} {dto.FirstName}".Trim());
        student.NickName = dto.NickName;
        student.ShortInfo = dto.ShortInfo;
        student.PhoneNumber = dto.PhoneNumber;
        student.ZaloNumber = dto.ZaloNumber;
        student.YearOfBirth = dto.YearOfBirth;
        student.Birthday = dto.Birthday;
        student.Email = dto.Email;
        uow.Students.Update(student);
        await uow.SaveChangesAsync(ct);
        return Ok(mapper.Map<StudentDto>(student));
    }

    [HttpGet("me")]
    public async Task<ActionResult<StudentDto>> Me(CancellationToken ct)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "studentId");
        if (claim == null) return Forbid();
        if (!int.TryParse(claim.Value, out var studentId)) return Forbid();
        var student = await uow.Students.Query().FirstOrDefaultAsync(x => x.Id == studentId, ct);
        if (student == null) return NotFound();
        return Ok(mapper.Map<StudentDto>(student));
    }
}

