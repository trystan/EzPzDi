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

## Only scan specific assemblies

If you have multiple entrypoints and multiple DI containers, then you may
only want to load the relevant services. If so, then just specify those
assemblies.

You can have this in your Lambda project:

```csharp
var sc = new ServiceCollection()
    .AddEzPzDi(c =>
    {
        c.FromAssemblies = new[]
        { 
            Assembly.GetAssembly(typeof(SomeCommonType)),
            Assembly.GetAssembly(typeof(LambdaEntryPoint))
        };
    });
```

And have this in your Rest API project:

```csharp
var sc = new ServiceCollection()
    .AddEzPzDi(c =>
    {
        c.FromAssemblies = new[]
        { 
            Assembly.GetAssembly(typeof(SomeCommonType)),
            Assembly.GetAssembly(typeof(Setup))
        };
    });
```

By default, EzPzDi will scan every assembly that has been loaded. You may need
to reference a type from an assembly to ensure it's loaded.

# TODO

* Support implementation factories: `.AddSingleton(sp => ...)`
* Maybe a simple linter? Eg, warn if a Singleton depends on a Transient.