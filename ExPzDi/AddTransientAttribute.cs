namespace EzPzDi;

[AttributeUsage(AttributeTargets.Class)]
public class AddTransientAttribute : Attribute
{
    public Type[] ServiceTypes { get; init; }

    public AddTransientAttribute()
    {
        ServiceTypes = Array.Empty<Type>();
    }

    public AddTransientAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes;
    }
};
