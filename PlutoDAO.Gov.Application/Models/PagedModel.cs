using PlutoDAO.Gov.Application.Extensions;
using System.Collections.Generic;

namespace PlutoDAO.Gov.Application.Models
{
    public class PagedModel<TModel> : IPage
    {
        const int MaxPageSize = 20;
        private int _pageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IList<TModel> Items { get; set; }
        public IDictionary<LinkedResourceType, LinkedResource> Links { get; set; }

        public PagedModel()
        {
            Items = new List<TModel>();
        }
    }
}
