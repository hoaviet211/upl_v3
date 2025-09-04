namespace Academy.Crm.Core.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
}

