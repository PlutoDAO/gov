namespace PlutoDAO.Gov.Domain
{
    public class ValidatedVote
    {
        public decimal Amount;
        public Asset Asset;
        public Option Option;
        public string Voter;

        public ValidatedVote(Vote vote)
        {
            Voter = vote.Voter;
            Option = vote.Option;
            Asset = vote.Asset;
            Amount = vote.Amount;
        }
    }
}
