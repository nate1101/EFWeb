using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EventFully.Models;

namespace EventFully.Services.Interfaces
{
    public interface IGenericSearchService<T> where T : class
    {
        SearchResponse<T> Search(Expression<Func<T, bool>> filterCriteria, Expression<Func<T, bool>> searchCriteria, dynamic orderBy, int take, int skip, string sortDirection = Constant.SortDirections.Ascending);
    }
}
