using eXtensionSharp;

namespace Jina.Lang;

public class ENUM_LOCALE_TYPE : XEnumBase<ENUM_LOCALE_TYPE>
{
    /// <summary>
    /// 영어
    /// </summary>
    public static ENUM_LOCALE_TYPE EN_US = Define("en-US");
    
    /// <summary>
    /// 한국어
    /// </summary>
    public static ENUM_LOCALE_TYPE KO_KR = Define("ko-KR");
    
    /// <summary>
    /// 중국어-간체
    /// </summary>
    public static ENUM_LOCALE_TYPE ZH_CN = Define("zh-CN");
    
    /// <summary>
    /// 일본어
    /// </summary>
    public static ENUM_LOCALE_TYPE JA_JP = Define("ja-JP");
}