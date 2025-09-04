using Academy.Crm.Core.Enums;

namespace Academy.Crm.Api.DTOs;

public record AttendanceDto
{
    public int Id { get; init; }
    public int EnrollmentId { get; init; }
    public int ClassSessionId { get; init; }
    public AttendanceStatus Status { get; init; }
    public AttendanceMethod? Method { get; init; }
    public DateTime? CheckInTime { get; init; }
    public string? Note { get; init; }
}

