namespace Jina.Utils.Mail.Data;

public class MailRequest
{
    /// <summary>
    /// 발송자 명
    /// </summary>
    public List<MailSender> FromMailers { get; set; }
    public List<MailSender> ToMailers { get; set; }
    public List<MailSender> CcMailers { get; set; }

    /// <summary>
    /// 타이틀
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// 바디
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// 바디 HTML 여부
    /// </summary>
    public bool IsBodyHtml { get; set; }

    public List<MailAttachFile> Files { get; set; }
    
    /// <summary>
    /// 발신자 SMTP 정보
    /// </summary>
    public SmtpInfo SmtpInfo { get; set; }

    public MailRequest()
    {
            
    }
        
    public MailRequest(SmtpInfo smtp)
    {
        this.SmtpInfo = smtp;
    }
}