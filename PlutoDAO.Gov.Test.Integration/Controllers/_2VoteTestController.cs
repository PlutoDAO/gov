using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PlutoDAO.Gov.Test.Integration.Fixtures;
using PlutoDAO.Gov.Test.Integration.Helpers;
using PlutoDAO.Gov.WebApi;
using Xunit;
using Xunit.Abstractions;

namespace PlutoDAO.Gov.Test.Integration.Controllers
{
    [Collection("Stellar collection")]
    [TestCaseOrderer("PlutoDAO.Gov.Test.Integration.Helpers.AlphabeticalOrderer", "PlutoDAO.Gov.Test.Integration")]
    public class _2VoteTestController : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;

        public _2VoteTestController(WebApplicationFactory<Startup> factory,
            StellarFixture fixture,
            ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            Config = fixture.Config;
            _testOutputHelper = testOutputHelper;
        }

        private TestConfiguration Config { get; }

        [Fact]
        public async Task Test_00_Vote()
        {
            await StellarHelper.CreateFeesPaymentClaimableBalance(Config.ProposalCreator1KeyPair,
                Config.PlutoDAOMicropaymentSenderKeyPair);

            var proposalRequestContent =
                $@"{{""name"": ""Proposal1NameTest"", ""description"": ""A testing proposal"", ""creator"": ""{
                    Config.ProposalCreator1Public
                }"", ""deadline"": ""2030-11-19T16:08:19.290Z"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": true, ""code"": ""XLM"", ""issuer"": ""{
                    ""
                }""}}, ""multiplier"": ""1""}}]}}";

            var httpClient = _factory.CreateClient();
            await PlutoDAOHelper.SaveProposal(httpClient, Config, proposalRequestContent);
            
            var transaction = await PlutoDAOHelper.VoteIntent(httpClient, Config, "PROP1", "50");
            Assert.Equal("PROP1 FOR", transaction.Memo.ToXdr().Text);

            await PlutoDAOHelper.VoteDirect(httpClient, Config, "PROP1","50");
            Assert.Equal("9949.9999900", await StellarHelper.GetAccountXlmBalance(Config.VoterPublic));

            var claimableBalanceResponse = await StellarHelper.GetClaimableBalances(Config.PlutoDAOEscrowPublic, Config.VoterPublic);
            Assert.Equal(Config.VoterPublic, claimableBalanceResponse.Sponsor);
            Assert.Equal("50.0000000", claimableBalanceResponse.Amount);
            Assert.Equal("native", claimableBalanceResponse.Asset);
            Assert.Equal(2, claimableBalanceResponse.Claimants.Length);
            Assert.Equal(Config.PlutoDAOEscrowPublic, claimableBalanceResponse.Claimants[0].Destination);
            Assert.Equal(Config.VoterPublic, claimableBalanceResponse.Claimants[1].Destination);
            Assert.Equal("2030-11-20T16:08:19Z", claimableBalanceResponse.Claimants[1].Predicate.Not.AbsBefore);
        }
    }
}
