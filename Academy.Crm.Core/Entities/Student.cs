namespace Academy.Crm.Core.Entities;

public class Student : BaseEntity
{
    public string StudentCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? NickName { get; set; }
    public string? ShortInfo { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ZaloNumber { get; set; }
    public int? YearOfBirth { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? ImagePath { get; set; }
    public string? Email { get; set; }

    public IdCard? IdCard { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

