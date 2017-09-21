using Aibe.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Aibe.Helpers {
  public partial class ViewHelper {
    public static ViewPerPageQueryResult<TEntity> ProcessView<TEntity>(int? page, IOrderedQueryable<TEntity> allOrderedMatches, int itemsPerPage) {
      page = page == null ? 1 : page;
      int skippedItems = 0;
      int queryFound = allOrderedMatches.Any() ? allOrderedMatches.Count() : 0;
      decimal maxPage = Math.Ceiling((decimal)queryFound / itemsPerPage);
      page = (int)Math.Min(Math.Max(1, page.Value), maxPage);
      skippedItems = (page.Value - 1) * itemsPerPage;
      var matchResults = allOrderedMatches
        .Skip(skippedItems).Take(itemsPerPage); //take some each time
      ViewPerPageQueryResult<TEntity> result = new ViewPerPageQueryResult<TEntity>();
      result.Page = page;
      result.QueryCount = queryFound;
      result.ItemsPerPage = itemsPerPage;
      result.Results = matchResults;
      return result;
    }
  }

  public class ViewPerPageQueryResult<TEntity> {
    public int? Page { get; set; }
    public int QueryCount { get; set; }
    public int ItemsPerPage { get; set; }
    public IQueryable<TEntity> Results { get; set; }
  }
}