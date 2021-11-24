namespace PlutoDAO.Gov.Infrastructure.Stellar
{
    public class SystemAccountConfiguration
    {
        public SystemAccountConfiguration(string senderPrivateKey, string receiverPrivateKey)
        {
            ReceiverPrivateKey = receiverPrivateKey;
            SenderPrivateKey = senderPrivateKey;
        }

        public string SenderPrivateKey { get; }
        public string ReceiverPrivateKey { get; }
    }
}
