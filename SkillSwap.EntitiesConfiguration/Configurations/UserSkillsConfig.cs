using SkillSwap.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        builder.Property(p => p.Type).IsRequired().HasMaxLength(50);

        builder.HasOne(p => p.User)
               .WithMany()
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Skill)
               .WithMany()
               .HasForeignKey(p => p.SkillId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
