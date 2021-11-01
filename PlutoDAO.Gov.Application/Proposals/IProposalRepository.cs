using PlutoDAO.Gov.Domain;
using System.Threading.Tasks;

namespace PlutoDAO.Gov.Application.Proposals
{
    public interface IProposalRepository
    {
        public Task<Proposal> FindProposal(string address);
    }
}
