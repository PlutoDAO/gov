namespace PlutoDAO.Gov.Application.Models
{
    interface IPage
    {
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
