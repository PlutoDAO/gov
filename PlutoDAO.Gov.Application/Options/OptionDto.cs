using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Options
{
    public class OptionDto : IOptionDto
    {
        public string Name { get; set; }
    
        public static explicit operator Option(OptionDto option)
        {
            return new Option(option.Name);
        }
    }
    
}
