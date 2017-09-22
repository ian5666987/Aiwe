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