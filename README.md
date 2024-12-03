# Dependency Injection in a Static Class

This project demonstrates how dependency injection can be effectively used in a`static` class.  
Static classes inherently lack dependency injection support, so using a mechanism like
`IServiceScope` is essential to ensure that service lifetimes are correctly maintained as per the services registered scopes.

When working with `scoped` or `transient` services, using
`IServiceScope` ensures proper disposal of these services after they are used,
preventing resource leaks and preserving intended lifetimes.

## TL;DR

* **Copy** the `ApplicationServiceProvider` [class](DependencyInjectionInStaticClass/ApplicationServiceProvider.cs) into your project.
* **[Set the application](#set-the-application-service-provider)** by calling `ApplicationServiceProvider.SetApplication(app)` after building your application.
* **Get a service** using `ApplicationServiceProvider.GetRequiredService<T>(out T service)` with proper disposal.

## Application Service Provider

To enable dependency injection within a static context, the
`ApplicationServiceProvider` [class](DependencyInjectionInStaticClass/ApplicationServiceProvider.cs) is created.  
This static service provider holds the application's host instance and provides methods for retrieving services with proper scope management.

Preview:

```csharp
public static class ApplicationServiceProvider
{
    private static  IHost?      ApplicationHost;
    public  static  void        SetApplication(IHost applicationHost);
    public  static  IDisposable GetServiceProvider(out IServiceProvider serviceProvider);
    public  static  IDisposable GetRequiredService<T>(out T service);
}
```

### Set the Application Service Provider

Before using the `ApplicationServiceProvider` for accessing services, it is necessary to set the application host.  
This is achieved by calling `ApplicationServiceProvider.SetApplication()`
and passing the built application (e.g., an `IHost` or `WebApplication`) as a parameter.  
This step ensures that the static service provider has access to the application's dependency injection container.

Ensure this is done **after creating the application from the builder**
but **before accessing any services through the** `ApplicationServiceProvider`.

Example usage in `Program.cs`:

```csharp
// Create the application (WebApplication)
WebApplication app = builder.Build();

// Set the application host for the service provider
ApplicationServiceProvider.SetApplication(app);
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
    return serviceScope.ServiceProvider.GetRequiredService<NestedService>();
}
```

### Simplified Service Access with Disposal Safety

To streamline access to services while ensuring that the `ServiceScope` is disposed of properly,
`ApplicationServiceProvider` provides a `GetRequiredService` method. This method uses an
`out` parameter for the service instance and returns an `IDisposable` scope that must be disposed of after usage.

GetRequiredService:

```csharp
public static IDisposable GetRequiredService<T>(out T service)
    where T : notnull
{
    if (ApplicationHost is null)
    {
        throw new InvalidOperationException($"{nameof(ApplicationHost)} is not set. Call {nameof(SetApplication)}() first.");
    }
    IDisposable serviceScope = GetServiceProvider(out IServiceProvider serviceProvider);
    service = serviceProvider.GetRequiredService<T>();
    return serviceScope;
}
```

Example usage:

```csharp
using (ApplicationServiceProvider.GetRequiredService(out NestedService nestedService))
{
    nestedService.MakeSound();
}
```

This pattern ensures that all resources are managed correctly while making service access straightforward and reducing boilerplate code.

### Full Implementation

The full implementation of `ApplicationServiceProvider` can be found here:
[ApplicationServiceProvider.cs](DependencyInjectionInStaticClass/ApplicationServiceProvider.cs).

You can start this Blazor project and view the dependency injection setup in action on the `Static Injection` page,
which demonstrates the injection process using [StaticClass.cs](DependencyInjectionInStaticClass/StaticClass.cs).