using System;
using System.Linq;
using PlutoDAO.Gov.Domain;
using PlutoDAO.Gov.Domain.Exceptions;
using PlutoDAO.Gov.Test.Helpers;
using Xunit;

namespace PlutoDAO.Gov.Test.Domain
{
    public class ProposalTest
    {
        [Fact]
        public void TestCreateProposal()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null
            );

            Assert.Equal("FakeProposal", proposal.Name);
            Assert.Equal("FakeDescription", proposal.Description);
            Assert.Equal("GAE2DCGCQX73JCSKYFU6GPMKAWTJGE5QWFY63HLL3LMVP7327OA3GCF5", proposal.Creator);
            Assert.IsType<DateTime>(proposal.Deadline);
            Assert.IsType<DateTime>(proposal.Created);
            Assert.Equal("PLT", proposal.WhitelistedAssets.ToArray()[0].Asset.Code);
            Assert.Equal(2m, proposal.WhitelistedAssets.ToArray()[0].Multiplier);
            Assert.Equal("GASBEY5ZIN2TMX2FGPCXA35BMPHA4DYLYKLNELYB2BNNEAK6UHGPTPT5",
                proposal.WhitelistedAssets.ToArray()[0].Asset.Issuer.Address);
            Assert.Equal("ARS", proposal.WhitelistedAssets.ToArray()[1].Asset.Code);
            Assert.Equal(0.5m, proposal.WhitelistedAssets.ToArray()[1].Multiplier);
            Assert.Equal("GBULTDG6BUINYKK3QDKB2MHXLK7U2ZHN42D4ILQE7IKV23K22QVD2SSK",
                proposal.WhitelistedAssets.ToArray()[1].Asset.Issuer.Address);
            Assert.Equal("USDC", proposal.WhitelistedAssets.ToArray()[2].Asset.Code);
            Assert.Equal(1m, proposal.WhitelistedAssets.ToArray()[2].Multiplier);
            Assert.Equal("GDFC47X4UKIAFMYV3EFRFSMDGIYQRUZGTCGATU6JX2D2M6S2KXRUHPUZ",
                proposal.WhitelistedAssets.ToArray()[2].Asset.Issuer.Address);

            Assert.Throws<ArgumentException>(() => new Proposal("",
                "FakeDescription",
                "ProposalHelper.GetFakeCreator()",
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null));

            Assert.Throws<ArgumentException>(() => new Proposal("     ",
                "FakeDescription",
                "ProposalHelper.GetFakeCreator()",
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null));

            Assert.Throws<ArgumentException>(() => new Proposal(null,
                "FakeDescription",
                "ProposalHelper.GetFakeCreator()",
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null));
        }

        [Fact]
        public void TestCannotVoteIfVoteIsInvalid()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null
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
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null
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
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null
            );
            var plt = AssetHelper.GetPlt();
            var vote = new Vote("FakeVoter", new Option("FOR"), plt, 1);
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
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Parse("2021-11-19T16:08:19.290"),
                DateTime.Parse("2021-11-20T16:08:19.290Z"),
                null
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
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null
            );
            var fakeAsset = AssetHelper.GetFakeAsset();
            var vote = new Vote("FakeVoter", new Option("FOR"), fakeAsset, 1);

            Assert.Throws<AssetNotWhitelistedException>(() => proposal.CastVote(vote));
        }

        [Fact]
        public void TestDeclareWinnerFindsTheWinnerOption()
        {
            var plt = AssetHelper.GetPlt();
            var validatedVote1 = new ValidatedVote(new Vote("voter1", new Option("FOR"), plt, 1));
            var validatedVote2 = new ValidatedVote(new Vote("voter2", new Option("FOR"), plt, 2));
            var validatedVote3 = new ValidatedVote(new Vote("voter3", new Option("AGAINST"), plt, 2));
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMilliseconds(10),
                null
            );

            var winningOption = proposal.DeclareWinner(new[] {validatedVote1, validatedVote2, validatedVote3});

            Assert.Equal("FOR", winningOption.Name);
        }

        [Fact]
        public void TestDeclareWinnerAllowsWhitelistedAssets()
        {
            var plt = AssetHelper.GetPlt();
            var ars = AssetHelper.GetArs();
            var usdc = AssetHelper.GetUsdc();
            var validatedVote1 = new ValidatedVote(new Vote("voter1", new Option("FOR"), plt, 2));
            var validatedVote2 = new ValidatedVote(new Vote("voter2", new Option("AGAINST"), ars, 2));
            var validatedVote3 = new ValidatedVote(new Vote("voter3", new Option("AGAINST"), usdc, 2));
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                null
            );

            var winningOption = proposal.DeclareWinner(new[] {validatedVote1, validatedVote2, validatedVote3});

            Assert.Equal("FOR", winningOption.Name);
        }
    }
}
