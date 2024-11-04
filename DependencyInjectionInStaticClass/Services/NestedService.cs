namespace DependencyInjectionInStaticClass.Services;

public class NestedService : AbstractService
{
    private readonly SingletonService _singletonService;
    private readonly ScopedService    _scopedService;
    private readonly TransientService _transientService;

    public NestedService(SingletonService singletonService, ScopedService scopedService, TransientService transientService)
    {
        _singletonService = singletonService;
        _scopedService    = scopedService;
        _transientService = transientService;
    }

    public override void MakeSound()
    {
        base.MakeSound();

        _singletonService.MakeSound();
        _scopedService.MakeSound();
        _transientService.MakeSound();
    }
}