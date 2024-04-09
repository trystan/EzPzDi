using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EzPzDi;

public static class EzPzDi
{
    public static IServiceCollection AddEzPzDi(this IServiceCollection services)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
        foreach (var implementationType in types)
        {
            AddEzPzTransients(services, implementationType);
            AddEzPzSingletons(services, implementationType);
        }
        return services;
    }

    private static void AddEzPzTransients(IServiceCollection services, Type implementationType)
    {
        if (implementationType.GetCustomAttribute(typeof(AddTransientAttribute)) is AddTransientAttribute transient)
        {
            if (transient.ServiceTypes.Any())
            {
                foreach (var serviceType in transient.ServiceTypes)
                {
                    services.AddTransient(serviceType, implementationType);
                }
            }
            else if (implementationType.GetInterfaces().Any())
            {
                foreach (var serviceType in implementationType.GetInterfaces())
                {
                    services.AddTransient(serviceType, implementationType);
                }
            }
            else
            {
                services.AddTransient(implementationType);
            }
        }
    }

    private static void AddEzPzSingletons(IServiceCollection services, Type implementationType)
    {
        if (implementationType.GetCustomAttribute(typeof(AddSingletonAttribute)) is AddSingletonAttribute transient)
        {
            if (transient.ServiceTypes.Any())
            {
                foreach (var serviceType in transient.ServiceTypes)
                {
                    services.AddSingleton(serviceType, implementationType);
                }
            }
            else if (implementationType.GetInterfaces().Any())
            {
                foreach (var serviceType in implementationType.GetInterfaces())
                {
                    services.AddSingleton(serviceType, implementationType);
                }
            }
            else
            {
                services.AddSingleton(implementationType);
            }
        }
    }
}
