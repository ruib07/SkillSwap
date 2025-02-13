using SkillSwap.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;

namespace SkillSwap.Server.Configurations;

public static class DatabaseConfig
{
    public static void AddCustomDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SkillSwapDb");

        services.AddDbContext<SkillSwapDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                var assembly = typeof(SkillSwapDbContext).Assembly;
                var assemblyName = assembly.GetName();

                sqlServerOptions.MigrationsAssembly(assemblyName.Name);
                sqlServerOptions.EnableRetryOnFailure(
                                 maxRetryCount: 2,
                                 maxRetryDelay: TimeSpan.FromSeconds(30),
                                 errorNumbersToAdd: null);
            });
        });
    }
}
