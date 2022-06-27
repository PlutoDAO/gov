using PlutoDAO.Gov.Application.Extensions;
using PlutoDAO.Gov.Application.Proposals.Responses;
using System.Collections.Generic;

namespace PlutoDAO.Gov.Application.Dtos
{
    public class ListResponseDto : ILinkedResource
    {
        public int CurrentPage { get; init; }
        public int TotalItems { get; init; }
        public int TotalPages { get; init; }
        public List<IProposalIdentifier> Items { get; init; }
        public IDictionary<LinkedResourceType, LinkedResource> Links { get; set; }
    }
}
