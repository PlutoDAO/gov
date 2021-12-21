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
    public class _1GetProposalTestController : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;

        public _1GetProposalTestController(WebApplicationFactory<Startup> factory,
            StellarFixture fixture,
            ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            Config = fixture.Config;
            _testOutputHelper = testOutputHelper;
        }

        private TestConfiguration Config { get; }

        [Fact]
        public async Task Test_00_Get_Proposal_Name_List()
        {
            var request2Content =
                $@"{{""name"": ""Proposal2NameTest"", ""description"": ""A testing proposal"", ""creator"": ""Creator"", ""deadline"": ""2030-11-19T16:08:19.290Z"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": false, ""code"": ""pUSD"", ""issuer"": ""{
                    Config.PlutoDAOReceiverPublic
                }""}}, ""multiplier"": ""1""}}]}}";

            var request3Content =
                $@"{{""name"": ""Proposal3NameTest"", ""description"": ""A testing proposal"", ""creator"": ""Creator"", ""deadline"": ""2030-11-19T16:08:19.290Z"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": false, ""code"": ""pUSD"", ""issuer"": ""{
                    Config.PlutoDAOReceiverPublic
                }""}}, ""multiplier"": ""1""}}]}}";
            var httpClient = _factory.CreateClient();
            await PlutoDAOHelper.SaveProposal(httpClient, Config, request2Content);
            await PlutoDAOHelper.SaveProposal(httpClient, Config, request3Content);

            var proposalList = await PlutoDAOHelper.GetList(httpClient, Config);
            Assert.Equal(4, proposalList.Length);
            Assert.Equal("Proposal2NameTest", proposalList[2].Name);
            Assert.Equal("Proposal3NameTest", proposalList[3].Name);
        }
    }
}
