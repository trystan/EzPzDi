namespace EzPzDi;

[AttributeUsage(AttributeTargets.Class)]
public class AddSingletonAttribute : Attribute
{
    public Type[] ServiceTypes { get; init; }

    public AddSingletonAttribute()
    {
        ServiceTypes = Array.Empty<Type>();
    }

    public AddSingletonAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes;
    }
};
