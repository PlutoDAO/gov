using System;

namespace PlutoDAO.Gov.Domain
{
    public class Option
    {
        public readonly string Name;
        
        public Option(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
        }
    }
}
