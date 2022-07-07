using Newtonsoft.Json;
using PlutoDAO.Gov.Application.Dtos;
using PlutoDAO.Gov.Application.Proposals.Responses;
using stellar_dotnet_sdk;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FormatException = System.FormatException;

namespace PlutoDAO.Gov.Test.Integration.Helpers
{
    public static class PlutoDAOHelper
    {
        public static async Task<ProposalResponse> GetProposalByAssetCode(
            HttpClient client,
            string assetCode
        )
        {
            var response = await client.GetAsync($"/proposal/{assetCode}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProposalResponse>(content);
        }

        public static async Task<HttpResponseMessage> SaveProposal(
            HttpClient client,
            TestConfiguration config,
            string requestContent
        )
        {
            var data = new StringContent(requestContent, Encoding.UTF8, "application/json");
            return await client.PostAsync("proposal", data);
        }

        public static async Task<ListResponseDto> GetList(
            HttpClient client,
            TestConfiguration config,
            int limit = 6,
            int page = 1
        )
        {
            var response = await client.GetAsync($"/proposal?limit={limit}&page={page}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ListResponseDto>(content);
        }

        public static async Task<string> VoteDirect(
            HttpClient client,
            TestConfiguration config,
            string proposalId,
            string amount
        )
        {
            return await Vote(client, config, proposalId, amount, false);
        }

        public static async Task<Transaction> VoteIntent(
            HttpClient client,
            TestConfiguration config,
            string proposalId,
            string amount
        )
        {
            var content = await Vote(client, config, proposalId, amount, true);
            try
            {
                return Transaction.FromEnvelopeXdr(content);
            }
            catch (FormatException)
            {
                throw new Exception(content);
            }
        }

        private static async Task<string> Vote(
            HttpClient client,
            TestConfiguration config,
            string proposalId,
            string amount,
            bool isIntent
        )
        {
            var voteRequestContent =
                $@"{{""voter"": ""{config.VoterPublic}"",""option"": {{""name"":""FOR""}}, ""asset"": {{ ""isNative"": false, ""code"": ""pUSD"", ""issuer"": ""{config.PusdAsset.Issuer}"" }}, ""amount"": ""{amount}"", ""privateKey"": ""{config.VoterPrivate}""}}";

            var data = new StringContent(voteRequestContent, Encoding.UTF8, "application/json");
            var requestUri = isIntent ? $"/{proposalId}/VoteIntent" : $"/{proposalId}/Vote";
            var response = await client.PostAsync(requestUri, data);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
