using System.Collections.Generic;

namespace PlutoDAO.Gov.Application.Extensions
{
    public interface ILinkedResource
    {
        public IDictionary<LinkedResourceType, LinkedResource> Links { get; set; }
    }
}
