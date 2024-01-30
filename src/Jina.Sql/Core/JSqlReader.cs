using System.Collections.Concurrent;
using eXtensionSharp;
using JSqlEngine.Data;
using Microsoft.Extensions.Options;

namespace JSqlEngine.Core;

public class JSqlReader
{
    private readonly string _rootPath;
    public JSqlReader(IOptions<JSqlOption> options)
    {
        _rootPath = options.Value.Path;
    }

    private readonly ConcurrentDictionary<string, JSqlFileInfo> _jsqlFileInfos = new();

    public string GetJSql(string fileName)
    {
        if (_jsqlFileInfos.TryGetValue(fileName, out var info))
        {
            return info.Sql;
        }

        return string.Empty;
    }

    public void Initialize()
    {
        var jsqlFiles = Directory.GetFiles(_rootPath, "*.jsql", SearchOption.AllDirectories);
        var jsqlFileInfos = ReadFile(jsqlFiles);
        Parallel.ForEach(jsqlFileInfos, info =>
        {
            _jsqlFileInfos.TryAdd(info.FileName, info);
        });
    }

    public void Read()
    {
        var jsqlFiles = Directory.GetFiles(_rootPath, "*.jsql", SearchOption.AllDirectories);
        var jsqlFileInfos = ReadFile(jsqlFiles);
        foreach (var info in jsqlFileInfos)
        {
            var exist = _jsqlFileInfos.FirstOrDefault(m => m.Key == info.FileName);
            if (exist.xIsNotEmpty())
            {
                if (exist.Value.FileDateNum > info.FileDateNum)
                {
                    _jsqlFileInfos.AddOrUpdate(info.FileName, info, (key, oldValue) => info);               
                }
            }
            else
            {
                _jsqlFileInfos.AddOrUpdate(info.FileName, info, (key, oldValue) => info);
            }
        }
    }

    private List<JSqlFileInfo> ReadFile(string[] files)
    {
        var jsqls = new List<JSqlFileInfo>(); 
        foreach (var file in files.AsSpan())
        {
            var fileInfo = new FileInfo(file);
            var name = fileInfo.Name;
            var date = fileInfo.LastWriteTime;
            var sql = File.ReadAllText(file);
            jsqls.Add(new JSqlFileInfo()
            {
                FileName = Path.GetFileNameWithoutExtension(fileInfo.Name),
                FileDate = date.ToString("yyyy-MM-dd HH:mm:ss"),
                FileDateNum = date.ToString("yyyyMMddHHmmss").xValue<int>(),
                Sql = sql
            });
        }

        return jsqls;
    }
}