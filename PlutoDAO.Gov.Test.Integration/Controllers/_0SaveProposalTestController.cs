using System.Globalization;
using System.Linq;
using System.Net;
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
    public class _0SaveProposalTestController : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;

        public _0SaveProposalTestController(WebApplicationFactory<Startup> factory,
            StellarFixture fixture,
            ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            Config = fixture.Config;
            _testOutputHelper = testOutputHelper;
        }

        private TestConfiguration Config { get; }

        [Fact]
        public async Task Test_00_Save_Proposal()
        {
            var requestContent =
                $@"{{""name"": ""Proposal1NameTest"", ""description"": ""A testing proposal"", ""creator"": ""Creator"", ""deadline"": ""2030-11-19T16:08:19.290Z"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": false, ""code"": ""pUSD"", ""issuer"": ""{
                    Config.PlutoDAOReceiverPublic
                }""}}, ""multiplier"": ""1""}}]}}";

            var httpClient = _factory.CreateClient();
            await PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent);

            var proposal = (await PlutoDAOHelper.GetProposals(httpClient, Config))[0];
            var whitelistedAssets = proposal.WhitelistedAssets.ToArray();

            Assert.Equal("Proposal1NameTest", proposal.Name);
            Assert.Equal("A testing proposal", proposal.Description);
            Assert.Equal("Creator", proposal.Creator);
            Assert.Equal("11/19/2030 16:08:19",
                proposal.Deadline.ToUniversalTime().ToString(CultureInfo.InvariantCulture));
            Assert.Equal("pUSD", whitelistedAssets[0].Asset.Code);
            Assert.Equal(Config.PlutoDAOReceiverPublic, whitelistedAssets[0].Asset.Issuer);
            Assert.Equal(1.0m, whitelistedAssets[0].Multiplier);
        }

        [Fact]
        public async Task Test_01_Save_Proposal_Throws_Error_If_Name_Exceeds_28_Characters()
        {
            var requestContent =
                $@"{{""name"": ""A proposal name that exceeds 28 characters"", ""description"": ""A testing proposal"", ""creator"": ""Creator"", ""deadline"": ""2030-11-19T16:08:19.290Z"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": false, ""code"": ""pUSD"", ""issuer"": ""{
                    Config.PlutoDAOReceiverPublic
                }""}}, ""multiplier"": ""1""}}]}}";

            var httpClient = _factory.CreateClient();
            var response = await PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("The proposal name cannot exceed 28 characters",
                response.Content.ReadAsStringAsync().Result);
        }
    }
}
