using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Entities.Entities;

namespace SkillSwap.EntitiesConfiguration.Configurations;

public class UserSkillsConfig : IEntityTypeConfiguration<UserSkills>
{
    public void Configure(EntityTypeBuilder<UserSkills> builder)
    {
        builder.ToTable("UserSkills");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.SkillId).IsRequired();

        builder.Property(us => us.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()")
               .ValueGeneratedOnAdd();

        builder.Property(us => us.UpdatedAt)
               .HasDefaultValueSql("GETUTCDATE()")
               .ValueGeneratedOnAddOrUpdate();

        builder.HasOne(us => us.User)
               .WithMany(u => u.UserSkills)
               .HasForeignKey(us => us.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(us => us.Skill)
               .WithMany(s => s.UserSkills)
               .HasForeignKey(us => us.SkillId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
