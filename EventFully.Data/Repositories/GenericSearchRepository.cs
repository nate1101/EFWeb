using EventFully.Models;
using EventFully.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventFully.Repositories
{
    public class GenericSearchRepository<T> : IGenericSearchRepository<T> where T : class
    {
        private EventfullyDBContext _dbContext;

        public GenericSearchRepository(EventfullyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SearchResponse<T> Search(Expression<Func<T, bool>> filterCriteria, Expression<Func<T, bool>> searchCriteria, Expression<Func<T, object>> primaryOrderBy, int take, int skip, string primarySortDirection)
        {
            try
            {
                IQueryable<T> dbQuery = _dbContext.Set<T>();

                SearchResponse<T> result = new SearchResponse<T>();

                if (filterCriteria != null && searchCriteria != null)
                {
                    result.iTotalRecords = dbQuery.Where(filterCriteria).Count();
                    result.iTotalDisplayRecords = dbQuery.Where(filterCriteria).Where(searchCriteria).Count();
                    if (primarySortDirection.Equals(Constant.SortDirections.Ascending))
                    {
                        if (take > 0)
                            result.Results = dbQuery.Where(filterCriteria).Where(searchCriteria).OrderBy(primaryOrderBy).Take(take).Skip(skip).ToList();
                        else
                            result.Results = dbQuery.Where(filterCriteria).Where(searchCriteria).OrderBy(primaryOrderBy).Skip(skip).ToList();
                    }
                    else
                    {
                        if (take > 0)
                            result.Results = dbQuery.Where(filterCriteria).Where(searchCriteria).OrderByDescending(primaryOrderBy).Take(take).Skip(skip).ToList();
                        else
                            result.Results = dbQuery.Where(filterCriteria).Where(searchCriteria).OrderByDescending(primaryOrderBy).Skip(skip).ToList();
                    }
                }
                else if (filterCriteria != null && searchCriteria == null)
                {
                    result.iTotalRecords = dbQuery.Where(filterCriteria).Count();
                    result.iTotalDisplayRecords = dbQuery.Where(filterCriteria).Count();
                    if (primarySortDirection.Equals(Constant.SortDirections.Ascending))
                        if (take > 0)
                            result.Results = dbQuery.Where(filterCriteria).OrderBy(primaryOrderBy).Take(take).Skip(skip).ToList();
                        else
                            result.Results = dbQuery.Where(filterCriteria).OrderBy(primaryOrderBy).Skip(skip).ToList();
                    else
                    {
                        if (take > 0)
                            result.Results = dbQuery.Where(filterCriteria).OrderByDescending(primaryOrderBy).Take(take).Skip(skip).ToList();
                        else
                            result.Results = dbQuery.Where(filterCriteria).OrderByDescending(primaryOrderBy).Skip(skip).ToList();
                    }
                }
                else if (filterCriteria == null && searchCriteria != null)
                {
                    result.iTotalRecords = dbQuery.Where(searchCriteria).Count();
                    result.iTotalDisplayRecords = dbQuery.Where(searchCriteria).Count();
                    if (primarySortDirection.Equals(Constant.SortDirections.Ascending))
                        if (take > 0)
                            result.Results = dbQuery.Where(searchCriteria).OrderBy(primaryOrderBy).Take(take).Skip(skip).ToList();
                        else
                            result.Results = dbQuery.Where(searchCriteria).OrderBy(primaryOrderBy).Skip(skip).ToList();
                    else
                    {
                        if (take > 0)
                            result.Results = dbQuery.Where(searchCriteria).OrderByDescending(primaryOrderBy).Take(take).Skip(skip).ToList();
                        else
                            result.Results = dbQuery.Where(searchCriteria).OrderByDescending(primaryOrderBy).Skip(skip).ToList();
                    }
                }
                else
                {
                    result.iTotalRecords = dbQuery.Count();
                    result.iTotalDisplayRecords = dbQuery.Count();
                    if (primarySortDirection.Equals(Constant.SortDirections.Ascending))
                        if (take > 0)
                            result.Results = dbQuery.OrderBy(primaryOrderBy).Take(take).Skip(skip).ToList();
                        else
                            result.Results = dbQuery.OrderBy(primaryOrderBy).Skip(skip).ToList();
                    else
                    {
                        if (take == 0)
                            result.Results = dbQuery.OrderByDescending(primaryOrderBy).Take(take).Skip(skip).ToList();
                        else
                            result.Results = dbQuery.OrderByDescending(primaryOrderBy).Skip(skip).ToList();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //}
        }
    }
}
