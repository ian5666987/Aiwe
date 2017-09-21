using System.Collections.Generic;
using System.Linq;
using Extension.String;

namespace Aibe.Models.Core {
  public partial class TableValueRefInfo : BaseInfo {
    private static List<string> defaultComparators = new List<string> { "=" }; //as of now, only accepts the "=" comparator for taking reference from other tables
    private static List<string> nonAllowedStrings = new List<string> { ",", ";", ":", "--" }; //by right all but double dashes "--" should have been handled outside, but just in case
    private static string skip = "SKIP"; //keyword to skip checking the RefAnotherColumnName=ThisOtherColumnName part
    public string RefTableName { get; private set; }

    public string Column { get; private set; }
    public string CondColumn { get; private set; }
    public string CondComparator { get; private set; }
    public string CrossTableCondColumn { get; private set; }
    public string StaticCrossTableCondColumn { get; private set; }
    public bool CrossTableColumnIsStatic { get { return !string.IsNullOrWhiteSpace(StaticCrossTableCondColumn); } }
    public string AdditionalWhereClause { get; private set; }
    public bool CrossTableCheckIsSkipped { get; private set; }
    public TableValueRefInfo(string desc) : base (desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      var parts = desc.GetTrimmedNonEmptyParts(':');
      if (parts.Count < 2) //minimum contains of two parts
        return;
      IsValid = true;
      RefTableName = parts[0];
      Column = parts[1];
      if (parts.Count < 3)
        return;

      if (parts[2].EqualsIgnoreCaseTrim(skip))
        CrossTableCheckIsSkipped = true;
      else { //Only if cross table check is not skipped then we could check for the table reference validity, else, go directly to AdditionalWhereClause
        SimpleExpression exp = new SimpleExpression(parts[2], defaultComparators); //TODO as of now, only split by equality. Subsequently could be different.
        if (!exp.IsValid || exp.IsSingular) { //if it contains false expression, it cannot be singular too.
          IsValid = false; //revoke the validity
          return;
        }

        CondColumn = exp.LeftSide;
        CondComparator = exp.MiddleSign;
        string testRightSide = exp.RightSide.GetNonEmptyTrimmedInBetween("\"", "\"");
        if (!string.IsNullOrWhiteSpace(testRightSide)) //check if it is static
          StaticCrossTableCondColumn = testRightSide;
        else //then it must be dynamic
          CrossTableCondColumn = exp.RightSide;
      }

      if (parts.Count != 4) //parts cannot be more than 4
        return;

      //At this point, there is additional where clause, but it will not change the validity if wrong. It will simply be unused.
      if (nonAllowedStrings.Any(x => parts[3].IndexOf(x) != -1))
        return; //cannot continue if any of such things exist

      AdditionalWhereClause = parts[3];
    }
  }
}