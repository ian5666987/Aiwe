using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

//http://stackoverflow.com/questions/28155169/how-to-programmatically-choose-a-constructor-during-deserialization
namespace Aiwe.Models.API {
  public class ApiRequestResultConverter : JsonConverter {
    public override bool CanConvert(Type objectType) {
      return (objectType == typeof(ApiRequestResult));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
      JObject jo = JObject.Load(reader);
      var columns = jo["Columns"];
      var rows = jo["Rows"];
      var success = (bool)jo["Success"];
      var message = (string)jo["Message"];

      List<ApiRequestColumnInfo> cInfos = new List<ApiRequestColumnInfo>();
      foreach (var item in columns.AsEnumerable().ToList()) {
        ApiRequestColumnInfo cInfo = new ApiRequestColumnInfo();
        cInfo.ColumnName = (string)item["ColumnName"];
        cInfo.ColumnIndex = (int)item["ColumnIndex"];
        cInfo.DataType = (string)item["DataType"];
        cInfos.Add(cInfo);
      }

      List<ApiRequestRowInfo> rInfos = new List<ApiRequestRowInfo>();
      foreach (var item in rows.AsEnumerable().ToList()) {
        ApiRequestRowInfo rInfo = new ApiRequestRowInfo();

        rInfo.RowIndex = (int)item["RowIndex"];

        List<string> columnNames = new List<string>();
        var columnNameRaws = item["RowColumnNames"];
        columnNames.AddRange(columnNameRaws.Values<string>());
        rInfo.RowColumnNames = columnNames;

        List<object> rowDatas = new List<object>();
        var rowDataRaws = item["RowData"];
        rowDatas.AddRange(rowDataRaws.Values<object>());
        rInfo.RowData = rowDatas;

        //Additional attachments are needed so that picture data are also obtained
        List<ClientApiRequestAttachment> attachments = new List<ClientApiRequestAttachment>();
        var attachmentRaws = item["Attachments"]; //This is a JToken
        foreach (var attachmentRaw in attachmentRaws.AsEnumerable().ToList()) {
          ClientApiRequestAttachment attachment = new ClientApiRequestAttachment(
            (string)attachmentRaw["ColumnName"],
            (string)attachmentRaw["FileName"],
            (string)attachmentRaw["Format"],
            (string)attachmentRaw["Data"]
            );
          attachments.Add(attachment);
        }
        rInfo.Attachments = attachments;

        rInfos.Add(rInfo);
      }

      return new ApiRequestResult(cInfos, rInfos, success, message);
    }

    //Cannot write
    public override bool CanWrite { get { return false; } }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      throw new NotImplementedException();
    }

  }
}