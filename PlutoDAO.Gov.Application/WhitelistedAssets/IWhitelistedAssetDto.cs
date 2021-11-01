using System.Collections.Generic;
using PlutoDAO.Gov.Application.Assets;

namespace PlutoDAO.Gov.Application.WhitelistedAssets
{
    public interface IWhitelistedAssetDto
    {
        public Dictionary<AssetDto, decimal> WhitelistedAsset { get; set; }
    }
}
