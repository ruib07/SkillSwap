using SkillSwap.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkillSwap.EntitiesConfiguration.Configurations;

public class ReviewsConfig : IEntityTypeConfiguration<Reviews>
{
    public void Configure(EntityTypeBuilder<Reviews> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.SessionId).IsRequired();
        builder.Property(p => p.ReviewerId).IsRequired();
        builder.Property(p => p.Rating).IsRequired().HasDefaultValue(1);
        builder.Property(p => p.Comments).HasColumnType("varchar(MAX)");

        builder.HasOne(p => p.Session)
               .WithMany()
               .HasForeignKey(p => p.SessionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Reviewer)
               .WithMany()
               .HasForeignKey(p => p.ReviewerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()")
               .ValueGeneratedOnAdd()
               .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
