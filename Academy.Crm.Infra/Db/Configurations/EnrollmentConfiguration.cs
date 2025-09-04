using Academy.Crm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Crm.Infra.Db.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.CourseId);
        builder.HasIndex(x => new { x.StudentId, x.CourseId }).IsUnique();

        builder.Property(x => x.TuitionFee).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Discount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Note).HasMaxLength(1000);

        builder.HasOne(x => x.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

