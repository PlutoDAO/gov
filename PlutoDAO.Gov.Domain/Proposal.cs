using System;
using System.Collections.Generic;
using System.Linq;
using PlutoDAO.Gov.Domain.Exceptions;

namespace PlutoDAO.Gov.Domain
{
    public class Proposal
    {
        public string Name;
        public string Description;
        public string Creator;
        public DateTime Deadline;
        public DateTime Created;
        public WhitelistedAsset WhitelistedAssets;
        private Option[] Options;
        
        public Proposal(string name, string description, string creator, DateTime deadline, DateTime created, WhitelistedAsset whitelistedAsset)
        {
            Name = name;
            Description = description;
            Creator = creator;
            Deadline = deadline;
            Created = created;
            WhitelistedAssets = whitelistedAsset;
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

            if (!WhitelistedAssets.ContainsAsset(vote.Asset))
            {
                throw new AssetNotWhitelistedException($"The selected asset is not allowed in this proposal");
            }
            
            return new ValidatedVote(vote);
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
                var calculatedVote = votes[i].Amount * WhitelistedAssets.GetMultiplier(votes[i].Asset);
                if (voteCount.ContainsKey(votes[i].Option))
                {
                    voteCount[votes[i].Option] += calculatedVote;
                }
                else
                {
                    voteCount[votes[i].Option] = calculatedVote;
                }
            }

            return voteCount.OrderByDescending(pair => pair.Value).First().Key;
        }
    }
}
