using System;
using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Application.Votes.Requests;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Votes
{
    public class VoteService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IProposalRepository _proposalRepository;

        public VoteService(IVoteRepository voteRepository, IProposalRepository proposalRepository)
        {
            _voteRepository = voteRepository;
            _proposalRepository = proposalRepository;
        }

        public async Task Vote(IDirectVoteRequest request, string proposalId)
        {
            var proposal = await _proposalRepository.GetProposal(proposalId);
            var vote = new Vote(request.Voter, (Option) request.Option, (Asset) request.Asset,
                request.Amount);
            var validatedVote = proposal.CastVote(vote);
            await _voteRepository.SaveVote(validatedVote, proposal, proposalId, request.PrivateKey);
        }

        public async Task<string> Vote(IVoteIntentRequest request, string proposalId)
        {
            var proposal = await _proposalRepository.GetProposal(proposalId);
            var vote = new Vote(request.Voter, (Option) request.Option, (Asset) request.Asset,
                request.Amount);
            var validatedVote = proposal.CastVote(vote);
            return await _voteRepository.GetVoteIntent(validatedVote, proposal, proposalId);
        }
    }
}
