using System;

namespace PlutoDAO.Gov.Application.Proposals.Responses
{
    public interface IProposalIdentifier
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
    }
}
