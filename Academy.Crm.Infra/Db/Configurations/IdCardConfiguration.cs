using Academy.Crm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Crm.Infra.Db.Configurations;

public class IdCardConfiguration : IEntityTypeConfiguration<IdCard>
{
    public void Configure(EntityTypeBuilder<IdCard> builder)
    {
        builder.ToTable("IdCards");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CardNumber).HasMaxLength(50);
        builder.Property(x => x.DateOfIssue).HasColumnType("date");
        builder.Property(x => x.PlaceOfIssue).HasMaxLength(100);

        builder.HasIndex(x => x.StudentId).IsUnique();
    }
}

