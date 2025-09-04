using Academy.Crm.Core.Enums;

namespace Academy.Crm.Core.Entities;

public class ClassSession : BaseEntity
{
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public ClassSessionMode? Mode { get; set; }
    public string? MeetingUrl { get; set; }
    public bool? IsLockedAttendance { get; set; }
    public string? LockNote { get; set; }
    public ClassSessionStatus? Status { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}

