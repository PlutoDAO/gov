using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Assets
{
    public class AssetDto : IAssetDto
    {
        private string _issuer = "";
        public bool IsNative { get; set; }
        public string Code { get; set; }

        public string Issuer
        {
            get => _issuer;
            set => _issuer = value == "STELLAR" ? "" : value;
        }

        public static explicit operator Asset(AssetDto assetDto)
        {
            return new Asset(new AccountAddress(assetDto._issuer), assetDto.Code, assetDto.IsNative);
        }
    }
}
