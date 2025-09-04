namespace Academy.Crm.Api.DTOs;

public record ProgrammeDto
{
    public int Id { get; init; }
    public string ProgrammeName { get; init; } = string.Empty;
    public string? AdmissionCycle { get; init; }
}

