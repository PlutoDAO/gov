using Answap.Gov.Domain;
using System;
using Answap.Gov.Domain.Exceptions;
using Answap.Gov.Test.Helpers;
using Xunit;

namespace Answap.Gov.Test.Domain
{
    public class ProposalTest
    {
        [Fact]
        public void TestCannotVoteIfVoteIsInvalid()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                ProposalHelper.GetCreationDate(),
                ProposalHelper.GetDeadline()
            );

            var invalidOption = new Option("NO");
            var invalidVote = new Vote(VoteHelper.GetFakeVoter(), invalidOption, VoteHelper.GetFakeToken(), 1000);

            Assert.Throws<InvalidOptionException>(() => proposal.CastVote(invalidVote));
        }

        [Fact]
        public void TestIsVoteClosedReturnsFalseIfDeadlineIsAhead()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                DateTime.Now.AddMinutes(1),
                DateTime.Now
            );
            
            Assert.False(proposal.IsVoteClosed());
        }
        
        [Fact]
        public void TestIsVoteClosedReturnsTrueIfDeadlineHasPassed()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                DateTime.Now.AddMinutes(-1),
                DateTime.Now
            );
            
            Assert.True(proposal.IsVoteClosed());
        }

        [Fact]
        public void TestDeclareWinnerFindsTheWinnerOption()
        {
            var vote1 = new Vote("voter1", new Option("FOR"), "token1", 1);
            var vote2 = new Vote("voter2", new Option("FOR"), "token2", 2);
            var vote3 = new Vote("voter3", new Option("AGAINST"), "token3", 2);
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                DateTime.Now.AddMinutes(-1),
                DateTime.Now
            );
            
            Assert.Equal("FOR", proposal.DeclareWinner(new []{vote1, vote2, vote3}));
        }
    }
}
