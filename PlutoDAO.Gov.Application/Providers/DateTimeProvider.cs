using System;

namespace PlutoDAO.Gov.Application.Providers
{
    public class DateTimeProvider
    {
        private readonly DateTime? _dateTime;

        public DateTimeProvider(DateTime fixedDateTime)
        {
            _dateTime = fixedDateTime;
        }

        public DateTime Now => _dateTime ?? DateTime.Now;
    }
}
