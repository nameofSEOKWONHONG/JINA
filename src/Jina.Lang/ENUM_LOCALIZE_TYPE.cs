using Ardalis.SmartEnum;
using eXtensionSharp;

namespace Jina.Lang;

public class ENUM_LOCALIZE_TYPE : SmartEnum<ENUM_LOCALIZE_TYPE>
{
    /// <summary>
    /// 영어
    /// </summary>
    public static ENUM_LOCALIZE_TYPE EN_US = new ENUM_LOCALIZE_TYPE("en-US", 1);
    
    /// <summary>
    /// 한국어
    /// </summary>
    public static ENUM_LOCALIZE_TYPE KO_KR = new ENUM_LOCALIZE_TYPE("ko-KR", 2);
    
    /// <summary>
    /// 중국어-간체
    /// </summary>
    public static ENUM_LOCALIZE_TYPE ZH_CN = new ENUM_LOCALIZE_TYPE("zh-CN", 2);
    
    /// <summary>
    /// 일본어
    /// </summary>
    public static ENUM_LOCALIZE_TYPE JA_JP = new ENUM_LOCALIZE_TYPE("ja-JP", 2);

    public ENUM_LOCALIZE_TYPE(string name, int value) : base(name, value)
    {
    }
}