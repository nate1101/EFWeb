using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EventFully.Services.Interfaces;
using EventFully.Repositories.Interfaces;
using EventFully.Models;

namespace EventFully.Services
{
    public class GenericSearchService<T> : IGenericSearchService<T> where T : class
    {
        private readonly IGenericSearchRepository<T> _genericSearchRepository;

        public GenericSearchService(IGenericSearchRepository<T> genericSearchRepository)
        {
            _genericSearchRepository = genericSearchRepository;
        }

        public SearchResponse<T> Search(Expression<Func<T, bool>> filterCriteria, Expression<Func<T, bool>> searchCriteria, dynamic orderBy, int take, int skip, string sortDirection = Constant.SortDirections.Ascending)
        {
            return _genericSearchRepository.Search(filterCriteria, searchCriteria, orderBy, take, skip, sortDirection);
        }
    }
}
