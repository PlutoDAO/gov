using System;
using PlutoDAO.Gov.Domain;
using PlutoDAO.Gov.Test.Helpers;
using Xunit;

namespace PlutoDAO.Gov.Test.Domain
{
    public class VoteTest
    {
        [Fact]
        public void TestCreateVote()
        {
            var plt = AssetHelper.GetPlt();
            var vote = new Vote("FakeVoter", new Option("FOR"), plt, 1);
            
            Assert.Equal("FakeVoter", vote.Voter);
            Assert.Equal("FOR", vote.Option.Name);
            Assert.Equal(plt, vote.Asset);
            Assert.Equal(1, vote.Amount);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vote("FakeVoter", new Option("FOR"), plt, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vote("FakeVoter", new Option("FOR"), plt, -1));
        }
    }
}
