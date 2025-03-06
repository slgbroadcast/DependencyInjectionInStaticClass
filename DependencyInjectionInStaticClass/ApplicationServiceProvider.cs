namespace DependencyInjectionInStaticClass;

/// <summary>
///     A static service provider that holds the current used <see cref="IServiceProvider" />.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines#disposal-of-services" />
public static class ApplicationServiceProvider
{
    private static IServiceProvider? ServiceProvider;

    /// <summary>
    ///     Sets the <see cref="ServiceProvider" /> from the given <paramref name="applicationHost" />.
    /// </summary>
    /// <param name="applicationHost">The application host instance.</param>
    /// <remarks>
    ///     You get the host from the <see cref="IHostApplicationBuilder" /> after build.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="applicationHost" /> is <c>null</c>.</exception>
    public static void SetApplication(IHost applicationHost)
    {
        ArgumentNullException.ThrowIfNull(applicationHost);

        ServiceProvider = applicationHost.Services;
    }

    /// <summary>
    ///     Sets the <see cref="ServiceProvider" /> that will be used to get services from.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use.</param>
    /// <exception cref="ArgumentNullException"><paramref name="serviceProvider" /> is <c>null</c>.</exception>
    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        ServiceProvider = serviceProvider;
    }

    /// <summary>
    ///     Offers a scoped <see cref="IServiceProvider" />.<br />
    ///     Returns an <see cref="IDisposable" /> (<see cref="IServiceScope" />)
    ///     which must be disposed to close the scope and release the service resources.
    /// </summary>
    /// <remarks>
    ///     If only one service is required, use <see cref="GetRequiredService{T}" />.
    /// </remarks>
    /// <param name="serviceProvider"><b>out</b> The scoped <see cref="IServiceProvider" /> from the <see cref="ServiceProvider" />.</param>
    /// <returns>The <see cref="IServiceScope" /> that <b>must</b> be disposed!</returns>
    /// <exception cref="InvalidOperationException">The <see cref="ServiceProvider" /> is not set.</exception>
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
    public static IDisposable GetScopedServiceProvider(out IServiceProvider serviceProvider)
    {
        ThrowIfIsNull(ServiceProvider);

        IServiceScope serviceScope = ServiceProvider.CreateScope();

        serviceProvider = serviceScope.ServiceProvider;
        return serviceScope;
    }

    /// <summary>
    ///     Offers an instance of a required service <typeparamref name="T" />.<br />
    ///     Returns an <see cref="IDisposable" /> (<see cref="IServiceScope" />)
    ///     which must be disposed to close the scope and release the service resources.
    /// </summary>
    /// <remarks>
    ///     If multiple services are required, use <see cref="GetScopedServiceProvider" />.
    /// </remarks>
    /// <param name="service"><b>out</b> The required service.</param>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>The <see cref="IServiceScope" /> that <b>must</b> be disposed!</returns>
    /// <exception cref="InvalidOperationException">The <see cref="ServiceProvider" /> is not set.</exception>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T" />.</exception>
    /// <example>
    ///     <code>
    /// using (ApplicationServiceProvider.GetRequiredService(out MyService service))
    /// {
    ///     service.MakeSound();
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="GetScopedServiceProvider" />
    public static IDisposable GetRequiredService<T>(out T service)
        where T : notnull
    {
        ThrowIfIsNull(ServiceProvider);

        IDisposable serviceScope = GetScopedServiceProvider(out IServiceProvider serviceProvider);
        service = serviceProvider.GetRequiredService<T>();

        return serviceScope;
    }

    /// <summary>
    ///     Throws an <see cref="InvalidOperationException" />
    ///     if the given <paramref name="serviceProvider" /> is <c>null</c>.
    /// </summary>
    /// <param name="serviceProvider">The service provider to check.</param>
    /// <exception cref="InvalidOperationException"><paramref name="serviceProvider" /> is <c>null</c>.</exception>
    private static void ThrowIfIsNull([System.Diagnostics.CodeAnalysis.NotNull] IServiceProvider? serviceProvider)
    {
        if (serviceProvider is null)
        {
            throw new InvalidOperationException($"Unable to get service. {nameof(ServiceProvider)} is not set. " +
                                                $"Call '{nameof(ApplicationServiceProvider)}.{nameof(SetServiceProvider)}()' first.");
        }
    }
}