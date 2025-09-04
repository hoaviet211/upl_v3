namespace Academy.Crm.Api.DTOs;

public record CourseDto
{
    public int Id { get; init; }
    public string CourseCode { get; init; } = string.Empty;
    public string CourseName { get; init; } = string.Empty;
    public int ProgrammeId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
}

