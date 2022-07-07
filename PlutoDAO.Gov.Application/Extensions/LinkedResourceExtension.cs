using System.Collections.Generic;

namespace PlutoDAO.Gov.Application.Extensions
{
    public static class LinkedResourceExtension
    {
        public static void AddResourceLink(
            this ILinkedResource resources,
            LinkedResourceType resourceType,
            string routeUrl
        )
        {
            resources.Links ??= new Dictionary<LinkedResourceType, LinkedResource>();
            resources.Links[resourceType] = new LinkedResource(routeUrl);
        }
    }

    public record LinkedResource(string Href);

    public enum LinkedResourceType
    {
        None,
        Prev,
        Next
    }
}
