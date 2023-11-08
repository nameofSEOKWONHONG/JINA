using eXtensionSharp;
using Jina.Application.Configuration;
using Jina.Utils.Mail.Data;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Jina.Utils.Mail;

public class MailSendProvider
{
    private readonly MailConfigOption _configOption;
    public MailSendProvider(MailConfigOption configOption)
    {
        _configOption = configOption;
    }

    public async Task ExecuteAsync(MailRequest request)
    {
        if (request.FromMailers.xIsEmpty())
        {
            request.FromMailers = new List<MailSender>();
            request.FromMailers.Add(new MailSender() {Name = _configOption.DisplayName, Mail = _configOption.From});
        }
        var message = CreateMessage(request);
        using var smtp = new SmtpClient();
        if (request.SmtpInfo.xIsEmpty())
        {
            await smtp.ConnectAsync(_configOption.Host, _configOption.Port, true);
            await smtp.AuthenticateAsync(_configOption.UserName, _configOption.Password);
        }
        else
        {
            if (request.SmtpInfo.Host.ToLower().Contains("office") || request.SmtpInfo.Host.ToLower().Contains("outlook"))
            {
                await smtp.ConnectAsync(request.SmtpInfo.Host, request.SmtpInfo.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(request.SmtpInfo.LoginId, request.SmtpInfo.Password);
            }
            else
            {
                await smtp.ConnectAsync(request.SmtpInfo.Host, request.SmtpInfo.Port, request.SmtpInfo.UseSsl);
                await smtp.AuthenticateAsync(request.SmtpInfo.LoginId, request.SmtpInfo.Password);                    
            }
        }
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

    /// <summary>
    /// white list states
    /// </summary>
    private Dictionary<string, (string mimePartType, string mediaSubType)> _minePartTypes
        = new()
        {
            {
                ".xlsx", new ("application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            },
            {
                ".png", new ("image", "png")
            },
            {
                ".jpg", new ("image", "jpg")
            }
        };
    
    /// <summary>
    /// 발송 메세지 생성
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private MimeMessage CreateMessage(MailRequest request)
    {
        var message = new MimeMessage();
        request.FromMailers.xForEach(item =>
        {
            message.From.Add(new MailboxAddress(item.Name, item.Mail));    
        });
        request.ToMailers.xForEach(item =>
        {
            message.To.Add(new MailboxAddress(item.Name, item.Mail));    
        });
        request.CcMailers.xForEach(item =>
        {
            message.Cc.Add(new MailboxAddress(item.Name, item.Mail));    
        });           
        
        message.Subject = request.Subject;
        if (!request.IsBodyHtml)
        {
            message.Body = new BodyBuilder()
            {
                TextBody = request.Body
            }.ToMessageBody();
        }
        else
        {
            message.Body = new BodyBuilder()
            {
                HtmlBody = request.Body
            }.ToMessageBody();
        };

        if (request.Files.xIsNotEmpty())
        {
            var multipart = new Multipart("mixed");
            multipart.Add(message.Body);
            
            request.Files.xForEach(item =>
            {
                var selectedMinePartType = _minePartTypes[item.FileName.xGetExtension()];
                var attachment = new MimePart (selectedMinePartType.mimePartType, selectedMinePartType.mediaSubType) {
                    Content = new MimeContent(new MemoryStream(item.File)),
                    ContentDisposition = new ContentDisposition (ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = item.FileName
                };
                multipart.Add(attachment);
            });     
            
            message.Body = multipart;
            return message;
        }
        
        return message;
    }    
}