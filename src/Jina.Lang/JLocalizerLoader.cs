using System.Text;
using System.Text.Json;

namespace Jina.Lang;

public class JLocalizerLoader
{
    public static JLocalizerLoader Instance => _instance.Value;

    private static Lazy<JLocalizerLoader> _instance =
        new Lazy<JLocalizerLoader>(() => new JLocalizerLoader());

    private const string LANG_PATH_NAME = "language";
    private readonly string LANG_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LANG_PATH_NAME);
    public Dictionary<string, Dictionary<string, string>> Loader { get; private set; } = new();
    public static object _sync = new();
    
    private JLocalizerLoader()
    {
        ReadLanguageFile();
    }

    private void ReadLanguageFile()
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