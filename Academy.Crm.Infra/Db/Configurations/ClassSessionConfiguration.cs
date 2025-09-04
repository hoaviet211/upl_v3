using Academy.Crm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Crm.Infra.Db.Configurations;

public class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    public void Configure(EntityTypeBuilder<ClassSession> builder)
    {
        builder.ToTable("ClassSessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SessionCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => x.CourseId);

        builder.HasOne(x => x.Course)
            .WithMany(c => c.Sessions)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

