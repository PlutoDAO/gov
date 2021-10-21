using System;

namespace Answap.Gov.Domain.Exceptions
{
    public abstract class DomainException : ApplicationException
    {
        protected DomainException(string message) : base(message)
        {
        }
    }
}