using System.Text;
using System.Text.Json;

namespace Jina.Lang;

public class LocalizeLoader
{
    public static LocalizeLoader Instance => _instance.Value;

    private static Lazy<LocalizeLoader> _instance =
        new Lazy<LocalizeLoader>(() => new LocalizeLoader());

    private const string LANG_PATH_NAME = "language";
    private readonly string LANG_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LANG_PATH_NAME);    

    public Dictionary<string, Dictionary<string, string>> Loader { get; private set; } = new();
    public static object _sync = new();
    
    private LocalizeLoader()
    {
        Init();
    }

    private void Init()
    {
        var newLoader = new Dictionary<string, Dictionary<string, string>>();
        var files = Directory.GetFiles(LANG_PATH);
        foreach (var file in files)
        {
            var jsonData = File.ReadAllText(file, Encoding.UTF8);
            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonData);

            newLoader.TryAdd(Path.GetFileNameWithoutExtension(file), map);
        }

        lock (_sync)
        {
            Loader = newLoader;    
        }
    }
}