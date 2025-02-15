using SkillSwap.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkillSwap.EntitiesConfiguration.Configurations;

public class UsersConfig : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(60);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(60);
        builder.Property(p => p.Password).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Bio).HasColumnType("varchar(MAX)");
        builder.Property(p => p.ProfilePicture).HasMaxLength(255);
        builder.Property(p => p.Balance).HasColumnType("decimal(10, 2)");
        builder.Property(p => p.IsMentor).IsRequired().HasDefaultValue(false);

        builder.Property(p => p.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()")
               .ValueGeneratedOnAdd()
               .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(p => p.UpdatedAt)
               .HasDefaultValueSql("GETUTCDATE()")
               .ValueGeneratedOnAddOrUpdate();

        builder.HasMany(p => p.Skills)
               .WithMany(p => p.Users)
               .UsingEntity(p => p.ToTable("UserSkills"));
    }
}
