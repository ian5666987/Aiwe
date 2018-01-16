using Aibe.Helpers;
using Aibe.Models;
using Aibe.Models.DB;
using Extension.Models;
using System;
using System.Configuration;

namespace AiweEmailSender {
  public class ConsoleEmailSender {
    private static EmailServiceType defaultEmailServiceType = EmailServiceType.NETMAIL;
    private static EmailServiceType emailServiceType;
    public static void Main(string[] args) {
      if (args.Length == 1 && (args[0] == "/h" || args[0] == "/?")) {
        ShowHelp();
        return;
      }

      //using System.Net.Mail;
      //  Port 25 is the normal unencrypted pop port; not available on gmail.
      //  The other two ports have encryption; 587 uses TLS, 465 uses SSL.
      //  To use 587 you should set SmtpClient.EnableSsl = true.
      //  465 wont work with SmtpClient, it will work with the deprecated class SmtpMail instead.

      string webNetMailString = ConfigurationManager.AppSettings["WebNetMail"];
      bool result = Enum.TryParse(webNetMailString, out emailServiceType);
      if (!result)
        emailServiceType = defaultEmailServiceType;

      if (args.Length == 0) {
        AiweEmailHelper emailHelper = new AiweEmailHelper(emailServiceType);
        emailHelper.SendNotSentEmails();
      } else if (args.Length == 1) {
        TestSendEmail_1(args);
      } else if (args.Length == 4) {
        TestSendEmail_4(args);
      } else {
        ShowHelp();
      }
    }

    public static void ShowHelp() {
      Console.WriteLine("Format of command: ");
      Console.WriteLine();
      Console.WriteLine("1. One parameter    - toEmailAddress");
      Console.WriteLine("      sample command: AiweEmailSender to@test.com");
      Console.WriteLine();
      Console.WriteLine("2. Four parameters  - fromEmailaddress toEmailAddress subject body");
      Console.WriteLine("      sample command: AiweEmailSender from@test.com to@test.com \"my subject\" \"my body\"");
      Console.WriteLine();
      Console.WriteLine("3. No parameter - This is use as task program to send email based on database.");
      Console.WriteLine("      sample command: AiweEmailSender");
    }

    public static void TestSendEmail_1(string[] args) {
      if (args.Length != 1) return;
      BaseEmailInfo emailInfo = new BaseEmailInfo {
        EmailFrom = "abc@test.com", EmailTo = args[0],
        EmailCc = "", EmailSubject = "Testing email",
        EmailBody = "This is a testing email.",
        AttachmentFilePaths = "",
      };
      BaseErrorModel em = new EmailHelper<BaseEmailInfo>(emailServiceType).SendEmail(emailInfo);
      if (!em.HasError) {
        Console.WriteLine("Successful");
      } else {
        Console.WriteLine("Fail");
      }
    }

    public static void TestSendEmail_4(string[] args) {
      if (args.Length != 4) return;
      BaseEmailInfo emailInfo = new BaseEmailInfo {
        EmailFrom = args[0], EmailTo = args[1],
        EmailCc = "", EmailSubject = args[2],
        EmailBody = args[3],
        AttachmentFilePaths = "",
      };
      BaseErrorModel em = new EmailHelper<BaseEmailInfo>(emailServiceType).SendEmail(emailInfo);
      if (!em.HasError) {
        Console.WriteLine("Successful");
      } else {
        Console.WriteLine("Fail");
      }
    }
  }
}
