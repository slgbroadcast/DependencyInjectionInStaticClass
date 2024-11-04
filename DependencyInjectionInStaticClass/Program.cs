using DependencyInjectionInStaticClass.Components;
using DependencyInjectionInStaticClass.Services;

namespace DependencyInjectionInStaticClass;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
               .AddInteractiveServerComponents();

        // *********************
        // Register the services
        // *********************
        builder.Services.AddSingleton<SingletonService>();
        builder.Services.AddScoped<ScopedService>();
        builder.Services.AddTransient<TransientService>();
        builder.Services.AddScoped<NestedService>();

        WebApplication app = builder.Build();

        // *******************************************
        // We have to set the built application here,
        // so we can use it later in static classes.
        // *******************************************
        ApplicationServiceProvider.SetApplication(app);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode();

        app.Run();
    }
}