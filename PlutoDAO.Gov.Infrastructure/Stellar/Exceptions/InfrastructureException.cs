using System;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Exceptions
{
    public abstract class InfrastructureException : ApplicationException
    {
        protected InfrastructureException(string message) : base(message)
        {
        }
    }
}
