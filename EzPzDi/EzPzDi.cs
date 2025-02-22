using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EzPzDi;

public class EzPzDiConfig
{
    public IEnumerable<Assembly> FromAssemblies { get; set; }
    public Func<Type, bool> TypeFilter { get; set; }

    public EzPzDiConfig()
    {
        FromAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    }
}

public static class EzPzDi
{
    public static IServiceCollection AddEzPzDi(this IServiceCollection services)
    {
        return AddEzPzDi(services, c => { });
    }

    public static IServiceCollection AddEzPzDi(this IServiceCollection services, Action<EzPzDiConfig> setupAction)
    {
        var configuration = new EzPzDiConfig();

        setupAction(configuration);

        return AddEzPzDi(services, configuration);
    }

    public static IServiceCollection AddEzPzDi(this IServiceCollection services, EzPzDiConfig configuration)
    {
        static bool KeepAll(Type type) { return true; }

        var typeFilter = configuration.TypeFilter ?? KeepAll;

        var types = configuration.FromAssemblies
            .SelectMany(a => a.GetTypes())
            .Distinct();

        foreach (var implementationType in types)
        {
            if (implementationType.GetCustomAttribute(typeof(AddTransientAttribute)) is AddTransientAttribute transient)
            {
                if (typeFilter(implementationType)) {
                    Add(services, transient.StaticFactoryMethod, transient.ServiceTypes, implementationType, ServiceLifetime.Transient);
                }
            }

            if (implementationType.GetCustomAttribute(typeof(AddSingletonAttribute)) is AddSingletonAttribute singleton)
            {
                if (typeFilter(implementationType))
                {
                    Add(services, singleton.StaticFactoryMethod, singleton.ServiceTypes, implementationType, ServiceLifetime.Singleton);
                }
            }

            if (implementationType.GetCustomAttribute(typeof(AddScopedAttribute)) is AddScopedAttribute scoped)
            {
                if (typeFilter(implementationType))
                {
                    Add(services, scoped.StaticFactoryMethod, scoped.ServiceTypes, implementationType, ServiceLifetime.Scoped);
                }
            }
        }

        return services;
    }

    private static void Add(IServiceCollection services, string? staticFactoryMethod, Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime)
    {
        if (staticFactoryMethod != null)
        {
            var factoryMethod = implementationType.GetMethod(staticFactoryMethod, BindingFlags.Static | BindingFlags.Public);
            var serviceType = factoryMethod.ReturnType;
            var factory = (IServiceProvider sp) => { return factoryMethod.Invoke(null, new[] { sp }); };
            services.Add(new ServiceDescriptor(serviceType, factory, ServiceLifetime.Scoped));
        }
        else if (serviceTypes.Any())
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
