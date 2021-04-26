using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EventFully.Models;

namespace EventFully.Repositories.Interfaces
{
    public interface IGenericSearchRepository<T> where T : class
    {
        SearchResponse<T> Search(Expression<Func<T, bool>> filterCriteria, Expression<Func<T, bool>> searchCriteria, Expression<Func<T, object>> orderBy, int take, int skip, string sortDirection);
    }
}
