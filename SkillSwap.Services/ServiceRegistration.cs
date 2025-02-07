using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SkillSwap.Services.Services;
using System.Reflection;

namespace SkillSwap.Services;

public static class ServiceRegistration
{
    public static void AddCustomServiceDependencyRegister(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceAssemblies = new[]
        {
            typeof(UsersService),
            typeof(SkillsService),
            typeof(MentorshipRequestsService),
            typeof(SessionsService),
            typeof(ReviewsService),
            typeof(PaymentsService),
        };

        foreach (var serviceType in serviceAssemblies)
        {
            RegisterServicesFromAssembly(services, serviceType.Assembly);
        }
    }

    private static void RegisterServicesFromAssembly(IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes.Where(p =>
                    p.Name != null &&
                    p.Name.EndsWith("Service") &&
                    !p.IsInterface))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    }
}
