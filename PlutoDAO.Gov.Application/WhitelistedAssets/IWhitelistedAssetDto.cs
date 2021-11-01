using PlutoDAO.Gov.Application.Assets;

namespace PlutoDAO.Gov.Application.WhitelistedAssets
{
    public interface IWhitelistedAssetDto
    {
        public AssetDto Asset { get; set; }
        public decimal Multiplier { get; set; }
    }
}
