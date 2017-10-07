v0.7.0.0 alpha (7-Oct-2017 19:25)
bugs fixed:
- Coloring "now" not working. Fixed.
- UserNotFound and UserClaimNotFound is not distinguished. Fixed.

updates:
- Aibe and Aiwe now support localization!
- Aibe and Aiwe DataHolder expanded.
- Aibe and Aiwe implement localization.
- GetBooleanOptions() added.
- Wrong error messages corrected.
- Add default date, time, and date time format for Aibe.

-----------------------------------------------------------------------------------------------
v0.6.6.0 alpha (6-Oct-2017 00:17)
bugs fixed:
- Widths for ListColumns can be empty, not initialized correctly with three default values. Fixed.
- ListColumn javascript breaks due to ".contains()". Fixed.
- ListColumn add or delete does not work when the table has more than one ListColumn because javascript refers to all add subitems instead of the changed ones. Fixed.
- ListColumn ListType "check" legacy mapping is to "C". It is supposed to be to "LC". Fixed.
- ListColumn dynamic template fails due to wrong condition in the ListColumnInfo.GetRefDataValue. Fixed.

updates:
- ListColumn now supports table-referenced options for ListType "O"! Read the guideline for more details.

-----------------------------------------------------------------------------------------------
v0.6.5.0 alpha (4-Oct-2017 23:40)
updates:
- PictureColumn now supports height definition and non-retaining-aspect-ratio mode! Read the guideline for more details.
- PictureColumn now supports different size and rules for index and non-index pages! Read the guideline for more details.

-----------------------------------------------------------------------------------------------
v0.6.4.0 alpha (4-Oct-2017 17:40)
updates:
- ListColumn now supports different width for different columns! Read the guideline for more details.
- CreateEdit Common View is reorganized.

-----------------------------------------------------------------------------------------------
v0.6.3.1 alpha (3-Oct-2017 13:22)
bugs fixed:
- _SharedBody and index pages of Meta, Role, Team and User take NavDataModel from the old Aibe.Models.Core instead of Aibe.Models. Fixed.

-----------------------------------------------------------------------------------------------
v0.6.3.0 alpha (2-Oct-2017 18:00)
updates:
- AiweCheckerHelper components reduced and moved to Aibe's CheckerHelper - made it compatible with .Net 4.0.

-----------------------------------------------------------------------------------------------
v0.6.2.1 alpha (2-Oct-2017 13:52)
bugs fixed:
- _SharedNavigation takes NavDataModel from the old Aibe.Models.Core instead of Aibe.Models. Fixed.

-----------------------------------------------------------------------------------------------
v0.6.2.0 alpha (2-Oct-2017 13:30)
bugs fixed:
- Authentication does not take from right table. Fixed.

updates:
- Created CheckerHelper astract class in Aibe. AiweCheckerHelper is derived from CheckerHelper now.
- MetaItem's Columns related models are moved to more specific folder: "Core/Columns"
- BaseTableModel and FilterIndexModel are created/updated in Aibe.
- AiweTranslationHelper is reduced till only the Aiwe items left. Previous methods are moved to LogicHelper.
- ...Info names changed to Aiwe...Model
- Default MVC models are put under special folder: "Models/MVC"

-----------------------------------------------------------------------------------------------
v0.6.1.0 alpha (30-Sep-2017 20:40)
bugs fixed:
- Time value is not displayed in the common create-edit view. Fixed.
- DetailsPart does not show ScriptColumns. Fixed.

updates:
- AiweDropDownHelper removed from Aiwe, moved to Aibe MetaInfo. 
- LiveDropDownResult is moved to Aibe, LiveDropDownResultExtension created in Aiwe. 
- ColumnInfo and ScTableInfo are moved to Aibe. 
- MetaInfo can now create ColumnInfo

-----------------------------------------------------------------------------------------------
v0.6.0.0 alpha (29-Sep-2017 18:25)
updates:
- ListColumns are updated to support generic subcolumns making!
- Please read the "AIWE Table Making guideline v20170929.xlsx" or later version, "ListColumns" tab to see the complete explanation of the updates.
- Target framework is changed (downgraded) from .NET v4.6.2 to .NET v4.6.1. This is to prepare for further migration to ASP.NET Core 2.0 in the future.
  Unfortunately, migration to ASP.NET Core 2.0 is only supported up to .NET v4.6.1.

-----------------------------------------------------------------------------------------------
v0.5.0.0 alpha (28-Sep-2017 15:26)
updates:
- ScriptConstructorColumns and ScriptColumns are now available!
- The two columns are major updates! Make sure you read the "AIWE Table Making guideline v20170928.xlsx" or later version to understand them!
- TempData is no longer used for any data-passing between controllers and views except for "DataTableForExcel" in "Index.cshtml" View and its respective Controller's method

-----------------------------------------------------------------------------------------------
v0.4.2.1 alpha
bugs fixed:
- Cid is missing in Index view when column is excluded. Fixed.

-----------------------------------------------------------------------------------------------
v0.4.2.0 alpha
updates:
- EditShowOnlyColumns is now available!

-----------------------------------------------------------------------------------------------
v0.4.1.1 alpha
bugs fixed:
- First time dropdown is static instead of dynamic. Fixed.

-----------------------------------------------------------------------------------------------
v0.4.1.0 alpha
updates:
- ColumnSequence is now applied to all actions and filter.

-----------------------------------------------------------------------------------------------
v0.4.0.1 alpha
bugs fixed:
- ColumnAliases do not work. Fixed.
- ColumnSequence does not work. Fixed.

updates:
- Limits ColumnSequence only on Create-Edit actions

-----------------------------------------------------------------------------------------------
v0.4.0.0 alpha
bugs fixed:
- Meta filter only searches for table name. Fixed.

updates:
- Aibe and Aiwe now support ColumnSequence and ColumnAliases!

-----------------------------------------------------------------------------------------------
v0.3.1.0 alpha
bugs fixed:
- (Aibe) collections is more fragile towards non-existing key because it is now a dictionary. Fixed.

updates:
- FilterIndexModel is created in Aibe, CommonController in Aiwe is simplified further.

-----------------------------------------------------------------------------------------------
v0.3.0.2 alpha
updates:
- RegexCheckedColumn for ListColumn is now applied per item line, not to the entire string.

-----------------------------------------------------------------------------------------------
v0.3.0.1 alpha
bugs fixed:
- EqualsIgnoreCase is used on MetaController LINQ. Fixed.
- team, instead of teamModel is used on TeamController. Fixed.

-----------------------------------------------------------------------------------------------
v0.3.0.0 alpha
updates:
- More items put in the Aibe.dll.
- KeyInfo helper and extension no longer exists in Aiwe.
- Logic helper created to handle the logic and query script making parts in the Common Controller, allowing further separation.
- AiweQueryHelper is added to Aiwe to handle user-related query in the web-specific environment.
- AiweTranslationHelper added in the Aiwe.
- AiweCheckerHelper added in Aiwe. Checking using reflection cannot be put in the non-running assembly.

-----------------------------------------------------------------------------------------------
v0.2.1.0 alpha
updates:
- Aibe ViewHelper improved, AiweViewHelper reduced.

-----------------------------------------------------------------------------------------------
v0.2.0.0 alpha
updates:
- all Aibe files are removed and replaced with Aibe.dll!
- further separation between Aibe and Aiwe: the helpers, the models, and the filters.
- no Aibe item has 'namespace Aibe' anymore.
- core DB models are moved to Aiwe.
- Aibe does not use System.Data.Entity at all.
- IMetaItem is created for Aibe, used in MetaInfo and DataFilterHelper.ApplyMetaFilter. MetaItem is moved to Aiwe.
- allows "Release" version compilation.

-----------------------------------------------------------------------------------------------
v0.1.2.1 alpha
updates:
- quick fix on non-fully-qualified-named DH.

-----------------------------------------------------------------------------------------------
v0.1.2.0 alpha
updates:
- separation between Aibe.DH and Aiwe.DH.

-----------------------------------------------------------------------------------------------
v0.1.1.0 alpha
bugs fixed:
- MetaItem contains BasicColoringList in the views. Changed to ColoringList. Fixed.
- User Edit allows null Working and Admin roles. Fixed.
- ExclusionLists do not work for Index, Details/Delete, and Filter. Fixed.
- API ValueActionFilter has invalid LINQ to Entity methods. Fixed, but requires further test.

updates:
- cleaning up codes.
- minor code reorganization.
- more SQL executions are encapsulated in the Extension.Database.SqlServer

-----------------------------------------------------------------------------------------------
v0.1.0.0 alpha first release