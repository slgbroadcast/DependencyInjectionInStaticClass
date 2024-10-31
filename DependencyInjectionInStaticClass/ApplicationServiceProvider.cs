using JetBrains.Annotations;

namespace DependencyInjectionInStaticClass;

/// <summary>
///     A static service provider that holds the current running application.
/// </summary>
public static class ApplicationServiceProvider
{
    private static IHost? ApplicationHost;

    /// <summary>
    ///     Sets the application host from which services can be used.
    /// </summary>
    /// <param name="applicationHost">The application host instance.</param>
    /// <remarks>
    ///     You get the host from the <see cref="IHostApplicationBuilder" /> after build.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="applicationHost" /> is <c>null</c></exception>
    public static void SetApplication(IHost applicationHost)
    {
        ArgumentNullException.ThrowIfNull(applicationHost);

        ApplicationHost = applicationHost;
    }

    /// <summary>
    ///     Offers a scoped <see cref="IServiceProvider" />.<br />
    ///     Returns an <see cref="IDisposable" /> scope which must be disposed to close the scope and release the service resources.
    /// </summary>
    /// <remarks>
    ///     If only one service is required, use <see cref="GetRequiredService{T}" />.
    /// </remarks>
    /// <param name="serviceProvider"><b>out</b> The <see cref="IServiceProvider" /> from the <see cref="ApplicationHost" />.</param>
    /// <returns>The <see cref="IServiceScope" /> that <b>must</b> be disposed!</returns>
    /// <exception cref="InvalidOperationException">The <see cref="ApplicationHost" /> is not set.</exception>
    /// <example>
    ///     <code>
    /// using (ApplicationServiceProvider.GetServiceProvider(out IServiceProvider serviceProvider))
    /// {
    ///     SingletonService singletonService = serviceProvider.GetRequiredService&lt;SingletonService&gt;();
    ///     ScopedService    scopedService    = serviceProvider.GetRequiredService&lt;ScopedService&gt;();
    ///     TransientService transientService = serviceProvider.GetRequiredService&lt;TransientService&gt;();
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="GetRequiredService{T}" />
    [MustDisposeResource]
    public static IDisposable GetServiceProvider(out IServiceProvider serviceProvider)
    {
        if (ApplicationHost is null)
        {
            throw new InvalidOperationException($"{nameof(ApplicationHost)} is not set. Call {nameof(SetApplication)}() first.");
        }

        IServiceScope serviceScope = ApplicationHost.Services.CreateScope();

        serviceProvider = serviceScope.ServiceProvider;
        return serviceScope;
    }

    /// <summary>
    ///     Offers an instance of a required service <typeparamref name="T" />.<br />
    ///     This method returns a scoped <see cref="IDisposable" />, which must be disposed to close the scope.
    /// </summary>
    /// <remarks>
    ///     If multiple services are required, use <see cref="GetServiceProvider" />.
    /// </remarks>
    /// <param name="service"><b>out</b> The required service.</param>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>The <see cref="IServiceScope" /> that <b>must</b> be disposed!</returns>
    /// <exception cref="InvalidOperationException">The <see cref="ServiceProvider" /> is not set.</exception>
    /// <example>
    ///     <code>
    /// using (ApplicationServiceProvider.GetRequiredService(out MyService service))
    /// {
    ///     service.MakeSound();
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="GetServiceProvider"/>
    [MustDisposeResource]
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
}