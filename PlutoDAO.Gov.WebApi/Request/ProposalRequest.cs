using PlutoDAO.Gov.Application.Proposals.Requests;

namespace PlutoDAO.Gov.WebApi.Request
{
    public class ProposalRequest : IProposalRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
    }
}
