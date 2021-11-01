using System;
using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Proposals
{
    public class ProposalRepository : IProposalRepository
    {
        public async Task<Proposal> FindProposal(string address)
        {
            return new Proposal(
                "name",
                "description",
                "creator",
                DateTime.Today,
                DateTime.Now,
                new[]
                {
                    new WhitelistedAsset(new Asset(new AccountAddress("STELLAR"), "XLM", true), 1)
                }
            );
        }
    }
}
