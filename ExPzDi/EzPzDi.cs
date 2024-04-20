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
            if (implementationType.GetCustomAttribute(typeof(AddTransientAttribute)) is AddTransientAttribute transient)
            {
                Add(services, transient.ServiceTypes, implementationType, ServiceLifetime.Transient);
            }

            if (implementationType.GetCustomAttribute(typeof(AddSingletonAttribute)) is AddSingletonAttribute singleton)
            {
                Add(services, singleton.ServiceTypes, implementationType, ServiceLifetime.Singleton);
            }

            if (implementationType.GetCustomAttribute(typeof(AddScopedAttribute)) is AddScopedAttribute scoped)
            {
                Add(services, scoped.ServiceTypes, implementationType, ServiceLifetime.Scoped);
            }
        }
        return services;
    }

    private static void Add(IServiceCollection services, Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime)
    {
        if (serviceTypes.Any())
        {
            foreach (var serviceType in serviceTypes)
            {
                services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            }
        }
        else if (implementationType.GetInterfaces().Any())
        {
            foreach (var serviceType in implementationType.GetInterfaces())
            {
                services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            }
        }
        else
        {
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
        }
    }
}
