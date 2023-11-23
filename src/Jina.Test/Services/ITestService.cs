using Jina.Base.Service.Abstract;

namespace Jina.Test.Services;

public interface ITestService
    : IServiceImplBase<Request, string>
        , IScopeService
{
}