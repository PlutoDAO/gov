using PlutoDAO.Gov.Application.Assets;

namespace PlutoDAO.Gov.Application.WhitelistedAssets
{
    public class WhitelistedAssetDto : IWhitelistedAssetDto
    {
        public AssetDto Asset { get; set; }
        public decimal Multiplier { get; set; }
    }
}
