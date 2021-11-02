using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PlutoDAO.Gov.Test.Integration.Helpers;
using PlutoDAO.Gov.WebApi;
using Xunit;
using Xunit.Abstractions;

namespace PlutoDAO.Gov.Test.Integration.Controllers
{
    public class _1GetProposalTestController : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;

        public _1GetProposalTestController(WebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            _testOutputHelper = testOutputHelper;
        }

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
