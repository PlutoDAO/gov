using PlutoDAO.Gov.Test.Integration.Fixtures;
using PlutoDAO.Gov.Test.Integration.Helpers;
using PlutoDAO.Gov.WebApi;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PlutoDAO.Gov.Test.Integration.Controllers
{
    [Collection("Stellar collection")]
    [TestCaseOrderer(
        "PlutoDAO.Gov.Test.Integration.Helpers.AlphabeticalOrderer",
        "PlutoDAO.Gov.Test.Integration"
    )]
    public class _3VoteResultTestController : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;

        public _3VoteResultTestController(
            CustomWebApplicationFactory<Startup> factory,
            StellarFixture fixture,
            ITestOutputHelper testOutputHelper
        )
        {
            _factory = factory;
            Config = fixture.Config;
            _testOutputHelper = testOutputHelper;
        }

        private TestConfiguration Config { get; }

        [Fact]
        public async Task Test_00_Get_Voting_Result()
        {
            await StellarHelper.CreateFeesPaymentClaimableBalance(
                Config.ProposalCreator1KeyPair,
                Config.PlutoDAOMicropaymentSenderKeyPair
            );
            var requestContent =
                $@"{{""name"": ""Test_00_Voting_Result"", ""description"": ""A testing proposal"", ""creator"": ""{Config.ProposalCreator1Public}""}}";

            var httpClient = _factory.CreateClient();

            await PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent);
            var proposalList = (await PlutoDAOHelper.GetList(httpClient, Config)).Items;
            var proposalId = proposalList.Last().Id;

            await StellarHelper.Pay(
                Config.PlutoDAOMicropaymentReceiverKeyPair,
                Config.PlutoDAOResultsKeyPair,
                proposalId,
                1
            );
            var proposal = await PlutoDAOHelper.GetProposalByAssetCode(httpClient, proposalId);
            Assert.Equal("FOR", proposal.VotingResult);
        }
    }
}
