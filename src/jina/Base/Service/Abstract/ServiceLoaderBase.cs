namespace Jina.Base.Service.Abstract;

public abstract class ServiceLoaderBase : DisposeBase
{
    public IServiceImplBase Self { get; init; }
    public abstract Task ExecuteCore();
}