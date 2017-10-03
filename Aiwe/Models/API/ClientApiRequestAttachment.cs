namespace Aiwe.Models.API {
  public class ClientApiRequestAttachment {
    public string ColumnName { get; private set; }
    public string FileName { get; private set; }
    public string Format { get; private set; }
    public string Data { get; private set; }
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