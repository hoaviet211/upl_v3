using Academy.Crm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Crm.Infra.Db.Configurations;

public class ProgrammeConfiguration : IEntityTypeConfiguration<Programme>
{
    public void Configure(EntityTypeBuilder<Programme> builder)
    {
        builder.ToTable("Programmes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProgrammeName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.AdmissionCycle).HasMaxLength(100);
        builder.HasIndex(x => x.ProgrammeName);
    }
}

