using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PlutoDAO.Gov.Application.Proposals.Responses;
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
            await StellarHelper.CreateFeesPaymentClaimableBalance(Config.ProposalCreator1KeyPair,
                Config.PlutoDAOMicropaymentSenderKeyPair);
            var requestContent =
                $@"{{""name"": ""Proposal1NameTest"", ""description"": ""A testing proposal"", ""creator"": ""{
                    Config.ProposalCreator1Public
                }"",""whitelistedAssets"": [{{""asset"": {{ ""isNative"": true, ""code"": ""XLM"", ""issuer"": ""{
                    ""
                }""}}, ""multiplier"": ""1""}}, {{""asset"": {{ ""isNative"": false, ""code"": ""PNT"", ""issuer"": ""GCDNASAGVK2QYBB5P2KS75VG5YP7MOVAOUPCHAFLESX6WAI2Z46TNZPY""}}, ""multiplier"": ""2""}}]}}";

            var httpClient = _factory.CreateClient();

            await PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent);
            var proposalList = await PlutoDAOHelper.GetList(httpClient, Config);
            var proposal = await PlutoDAOHelper.GetProposalByAssetCode(httpClient, proposalList.Last().Id);
            var whitelistedAssets = proposal.WhitelistedAssets.ToArray();

            Assert.Equal("Proposal1NameTest", proposal.Name);
            Assert.Equal("A testing proposal", proposal.Description);
            Assert.Equal(Config.ProposalCreator1Public, proposal.Creator);
            Assert.Equal(proposal.Deadline, proposal.Created.Date.AddDays(31));
            Assert.Equal("XLM", whitelistedAssets[0].Asset.Code);
            Assert.Equal(1.0m, whitelistedAssets[0].Multiplier);
            Assert.Equal("9999.9991800", await StellarHelper.GetAccountXlmBalance(proposal.Creator));
            Assert.Equal("10000.0000000",
                await StellarHelper.GetAccountXlmBalance(Config.PlutoDAOMicropaymentSenderPublic));
        }

        [Fact]
        public async Task Test_01_Save_Proposal_Throws_Error_If_Name_Exceeds_28_Characters()
        {
            var requestContent =
                $@"{{""name"": ""A proposal name that exceeds 28 characters"", ""description"": ""A testing proposal"", ""creator"": ""Creator"", ""deadline"": ""2030-11-19T16:08:19.290Z"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": true, ""code"": ""XLM"", ""issuer"": ""{
                    ""
                }""}}, ""multiplier"": ""1""}}]}}";

            var httpClient = _factory.CreateClient();
            var response = await PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("The proposal name cannot exceed 28 characters",
                response.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public async Task Test_02_Save_Proposal_3500_Characters()
        {
            await StellarHelper.CreateFeesPaymentClaimableBalance(Config.ProposalCreator1KeyPair,
                Config.PlutoDAOMicropaymentSenderKeyPair);
            const string proposalDescription =
                "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222233333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333334444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555566666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666";

            var requestContent = $@"{{""name"": ""Proposal"", ""description"": ""{
                proposalDescription
            }"", ""creator"": ""{
                Config.ProposalCreator1Public
            }"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": true, ""code"": ""XLM"", ""issuer"": ""{
                ""
            }""}}, ""multiplier"": ""1""}}]}}";

            var httpClient = _factory.CreateClient();
            await PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent);
            var proposalList = await PlutoDAOHelper.GetList(httpClient, Config);
            var proposal = await PlutoDAOHelper.GetProposalByAssetCode(httpClient, proposalList.Last().Id);
            var whitelistedAssets = proposal.WhitelistedAssets.ToArray();

            Assert.Equal("Proposal", proposal.Name);
            Assert.Equal(proposalDescription, proposal.Description);
            Assert.Equal(Config.ProposalCreator1Public, proposal.Creator);
            Assert.Equal(proposal.Deadline, proposal.Created.Date.AddDays(31));
            Assert.Equal("XLM", whitelistedAssets[0].Asset.Code);
            Assert.Equal(1.0m, whitelistedAssets[0].Multiplier);
            Assert.Equal("9999.9930900", await StellarHelper.GetAccountXlmBalance(proposal.Creator));
            Assert.Equal("10000.0000000",
                await StellarHelper.GetAccountXlmBalance(Config.PlutoDAOMicropaymentSenderPublic));
        }

        [Fact]
        public async Task Test_03_Concurrently_Save_Two_Proposals()
        {
            var requestContent =
                $@"{{""name"": ""ConcurrentProposal1"", ""description"": ""A testing proposal"", ""creator"": ""{
                    Config.ProposalCreator1Public
                }"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": false, ""code"": ""pUSD"", ""issuer"": ""GCDNASAGVK2QYBB5P2KS75VG5YP7MOVAOUPCHAFLESX6WAI2Z46TNZPY""}}, ""multiplier"": ""1""}}]}}";

            var requestContent2 =
                $@"{{""name"": ""ConcurrentProposal2"", ""description"": ""A testing proposal"", ""creator"": ""{
                    Config.ProposalCreator2Public
                }"", ""whitelistedAssets"": [{{""asset"": {{ ""isNative"": false, ""code"": ""pUSD"", ""issuer"": ""GCDNASAGVK2QYBB5P2KS75VG5YP7MOVAOUPCHAFLESX6WAI2Z46TNZPY""}}, ""multiplier"": ""1""}}]}}";

            await StellarHelper.CreateFeesPaymentClaimableBalance(Config.ProposalCreator1KeyPair,
                Config.PlutoDAOMicropaymentSenderKeyPair);
            await StellarHelper.CreateFeesPaymentClaimableBalance(Config.ProposalCreator2KeyPair,
                Config.PlutoDAOMicropaymentSenderKeyPair);

            var httpClient = _factory.CreateClient();

            Task.WaitAll(PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent),
                PlutoDAOHelper.SaveProposal(httpClient, Config, requestContent2));

            var proposalIdentifierList = await PlutoDAOHelper.GetList(httpClient, Config);

            Assert.Contains(proposalIdentifierList, p => p.Name == "ConcurrentProposal1");
            Assert.Contains(proposalIdentifierList, p => p.Name == "ConcurrentProposal2");
        }
    }
}
