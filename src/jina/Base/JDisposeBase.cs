namespace Jina.Base;

public class JDisposeBase : IDisposable
{
    private bool _disposed;
    
    ~JDisposeBase() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
        }

        _disposed = true;
    }
}