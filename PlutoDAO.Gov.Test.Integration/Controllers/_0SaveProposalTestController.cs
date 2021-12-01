using System.Globalization;
using System.Linq;
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
                $@"{{""name"": ""Proposal"", ""description"": ""A testing proposal"", ""creator"": ""Creator"", ""deadline"": ""2030-11-19T16:08:19.290Z"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": false, ""code"": ""PROPCOIN1"", ""issuer"": ""{
                    Config.PlutoDAOReceiverPublic
                }""}}, ""multiplier"": ""1""}}]}}";
            
            var httpClient = _factory.CreateClient();
            await PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent);

            var proposal = (await PlutoDAOHelper.GetProposals(httpClient, Config))[0];
            var whitelistedAssets = proposal.WhitelistedAssets.ToArray();
            
            Assert.Equal("Proposal", proposal.Name);
            Assert.Equal("A testing proposal", proposal.Description);
            Assert.Equal("Creator", proposal.Creator);
            Assert.Equal("11/19/2030 16:08:19", proposal.Deadline.ToUniversalTime().ToString(CultureInfo.InvariantCulture));
            Assert.Equal("PROPCOIN1", whitelistedAssets[0].Asset.Code);
            Assert.Equal(Config.PlutoDAOReceiverPublic, whitelistedAssets[0].Asset.Issuer);
            Assert.Equal(1.0m, whitelistedAssets[0].Multiplier);
        }
    }
}
