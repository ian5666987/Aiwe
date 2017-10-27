using Aibe.Helpers;
using Aibe.Models;
using Extension.Models;
using Extension.Database.SqlServer;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AiweEmailSender {
  public class AiweEmailHelper : EmailHelper<AiweEmailInfo> {

    public AiweEmailHelper(EmailServiceType emailServiceType) : base(emailServiceType) { }

    public override DataTable GetNotSentEmailInfoTable() {
      //Put overriding way to GetNotSentEmails here
      //by default, the function gets the not-sent table the default way (IsSent=0)
      return base.GetNotSentEmailInfoTable();
    }

    public List<BaseErrorModel> SendNotSentEmails() {
      //get all not sent table here
      DataTable notSentTable = GetNotSentEmailInfoTable();

      //put whatever is obtained from the table to SendEmails method
      //if notSentTable is null, the function SendEmails gets the not-sent table the default way (IsSent=0), as defined in EmailHelper<T>, and sends them
      return SendEmails(notSentTable); 
    }

    //Additional functions can also be put here, below are some examples
    private DataTable getNotSendEmails3DaysBefore() {
      //To get all data table in the CoreEmailInfo table where "Send3DaysBefore=0"
      return SQLServerHandler.GetFullDataTableWhere(Aibe.DH.DataDBConnectionString, Aibe.DH.EmailInfoTableName,
        "IsSent=0 OR Send3DaysBefore=0");
    }

    //Create update string to set IsSent=1 and Send3DaysBefore=1 after successful sending
    //Provide this to replace AiweEmailInfo.GetUpdateIsSentSqlString() being called by SendEmails() in the method SendReminder3DaysBefore()
    private string updateSQLString3DaysBefore(int cid) {
      StringBuilder sb = new StringBuilder();
      sb.Append(string.Concat("UPDATE ", Aibe.DH.EmailInfoTableName, " SET "));
      sb.Append(string.Concat(Aibe.DH.EmailMakerIsSentColumnName, "=1, Send3DaysBefore=1"));
      sb.Append(string.Concat(" WHERE ", Aibe.DH.Cid, "=", cid));
      return sb.ToString();
    }

    //Create a custom method to send the emails where Send3DaysBefore=0
    //And update IsSent=1 and Send3DaysBefore=1 when the email is successfully sent
    public List<BaseErrorModel> SendReminder3DaysBefore() {
      DataTable reminder3DaysBefore = getNotSendEmails3DaysBefore(); //get data table from custom method GetNotSendEmails3DaysBefore()
      GetUpdateSqlStringDelegate updateSQLString = new GetUpdateSqlStringDelegate(updateSQLString3DaysBefore); //use custom update SQL string
      return SendEmails(reminder3DaysBefore, updateSQLString); //use default SendEmails() but provide the table and the update string method
    }
  }
}
