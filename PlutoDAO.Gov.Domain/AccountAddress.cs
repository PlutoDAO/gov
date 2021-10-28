namespace PlutoDAO.Gov.Domain
{
    public class AccountAddress
    {
        public AccountAddress(string address)
        {
            Address = address;
        }

        public string Address { get; set; }

        public bool IsEmpty()
        {
            return Address == "";
        }
    }
}
