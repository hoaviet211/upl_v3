namespace Academy.Crm.Core.Entities;

public class Programme : BaseEntity
{
    public string ProgrammeName { get; set; } = string.Empty;
    public string? AdmissionCycle { get; set; }
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}

