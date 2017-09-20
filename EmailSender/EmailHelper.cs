using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Web.Mail;
using System.Configuration;
using System.Net.Configuration;

namespace EmailSender
{
    public class EmailHelper
    {
        string SmtpHost;
        int PORT;
        string UserID, Password;
        Boolean EnableSsl;
        string WebNetMail;
        const string NETMAIL = "NETMAIL";
        const string WEBMAIL = "WEBMAIL";

        System.Net.Mail.MailMessage NetMail_mailMessage;
        System.Web.Mail.MailMessage WebMail_mailMessage;

        public EmailHelper()
        {
        }

        public int SendEmail(string fromAddress, string to_List, string cc_List,
            string Subject, string Body, string Attachments_List)
        {
            int ret = 0;
            Set_SmtpHost();
            /*
            if (PORT==587)
            {
                Prepare_NetMail(fromAddress, to_List, cc_List, Subject, Body, Attachments_List);
                ret = SendEmail_NetMail();
            }
            else if (PORT == 465 || PORT == 25)
            {
                ret = SendEmail_WebMail(fromAddress, to_List, cc_List, Subject, Body, Attachments_List);
            }
            else
            {
                Prepare_NetMail(fromAddress, to_List, cc_List, Subject, Body, Attachments_List);
                ret = SendEmail_NetMail();
            }
            */
            if (WebNetMail.ToUpper() == NETMAIL)
            {
                Prepare_NetMail(fromAddress, to_List, cc_List, Subject, Body, Attachments_List);
                ret = SendEmail_NetMail();
            }
            else if (WebNetMail.ToUpper() == WEBMAIL)
            {
                ret = SendEmail_WebMail(fromAddress, to_List, cc_List, Subject, Body, Attachments_List);
            }
            else
            {
                Prepare_NetMail(fromAddress, to_List, cc_List, Subject, Body, Attachments_List);
                ret = SendEmail_NetMail();
            }

            return ret;
        }

        private void Set_SmtpHost()
        {
            //var smtp = new System.Net.Mail.SmtpClient();
            //this.SmtpHost = smtp.Host;
            //this.PORT = smtp.Port;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            this.SmtpHost = smtpSection.Network.Host;
            this.PORT = smtpSection.Network.Port;
            this.UserID = smtpSection.Network.UserName;
            this.Password = smtpSection.Network.Password;
            this.EnableSsl = smtpSection.Network.EnableSsl;
            this.WebNetMail = ConfigurationManager.AppSettings["WebNetMail"];
        }

        private int SendEmail_WebMail(string fromAddress, string to_List, string cc_List,
            string Subject, string Body, string Attachments_List)
        {
            try
            {
                try
                {
                    string SMTP_SERVER = "http://schemas.microsoft.com/cdo/configuration/smtpserver";
                    string SMTP_SERVER_PORT = "http://schemas.microsoft.com/cdo/configuration/smtpserverport";
                    string SEND_USING = "http://schemas.microsoft.com/cdo/configuration/sendusing";
                    string SMTP_USE_SSL = "http://schemas.microsoft.com/cdo/configuration/smtpusessl";
                    string SMTP_AUTHENTICATE = "http://schemas.microsoft.com/cdo/configuration/smtpauthenticate";
                    string SEND_USERNAME = "http://schemas.microsoft.com/cdo/configuration/sendusername";
                    string SEND_PASSWORD = "http://schemas.microsoft.com/cdo/configuration/sendpassword";

                    WebMail_mailMessage = new System.Web.Mail.MailMessage();

                    //ok
                    WebMail_mailMessage.Fields[SMTP_SERVER] = SmtpHost;
                    WebMail_mailMessage.Fields[SMTP_SERVER_PORT] = PORT;
                    //if (PORT== 465)
                    //    WebMail_mailMessage.Fields[SMTP_USE_SSL] = true;
                    //else if (PORT == 25)
                    //    WebMail_mailMessage.Fields[SMTP_USE_SSL] = false;
                    WebMail_mailMessage.Fields[SMTP_USE_SSL] = EnableSsl;
                    WebMail_mailMessage.Fields[SEND_USERNAME] = UserID;
                    WebMail_mailMessage.Fields[SEND_PASSWORD] = Password;

                    WebMail_mailMessage.Fields[SEND_USING] = 2;
                    WebMail_mailMessage.Fields[SMTP_AUTHENTICATE] = 1;
                    WebMail_mailMessage.To = to_List;
                    WebMail_mailMessage.From = fromAddress;
                    WebMail_mailMessage.Cc = cc_List;
                    WebMail_mailMessage.Subject = Subject;
                    //WebMail_mailMessage.BodyFormat = MailFormat.Html;
                    WebMail_mailMessage.Body = Body;

                    if (!string.IsNullOrEmpty(Attachments_List))
                    {
                        string[] Attachments = Attachments_List.Split(',');
                        foreach (string filePaths in Attachments)
                        {
                            System.Web.Mail.MailAttachment attachment = new System.Web.Mail.MailAttachment(filePaths);
                            WebMail_mailMessage.Attachments.Add(attachment);
                        }
                    }
                    try
                    {
                        System.Web.Mail.SmtpMail.Send(WebMail_mailMessage);
                        System.Threading.Thread.Sleep(1000);
                        return 0;
                    }
                    catch (System.Web.HttpException ehttp)
                    {
                        Console.WriteLine("{0}", ehttp.Message);
                        Console.WriteLine("Here is the full error message output");
                        Console.Write("{0}", ehttp.ToString());
                        return 1;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("");
                    return 2;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Unknown Exception occurred {0}", e.Message);
                Console.WriteLine("Here is the Full Message output");
                Console.WriteLine("{0}", e.ToString());
                return 3;
            }
        }


        private void Prepare_NetMail(string fromAddress, string to_List, string cc_List, 
            string Subject, string Body, string Attachments_List)
        {

            string[] toList = to_List.Split(',');
            string toAddress = toList[0];
            NetMail_mailMessage = new System.Net.Mail.MailMessage(fromAddress, toAddress);

            foreach (string to in toList)
            {
                if (to != toAddress)
                    NetMail_mailMessage.To.Add(to);
            }

            if (!string.IsNullOrEmpty(cc_List))
            {
                string[] ccList = cc_List.Split(',');
                foreach (string cc in ccList)
                {
                    NetMail_mailMessage.CC.Add(cc);
                }
            }

            NetMail_mailMessage.Subject = Subject;
            NetMail_mailMessage.Body = Body;

            if (!string.IsNullOrEmpty(Attachments_List))
            {
                string[] Attachments = Attachments_List.Split(',');
                foreach (string filePaths in Attachments)
                {
                    NetMail_mailMessage.Attachments.Add(new Attachment(filePaths));
                }
            }
        }


        private int SendEmail_NetMail()
        {
            try
            {
                SmtpClient client = new SmtpClient(SmtpHost, PORT);
                client.EnableSsl = EnableSsl;// true;
                client.Credentials = new System.Net.NetworkCredential(UserID, Password);
                client.Send(NetMail_mailMessage);
                System.Threading.Thread.Sleep(1000);
                return 0;
            }
            catch (Exception ex)
            {
                //message here
                return 1;
            }
        }

    }
}
