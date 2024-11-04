using DependencyInjectionInStaticClass.Services;

namespace DependencyInjectionInStaticClass;

public static class StaticClass
{
    public static void UseSimpleServices()
    {
        Console.Out.WriteLine("Make sound with simple services:");

        // We use "GetServiceProvider" so we are able to use multiple services.
        using (ApplicationServiceProvider.GetServiceProvider(out IServiceProvider serviceProvider))
        {
            SingletonService singletonService = serviceProvider.GetRequiredService<SingletonService>();
            singletonService.MakeSound();

            ScopedService scopedService = serviceProvider.GetRequiredService<ScopedService>();
            scopedService.MakeSound();

            TransientService transientService = serviceProvider.GetRequiredService<TransientService>();
            transientService.MakeSound();
            Console.Out.WriteLine("Close scope");
        }

        Console.Out.WriteLine("--------------");
        Console.Out.WriteLine("");
    }

    public static void UseSimpleServicesMultipleTimes()
    {
        Console.Out.WriteLine("Make multiple sounds with simple services:");

        // We use "GetServiceProvider" so we are able to use multiple services.
        using (ApplicationServiceProvider.GetServiceProvider(out IServiceProvider serviceProvider))
        {
            Console.Out.WriteLine("Get services for the first time");
            SingletonService singletonService = serviceProvider.GetRequiredService<SingletonService>();
            ScopedService    scopedService    = serviceProvider.GetRequiredService<ScopedService>();
            TransientService transientService = serviceProvider.GetRequiredService<TransientService>();

            singletonService.MakeSound();
            scopedService.MakeSound();
            transientService.MakeSound();

            Console.Out.WriteLine("");
            Console.Out.WriteLine("Get services for the second time");
            singletonService = serviceProvider.GetRequiredService<SingletonService>();
            scopedService    = serviceProvider.GetRequiredService<ScopedService>();
            transientService = serviceProvider.GetRequiredService<TransientService>();

            singletonService.MakeSound();
            scopedService.MakeSound();
            transientService.MakeSound();

            Console.Out.WriteLine("");
            Console.Out.WriteLine("Get services for the third time");
            singletonService = serviceProvider.GetRequiredService<SingletonService>();
            scopedService    = serviceProvider.GetRequiredService<ScopedService>();
            transientService = serviceProvider.GetRequiredService<TransientService>();

            singletonService.MakeSound();
            scopedService.MakeSound();
            transientService.MakeSound();
            Console.Out.WriteLine("Close scope");
        }

        Console.Out.WriteLine("--------------");
        Console.Out.WriteLine("");
    }

    public static void UseNestedServices()
    {
        Console.Out.WriteLine("Make sound with the nested service:");

        // Since we only need 1 service here, we use "GetRequiredService".
        using (ApplicationServiceProvider.GetRequiredService(out NestedService nestedService))
        {
            nestedService.MakeSound();
            Console.Out.WriteLine("Close scope");
        }

        Console.Out.WriteLine("--------------");
        Console.Out.WriteLine("");
    }
}