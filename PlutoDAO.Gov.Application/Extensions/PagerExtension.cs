using PlutoDAO.Gov.Application.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlutoDAO.Gov.Application.Extensions
{
    public static class PagerExtension
    {
        public static Task<PagedModel<TModel>> Paginate<TModel>(
            IQueryable<TModel> query,
            int page,
            int limit
        ) where TModel : class
        {
            var paged = new PagedModel<TModel>();

            page = (page < 0) ? 1 : page;

            paged.CurrentPage = page;
            paged.PageSize = limit;

            paged.TotalItems = query.Count();

            var startRow = (page - 1) * limit;
            paged.Items = query.Skip(startRow).Take(limit).ToList();

            paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)limit);

            return Task.FromResult(paged);
        }
    }
}
