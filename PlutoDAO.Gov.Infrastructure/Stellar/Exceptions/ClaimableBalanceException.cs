namespace PlutoDAO.Gov.Infrastructure.Stellar.Exceptions;

public class ClaimableBalanceException : InfrastructureException
{
    public ClaimableBalanceException(string message) : base(message)
    {
    }
}
