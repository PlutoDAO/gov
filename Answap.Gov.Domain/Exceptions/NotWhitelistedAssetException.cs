namespace Answap.Gov.Domain.Exceptions
{
    public class NotWhitelistedAssetException : DomainException
    {
        public NotWhitelistedAssetException(string message) : base(message)
        {
        }
    }
}
