using Aibe.Helpers;
using System;
using System.Reflection;

namespace Aiwe.Helpers {
  public class AiweCheckerHelper : CheckerHelper { //This, unfortunately, is bound to the running assembly and thus must be declared in its own assembly everytime
    protected override Type getTableType(string prefix, string tableName) {
      return Type.GetType(string.Concat(prefix, tableName)); //it is always in the Aiwe.Models.DB. TODO not sure if it is the best way
    }

    protected override PropertyInfo getColumnPropertyInfo(string prefix, string tableName, string columnName) {
      Type tempClass = getTableType(prefix, tableName);
      object tableObject = Activator.CreateInstance(tempClass);
      return tempClass.GetProperty(columnName);
    }
  }
}
