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

And that's it.

## DI a concrete implementation

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

## DI abstract implementations

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

## DI specific implementations

If you don't want any of that, then you can specify the services you want to
register and it won't be registered as the other interfaces.

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
