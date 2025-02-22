namespace EzPzDi;

[AttributeUsage(AttributeTargets.Class)]
public class AddScopedAttribute : Attribute
{
    public Type[] ServiceTypes { get; init; }
    public string? StaticFactoryMethod { get; set; }

    public AddScopedAttribute()
    {
        ServiceTypes = Array.Empty<Type>();
    }

    public AddScopedAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes;
    }
};
