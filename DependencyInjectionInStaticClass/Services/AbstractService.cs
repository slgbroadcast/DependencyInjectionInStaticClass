namespace DependencyInjectionInStaticClass.Services;

public abstract class AbstractService : IDisposable
{
    protected AbstractService()
    {
        Console.Out.WriteLine($"{GetType().Name}: initialized");
    }

    public virtual void MakeSound()
    {
        Console.Out.WriteLine($"{GetType().Name}: Wuff");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Console.Out.WriteLine($"{GetType().Name}: Disposed");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}