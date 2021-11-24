using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PlutoDAO.Gov.Test.Integration.Fixtures;
using PlutoDAO.Gov.Test.Integration.Helpers;
using PlutoDAO.Gov.WebApi;
using Xunit;
using Xunit.Abstractions;

namespace PlutoDAO.Gov.Test.Integration.Controllers._00_Base
{
    [Collection("Stellar collection")]
    [TestCaseOrderer("PlutoDAO.Gov.Test.Integration.Helpers.AlphabeticalOrderer", "PlutoDAO.Gov.Test.Integration")]
    public class AaBaseControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;
        private TestConfiguration Config { get; }
        
        public AaBaseControllerTest(
            WebApplicationFactory<Startup> factory,
            StellarFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            _testOutputHelper = testOutputHelper;
            Config = fixture.Config;
        }

        [Fact]
        public async Task Test_0_SETUP()
        {
            await StellarHelper.AddXlmFunds(Config.PlutoDAOSenderKeyPair);
            await StellarHelper.AddXlmFunds(Config.PlutoDAOReceiverKeyPair);
            Assert.True(true);
        }
    }
}