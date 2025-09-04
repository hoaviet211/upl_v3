using Academy.Crm.Core.Enums;

namespace Academy.Crm.Core.Entities;

public class Attendance : BaseEntity
{
    public int EnrollmentId { get; set; }
    public Enrollment? Enrollment { get; set; }
    public int ClassSessionId { get; set; }
    public ClassSession? ClassSession { get; set; }
    public AttendanceStatus Status { get; set; }
    public AttendanceMethod? Method { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string? Note { get; set; }
}

