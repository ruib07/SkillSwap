using SkillSwap.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SkillSwap.EntitiesConfiguration.Configurations;

public class MentorshipRequestsConfig : IEntityTypeConfiguration<MentorshipRequests>
{
    public void Configure(EntityTypeBuilder<MentorshipRequests> builder)
    {
        builder.ToTable("MentorshipRequests");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.MentorId).IsRequired();
        builder.Property(p => p.LearnerId).IsRequired();
        builder.Property(p => p.SkillId).IsRequired();
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.ScheduledAt).IsRequired();

        builder.HasOne(p => p.Mentor)
              .WithMany()
              .HasForeignKey(p => p.MentorId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Learner)
              .WithMany()
              .HasForeignKey(p => p.LearnerId)
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
