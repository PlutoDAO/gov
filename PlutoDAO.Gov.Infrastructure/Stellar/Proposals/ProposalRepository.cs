using System;
using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Proposals
{
    public class ProposalRepository : IProposalRepository
    {
    public ProposalRepository(){}

    public async Task<Proposal> FindProposal(string address)
    {
        return new Proposal("nombre", "description", "creator", DateTime.Today, DateTime.Now, new WhitelistedAsset());
    }
    }
}
