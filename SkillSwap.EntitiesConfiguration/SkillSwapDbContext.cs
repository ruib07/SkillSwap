using SkillSwap.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace SkillSwap.EntitiesConfiguration;

public class SkillSwapDbContext : DbContext
{
    public SkillSwapDbContext(DbContextOptions<SkillSwapDbContext> options) : base(options) { }

    public DbSet<Users> Users { get; set; }
    public DbSet<PasswordResetToken> PasswordResetsToken { get; set; }
    public DbSet<Skills> Skills { get; set; }
    public DbSet<MentorshipRequests> MentorshipRequests { get; set; }
    public DbSet<Sessions> Sessions { get; set; }
    public DbSet<Reviews> Reviews { get; set; }
    public DbSet<Payments> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
