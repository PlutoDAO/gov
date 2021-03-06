using Microsoft.AspNetCore.Mvc.Testing;
using PlutoDAO.Gov.Test.Integration.Fixtures;
using PlutoDAO.Gov.Test.Integration.Helpers;
using PlutoDAO.Gov.WebApi;
using System;
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
    public class _1GetProposalTestController : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;

        public _1GetProposalTestController(
            WebApplicationFactory<Startup> factory,
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
        public async Task Test_00_Get_Proposal_Name_List()
        {
            await StellarHelper.CreateFeesPaymentClaimableBalance(
                Config.ProposalCreator1KeyPair,
                Config.PlutoDAOMicropaymentSenderKeyPair
            );
            await StellarHelper.CreateFeesPaymentClaimableBalance(
                Config.ProposalCreator2KeyPair,
                Config.PlutoDAOMicropaymentSenderKeyPair
            );

            var request2Content =
                $@"{{""name"": ""Proposal2NameTest"", ""description"": ""A testing proposal"", ""creator"": ""{Config.ProposalCreator1Public}""}}";

            var request3Content =
                $@"{{""name"": ""Proposal3NameTest"", ""description"": ""A testing proposal"", ""creator"": ""{Config.ProposalCreator2Public}""}}";

            var httpClient = _factory.CreateClient();
            await PlutoDAOHelper.SaveProposal(httpClient, Config, request2Content);
            await PlutoDAOHelper.SaveProposal(httpClient, Config, request3Content);

            var proposalList = (await PlutoDAOHelper.GetList(httpClient, Config)).Items;
            Assert.Equal(6, proposalList.Count);
            Assert.Equal("Proposal2NameTest", proposalList[4].Name);
            Assert.Equal("Proposal3NameTest", proposalList[5].Name);
            Assert.Equal(DateTime.Today.AddDays(31), proposalList[4].Deadline);
            Assert.Equal(DateTime.Today.AddDays(31), proposalList[5].Deadline);
        }
    }
}
