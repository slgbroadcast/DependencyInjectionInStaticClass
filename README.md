# Dependency Injection in a Static Class

This project demonstrates how dependency injection can be effectively used in a`static` class.  
Static classes inherently lack dependency injection support, so using a mechanism like
`IServiceScope` is essential to ensure that service lifetimes are correctly maintained as per the services registered scopes.

When working with `scoped` or `transient` services, using
`IServiceScope` ensures proper disposal of these services after they are used,
preventing resource leaks and preserving intended lifetimes.

## Application Service Provider

To enable dependency injection within a static context, the
`ApplicationServiceProvider` class ([ApplicationServiceProvider.cs](DependencyInjectionInStaticClass/ApplicationServiceProvider.cs)) is created.  
This static service provider holds the application's host instance and provides methods for retrieving services with proper scope management.  
The class offers two key methods:

```csharp
public static class ApplicationServiceProvider
{
    private static  IHost?      ApplicationHost;
    public  static  void        SetApplication(IHost applicationHost);
    public  static  IDisposable GetServiceProvider(out IServiceProvider serviceProvider);
    public  static  IDisposable GetRequiredService<T>(out T service);
}
```

### Ensuring Correct Service Lifetime Management

Since `Scoped` and `Transient` services need to be disposed of after usage, it’s necessary to create a new
`ServiceScope` each time services are accessed within the static class.  
This can be achieved by calling `ApplicationHost.Services.CreateScope()`.
Using `ServiceProvider` from this `serviceScope`,
we can access services while guaranteeing that they are disposed of correctly once their usage is complete.

Example:

```csharp
using (IServiceScope serviceScope = ApplicationHost.Services.CreateScope())
{
    var serviceProvider = serviceScope.ServiceProvider;
    return serviceProvider.GetRequiredService<NestedService>();
}
```

### Simplified Service Access with Disposal Safety

To streamline access to services while ensuring that the `ServiceScope` is disposed of properly,
`ApplicationServiceProvider` provides a `GetRequiredService` method. This method uses an
`out` parameter for the service instance and returns an `IDisposable` scope that must be disposed of after usage.

Example usage:

```csharp
using (ApplicationServiceProvider.GetRequiredService(out NestedService nestedService))
{
    nestedService.MakeSound();
}
```

This pattern ensures that all resources are managed correctly while making service access straightforward and reducing boilerplate code.