using System;
using System.Collections.Generic;
using System.Linq;
using PlutoDAO.Gov.Domain.Exceptions;

namespace PlutoDAO.Gov.Domain
{
    public class Proposal
    {
        private readonly IEnumerable<Option> _options;
        public readonly string Creator;
        public readonly string Description;
        public readonly string Name;
        public readonly IEnumerable<WhitelistedAsset> WhitelistedAssets;
        public DateTime Created;
        public DateTime Deadline;

        public Proposal(string name, string description, string creator, DateTime deadline, DateTime? created,
            IEnumerable<WhitelistedAsset> whitelistedAssets)
        {
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException("The proposal name field cannot or empty")
                : name;
            Description = string.IsNullOrWhiteSpace(description)
                ? throw new ArgumentException("The proposal description field cannot or empty")
                : description;
            Creator = string.IsNullOrWhiteSpace(creator)
                ? throw new ArgumentException("The proposal creator field cannot or empty")
                : creator;
            Deadline = deadline < created
                ? throw new ArgumentException("The deadline cannot be before the creation date")
                : deadline;
            Created = created ?? DateTime.Now;
            WhitelistedAssets = !whitelistedAssets.Any()
                ? throw new ArgumentException("The allowed asset list cannot be empty")
                : whitelistedAssets;
            _options = new[]
            {
                new Option("FOR"), new Option("AGAINST")
            };
        }

        public ValidatedVote CastVote(Vote vote)
        {
            if (IsVoteClosed()) throw new DeadlinePassedException("The deadline for this proposal has passed");

            if (!_options.Contains(vote.Option))
                throw new InvalidOptionException($"The option {vote.Option.Name} is not valid in this proposal");

            if (!WhitelistedAssets.Any(asset => asset.Equals(vote.Asset)))
                throw new AssetNotWhitelistedException("The selected asset is not allowed in this proposal");

            return new ValidatedVote(vote);
        }

        public bool IsVoteClosed()
        {
            return DateTime.Now >= Deadline;
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
