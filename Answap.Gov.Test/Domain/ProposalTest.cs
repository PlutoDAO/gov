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
                ProposalHelper.GetDeadline(),
                WhitelistedAssetHelper.GetWhitelistedAssets()
            );

            var invalidOption = new Option("NO");
            var invalidVote = new Vote("FakeVoter", invalidOption, AssetHelper.GetFakeAsset(), 1000);

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
                DateTime.Now,
                WhitelistedAssetHelper.GetWhitelistedAssets()
            );
            
            Assert.False(proposal.IsVoteClosed());
        }
        
        [Fact]
        public void TestCastVote()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                DateTime.Now.AddMinutes(1),
                DateTime.Now,
                WhitelistedAssetHelper.GetWhitelistedAssets()
            );
            var ani = AssetHelper.GetAni();
            var vote = new Vote("FakeVoter", new Option("FOR"), ani, 1);
            var validatedVote = proposal.CastVote(vote);

            Assert.IsType<ValidatedVote>(validatedVote);
        }
        
        [Fact]
        public void TestIsVoteClosedReturnsTrueIfDeadlineHasPassed()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                DateTime.Now.AddMinutes(-1),
                DateTime.Now,
                WhitelistedAssetHelper.GetWhitelistedAssets()
            );
            
            Assert.True(proposal.IsVoteClosed());
        }

        [Fact]
        public void TestAssetNotWhitelistedThrowsError()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                DateTime.Now.AddMinutes(1),
                DateTime.Now,
                WhitelistedAssetHelper.GetWhitelistedAssets()
            );
            var fakeAsset = AssetHelper.GetFakeAsset();
            var vote = new Vote("FakeVoter", new Option("FOR"), fakeAsset, 1);

            Assert.Throws<AssetNotWhitelistedException>(() => proposal.CastVote(vote));
        }

        [Fact]
        public void TestDeclareWinnerFindsTheWinnerOption()
        {
            var ani = AssetHelper.GetAni();
            var validatedVote1 = new ValidatedVote("voter1", new Option("FOR"), ani, 1);
            var validatedVote2 = new ValidatedVote("voter2", new Option("FOR"), ani, 2);
            var validatedVote3 = new ValidatedVote("voter3", new Option("AGAINST"), ani, 2);
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                DateTime.Now.AddMinutes(-1),
                DateTime.Now,
                WhitelistedAssetHelper.GetWhitelistedAssets()
            );

            var winningOption = proposal.DeclareWinner(new[] {validatedVote1, validatedVote2, validatedVote3});
            
            Assert.Equal("FOR", winningOption.Name);
        }

        [Fact]
        public void TestDeclareWinnerAllowsWhitelistedAssets()
        {
            var ani = AssetHelper.GetAni();
            var ars = AssetHelper.GetArs();
            var usdc = AssetHelper.GetUsdc();
            var validatedVote1 = new ValidatedVote("voter1", new Option("FOR"), ani, 2);
            var validatedVote2 = new ValidatedVote("voter2", new Option("AGAINST"), ars, 2);
            var validatedVote3 = new ValidatedVote("voter3", new Option("AGAINST"), usdc, 2);
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                DateTime.Now.AddMinutes(1),
                DateTime.Now,
                WhitelistedAssetHelper.GetWhitelistedAssets()
            );
            
            var winningOption = proposal.DeclareWinner(new[] {validatedVote1, validatedVote2, validatedVote3});
            
            Assert.Equal("FOR", winningOption.Name);
        }
    }
}
