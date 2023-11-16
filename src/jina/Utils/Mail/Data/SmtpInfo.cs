namespace Jina.Utils.Mail.Data;

public class SmtpInfo
{
    /// <summary>
    /// 발송자 SMTP HOST주소
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 발송자 포트
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 발송자 로그인 ID
    /// </summary>
    public string LoginId { get; set; }

    /// <summary>
    /// 발송자 암호
    /// </summary>
    public string Password { get; set; }

    public bool UseSsl { get; set; }

    public SmtpInfo()
    {
    }

    public SmtpInfo(string host, int port, string loginId, string password)
    {
        Host = host;
        Port = port;
        LoginId = loginId;
        Password = password;
    }
}