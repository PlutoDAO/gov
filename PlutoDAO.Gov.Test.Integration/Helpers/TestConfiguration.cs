using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using stellar_dotnet_sdk;

namespace PlutoDAO.Gov.Test.Integration.Helpers
{
    public class TestConfiguration
    {
        private const string MasterAccountConfigKey = "MASTER_ACCOUNT";
        public const string PlutoDAOMicropaymentSenderConfigKey = "PLUTODAO_PROPOSAL_MICROPAYMENT_SENDER_ACCOUNT";
        public const string PlutoDAOMicropaymentReceiverConfigKey = "PLUTODAO_PROPOSAL_MICROPAYMENT_RECEIVER_ACCOUNT";
        public const string PlutoDAOEscrowConfigKey = "PLUTODAO_ESCROW_ACCOUNT";
        public const string PlutoDAOResultsConfigKey = "PLUTODAO_RESULTS_ACCOUNT";
        public const string PlutoDAOpUSDIssuerConfigKey = "PLUTODAO_PUSD_ISSUER_ACCOUNT";
        public const string YusdcAssetIssuerConfigKey = "YUSDC_ISSUER_ACCOUNT";
        public const string UsdcAssetIssuerConfigKey = "USDC_ISSUER_ACCOUNT";
        public const string ProposalCreator1ConfigKey = "TEST_PROPOSAL_CREATOR_1_ACCOUNT";
        public const string ProposalCreator2ConfigKey = "TEST_PROPOSAL_CREATOR_2_ACCOUNT";
        public const string VoterConfigKey = "TEST_VOTER_ACCOUNT";

        public TestConfiguration()
        {
            var fileToLoad = "appsettings.staging.json";
            var baseConfigFile = "appsettings.staging.json.dist";

            if (File.Exists("appsettings.test.json"))
            {
                baseConfigFile = "appsettings.test.json.dist";
                fileToLoad = "appsettings.test.json";
            }

            if (File.Exists("appsettings.dev.json"))
            {
                baseConfigFile = "appsettings.dev.json.dist";
                fileToLoad = "appsettings.dev.json";
            }

            ConfigFile = fileToLoad;
            BaseConfigFile = baseConfigFile;
            Console.WriteLine($"Loading file: {ConfigFile}");
            Console.WriteLine($"Base config file: {BaseConfigFile}");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(fileToLoad, false, true)
                .Build();

            var environment = configuration.GetValue<string>("ENVIRONMENT");
            Environment.SetEnvironmentVariable("ENVIRONMENT", environment);

            var passphrase = configuration.GetValue<string>("HORIZON_NETWORK_PASSPHRASE");
            Network.Use(new Network(passphrase));
            Environment.SetEnvironmentVariable("HORIZON_NETWORK_PASSPHRASE", passphrase);

            var baseFeeInXlm = configuration.GetValue<string>("BASE_FEE_IN_XLM");
            Environment.SetEnvironmentVariable("BASE_FEE_IN_XLM", baseFeeInXlm);

            TestHorizonUrl = configuration.GetValue<string>("HORIZON_URL");
            Environment.SetEnvironmentVariable("HORIZON_URL", TestHorizonUrl);

            MasterAccountPublic = configuration.GetValue<string>(GetPublicConfigKey(MasterAccountConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(MasterAccountConfigKey), MasterAccountPublic);
            MasterAccountPrivate = configuration.GetValue<string>(GetPrivateConfigKey(MasterAccountConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(MasterAccountConfigKey), MasterAccountPrivate);
            KeyPair.FromSecretSeed(MasterAccountPrivate);

            PlutoDAOMicropaymentSenderPublic =
                configuration.GetValue<string>(GetPublicConfigKey(PlutoDAOMicropaymentSenderConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(PlutoDAOMicropaymentSenderConfigKey),
                PlutoDAOMicropaymentSenderPublic);
            PlutoDAOMicropaymentSenderPrivate =
                configuration.GetValue<string>(GetPrivateConfigKey(PlutoDAOMicropaymentSenderConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(PlutoDAOMicropaymentSenderConfigKey),
                PlutoDAOMicropaymentSenderPrivate);
            if (PlutoDAOMicropaymentSenderPrivate != null)
                PlutoDAOMicropaymentSenderKeyPair = KeyPair.FromSecretSeed(PlutoDAOMicropaymentSenderPrivate);

            PlutoDAOMicropaymentReceiverPublic =
                configuration.GetValue<string>(GetPublicConfigKey(PlutoDAOMicropaymentReceiverConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(PlutoDAOMicropaymentReceiverConfigKey),
                PlutoDAOMicropaymentReceiverPublic);
            PlutoDAOMicropaymentReceiverPrivate =
                configuration.GetValue<string>(GetPrivateConfigKey(PlutoDAOMicropaymentReceiverConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(PlutoDAOMicropaymentReceiverConfigKey),
                PlutoDAOMicropaymentReceiverPrivate);
            if (PlutoDAOMicropaymentReceiverPrivate != null)
                PlutoDAOMicropaymentReceiverKeyPair = KeyPair.FromSecretSeed(PlutoDAOMicropaymentReceiverPrivate);

            YusdcAssetIssuerPublic = configuration.GetValue<string>(GetPublicConfigKey(YusdcAssetIssuerConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(YusdcAssetIssuerConfigKey), YusdcAssetIssuerPublic);
            YusdcAssetIssuerPrivate = configuration.GetValue<string>(GetPrivateConfigKey(YusdcAssetIssuerConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(YusdcAssetIssuerConfigKey), YusdcAssetIssuerPrivate);
            if (YusdcAssetIssuerPrivate != null)
                YusdcAssetIssuerKeyPair = KeyPair.FromSecretSeed(YusdcAssetIssuerPrivate);

            UsdcAssetIssuerPublic = configuration.GetValue<string>(GetPublicConfigKey(UsdcAssetIssuerConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(UsdcAssetIssuerConfigKey), UsdcAssetIssuerPublic);
            UsdcAssetIssuerPrivate = configuration.GetValue<string>(GetPrivateConfigKey(UsdcAssetIssuerConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(UsdcAssetIssuerConfigKey), UsdcAssetIssuerPrivate);
            if (UsdcAssetIssuerPrivate != null)
                UsdcAssetIssuerKeyPair = KeyPair.FromSecretSeed(UsdcAssetIssuerPrivate);

            PlutoDAOpUSDIssuerPublic = configuration.GetValue<string>(GetPublicConfigKey(PlutoDAOpUSDIssuerConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(PlutoDAOpUSDIssuerConfigKey), PlutoDAOpUSDIssuerPublic);
            PlutoDAOpUSDIssuerPrivate = configuration.GetValue<string>(GetPrivateConfigKey(PlutoDAOpUSDIssuerConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(PlutoDAOpUSDIssuerConfigKey), PlutoDAOpUSDIssuerPrivate);
            if (PlutoDAOpUSDIssuerPrivate != null)
                PlutoDAOpUSDIssuerKeyPair = KeyPair.FromSecretSeed(PlutoDAOpUSDIssuerPrivate);

            ProposalCreator1Public = configuration.GetValue<string>(GetPublicConfigKey(ProposalCreator1ConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(ProposalCreator1ConfigKey), ProposalCreator1Public);
            ProposalCreator1Private = configuration.GetValue<string>(GetPrivateConfigKey(ProposalCreator1ConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(ProposalCreator1ConfigKey), ProposalCreator1Private);
            if (ProposalCreator1Private != null)
                ProposalCreator1KeyPair = KeyPair.FromSecretSeed(ProposalCreator1Private);

            ProposalCreator2Public = configuration.GetValue<string>(GetPublicConfigKey(ProposalCreator2ConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(ProposalCreator2ConfigKey), ProposalCreator2Public);
            ProposalCreator2Private = configuration.GetValue<string>(GetPrivateConfigKey(ProposalCreator2ConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(ProposalCreator2ConfigKey), ProposalCreator2Private);
            if (ProposalCreator2Private != null)
                ProposalCreator2KeyPair = KeyPair.FromSecretSeed(ProposalCreator2Private);

            PlutoDAOEscrowPublic = configuration.GetValue<string>(GetPublicConfigKey(PlutoDAOEscrowConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(PlutoDAOEscrowConfigKey), PlutoDAOEscrowPublic);
            PlutoDAOEscrowPrivate = configuration.GetValue<string>(GetPrivateConfigKey(PlutoDAOEscrowConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(PlutoDAOEscrowConfigKey), PlutoDAOEscrowPrivate);
            if (PlutoDAOEscrowPrivate != null)
                PlutoDAOEscrowKeyPair = KeyPair.FromSecretSeed(PlutoDAOEscrowPrivate);

            PlutoDAOResultsPublic = configuration.GetValue<string>(GetPublicConfigKey(PlutoDAOResultsConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(PlutoDAOResultsConfigKey), PlutoDAOResultsPublic);
            PlutoDAOResultsPrivate = configuration.GetValue<string>(GetPrivateConfigKey(PlutoDAOResultsConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(PlutoDAOResultsConfigKey), PlutoDAOResultsPrivate);
            if (PlutoDAOResultsPrivate != null)
                PlutoDAOResultsKeyPair = KeyPair.FromSecretSeed(PlutoDAOResultsPrivate);

            VoterPublic = configuration.GetValue<string>(GetPublicConfigKey(VoterConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(VoterConfigKey), VoterPublic);
            VoterPrivate = configuration.GetValue<string>(GetPrivateConfigKey(VoterConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(VoterConfigKey), VoterPrivate);
            if (VoterPrivate != null)
                VoterKeyPair = KeyPair.FromSecretSeed(VoterPrivate);
        }

        public string ConfigFile { get; }
        public string BaseConfigFile { get; }
        public AssetTypeCreditAlphaNum PusdAsset { get; set; } = null!;
        public AssetTypeCreditAlphaNum YusdcAsset { get; set; } = null!;
        public AssetTypeCreditAlphaNum UsdcAsset { get; set; } = null!;
        public string MasterAccountPrivate { get; }
        private string MasterAccountPublic { get; }
        public string PlutoDAOMicropaymentSenderPublic { get; set; }
        public string? PlutoDAOMicropaymentSenderPrivate { get; set; }
        public KeyPair PlutoDAOMicropaymentSenderKeyPair { get; set; } = null!;
        public string PlutoDAOMicropaymentReceiverPublic { get; set; }
        public string? PlutoDAOMicropaymentReceiverPrivate { get; set; }
        public KeyPair PlutoDAOMicropaymentReceiverKeyPair { get; set; } = null!;
        public string PlutoDAOEscrowPublic { get; set; }
        public string PlutoDAOEscrowPrivate { get; set; }
        public KeyPair PlutoDAOEscrowKeyPair { get; set; } = null!;
        public string PlutoDAOResultsPublic { get; set; }
        public string PlutoDAOResultsPrivate { get; set; }
        public KeyPair PlutoDAOResultsKeyPair { get; set; } = null!;
        public string PlutoDAOpUSDIssuerPublic { get; set; }
        public string? PlutoDAOpUSDIssuerPrivate { get; set; }
        public KeyPair PlutoDAOpUSDIssuerKeyPair { get; set; } = null!;
        public string YusdcAssetIssuerPublic { get; set; }
        public string YusdcAssetIssuerPrivate { get; set; }
        public KeyPair YusdcAssetIssuerKeyPair { get; set; } = null!;
        public string UsdcAssetIssuerPublic { get; set; }
        public string UsdcAssetIssuerPrivate { get; set; }
        public KeyPair UsdcAssetIssuerKeyPair { get; set; } = null!;
        public string ProposalCreator1Public { get; set; }
        public string ProposalCreator1Private { get; set; }
        public KeyPair ProposalCreator1KeyPair { get; set; } = null!;
        public string ProposalCreator2Public { get; set; }
        public string ProposalCreator2Private { get; set; }
        public KeyPair ProposalCreator2KeyPair { get; set; } = null!;
        public string VoterPublic { get; set; }
        public string VoterPrivate { get; set; }
        public KeyPair VoterKeyPair { get; set; } = null!;
        public string TestHorizonUrl { get; }

        private static string GetPrivateConfigKey(string baseString)
        {
            return $"{baseString}_PRIVATE_KEY";
        }

        private static string GetPublicConfigKey(string baseString)
        {
            return $"{baseString}_PUBLIC_KEY";
        }
    }
}
