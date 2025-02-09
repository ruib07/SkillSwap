using SkillSwap.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkillSwap.EntitiesConfiguration.Configurations;

public class PaymentsConfig : IEntityTypeConfiguration<Payments>
{
    public void Configure(EntityTypeBuilder<Payments> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.PayerId).IsRequired();
        builder.Property(p => p.MentorId).IsRequired();
        builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(10, 2)");
        builder.Property(p => p.Status).IsRequired();

        builder.HasOne(p => p.Payer)
               .WithMany()
               .HasForeignKey(p => p.PayerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Mentor)
               .WithMany()
               .HasForeignKey(p => p.MentorId)
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
