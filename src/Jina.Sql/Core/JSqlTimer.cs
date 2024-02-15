using eXtensionSharp;

namespace JSqlEngine.Core;

/// <summary>
/// 싱글톤으로 동작해야 함.
/// </summary>
internal class JSqlTimer(JSqlReader jSqlReader) : IDisposable
{
    private readonly JSqlReader _jSqlReader = jSqlReader;
    private Timer _timer;
    private bool _isWorking = false;
    private static object _sync = new object();

    public string this[string name] => _jSqlReader.GetJSql(name);

    public void Initialize()
    {
        _timer = new Timer(TimerOnElapsed, null, 0, (1000 * 3));
    }

    private void TimerOnElapsed(object state)
    {
        if (_isWorking == false)
        {
            _isWorking = true;
            lock (_sync)
            {
                _jSqlReader.Read();
            }
            _isWorking = false;
        }
    }

    public void Dispose()
    {
        if(_timer.xIsNotEmpty())
        {
            _timer.Dispose();
        }
    }
}