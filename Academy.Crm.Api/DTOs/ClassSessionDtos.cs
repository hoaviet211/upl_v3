using Academy.Crm.Core.Enums;

namespace Academy.Crm.Api.DTOs;

public record ClassSessionDto
{
    public int Id { get; init; }
    public int CourseId { get; init; }
    public string SessionCode { get; init; } = string.Empty;
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int? DurationMinutes { get; init; }
    public ClassSessionMode? Mode { get; init; }
    public string? MeetingUrl { get; init; }
    public bool? IsLockedAttendance { get; init; }
    public string? LockNote { get; init; }
    public ClassSessionStatus? Status { get; init; }
}

