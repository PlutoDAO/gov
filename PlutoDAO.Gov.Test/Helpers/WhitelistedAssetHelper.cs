using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Test.Helpers
{
    public class WhitelistedAssetHelper
    {
        public static WhitelistedAsset GetWhitelistedAssets()
        {
            var plt = AssetHelper.GetPlt();
            var ars = AssetHelper.GetArs();
            var usdc = AssetHelper.GetUsdc();
            return new WhitelistedAsset(){{plt, 2}, {usdc, 1}, {ars, 0.5m}};
        }   
    }
}
