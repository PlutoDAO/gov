namespace PlutoDAO.Gov.Infrastructure.Stellar
{
    public class SystemAccountConfiguration
    {
        public SystemAccountConfiguration(string micropaymentSenderPrivateKey, string micropaymentReceiverPrivateKey,
            string escrowPrivateKey, string resultsPrivateKey)
        {
            MicropaymentReceiverPrivateKey = micropaymentReceiverPrivateKey;
            MicropaymentSenderPrivateKey = micropaymentSenderPrivateKey;
            EscrowPrivateKey = escrowPrivateKey;
            ResultsPrivateKey = resultsPrivateKey;
        }

        public string MicropaymentSenderPrivateKey { get; }
        public string MicropaymentReceiverPrivateKey { get; }
        public string EscrowPrivateKey { get; }
        public string ResultsPrivateKey { get; }
    }
}
