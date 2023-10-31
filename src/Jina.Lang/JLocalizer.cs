using System.Globalization;
using Jina.Lang.Abstract;

namespace Jina.Lang;

public class JLocalizer : ILocalizer
{
    private readonly string[] _supportLanguage = new[]
    {
        ENUM_LOCALIZER_SUPPORT_TYPE.EN_US.ToString(), 
        ENUM_LOCALIZER_SUPPORT_TYPE.KO_KR.ToString(),
        ENUM_LOCALIZER_SUPPORT_TYPE.ZH_CN.ToString(),
        ENUM_LOCALIZER_SUPPORT_TYPE.JA_JP.ToString()
    };
    
    public string this[string resCode]
    {
        get
        {
            var resName = resCode.Substring(0, 3).ToLower();
            var supportLang = this._supportLanguage.FirstOrDefault(m => m.Contains(CultureInfo.CurrentCulture.Name));
            if (!string.IsNullOrWhiteSpace(supportLang))
            {
                if (JLocalizerLoader.Instance.Loader.TryGetValue($"{resName}.{supportLang}",
                        out Dictionary<string, string> val))
                {
                    if (val.TryGetValue(resCode, out string result))
                    {
                        return result;
                    }
                }
            }

            return string.Empty;
        }
    }
}