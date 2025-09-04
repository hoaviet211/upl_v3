using Academy.Crm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Crm.Infra.Db.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StudentCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.StudentCode).IsUnique();

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.FullName).HasMaxLength(200);
        builder.Property(x => x.NickName).HasMaxLength(100);
        builder.Property(x => x.ShortInfo).HasMaxLength(500);
        builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(30);
        builder.Property(x => x.ZaloNumber).HasMaxLength(30);
        builder.Property(x => x.Birthday).HasColumnType("date");
        builder.Property(x => x.ImagePath).HasMaxLength(400);
        builder.Property(x => x.Email).HasMaxLength(200);

        builder.HasOne(x => x.IdCard)
            .WithOne(i => i.Student)
            .HasForeignKey<IdCard>(i => i.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

