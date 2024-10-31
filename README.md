# Dependency injection in a static class

This is an example project which shows how dependency injection can be used in a `static` class.

To ensure that the [lifetime](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)
of the services continues to work the same way as the services were registered,
it is important that if a service is used in a static class, this is done via a `IServiceScope`.  
The `IServiceScope` guaranties that the services will be correctly disposed if it is required.

To achieve these, we create a
`ApplicationServiceProvider` ([ApplicationServiceProvider.cs](DependencyInjectionInStaticClass/ApplicationServiceProvider.cs))
class which holds the application.
From this application we can get the services.

```csharp asd
public static class ApplicationServiceProvider
{
    private static  IHost?      ApplicationHost;
    public  static  void        SetApplication(IHost applicationHost);
    public  static  IDisposable GetServiceProvider(out IServiceProvider serviceProvider);
    public  static  IDisposable GetRequiredService<T>(out T service);
}
```

Since we have to respect the lifetime of services, we create a new `ServiceScope` and use the
`ServiceProvider` from the `serviceScope`.  
Now we have access to all the services.
It is important, that `Scoped` and `Transient` services are disposed correctly after using.  
To achieve this, we have to dispose the `serviceScope`.

```csharp
using (IServiceScope serviceScope = ApplicationHost.Services.CreateScope())
{
    var serviceProvider = serviceScope.ServiceProvider;
    return serviceProvider.GetRequiredService<NestedService>();
}
```

To get simple and fast access to the services and also remember to dispose the `ServiceScope`,
we create a function (`GetRequiredService`) that returns `IDisposable` and has the services as an `out` parameter.
Example:

```csharp
using (ApplicationServiceProvider.GetRequiredService(out NestedService nestedService))
{
    nestedService.MakeSound();
}
```