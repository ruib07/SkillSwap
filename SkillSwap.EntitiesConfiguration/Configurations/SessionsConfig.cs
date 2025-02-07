using SkillSwap.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkillSwap.EntitiesConfiguration.Configurations;

public class SessionsConfig : IEntityTypeConfiguration<Sessions>
{
    public void Configure(EntityTypeBuilder<Sessions> builder)
    {
        builder.ToTable("Sessions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.MentorshipRequestId).IsRequired();
        builder.Property(p => p.SessionTime).IsRequired();
        builder.Property(p => p.Duration).IsRequired();
        builder.Property(p => p.VideoLink).HasMaxLength(255);

        builder.HasOne(p => p.MentorshipRequest)
               .WithMany()
               .HasForeignKey(p => p.MentorshipRequestId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()")
               .ValueGeneratedOnAdd()
               .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();
    }
}
