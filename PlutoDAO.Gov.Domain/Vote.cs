using System;

namespace PlutoDAO.Gov.Domain
{
    public class Vote
    {
        public string Voter;
        public Option Option;
        public Asset Asset;
        public decimal Amount;
        
        public Vote(string voter, Option option, Asset asset, decimal amount)
        {
            Voter = voter;
            Option = option;
            Asset = asset;
            Amount = amount <= 0 ? throw new ArgumentOutOfRangeException(nameof(amount),$"Only positive amounts are allowed") : amount;
        }

    }
}
