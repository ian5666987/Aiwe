namespace Aiwe.Models.API {
  public class ClientApiRequestAttachment {
    public string ColumnName { get; set; } //since v1.5.0.0 must have public setter so that it can be serialized using XML
    public string FileName { get; set; } //since v1.5.0.0 must have public setter so that it can be serialized using XML
    public string Format { get; set; } //since v1.5.0.0 must have public setter so that it can be serialized using XML
    public string Data { get; set; } //since v1.5.0.0 must have public setter so that it can be serialized using XML
    public ClientApiRequestAttachment() { } //since v1.5.0.0 must have parameterless constructor so that it can be serialized using XML
    public ClientApiRequestAttachment(string columnName, string fileName, string format, string data) {
      ColumnName = columnName;
      FileName = fileName;
      Format = format;
      Data = data;
    }

    public bool IsValid() {
      return !string.IsNullOrWhiteSpace(ColumnName) && 
        !string.IsNullOrWhiteSpace(FileName) &&
        !string.IsNullOrWhiteSpace(Format) && 
        !string.IsNullOrWhiteSpace(Data);
    }
  }
}