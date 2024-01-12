using System.Collections.Concurrent;
using JSqlEngine.Data;
using Microsoft.Extensions.Options;

namespace JSqlEngine.Core;

public class JSqlReader
{
    private readonly string _rootPath;
    public JSqlReader(IOptions<SqlOption> options)
    {
        _rootPath = options.Value.Path;
    }

    private readonly ConcurrentDictionary<string, SqlFileInfo> _jsqlFileInfos = new();

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
            _jsqlFileInfos.AddOrUpdate(info.FileName, info, (key, oldValue) => info);
        });
    }

    public void SequentialRead()
    {
        var jsqlFiles = Directory.GetFiles(_rootPath, "*.jsql", SearchOption.AllDirectories);
        var jsqlFileInfos = ReadFile(jsqlFiles);
        foreach (var info in jsqlFileInfos)
        {
            _jsqlFileInfos.AddOrUpdate(info.FileName, info, (key, oldValue) => info);
        }
    }

    private List<SqlFileInfo> ReadFile(string[] files)
    {
        var jsqls = new List<SqlFileInfo>(); 
        foreach (var file in files.AsSpan())
        {
            var fileInfo = new FileInfo(file);
            var name = fileInfo.Name;
            var date = fileInfo.LastWriteTime;
            var sql = File.ReadAllText(file);
            jsqls.Add(new SqlFileInfo()
            {
                FileName = Path.GetFileNameWithoutExtension(fileInfo.Name),
                FileDate = date.ToString("yyyy-MM-dd HH:mm:ss"),
                Sql = sql
            });
        }

        return jsqls;
    }
}