namespace PlutoDAO.Gov.Domain
{
    public class ValidatedVote
    {
        public string Voter;
        public Option Option;
        public Asset Asset;
        public decimal Amount;
        
        public ValidatedVote(Vote vote)
        {
            Voter = vote.Voter;
            Option =vote.Option;
            Asset = vote.Asset;
            Amount = vote.Amount;
        }
    }
}
