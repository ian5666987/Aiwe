using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.IO;

using System.Net.Mail;

namespace EmailSender
{
    public class EmailJob
    {
        DataSet emailDataSet = new DataSet();
        string _emailFrom, _emailTo, _emailCc, _emailSubject, _emailBody, _emailAttachedFilesPath;
        string connectionString;

        public EmailJob()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MMSRedesignDataModel"].ConnectionString;
        }


        public int SendEmails()
        {
            SendEmails_NotSend();
            SendEmails_1daysB4();
            SendEmails_3daysB4();
            SendEmails_7daysB4();

            return 0;
        }

        private void SendEmails_NotSend()
        {
            emailDataSet = Find_NotSend_EmailListDS();
            if (emailDataSet!=null && emailDataSet.Tables[0].Rows.Count > 0)
            {
                for(int i=0; i < emailDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = emailDataSet.Tables[0].Rows[i];
                    int Cid = Convert.ToInt32(dr["Cid"].ToString());
                    ReadEmailDataRow(dr);

                    int ret = -1;
                    EmailHelper emailHelper = new EmailHelper();
                    ret = emailHelper.SendEmail(_emailFrom, _emailTo, _emailCc, _emailSubject, _emailBody, _emailAttachedFilesPath);

                    if (ret == 0)
                    {
                        string sql = "update EML_EMAILINFO set SendStatus=1, SendCount=SendCount + 1, SendDateTime=GetDate() "
                                   + " where Cid=" + Cid.ToString();
                        UpdateCmd(sql);
                    }
                }
            }

        }

        private void SendEmails_1daysB4()
        {
            emailDataSet = Find_Assigned_EmailListDS_1daysB4Schedule();
            if (emailDataSet != null && emailDataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < emailDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = emailDataSet.Tables[0].Rows[i];
                    int Cid = Convert.ToInt32(dr["Cid"].ToString());
                    ReadEmailDataRow(dr);
                    _emailSubject = "Reminder - " + _emailSubject;

                    int ret = -1;
                    EmailHelper emailHelper = new EmailHelper();
                    ret = emailHelper.SendEmail(_emailFrom, _emailTo, _emailCc, _emailSubject, _emailBody, _emailAttachedFilesPath);

                    if (ret == 0)
                    {
                        string sql = "update EML_EMAILINFO set SendStatus=1, SendCount=SendCount + 1, SendDateTime=GetDate() "
                               + " , Send1DaysBefore=1 "
                               + " where Cid=" + Cid.ToString();
                        UpdateCmd(sql);
                    }
                }
            }

        }

        private void SendEmails_3daysB4()
        {
            emailDataSet = Find_Assigned_EmailListDS_3daysB4Schedule();
            if (emailDataSet != null && emailDataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < emailDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = emailDataSet.Tables[0].Rows[i];
                    int Cid = Convert.ToInt32(dr["Cid"].ToString());
                    ReadEmailDataRow(dr);
                    _emailSubject = "Reminder - " + _emailSubject;

                    int ret = -1;
                    EmailHelper emailHelper = new EmailHelper();
                    ret = emailHelper.SendEmail(_emailFrom, _emailTo, _emailCc, _emailSubject, _emailBody, _emailAttachedFilesPath);

                    if (ret == 0)
                    {
                        string sql = "update EML_EMAILINFO set SendStatus=1, SendCount=SendCount + 1, SendDateTime=GetDate() "
                               + " , Send3DaysBefore=1 "
                               + " where Cid=" + Cid.ToString();
                        UpdateCmd(sql);
                    }
                }
            }

        }

        private void SendEmails_7daysB4()
        {
            emailDataSet = Find_Assigned_EmailListDS_7daysB4Schedule();
            if (emailDataSet != null && emailDataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < emailDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = emailDataSet.Tables[0].Rows[i];
                    int Cid = Convert.ToInt32(dr["Cid"].ToString());
                    ReadEmailDataRow(dr);
                    _emailSubject = "Reminder - " + _emailSubject;

                    int ret = -1;
                    EmailHelper emailHelper = new EmailHelper();
                    ret = emailHelper.SendEmail(_emailFrom, _emailTo, _emailCc, _emailSubject, _emailBody, _emailAttachedFilesPath);

                    if (ret == 0)
                    {
                        string sql = "update EML_EMAILINFO set SendStatus=1, SendCount=SendCount + 1, SendDateTime=GetDate() "
                               + " , Send7DaysBefore=1 "
                               + " where Cid=" + Cid.ToString();
                        UpdateCmd(sql);
                    }
                }
            }

        }

        private void ReadEmailDataRow(DataRow dr)
        {
            if (dr != null)
            {
                string emailFrom = dr["EmailFrom"].ToString();
                string emailTo = dr["EmailTo"].ToString();
                string emailCc = dr["EmailCc"].ToString();
                string emailSubject = dr["EmailSubject"].ToString();
                string emailBody = dr["EmailBody"].ToString();
                string emailAttachedFilesPath = dr["AttachedFilesPath"].ToString();
                string ParamValues = dr["ParamValues"].ToString();
                int requestNo = 0;
                try
                {
                    requestNo = Convert.ToInt32(dr["EmailFrom"].ToString());
                }
                catch (Exception ex) { }
                string defaultFrom = dr["DefaultFrom"].ToString();
                string defaultTo = dr["DefaultTo"].ToString();
                string defaultCc = dr["DefaultCc"].ToString();
                string defaultSubject = dr["DefaultSubject"].ToString();
                string defaultBody = dr["DefaultBody"].ToString();

                if (String.IsNullOrEmpty(emailFrom))
                    _emailFrom = defaultFrom;
                else _emailFrom = emailFrom;

                if (String.IsNullOrEmpty(emailTo))
                    _emailTo = defaultTo;
                else _emailTo = emailTo;

                if (String.IsNullOrEmpty(emailCc))
                    _emailCc = defaultCc;
                else _emailCc = emailCc;

                if (String.IsNullOrEmpty(emailSubject))
                    _emailSubject = defaultSubject;
                else _emailSubject = emailSubject;

                if (String.IsNullOrEmpty(emailBody))
                    _emailBody = defaultBody;
                else _emailBody = emailBody;

                _emailAttachedFilesPath = emailAttachedFilesPath;

                if (!String.IsNullOrEmpty(ParamValues))
                {
                    string[] ParamValuesList = ParamValues.Split(',');
                    foreach (string ParamValuePair in ParamValuesList)
                    {
                        string[] pair = ParamValuePair.Split('=');
                        _emailBody = _emailBody.Replace(pair[0], pair[1]);
                    }

                }

            }
        }

        private DataSet Find_NotSend_EmailListDS()
        {
            string sql = "select a.*, b.DefaultFrom, b.DefaultTo, b.DefaultCC, "
                        + " b.DefaultSubject, b.DefaultBody "
                        + " from EML_EMAILINFO a left join EML_TEMPLATE b "
                        + " on a.TemplateName = b.TemplateName "
                        + " where a.SendStatus=0 ";

            DataSet dataSet = Get_EmailListDS(sql);
            return dataSet;
        }

        private DataSet Find_Assigned_EmailListDS_1daysB4Schedule()
        {
            string sql = "select a.*, b.DefaultFrom, b.DefaultTo, b.DefaultCC, "
                        + " b.DefaultSubject, b.DefaultBody "
                        + " from EML_EMAILINFO a left join EML_TEMPLATE b "
                        + " on a.TemplateName = b.TemplateName "
                        + " where a.JobStatus in ('A','P') and a.TemplateName='ASSIGN_ALERT' and Send1DaysBefore=0 "
                        + "  and datediff(day, convert(date, getdate()), convert(date, a.scheduledatetime))=1";

            DataSet dataSet = Get_EmailListDS(sql);
            return dataSet;
        }

        private DataSet Find_Assigned_EmailListDS_3daysB4Schedule()
        {
            string sql = "select a.*, b.DefaultFrom, b.DefaultTo, b.DefaultCC, "
                        + " b.DefaultSubject, b.DefaultBody "
                        + " from EML_EMAILINFO a left join EML_TEMPLATE b "
                        + " on a.TemplateName = b.TemplateName "
                        + " where a.JobStatus in ('A','P') and a.TemplateName='ASSIGN_ALERT' and Send3DaysBefore=0 "
                        + "  and datediff(day, convert(date, getdate()), convert(date, a.scheduledatetime))=3";

            DataSet dataSet = Get_EmailListDS(sql);
            return dataSet;
        }

        private DataSet Find_Assigned_EmailListDS_7daysB4Schedule()
        {
            string sql = "select a.*, b.DefaultFrom, b.DefaultTo, b.DefaultCC, "
                        + " b.DefaultSubject, b.DefaultBody "
                        + " from EML_EMAILINFO a left join EML_TEMPLATE b "
                        + " on a.TemplateName = b.TemplateName "
                        + " where a.JobStatus in ('A','P') and a.TemplateName='ASSIGN_ALERT' and Send7DaysBefore=0 "
                        + "  and datediff(day, convert(date, getdate()), convert(date, a.scheduledatetime))=7";

            DataSet dataSet = Get_EmailListDS(sql);
            return dataSet;
        }

        private DataSet Get_EmailListDS(string sql)
        {
            //string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["MMSRedesignDataModel"].ConnectionString;
            DataSet dataSet = new DataSet();
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.CommandType = CommandType.Text;
                //parameter name is same with it in StoredProcedure
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dataSet);
            }
            return dataSet;
        }

        private void UpdateCmd(string sql)
        {
            //string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["MMSRedesignDataModel"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
