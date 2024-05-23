namespace Jina.Base.Service.Abstract;

public interface IServiceLoaderBase
{
    IServiceImplBase Self { get; }
    Task ExecuteCore();
}