namespace PlutoDAO.Gov.Application.Proposals.Requests
{
    public interface IProposalRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
    }
}
