using System;
using Answap.Gov.Domain;

namespace Answap.Gov.Test.Helpers
{
    public static class ProposalHelper
    {
        public static Proposal ProposalMock()
        {
            var options = new []{
            new Option("FOR"), new Option("AGAINST")
            };
            return new Proposal(
                "ProposalMock", 
                "ABC", 
                "abcdef", 
                new DateTime(),
                new DateTime(), 
                options
                );
        }

        public static string GetName()
        {
            return "FakeProposal";
        }

        public static string GetDescription()
        {
            return "FakeDescription";
        }
        public static string GetFakeCreator()
        {
            return "GAE2DCGCQX73JCSKYFU6GPMKAWTJGE5QWFY63HLL3LMVP7327OA3GCF5";
        }

        public static DateTime GetCreationDate()
        {
            return DateTime.Now;
        }

        public static DateTime GetDeadline()
        {
            return DateTime.Today.AddHours(1);
        }
    }
}