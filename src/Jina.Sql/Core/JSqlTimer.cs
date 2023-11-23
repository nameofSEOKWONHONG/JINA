namespace JSqlEngine.Core;

/// <summary>
/// 싱글톤으로 동작해야 함.
/// </summary>
internal class JSqlTimer : IDisposable
{
    private readonly JSqlReader _jSqlReader;
    private Timer _timer;
    private bool _isWorking = false;
    private static object _sync = new object();

    public string this[string name] => _jSqlReader.GetJSql(name);
    
#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
    public JSqlTimer(JSqlReader jSqlReader)
#pragma warning restore CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
    {
        _jSqlReader = jSqlReader;
    }

    public void Initialize()
    {
#pragma warning disable CS8622 // 매개 변수 형식에서 참조 형식의 Null 허용 여부가 대상 대리자와 일치하지 않습니다(Null 허용 여부 특성 때문일 수 있음).
        _timer = new Timer(TimerOnElapsed, null, 0, (1000 * 3));
#pragma warning restore CS8622 // 매개 변수 형식에서 참조 형식의 Null 허용 여부가 대상 대리자와 일치하지 않습니다(Null 허용 여부 특성 때문일 수 있음).
    }

    private void TimerOnElapsed(object state)
    {
        if (_isWorking == false)
        {
            _isWorking = true;
            lock (_sync)
            {
                _jSqlReader.SequentialRead();
            }
            _isWorking = false;
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}