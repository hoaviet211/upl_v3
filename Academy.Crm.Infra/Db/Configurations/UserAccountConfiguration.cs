using Academy.Crm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Crm.Infra.Db.Configurations;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("UserAccounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserName).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.UserName).IsUnique();
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Role).IsRequired().HasMaxLength(50);

        builder.HasOne(x => x.Student)
            .WithOne()
            .HasForeignKey<UserAccount>(x => x.StudentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

