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

//legacy
//By: YSZ
//class ConsoleEmailSender {

//  static void Main(string[] args) {
//    //Test1();//ok
//    //Test2();//ok
//    //Test3();

//    if (args.Length == 1 && (args[0] == "/h" || args[0] == "/?")) {
//      ShowHelp();
//      return;
//    }

//    if (args.Length == 0) {
//      EmailJob emailJob = new EmailJob();
//      emailJob.SendEmails();
//    } else if (args.Length == 1) {
//      TestSendEmail_1(args);
//    } else if (args.Length == 4) {
//      TestSendEmail_4(args);
//    } else {
//      ShowHelp();
//      return;
//    }
//  }

//  static void ShowHelp() {
//    Console.WriteLine("Format of command: ");
//    Console.WriteLine();
//    Console.WriteLine("1. One parameter    - toEmailAddress");
//    Console.WriteLine("      sample command: EmailSender to@test.com");
//    Console.WriteLine();
//    Console.WriteLine("2. Four parameters  - fromEmailaddress toEmailAddress subject body");
//    Console.WriteLine("      sample command: EmailSender from@test.com to@test.com \"my subject\" \"my body\"");
//    Console.WriteLine();
//    Console.WriteLine("3. No parameter - This is use as task program to send email based on database.");
//    Console.WriteLine("      sample command: EmailSender");
//  }

//  static void TestSendEmail_1(string[] args) {
//    if (args.Length != 1) return;
//    string _emailFrom = "abc@test.com";
//    string _emailTo = args[0];
//    string _emailCc = "";
//    string _emailSubject = "Testing email";
//    string _emailBody = "This is testing email.";
//    string _emailAttachedFilesPath = "";
//    EmailHelper emailHelper = new EmailHelper();
//    int ret = emailHelper.SendEmail(_emailFrom, _emailTo, _emailCc, _emailSubject, _emailBody, _emailAttachedFilesPath);
//    if (ret == 0) {
//      Console.WriteLine("Successful");
//    } else {
//      Console.WriteLine("Fail");
//    }
//  }

//  static void TestSendEmail_4(string[] args) {
//    if (args.Length != 4) return;
//    string _emailFrom = args[0];
//    string _emailTo = args[1];
//    string _emailCc = "";
//    string _emailSubject = args[2];
//    string _emailBody = args[3];
//    string _emailAttachedFilesPath = "";
//    EmailHelper emailHelper = new EmailHelper();
//    int ret = emailHelper.SendEmail(_emailFrom, _emailTo, _emailCc, _emailSubject, _emailBody, _emailAttachedFilesPath);
//    if (ret == 0) {
//      Console.WriteLine("Successful");
//    } else {
//      Console.WriteLine("Fail");
//    }
//  }

//  static void Test3() {
//    var smtp = new System.Net.Mail.SmtpClient();
//    var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
//    var credential = new System.Net.Configuration.SmtpSection().Network;
//    //var credential = (System.Net.NetworkCredential)smtp.Credentials;

//    string strHost = smtp.Host;
//    int port = smtp.Port;
//    string strUserName = credential.UserName;
//    string strFromPass = credential.Password;

//    strHost = smtpSection.Network.Host;
//    port = smtpSection.Network.Port;
//    string UserName = smtpSection.Network.UserName;
//    string FromPass = smtpSection.Network.Password;
//    Boolean enablessl = smtpSection.Network.EnableSsl;

//    string webnetmail = ConfigurationManager.AppSettings["WebNetMail"];

//  }

//  static void Test2() {
//    try {
//      try {
//        string SMTP_SERVER = "http://schemas.microsoft.com/cdo/configuration/smtpserver";
//        string SMTP_SERVER_PORT = "http://schemas.microsoft.com/cdo/configuration/smtpserverport";
//        string SEND_USING = "http://schemas.microsoft.com/cdo/configuration/sendusing";
//        string SMTP_USE_SSL = "http://schemas.microsoft.com/cdo/configuration/smtpusessl";
//        string SMTP_AUTHENTICATE = "http://schemas.microsoft.com/cdo/configuration/smtpauthenticate";
//        string SEND_USERNAME = "http://schemas.microsoft.com/cdo/configuration/sendusername";
//        string SEND_PASSWORD = "http://schemas.microsoft.com/cdo/configuration/sendpassword";

//        System.Web.Mail.MailMessage mail = new System.Web.Mail.MailMessage();

//        //ok
//        mail.Fields[SMTP_SERVER] = "smtp-auth.tng.de";
//        mail.Fields[SMTP_SERVER_PORT] = 465;
//        mail.Fields[SMTP_USE_SSL] = true;

//        //ok
//        //mail.Fields[SMTP_SERVER] = "smtp.tng.de";
//        //mail.Fields[SMTP_SERVER_PORT] = 25;
//        //mail.Fields[SMTP_USE_SSL] = false;

//        mail.Fields[SEND_USERNAME] = "MMS@sg.feinmetall.com";
//        mail.Fields[SEND_PASSWORD] = "ReB7Y!AA";

//        //fail
//        //mail.Fields[SMTP_SERVER] = "smtp.gmail.com";
//        //mail.Fields[SMTP_SERVER_PORT] = 587;
//        //mail.Fields[SMTP_USE_SSL] = true;//both true and false are fail
//        //mail.Fields[SEND_USERNAME] = "astrio4321@gmail.com";
//        //mail.Fields[SEND_PASSWORD] = "astrio1234";

//        mail.Fields[SEND_USING] = 2;
//        mail.Fields[SMTP_AUTHENTICATE] = 1;
//        mail.To = "yansong.zhang@astriotech.com, aaa@yahoo.com";
//        mail.From = "abc@tests.com";// "MMS@sg.feinmetall.com";
//        mail.Cc = "tttt@yahoo.com";
//        mail.Subject = "Testing Email 2";
//        mail.BodyFormat = MailFormat.Html;
//        mail.Body = "Good job.";

//        string filename = @"D:\Dev\Project13\20170829\MMSRedesignTest\MMSRedesignTest\Images\LOG_JOB_HST\41\ServiceOrderForm.pdf";
//        System.Web.Mail.MailAttachment attachment = new System.Web.Mail.MailAttachment(filename);
//        mail.Attachments.Add(attachment);

//        try {
//          System.Web.Mail.SmtpMail.Send(mail);
//        } catch (System.Web.HttpException ehttp) {
//          Console.WriteLine("{0}", ehttp.Message);
//          Console.WriteLine("Here is the full error message output");
//          Console.Write("{0}", ehttp.ToString());
//        }
//      } catch (IndexOutOfRangeException) {
//        Console.WriteLine("");
//      }
//    } catch (System.Exception e) {
//      Console.WriteLine("Unknown Exception occurred {0}", e.Message);
//      Console.WriteLine("Here is the Full Message output");
//      Console.WriteLine("{0}", e.ToString());
//    }
//  }


//  static void Test1() {
//    try {

//      //using System.Net.Mail;
//      //  Port 25 is the normal unencrypted pop port; not available on gmail.
//      //  The other two ports have encryption; 587 uses TLS, 465 uses SSL.
//      //  To use 587 you should set SmtpClient.EnableSsl = true.
//      //  465 wont work with SmtpClient, it will work with the deprecated class SmtpMail instead.


//      SmtpClient mailServer = new SmtpClient("smtp.gmail.com", 587);
//      mailServer.Credentials = new System.Net.NetworkCredential("astrio4321@gmail.com", "astrio1234");
//      mailServer.EnableSsl = true;//587 must set true

//      //Senders email.
//      string from = "abc@test.com";
//      //Receiver email
//      string to = "yansong.zhang@astriotech.com";

//      System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(from, to);

//      //Subject of the email.
//      msg.Subject = "Testing Email";

//      //Specify the body of the email here.
//      msg.Body = "Good job.";

//      mailServer.Send(msg);


//      Console.WriteLine("MAIL SENT. Press any key to exit...");
//    } catch (Exception ex) {
//      Console.WriteLine("Unable to send email. Error : " + ex);
//    }

//    //Console.ReadKey();
//  }


//}

