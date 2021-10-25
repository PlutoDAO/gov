using System;
using Answap.Gov.Domain;
using Answap.Gov.Test.Helpers;
using Xunit;

namespace Answap.Gov.Test.Domain
{
    public class VoteTest
    {
        [Fact]
        public void TestCreateVote()
        {
            var ani = AssetHelper.GetAni();
            var vote = new Vote("FakeVoter", new Option("FOR"), ani, 1);
            
            Assert.Equal("FakeVoter", vote.Voter);
            Assert.Equal("FOR", vote.Option.Name);
            Assert.Equal(ani, vote.Asset);
            Assert.Equal(1, vote.Amount);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vote("FakeVoter", new Option("FOR"), ani, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vote("FakeVoter", new Option("FOR"), ani, -1));
        }
    }
}
