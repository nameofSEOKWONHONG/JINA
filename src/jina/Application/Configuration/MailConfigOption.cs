namespace Jina.Application.Configuration;

public class MailConfigOption
{
    /// <summary>
    /// 발신자 Email
    /// </summary>
    public string From { get; set; }
    /// <summary>
    /// 발신자 표시명
    /// </summary>
    public string DisplayName { get; set; }
    /// <summary>
    /// SMTP Host
    /// </summary>
    public string Host { get; set; }
    /// <summary>
    /// Port
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// 계정명
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 계정암호
    /// </summary>
    public string Password { get; set; }
}