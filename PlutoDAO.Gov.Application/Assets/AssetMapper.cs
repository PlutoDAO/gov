using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.Assets
{
    public static class AssetMapper
    {
        public static Asset MapFromDto(AssetDto assetDto)
        {
            return new Asset(new AccountAddress(assetDto.Issuer), assetDto.Code, assetDto.IsNative);
        }

        public static AssetDto Map(Asset asset)
        {
            return new AssetDto {Issuer = asset.Issuer.Address, Code = asset.Code, IsNative = asset.IsNative};
        }
    }
}
