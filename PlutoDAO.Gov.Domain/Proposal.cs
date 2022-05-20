using System;
using System.Collections.Generic;
using System.Linq;
using PlutoDAO.Gov.Domain.Exceptions;

namespace PlutoDAO.Gov.Domain
{
    public class Proposal
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string Creator;
        public readonly IEnumerable<WhitelistedAsset> WhitelistedAssets;
        public DateTime Created;
        public DateTime Deadline;
        public readonly IEnumerable<Option> Options;

        public Proposal(string name, string description, string creator,
            IEnumerable<WhitelistedAsset> whitelistedAssets)
        {
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException("The proposal name field cannot be empty")
                : name;
            Description = string.IsNullOrWhiteSpace(description)
                ? throw new ArgumentException("The proposal description field cannot be empty")
                : description;
            Creator = string.IsNullOrWhiteSpace(creator)
                ? throw new ArgumentException("The proposal creator field cannot be empty")
                : creator;
            WhitelistedAssets = !whitelistedAssets.Any()
                ? throw new ArgumentException("The allowed asset list cannot be empty")
                : whitelistedAssets;
            Created = DateTime.UtcNow;
            Deadline = Created.Date.AddDays(31);
            Options = new[]
            {
                new Option("FOR"), new Option("AGAINST")
            };
        }

        public Proposal(string name, string description, string creator,
            IEnumerable<WhitelistedAsset> whitelistedAssets, DateTime created, DateTime deadline,
            IEnumerable<Option>? options)
        {
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException("The proposal name field cannot be empty")
                : name;
            Description = string.IsNullOrWhiteSpace(description)
                ? throw new ArgumentException("The proposal description field cannot be empty")
                : description;
            Creator = string.IsNullOrWhiteSpace(creator)
                ? throw new ArgumentException("The proposal creator field cannot be empty")
                : creator;
            WhitelistedAssets = !whitelistedAssets.Any()
                ? throw new ArgumentException("The allowed asset list cannot be empty")
                : whitelistedAssets;
            Created = created;
            Deadline = deadline < created
                ? throw new ArgumentException("The deadline cannot be before the creation date")
                : deadline;
            Options = options ?? new[]
            {
                new Option("FOR"), new Option("AGAINST")
            };
        }

        public ValidatedVote CastVote(Vote vote)
        {
            if (IsVoteClosed()) throw new DeadlinePassedException("The deadline for this proposal has passed");

            if (!Options.Contains(vote.Option))
                throw new InvalidOptionException($"The option {vote.Option.Name} is not valid in this proposal");

            if (!WhitelistedAssets.Any(asset => asset.Equals(vote.Asset)))
                throw new AssetNotWhitelistedException("The selected asset is not allowed in this proposal");

            return new ValidatedVote(vote);
        }

        public bool IsVoteClosed()
        {
            return DateTime.UtcNow >= Deadline;
        }

        public Option DeclareWinner(IEnumerable<ValidatedVote> votes)
        {
            var voteCount = new Dictionary<Option, decimal>();
            foreach (var vote in votes)
            {
                var multiplier = WhitelistedAssets.First(asset => asset.Equals(vote.Asset)).Multiplier;
                var calculatedVote = vote.Amount * multiplier;
                if (voteCount.ContainsKey(vote.Option))
                    voteCount[vote.Option] += calculatedVote;
                else
                    voteCount[vote.Option] = calculatedVote;
            }

            return voteCount.OrderByDescending(pair => pair.Value).First().Key;
        }
    }
}
