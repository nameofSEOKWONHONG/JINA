using System.Globalization;
using Jina.Lang.Abstract;

namespace Jina.Lang;

/// <summary>
/// server side localizer
/// </summary>
public class Localizer : ILocalizer
{
    private readonly string[] _supportLanguage = new[]
    {
        ENUM_LOCALIZE_TYPE.EN_US.Name, 
        ENUM_LOCALIZE_TYPE.KO_KR.Name,
        ENUM_LOCALIZE_TYPE.ZH_CN.Name,
        ENUM_LOCALIZE_TYPE.JA_JP.Name
    };
    
    public string this[string resCode]
    {
        get
        {
            var resName = resCode.Substring(0, 3).ToLower();
            var supportLang = this._supportLanguage.FirstOrDefault(m => m.Contains(CultureInfo.CurrentCulture.Name));
            if (!string.IsNullOrWhiteSpace(supportLang))
            {
                if (LocalizeLoader.Instance.Loader.TryGetValue($"{resName}.{supportLang}",
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