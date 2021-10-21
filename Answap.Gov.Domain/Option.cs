using System;

namespace Answap.Gov.Domain
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