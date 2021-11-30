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
        public async Task Test_00_Get_Proposal()
        {
            var httpClient = _factory.CreateClient();
            var proposal = await PlutoDAOHelper.GetProposalByAddress(httpClient,"1");
            
            Assert.Equal("name",proposal.Name);
            Assert.Equal("description",proposal.Description);
            Assert.Equal("creator",proposal.Creator);
            Assert.True(proposal.WhitelistedAssets.Any());
        }
        
    }
}
