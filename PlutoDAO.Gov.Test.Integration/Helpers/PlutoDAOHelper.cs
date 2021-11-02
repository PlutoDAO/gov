using System.Net.Http;
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
    }
}
