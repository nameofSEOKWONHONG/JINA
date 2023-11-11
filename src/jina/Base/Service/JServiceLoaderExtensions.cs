using Jina.Base.Service.Abstract;

namespace Jina.Base.Service;

// public static class JServiceLoaderExtensions
// {
//     public static JServiceLoader<TService, TRequest, TResult> Invoke<TService, TRequest, TResult>(this TService service)
//         where TService : IServiceImplBase<TRequest, TResult>
//     {
//         // ReSharper disable once HeapView.PossibleBoxingAllocation
//         return new JServiceLoader<TService, TRequest, TResult>(service);
//     }
// }

public class JServiceInvoker<TRequest, TResult>
{
    private JServiceInvoker()
    {
    }

    public static JServiceLoader<TRequest, TResult> Invoke(IServiceImplBase<TRequest, TResult> service)
    {
        return new JServiceLoader<TRequest, TResult>(service);
    }
}