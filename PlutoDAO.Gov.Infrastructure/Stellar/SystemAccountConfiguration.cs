namespace PlutoDAO.Gov.Infrastructure.Stellar
{
    public class SystemAccountConfiguration
    {
        public SystemAccountConfiguration(string micropaymentSenderPrivateKey, string micropaymentReceiverPrivateKey, string escrowPrivateKey)
        {
            MicropaymentReceiverPrivateKey = micropaymentReceiverPrivateKey;
            MicropaymentSenderPrivateKey = micropaymentSenderPrivateKey;
            EscrowPrivateKey = escrowPrivateKey;
        }

        public string MicropaymentSenderPrivateKey { get; }
        public string MicropaymentReceiverPrivateKey { get; }
        public string EscrowPrivateKey { get; }
    }
}
