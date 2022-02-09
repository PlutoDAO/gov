namespace PlutoDAO.Gov.Application.Votes.Requests
{
    public interface IDirectVoteRequest : IVoteIntentRequest
    {
        string PrivateKey { get; set; }
    }
}
