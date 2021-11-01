using System;

namespace PlutoDAO.Gov.Application.Exceptions
{
    public class ProposalNotFoundException : PlutoDAOException
    {
        public ProposalNotFoundException(string detail, Exception inner, string title, string type) : base(detail,
            inner, title, "PROPOSAL_NOT_FOUND")
        {
        }
    }
}
