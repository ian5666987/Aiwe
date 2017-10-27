using Aibe.Models.DB;
using System;

namespace AiweEmailSender {
  public class AiweEmailInfo : BaseEmailInfo {
    //Put additional properties according to your database model (CoreEmailInfo table) here
    //Basically, This class must contain all properties in the CoreEmailInfo class which are NOT in the BaseEmailInfo
    public DateTime? CreatedOn { get; set; }
    public DateTime? SendDateTime { get; set; }
    public int? SendCount { get; set; }
    public int? JobNo { get; set; }
    public string JobStatus { get; set; }
    public DateTime? ScheduleDateTime { get; set; }
    public int? Send1DaysBefore { get; set; }
    public int? Send3DaysBefore { get; set; }
    public int? Send7DaysBefore { get; set; }

    //Called by EmailSender<T>.SendEmails() everytime after an email is successfully sent and when the GetUpdateSqlStringDelegate is null
    //It is NOT called when GetUpdateSqlStringDelegate is given
    public override string GetUpdateIsSentSqlString() {
      //Put overriding way to get the update string after the emails are sent here...
      //By default, this returns "UPDATE CoreEmailInfo SET IsSent=1 WHERE Cid={BaseEmailInfo.Cid}"
      //Change this method if you want create different update string after an email is successfully sent
      return base.GetUpdateIsSentSqlString();
    }
  }
}
