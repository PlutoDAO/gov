using PlutoDAO.Gov.Application.Assets;
using PlutoDAO.Gov.Application.Options;
using PlutoDAO.Gov.Application.Votes.Requests;

namespace PlutoDAO.Gov.WebApi.Request
{
    public class DirectVoteRequest : IDirectVoteRequest
    {
        public decimal Amount { get; set; }
        public AssetDto Asset { get; set; }
        public OptionDto Option { get; set; }
        public string Voter { get; set; }
        public string PrivateKey { get; set; }
    }
}
