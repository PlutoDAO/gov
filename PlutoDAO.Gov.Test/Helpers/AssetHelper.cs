using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Test.Helpers
{
    public class AssetHelper
    {
        public static Asset GetPlt()
        {
            return new Asset(new AccountAddress("GASBEY5ZIN2TMX2FGPCXA35BMPHA4DYLYKLNELYB2BNNEAK6UHGPTPT5"), "PLT");
        }

        public static Asset GetArs()
        {
            return new Asset(new AccountAddress("GBULTDG6BUINYKK3QDKB2MHXLK7U2ZHN42D4ILQE7IKV23K22QVD2SSK"), "ARS");
        }

        public static Asset GetUsdc()
        {
            return new Asset(new AccountAddress("GDFC47X4UKIAFMYV3EFRFSMDGIYQRUZGTCGATU6JX2D2M6S2KXRUHPUZ"), "USDC");
        }

        public static Asset GetFakeAsset()
        {
            return new Asset(new AccountAddress("FAKEPUBLICACCOUNT"), "ARS");
        }
    }
}
