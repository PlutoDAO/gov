using System.Linq;
using PlutoDAO.Gov.Application.Assets;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Application.WhitelistedAssets
{
    public class WhitelistedAssetMapper
    {
        public static WhitelistedAssetDto Map(WhitelistedAsset whitelistedAsset)
        {
            return whitelistedAsset.ToDictionary(kvp => AssetMapper.Map(kvp.Key), kvp => kvp.Value)
        }
    }
}
