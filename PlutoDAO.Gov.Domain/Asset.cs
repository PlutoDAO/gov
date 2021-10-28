using System;

namespace PlutoDAO.Gov.Domain
{
    public class Asset
    {
        public Asset(AccountAddress issuer, string code, bool isNative = false)
        {
            Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentNullException(nameof(code)) : code;
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            IsNative = isNative;
        }

        public string Code { get; }
        public bool IsNative { get; }
        public AccountAddress Issuer { get; }

        public bool Equals(Asset asset)
        {
            if (asset.IsNative && IsNative)
                return true;

            return asset.Code == Code && Issuer.Address == asset.Issuer.Address;
        }
    }
}
