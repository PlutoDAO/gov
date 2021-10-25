using Answap.Gov.Domain;

namespace Answap.Gov.Test.Helpers
{
    public class WhitelistedAssetHelper
    {
        public static WhitelistedAsset GetWhitelistedAssets()
        {
            var ani = AssetHelper.GetAni();
            var ars = AssetHelper.GetArs();
            var usdc = AssetHelper.GetUsdc();
            return new WhitelistedAsset(){{ani, 2}, {usdc, 1}, {ars, 0.5m}};
        }   
    }
}
