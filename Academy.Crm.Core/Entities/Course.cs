using System.ComponentModel.DataAnnotations.Schema;

namespace Academy.Crm.Core.Entities;

public class Course : BaseEntity
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int ProgrammeId { get; set; }
    public Programme? Programme { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public ICollection<ClassSession> Sessions { get; set; } = new List<ClassSession>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

