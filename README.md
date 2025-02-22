# EzPzDi

Tired of having dozens or hundreds of `using` statements in your `Startup.cs` just so you
can wire up your dependencies? Well then `EzPzDi` is for you.

Add the appropriate attribute to your class

```csharp
[AddTransient]
public class Example
{
}
```

And wire it up in your service collection

```csharp
var sc = new ServiceCollection()
    .AddEzPzDi();
```

And that's it. `[AddScoped]` and `[AddSingleton]` are also supported.

## Register as a concrete type

If your class doesn't implement any interfaces, then it's added to the service
collection as itself.

```csharp
[AddTransient]
public class Example
{
}
```

Which is equivalent to

```csharp
var sc = new ServiceCollection()
    .AddTransient<Example>();
```

## Register as all service types

If your class implements one or more interfaces, then it's added to the service
collection as an implementation of each one.

```csharp
[AddTransient]
public class Example : IExample, IOtherExample
{
}
```

Which is equivalent to

```csharp
var sc = new ServiceCollection()
    .AddTransient<IExample, Example>()
    .AddTransient<IOtherExample, Example>();
```

## Register as explicit service types

You can also specify the services you want to register and it won't be
registered as the other interfaces.

```csharp
[AddTransient(ServiceClasses = Type[]{ IOtherExample })]
public class Example : IExample, IOtherExample
{
}
```

Which, because this is the only specified servce type, is equivalent to

```csharp
var sc = new ServiceCollection()
    .AddTransient<IOtherExample, Example>();
```

## Register a factory

You can also specify a static factory method. If specified, then this is the
only thing that will get registered.

```csharp
[AddScoped(StaticFactoryMethod = nameof(Factory))]
public class ReportServiceFactory
{
    public static IReportService Factory(IServiceProvider serviceProvider)
    {
        return new ReportService();
    }
}
```

Which is equivalent to

```csharp
var sc = new ServiceCollection()
    .AddScoped<IReportService>(ReportServiceFactory.Factory);
```

## Only scan specific assemblies

If you have multiple entrypoints and multiple DI containers, then you may
only want to load the relevant services. If so, then just specify those
assemblies.

You can have this in your LambdaHost project:

```csharp
var sc = new ServiceCollection()
    .AddEzPzDi(c =>
    {
        c.FromAssemblies = new[]
        { 
            Assembly.Load("MyCompany.MyProject.Common"),
            Assembly.Load("MyCompany.MyProject.LambdaHost")
        };
    });
```

And have this in your WebApi project:

```csharp
var sc = new ServiceCollection()
    .AddEzPzDi(c =>
    {
        c.FromAssemblies = new[]
        { 
            Assembly.Load("MyCompany.MyProject.Common"),
            Assembly.Load("MyCompany.MyProject.WebApi")
        };
    });
```

By default, EzPzDi will scan every assembly that has been loaded. You may need
to reference a type from an assembly to ensure it's loaded.

## Filter types before they're registered

For the rare cases when you need it, you can also do custom filtering of each
type to determine what gets registered.

```csharp
var sc = new ServiceCollection()
    .AddEzPzDi(c =>
    {
        c.TypeFilter = (Type type) =>
        {
            return !type.Name.Contains("Local");
        };
    });
```
