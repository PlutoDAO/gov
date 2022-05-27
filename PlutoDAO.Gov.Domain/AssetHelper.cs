using System;
using System.Collections.Generic;

namespace PlutoDAO.Gov.Domain;

public static class AssetHelper
{
    public const string PusdCode = "pUSD";
    public const string UsdcCode = "USDC";
    public const string YusdcCode = "yUSDC";

    public static readonly Asset pUSD = GetPusdAsset();
    public static readonly Asset USDC = GetUsdcAsset();
    public static readonly Asset yUSDC = GetYusdcAsset();

    public static IEnumerable<WhitelistedAsset> GetWhitelistedAssets()
    {
        return new List<WhitelistedAsset>() {new(pUSD, 1), new(USDC, 1), new(yUSDC, 1)};
    }

    private static Asset GetPusdAsset()
    {
        var plutoDaoPusdIssuerAccountAddress =
            Environment.GetEnvironmentVariable("PLUTODAO_PUSD_ISSUER_ACCOUNT_PUBLIC_KEY") ??
            throw new ApplicationException("PLUTODAO_PUSD_ISSUER_ACCOUNT_PUBLIC_KEY NOT FOUND");

        return new Asset(new AccountAddress(plutoDaoPusdIssuerAccountAddress), PusdCode);
    }

    private static Asset GetUsdcAsset()
    {
        var usdcIssuerAccountAddress =
            Environment.GetEnvironmentVariable("USDC_ISSUER_ACCOUNT_PUBLIC_KEY") ??
            throw new ApplicationException("USDC_ISSUER_ACCOUNT_PUBLIC_KEY NOT FOUND");

        return new Asset(new AccountAddress(usdcIssuerAccountAddress), $"{UsdcCode}");
    }

    private static Asset GetYusdcAsset()
    {
        var yusdcIssuerAccountAddress =
            Environment.GetEnvironmentVariable("YUSDC_ISSUER_ACCOUNT_PUBLIC_KEY") ??
            throw new ApplicationException("YUSDC_ISSUER_ACCOUNT_PUBLIC_KEY NOT FOUND");

        return new Asset(new AccountAddress(yusdcIssuerAccountAddress), $"{YusdcCode}");
    }
}
