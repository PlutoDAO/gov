using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlutoDAO.Gov.Application.Proposals.Responses;

namespace PlutoDAO.Gov.Test.Integration.Helpers
{
    public class PlutoDAOHelper
    {
        public static async Task<ProposalResponse> GetProposalByAddress(HttpClient client, string proposalAddress)
        {
            var response = await client.GetAsync($"/proposal/{proposalAddress}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProposalResponse>(content);
        }

        public static async Task<HttpResponseMessage> SaveProposal(HttpClient client, TestConfiguration config,
            string requestContent)
        {
            var data = new StringContent(requestContent, Encoding.UTF8, "application/json");
            return await client.PostAsync("proposal", data);
        }

        public static async Task<ProposalResponse[]> GetProposals(HttpClient client, TestConfiguration config)
        {
            var response = await client.GetAsync("proposal");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProposalResponse[]>(content);
        }

        public static async Task<ProposalIdentifier[]> GetList(HttpClient client, TestConfiguration config)
        {
            var response = await client.GetAsync("proposal/list");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProposalIdentifier[]>(content);
        }
    }
}
