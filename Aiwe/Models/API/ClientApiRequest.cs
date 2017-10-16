using System.Text;
using System.Linq;

namespace Aiwe.Models.API {
  public class ClientApiRequest {
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string RequestType { get; private set; }
    public string TableName { get; private set; } //The request should probably still use table name because it is used to get MetaInfo
    public string ColumnNames { get; private set; } //TODO currently is vulnerable to SQL injection attacks... but leave it for for now...
    public string FilterDesc { get; private set; }
    public string OrderByDesc { get; private set; }
    public int? ItemsLoaded { get; private set; }
    public int? ItemsSkipped { get; private set; }
    public int? Id { get; private set; }
    public string Value { get; private set; }
    public ClientApiRequestAttachment[] Attachments { get; private set; }

    public ClientApiRequest(string userName, string password, string requestType, string tableName,
      string columnNames, string filterDesc, string orderByDesc, int? itemsLoaded, int? itemsSkipped,
      int? id, string value, ClientApiRequestAttachment[] attachments) {
      UserName = userName;
      Password = password;
      RequestType = requestType;
      TableName = tableName;
      ColumnNames = columnNames;
      FilterDesc = filterDesc;
      OrderByDesc = orderByDesc;
      ItemsLoaded = itemsLoaded;
      ItemsSkipped = itemsSkipped;
      Id = id;
      Value = value;
      Attachments = attachments;
    }

    public string CreateLogValue(int lengthLimit = 3000) {
      StringBuilder sb = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(ColumnNames)) {
        sb.Append("ColumnNames: ");
        sb.Append(ColumnNames);
        sb.Append(System.Environment.NewLine);
      }
      if (Id.HasValue) {
        sb.Append("Id: ");
        sb.Append(Id.Value);
        sb.Append(System.Environment.NewLine);
      }
      if (!string.IsNullOrWhiteSpace(FilterDesc)) {
        sb.Append("FilterDesc: ");
        sb.Append(FilterDesc);
        sb.Append(System.Environment.NewLine);
      }
      if (!string.IsNullOrWhiteSpace(OrderByDesc)) {
        sb.Append("OrderByDesc: ");
        sb.Append(OrderByDesc);
        sb.Append(System.Environment.NewLine);
      }
      if (ItemsLoaded.HasValue) {
        sb.Append("ItemsLoaded: ");
        sb.Append(ItemsLoaded.Value);
        sb.Append(System.Environment.NewLine);
      }
      if (ItemsSkipped.HasValue) {
        sb.Append("ItemsSkipped: ");
        sb.Append(ItemsSkipped.Value);
        sb.Append(System.Environment.NewLine);
      }
      if (!string.IsNullOrWhiteSpace(Value)) {
        sb.Append("Values: ");
        sb.Append(Value);
        sb.Append(System.Environment.NewLine);
      }
      if (Attachments != null && Attachments.Length > 0) {
        sb.Append("Attachments: ");
        sb.Append(string.Join(",", Attachments.Select(x => x.FileName)));
      }
      string testStr = sb.ToString();
      if (testStr != null && testStr.Length > lengthLimit) //for now just hard code this as 3000
        testStr = testStr.Substring(0, lengthLimit);      
      return testStr;
    }
  }
}