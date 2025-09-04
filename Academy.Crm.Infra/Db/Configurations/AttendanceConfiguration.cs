using Academy.Crm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Crm.Infra.Db.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.EnrollmentId);
        builder.HasIndex(x => x.ClassSessionId);
        builder.HasIndex(x => new { x.EnrollmentId, x.ClassSessionId }).IsUnique();
        builder.Property(x => x.Note).HasMaxLength(500);

        builder.HasOne(x => x.Enrollment)
            .WithMany(e => e.Attendances)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ClassSession)
            .WithMany(s => s.Attendances)
            .HasForeignKey(x => x.ClassSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

