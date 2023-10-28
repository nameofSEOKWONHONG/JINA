using Jina.Base.Service.Abstract;

namespace Jina.Base.Service;

public static class ServiceLoaderExtensions
{
    public static ServiceLoader<TService, TRequest, TResult> Invoke<TService, TRequest, TResult>(this TService service)
        where TService : IServiceImplBase<TRequest, TResult>
    {
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        return new ServiceLoader<TService, TRequest, TResult>(service);
    }
}