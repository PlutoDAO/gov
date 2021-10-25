using System;
using System.Collections.Generic;
using System.Linq;
using Answap.Gov.Domain.Exceptions;

namespace Answap.Gov.Domain
{
    public class Proposal
    {
        public string Name;
        public string Description;
        public string Creator;
        public DateTime Deadline;
        public DateTime Created;
        public WhitelistedAsset WhiteListedAssets;
        private Option[] Options;
        
        public Proposal(string name, string description, string creator, DateTime deadline, DateTime created, WhitelistedAsset whitelistedAsset)
        {
            Name = name;
            Description = description;
            Creator = creator;
            Deadline = deadline;
            Created = created;
            WhiteListedAssets = whitelistedAsset;
            Options = new []{
                new Option("FOR"), new Option("AGAINST")
            };
        }

        public ValidatedVote CastVote(Vote vote)
        {
            if(!Array.Exists(Options, option => option.Name == vote.Option.Name))
            {
                throw new InvalidOptionException($"The option {vote.Option.Name} is not valid in this proposal");
            }

            if (IsVoteClosed())
            {
                throw new DeadlinePassedException($"The deadline for this proposal has passed");
            }

            if (!WhiteListedAssets.ContainsAsset(vote.Asset))
            {
                throw new NotWhitelistedAssetException($"The selected asset is not allowed in this proposal");
            }
            
            return new ValidatedVote(vote.Voter, vote.Option, vote.Asset, vote.Amount);
        }
        
        public bool IsVoteClosed()
        {
            return DateTime.Now >= Deadline;
        }
        
        public Option DeclareWinner(ValidatedVote[] votes)
        {
            var voteCount = new Dictionary<Option, decimal>();
            for (int i = 0; i < votes.Length; i++)
            {
                if (voteCount.ContainsKey(votes[i].Option))
                {
                    voteCount[votes[i].Option] += votes[i].Amount * WhiteListedAssets.GetMultiplier(votes[i].Asset);
                }
                else
                {
                    voteCount[votes[i].Option] = votes[i].Amount * WhiteListedAssets.GetMultiplier(votes[i].Asset);
                }
            }

            return voteCount.OrderByDescending(pair => pair.Value).First().Key;
        }
    }
}
