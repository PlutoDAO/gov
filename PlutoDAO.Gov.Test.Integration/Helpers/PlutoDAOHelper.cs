using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlutoDAO.Gov.Application.Proposals.Responses;
using stellar_dotnet_sdk;
using FormatException = System.FormatException;

namespace PlutoDAO.Gov.Test.Integration.Helpers
{
    public static class PlutoDAOHelper
    {
        public static async Task<ProposalResponse> GetProposalByAssetCode(HttpClient client, string assetCode)
        {
            var response = await client.GetAsync($"/proposal/{assetCode}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProposalResponse>(content);
        }

        public static async Task<HttpResponseMessage> SaveProposal(HttpClient client, TestConfiguration config,
            string requestContent)
        {
            var data = new StringContent(requestContent, Encoding.UTF8, "application/json");
            return await client.PostAsync("proposal", data);
        }

        public static async Task<ProposalIdentifier[]> GetList(HttpClient client, TestConfiguration config)
        {
            var response = await client.GetAsync("proposal/list");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProposalIdentifier[]>(content);
        }

        public static async Task<string> VoteDirect(HttpClient client, TestConfiguration config,
            string proposalId, string amount)
        {
            var content = await Vote(client, config, proposalId, amount, false);
            return content;
        }

        public static async Task<Transaction> VoteIntent(HttpClient client, TestConfiguration config,
            string proposalId, string amount)
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

        private static async Task<string> Vote(HttpClient client, TestConfiguration config,
            string proposalId, string amount, bool isIntent)
        {
            var voteRequestContent = $@"{{""voter"": ""{
                config.VoterPublic
            }"",""option"": {{""name"":""FOR""}}, ""asset"": {{ ""isNative"": true, ""code"": ""XLM"", ""issuer"": ""{
                ""
            }"" }}, ""amount"": ""{amount}"", ""privateKey"": ""{
                config.VoterPrivate
            }""}}";
            
            var data = new StringContent(voteRequestContent, Encoding.UTF8, "application/json");
            var requestUri = isIntent ? $"/{proposalId}/VoteIntent" : $"/{proposalId}/Vote";
            var response = await client.PostAsync(requestUri, data);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
