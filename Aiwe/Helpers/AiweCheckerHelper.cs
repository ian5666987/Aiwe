using Aibe.Helpers;
using System;
using System.Linq;
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

    protected override bool columnIsRequiredByAttribute(string prefix, string tableName, string columnName) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(prefix, tableName, columnName);
      CustomAttributeData cad = propertyInfo.CustomAttributes
        .FirstOrDefault(x => x.AttributeType.ToString().EndsWith("RequiredAttribute"));
      return cad != null;
    }

    protected override bool columnIsNullableByClass(string prefix, string tableName, string columnName) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(prefix, tableName, columnName);
      string propertyType = propertyInfo.PropertyType.ToString();
      return Aibe.DH.NullableIndicators.Any(x => propertyType.StartsWith(Aibe.DH.SharedPrefixDataType + x));
    }

    protected override int getStringLengthFor(string prefix, string tableName, string columnName) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(prefix, tableName, columnName);
      CustomAttributeData cad = propertyInfo.CustomAttributes
        .FirstOrDefault(x => x.AttributeType.ToString().EndsWith("StringLengthAttribute"));
      return (int)cad.ConstructorArguments[0].Value; //this should be Int32 type actually
    }
  }
}
