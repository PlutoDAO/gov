using PlutoDAO.Gov.Application.Assets;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.WhitelistedAssets
{
    public class WhitelistedAssetDto : IWhitelistedAssetDto
    {
        public AssetDto Asset { get; set; }
        public decimal Multiplier { get; set; }

        public static explicit operator WhitelistedAsset(WhitelistedAssetDto whitelistedAssetDto)
        {
            return new WhitelistedAsset((Asset) whitelistedAssetDto.Asset, whitelistedAssetDto.Multiplier);
        }
    }
}
