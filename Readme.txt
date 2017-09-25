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
updates:
- cleaning up codes.
- minor code reorganization.
- more SQL executions are encapsulated in the Extension.Database.SqlServer

bugs fixed:
- MetaItem contains BasicColoringList in the views. Changed to ColoringList. Fixed.
- User Edit allows null Working and Admin roles. Fixed.
- ExclusionLists do not work for Index, Details/Delete, and Filter. Fixed.
- API ValueActionFilter has invalid LINQ to Entity methods. Fixed, but requires further test.

-----------------------------------------------------------------------------------------------
v0.1.0.0 alpha first release