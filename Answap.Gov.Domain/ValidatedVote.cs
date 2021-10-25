namespace Answap.Gov.Domain
{
    public class ValidatedVote : Vote
    {
        public ValidatedVote(string voter, Option option, Asset asset, decimal amount) : base(voter, option, asset, amount)
        {
        }
    }
}
