namespace PlutoDAO.Gov.Infrastructure.Stellar
{
    public class SystemAccountConfiguration
    {
        public SystemAccountConfiguration(string senderPrivateKey, string receiverPrivateKey, string escrowPrivateKey)
        {
            ReceiverPrivateKey = receiverPrivateKey;
            SenderPrivateKey = senderPrivateKey;
            EscrowPrivateKey = escrowPrivateKey;
        }

        public string SenderPrivateKey { get; }
        public string ReceiverPrivateKey { get; }
        public string EscrowPrivateKey { get; }
    }
}
