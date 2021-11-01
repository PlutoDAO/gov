using PlutoDAO.Gov.Application.Assets;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.WhitelistedAssets
{
    public static class WhitelistedAssetMapper
    {
        public static WhitelistedAssetDto Map(WhitelistedAsset whitelistedAsset)
        {
            return new WhitelistedAssetDto
            {
                Asset = AssetMapper.Map(whitelistedAsset.Asset),
                Multiplier = whitelistedAsset.Multiplier
            };
        }
    }
}
