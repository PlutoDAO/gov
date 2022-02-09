using System;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Test.Helpers
{
    public static class ProposalHelper
    {
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
