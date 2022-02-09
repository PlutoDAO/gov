using PlutoDAO.Gov.Application.Assets;
using PlutoDAO.Gov.Application.Options;

namespace PlutoDAO.Gov.Application.Votes.Requests
{
    public interface IVoteIntentRequest
    {
        public decimal Amount { get; set; }
        public AssetDto Asset { get; set; }
        public OptionDto Option { get; set; }
        public string Voter { get; set; }
    }
}
