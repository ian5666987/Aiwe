using System;

namespace Aibe.Models {
  public partial class NavDataModel {
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int QueryCount { get; set; }
    public int MaxPage { get; set; }
    public int PrevPage { get; set; }
    public int Prev10Page { get; set; }
    public int Prev100Page { get; set; }
    public int NextPage { get; set; }
    public int Next10Page { get; set; }
    public int Next100Page { get; set; }
    public int ItemNoInPageFirst { get; set; }
    public int ItemNoInPageLast { get; set; }
    public string FirstKey { get; set; }
    public string LastKey { get; set; }
    public bool KeysUsed { get; set; }
    public bool DataNameUsed { get; set; }
    public string DataName { get; set; }
    public string ParentUri { get; set; }
    public NavDataModel(int currentPage, int itemsPerPage, int queryCount) {
      CurrentPage = currentPage;
      ItemsPerPage = itemsPerPage;
      QueryCount = queryCount;
      if (queryCount > 0) {
        MaxPage = (int)Math.Ceiling((decimal)QueryCount / ItemsPerPage);
        CurrentPage = Math.Max(1, Math.Min(MaxPage, CurrentPage));
        PrevPage = (int)Math.Max(1m, CurrentPage - 1);
        Prev10Page = (int)Math.Max(1m, CurrentPage - 10);
        Prev100Page = (int)Math.Max(1m, CurrentPage - 100);
        NextPage = (int)Math.Min((decimal)MaxPage, CurrentPage + 1);
        Next10Page = (int)Math.Min((decimal)MaxPage, CurrentPage + 10);
        Next100Page = (int)Math.Min((decimal)MaxPage, CurrentPage + 100);
        ItemNoInPageFirst = (CurrentPage - 1) * ItemsPerPage + 1;
        ItemNoInPageLast = Math.Min(CurrentPage * ItemsPerPage, QueryCount);
      }
    }
  }
}