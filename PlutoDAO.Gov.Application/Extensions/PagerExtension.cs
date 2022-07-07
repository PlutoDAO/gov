using PlutoDAO.Gov.Application.Dtos;
using PlutoDAO.Gov.Application.Models;
using PlutoDAO.Gov.Application.Proposals.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlutoDAO.Gov.Application.Extensions
{
    public static class PagerExtension
    {
        public static async Task<ListResponseDto> Paginate<TModel>(
            IQueryable<TModel> query,
            int page,
            int limit
        )
        {
            var paged = new PagedModel<TModel>();

            page = (page < 0) ? 1 : page;

            paged.CurrentPage = page;
            paged.PageSize = limit;

            paged.TotalItems = query.Count();

            var startRow = (page - 1) * limit;
            paged.Items = query.Skip(startRow).Take(limit).ToList();

            paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)limit);

            var pagedModel = await Task.FromResult(paged);

            return new ListResponseDto
            {
                CurrentPage = pagedModel.CurrentPage,
                TotalItems = pagedModel.TotalItems,
                TotalPages = pagedModel.TotalPages,
                Items = (List<ProposalIdentifier>)pagedModel.Items
            };
        }
    }
}
