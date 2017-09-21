using System.Collections.Generic;
using System.Configuration;

namespace Aibe {
  public class DH { //can be extended as wanted
    //User related global items
    public const string DevFullName = "Ian Kamajaya";
    public const string DevDisplayName = "developer";
    public const string DevName = "ian@astriotech.com";
    public const string DevPass = "D3vel0p3r";
    public const string DevRole = "Developer";
    public const string MainAdminFullName = "Administrator";
    public const string MainAdminDisplayName = "admin";
    public const string MainAdminRole = "Main Administrator";
    public const string AdminRole = "Administrator";
    public const string AnonymousRole = "Anonymous";
    public const string MobileAppRole = "MobileApp";
    public readonly static List<string> AdminRoles = new List<string> { DevRole, MainAdminRole, AdminRole };
    public readonly static List<string> MainAdminRoles = new List<string> { DevRole, MainAdminRole };
    public readonly static List<string> AllowedAdminRoles = new List<string> { AdminRole };

    //Use to help link and actions
    public const string CreateActionName = "Create";
    public const string EditActionName = "Edit";
    public const string DeleteActionName = "Delete";
    public const string DetailsActionName = "Details";
    public const string IndexActionName = "Index";
    public const string AuthenticateActionName = "Authenticate";

    //Base data types
    public const string SharedPrefixDataType = "System.";
    public const string UnknownDataType = "Unknown";
    public const string StringDataType = "String";
    public const string BooleanDataType = "Boolean";
    public const string CharDataType = "Char";
    public const string DateTimeDataType = "DateTime";
    public const string ByteDataType = "Byte";
    public const string SByteDataType = "SByte";
    public const string Int16DataType = "Int16";
    public const string Int32DataType = "Int32";
    public const string Int64DataType = "Int64";
    public const string UInt16DataType = "UInt16";
    public const string UInt32DataType = "UInt32";
    public const string UInt64DataType = "UInt64";
    public const string SingleDataType = "Single";
    public const string DoubleDataType = "Double";
    public const string DecimalDataType = "Decimal";
    public const string NullableIndicator = "Nullable";

    //DB related
    public const string DataDBConnectionStringName = "CoreDataModel";
    public const string UserDBConnectionStringName = "DefaultConnection";
    public const string AccessLogTableName = "CoreAccessLog";
    public const string ActionLogTableName = "CoreActionLog";
    public const string ErrorLogTableName = "CoreErrorLog";
    public const string UserMapTableName = "CoreUserMap";
    public const string UserTableName = "AspNetUsers";
    public const string UserNameColumnName = "UserName";
    public const string AscOrderWord = "ASC";
    public const string DescOrderWord = "DESC";
    public readonly static List<string> CoreTableNames = new List<string> {
      AccessLogTableName, ActionLogTableName, ErrorLogTableName, UserMapTableName
    };
    public readonly static List<string> NonRecordedActions = new List<string> {
      IndexActionName, DetailsActionName
    };


    public static string DataDBConnectionString { get { return ConfigurationManager.ConnectionStrings[DataDBConnectionStringName].ConnectionString; } }
    public static string UserDBConnectionString { get { return ConfigurationManager.ConnectionStrings[UserDBConnectionStringName].ConnectionString; } }

    //Constants
    public const string TableNameParameterName = "tableName";
    public const string BaseAppendixName = "CommonData";
    public const string BaseFilterAppendixName = BaseAppendixName + "Filter";
    public const string BaseCreateEditAppendixName = BaseAppendixName + "CreateEdit";
    public const string CreateEditPictureLinkAppendixName = BaseCreateEditAppendixName + "PictureLink";
    public const string CreateEditTimeAppendixName = BaseCreateEditAppendixName + "Time";
    public const string FromName = "From";
    public const string ToName = "To";
    public const string FilterTimeAppendixFrontName = BaseFilterAppendixName + "Time";
    public const string FilterDateAppendixFrontName = BaseFilterAppendixName + "Date";
    public readonly static List<string> ExemptedFilterFormCollection = new List<string>() {
      BaseFilterAppendixName + "Page",
      BaseFilterAppendixName + "No",
      BaseFilterAppendixName + "Msg",
      BaseFilterAppendixName + "UserName",
      BaseFilterAppendixName + "Type"
    };
    public readonly static List<string> FilterTimeAppendixNames = new List<string> {
      FilterTimeAppendixFrontName + FromName,
      FilterTimeAppendixFrontName + ToName
    };
    public readonly static List<string> FilterDateAppendixNames = new List<string> {
      FilterDateAppendixFrontName + FromName,
      FilterDateAppendixFrontName + ToName
    };
    public readonly static List<string> NumberDataTypes = new List<string> {
      Int16DataType, Int32DataType, Int64DataType,
      UInt16DataType, UInt32DataType, UInt64DataType,
      DecimalDataType, DoubleDataType, SingleDataType,
      ByteDataType, SByteDataType,
    };
    public readonly static List<string> NullableIndicators = new List<string> {
      NullableIndicator, StringDataType
    };
    public readonly static List<string> EqualNotEqualOnlyDataTypes = new List<string> {
      StringDataType, CharDataType, BooleanDataType
    };

    public readonly static List<string> NumberTypeFilterColumns = new List<string> {
      BaseFilterAppendixName + Int16DataType + FromName,
      BaseFilterAppendixName + Int32DataType + FromName,
      BaseFilterAppendixName + Int64DataType + FromName,
      BaseFilterAppendixName + UInt16DataType + FromName,
      BaseFilterAppendixName + UInt32DataType + FromName,
      BaseFilterAppendixName + UInt64DataType + FromName,
      BaseFilterAppendixName + DecimalDataType + FromName,
      BaseFilterAppendixName + DoubleDataType + FromName,
      BaseFilterAppendixName + SingleDataType + FromName,
      BaseFilterAppendixName + ByteDataType + FromName,
      BaseFilterAppendixName + SByteDataType + FromName,

      BaseFilterAppendixName + Int16DataType + ToName,
      BaseFilterAppendixName + Int32DataType + ToName,
      BaseFilterAppendixName + Int64DataType + ToName,
      BaseFilterAppendixName + UInt16DataType + ToName,
      BaseFilterAppendixName + UInt32DataType + ToName,
      BaseFilterAppendixName + UInt64DataType + ToName,
      BaseFilterAppendixName + DecimalDataType + ToName,
      BaseFilterAppendixName + DoubleDataType + ToName,
      BaseFilterAppendixName + SingleDataType + ToName,
      BaseFilterAppendixName + ByteDataType + ToName,
      BaseFilterAppendixName + SByteDataType + ToName,
    };
    public readonly static List<string> BooleanTypeFilterColumns = new List<string>() {
      BaseFilterAppendixName + BooleanDataType };
    public readonly static List<string> CharTypeFilterColumns = new List<string>() {
      BaseFilterAppendixName + CharDataType };
  }
}