using Academy.Crm.Core.Enums;

namespace Academy.Crm.Core.Entities;

public class Enrollment : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public EnrollmentStatus? Status { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public decimal? TuitionFee { get; set; }
    public decimal? Discount { get; set; }
    public string? Note { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}

