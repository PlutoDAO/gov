using System;
using Answap.Gov.Domain;
using Xunit;

namespace Answap.Gov.Test.Domain
{
    public class VoteTest
    {
        [Fact]
        public void TestCreateVote()
        {
            var vote = new Vote("FakeVoter", new Option("FOR"), "FakeToken", 1);
            
            Assert.Equal("FakeVoter", vote.Voter);
            Assert.Equal("FOR", vote.Option.Name);
            Assert.Equal("FakeToken", vote.Token);
            Assert.Equal(1, vote.Value);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vote("FakeVoter", new Option("FOR"), "FakeToken", 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vote("FakeVoter", new Option("FOR"), "FakeToken", -1));
        }
    }
}
