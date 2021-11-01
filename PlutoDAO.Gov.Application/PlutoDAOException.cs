using System;

namespace PlutoDAO.Gov.Application
{
    public class PlutoDAOException : ApplicationException
    {
        public PlutoDAOException(string detail, Exception inner, string title, string type) : base(detail, inner)
        {
            Title = title;
            Type = type;
        }

        public string Title { get; }
        public string Type { get; }
    }
}
