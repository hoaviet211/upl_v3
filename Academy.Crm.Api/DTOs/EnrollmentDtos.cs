using Academy.Crm.Core.Enums;

namespace Academy.Crm.Api.DTOs;

public record EnrollmentDto
{
    public int Id { get; init; }
    public int StudentId { get; init; }
    public int CourseId { get; init; }
    public EnrollmentStatus? Status { get; init; }
    public DateTime? RegisteredAt { get; init; }
    public PaymentStatus? PaymentStatus { get; init; }
    public decimal? TuitionFee { get; init; }
    public decimal? Discount { get; init; }
    public string? Note { get; init; }
}

