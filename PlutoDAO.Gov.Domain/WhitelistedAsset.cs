using System.Collections.Generic;
using System.Linq;

namespace PlutoDAO.Gov.Domain
{
    public class WhitelistedAsset : Dictionary<Asset, decimal>
    {
        public WhitelistedAsset()
        {
        }

        public bool ContainsAsset(Asset asset)
        {
            return this.Any(a => a.Key.Equals(asset));
        }

        public decimal GetMultiplier(Asset asset)
        {
            return this.Single(a => a.Key.Equals(asset)).Value;
        }
    }
}
