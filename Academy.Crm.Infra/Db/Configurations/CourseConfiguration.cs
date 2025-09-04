using Academy.Crm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Crm.Infra.Db.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CourseCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CourseName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.StartDate).HasColumnType("date");
        builder.Property(x => x.EndDate).HasColumnType("date");
        builder.HasIndex(x => x.ProgrammeId);
        builder.HasIndex(x => x.CourseCode).IsUnique(false);

        builder.HasOne(x => x.Programme)
            .WithMany(p => p.Courses)
            .HasForeignKey(x => x.ProgrammeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

