namespace PlutoDAO.Gov.Application.Assets
{
    public interface IAssetDto
    {
        public bool IsNative { get; set; }
        public string Code { get; set; }
        public string Issuer { get; set; }
    }
}
