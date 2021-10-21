namespace Answap.Gov.Domain
{
    public class ValidatedVote : Vote
    {
        public ValidatedVote(string voter, Option option, string token, decimal value) : base(voter, option, token, value)
        {
        }
    }
}
