using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Aiwe.Models.API {
  public class ApiRequestResult {
    public List<ApiRequestColumnInfo> Columns { get; set; } = new List<ApiRequestColumnInfo>();
    public List<ApiRequestRowInfo> Rows { get; set; } = new List<ApiRequestRowInfo>();
    public bool Success { get; set; }
    public string Message { get; set; }
    public ApiRequestResult() { }

    public ApiRequestResult(CheckedClientApiRequest checkedRequest, DataTable table) {
      try {
        if (table != null) {
          Success = true;
          int index = 0;
          foreach (DataColumn column in table.Columns) {
            ApiRequestColumnInfo cInfo = new ApiRequestColumnInfo {
              ColumnName = column.ColumnName,
              ColumnIndex = index++,
              DataType = column.DataType.ToString(),
            };
            Columns.Add(cInfo);
          }

          List<string> attachmentColumns = new List<string>();
          if(checkedRequest != null && checkedRequest.Meta != null)
            attachmentColumns = new List<string>(Columns
              .Where(x => checkedRequest.Meta.IsPictureColumn(x.ColumnName))
              .Select(x => x.ColumnName));
          List<string> nonPictureAttachmentColumns = new List<string>();
          if (checkedRequest != null && checkedRequest.Meta != null)
            nonPictureAttachmentColumns = new List<string>(Columns
              .Where(x => checkedRequest.Meta.IsNonPictureAttachmentColumn(x.ColumnName))
              .Select(x => x.ColumnName));
          if (nonPictureAttachmentColumns.Count > 0)
            attachmentColumns.AddRange(nonPictureAttachmentColumns);

          index = 0;
          foreach (DataRow row in table.Rows) {
            ApiRequestRowInfo rInfo = new ApiRequestRowInfo {
              RowIndex = index++,
              RowData = row.ItemArray.ToList(),
              RowColumnNames = Columns.Select(x => x.ColumnName).ToList(),
            };
            object cid = row[Aibe.DH.Cid];
            foreach (var attachmentColumn in attachmentColumns) { //if there is any picture link columns returned
              string fileName = row[attachmentColumn].ToString();
              if (!string.IsNullOrWhiteSpace(fileName)) { //and if it refers to something, try to read first
                try {
                  var folderPath = fileName.Contains("/") || fileName.Contains("\\") ? //if the fileName is "complex", for display, just take from whatever it is meant to be
                    System.Web.Hosting.HostingEnvironment.MapPath("~/" + Aibe.DH.DefaultAttachmentFolderName) :
                    System.Web.Hosting.HostingEnvironment.MapPath("~/" + Aibe.DH.DefaultAttachmentFolderName + "/" + checkedRequest.TableName + "/" + 
                      (checkedRequest.RequestType == ApiRequestType.SelectMany || checkedRequest.RequestType == ApiRequestType.Create ?  
                      cid?.ToString() : checkedRequest.Id.ToString())); //only if fileName is simple tableName and id are added in the loading path
                  Directory.CreateDirectory(folderPath);
                  var path = Path.Combine(folderPath, fileName);
                  string fileType = Path.GetExtension(path);
                  byte[] fileData = File.ReadAllBytes(path);
                  string fileDataString = Convert.ToBase64String(fileData);
                  ClientApiRequestAttachment attachment = new ClientApiRequestAttachment(
                    attachmentColumn, fileName, fileType, fileDataString);
                  if (rInfo.Attachments == null)
                    rInfo.Attachments = new List<ClientApiRequestAttachment>();
                  rInfo.Attachments.Add(attachment);
                } catch {
                }
              }
            }
            Rows.Add(rInfo);
          }
        }
      } catch (Exception ex) {
        Success = false;
        Message = ex.ToString();
      }
    }

    public ApiRequestResult(List<ApiRequestColumnInfo> columns, List<ApiRequestRowInfo> rows, bool success, string message) {
      Columns = columns;
      Rows = rows;
      Success = success;
      Message = message;
    }

  }

  public class ApiRequestColumnInfo {
    public string ColumnName { get; set; }
    public int ColumnIndex { get; set; }
    public string DataType { get; set; }
  }

  public class ApiRequestRowInfo {
    public int RowIndex { get; set; }
    public List<string> RowColumnNames { get; set; }
    public List<object> RowData { get; set; }
    public List<ClientApiRequestAttachment> Attachments { get; set; }

    public object this[string columnName] {
      get {
        return RowData[RowColumnNames.IndexOf(columnName)];
      }
      set {
        RowData[RowColumnNames.IndexOf(columnName)] = value;
      }
    }

  }
}
