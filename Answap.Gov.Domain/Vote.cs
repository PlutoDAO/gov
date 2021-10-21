namespace Answap.Gov.Domain
{
    public class Vote
    {
        public string Voter;
        public Option Option;
        public string Token;
        public decimal Value;
        
        public Vote(string voter, Option option, string token, decimal value)
        {
            Voter = voter;
            Option = option;
            Token = token;
            Value = value;
        }

    }
}