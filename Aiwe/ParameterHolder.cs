using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aiwe {
  public class PH {
    public static bool IsJson = false; //sets this to true for JSON [API data type] and false for XML [API data type]
    public static string WebApiDataType;
    static PH() { //Initialization of all the parameters
      WebApiDataType = IsJson ? DH.JsonType : DH.XmlType;
    }
  }
}