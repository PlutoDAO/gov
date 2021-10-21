namespace Answap.Gov.Domain.Exceptions
{
    public class InvalidOptionException : DomainException
    {
        public InvalidOptionException(string message) : base(message)
        {
        }
    }
}