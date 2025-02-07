using SkillSwap.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkillSwap.EntitiesConfiguration.Configurations;

public class SkillsConfig : IEntityTypeConfiguration<Skills>
{
    public void Configure(EntityTypeBuilder<Skills> builder)
    {
        builder.ToTable("Skills");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Description).IsRequired().HasColumnType("varchar(MAX)");

        builder.Property(p => p.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()")
               .ValueGeneratedOnAdd()
               .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(p => p.Name).IsUnique();
    }
}
