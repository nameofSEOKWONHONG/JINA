namespace Jina.Utils.Mail.Data;

public class MailSender
{
    public string Name { get; set; }
    public string Mail { get; set; }

    public MailSender()
    {
            
    }

    public MailSender(string name, string mail)
    {
        this.Name = name;
        this.Mail = mail;
    }
}