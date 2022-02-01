using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using stellar_dotnet_sdk;

namespace PlutoDAO.Gov.Test.Integration.Helpers
{
    public class TestConfiguration
    {
        private const string MasterAccountConfigKey = "MASTER_ACCOUNT";
        public const string PlutoDAOSenderConfigKey = "PLUTODAO_PROPOSAL_SENDER_ACCOUNT";
        public const string PlutoDAOReceiverConfigKey = "PLUTODAO_PROPOSAL_RECEIVER_ACCOUNT";
        public const string PlutoDAOEscrowConfigKey = "PLUTODAO_ESCROW_ACCOUNT";
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

            PlutoDAOSenderPublic = configuration.GetValue<string>(GetPublicConfigKey(PlutoDAOSenderConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(PlutoDAOSenderConfigKey), PlutoDAOSenderPublic);
            PlutoDAOSenderPrivate = configuration.GetValue<string>(GetPrivateConfigKey(PlutoDAOSenderConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(PlutoDAOSenderConfigKey), PlutoDAOSenderPrivate);
            if (PlutoDAOSenderPrivate != null)
                PlutoDAOSenderKeyPair = KeyPair.FromSecretSeed(PlutoDAOSenderPrivate);

            PlutoDAOReceiverPublic = configuration.GetValue<string>(GetPublicConfigKey(PlutoDAOReceiverConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(PlutoDAOReceiverConfigKey), PlutoDAOReceiverPublic);
            PlutoDAOReceiverPrivate = configuration.GetValue<string>(GetPrivateConfigKey(PlutoDAOReceiverConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(PlutoDAOReceiverConfigKey), PlutoDAOReceiverPrivate);
            if (PlutoDAOReceiverPrivate != null)
                PlutoDAOReceiverKeyPair = KeyPair.FromSecretSeed(PlutoDAOReceiverPrivate);
            
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
            
            VoterPublic = configuration.GetValue<string>(GetPublicConfigKey(VoterConfigKey));
            Environment.SetEnvironmentVariable(GetPublicConfigKey(VoterConfigKey), VoterPublic);
            VoterPrivate = configuration.GetValue<string>(GetPrivateConfigKey(VoterConfigKey));
            Environment.SetEnvironmentVariable(GetPrivateConfigKey(VoterConfigKey), VoterPrivate);
            if (VoterPrivate != null)
                VoterKeyPair = KeyPair.FromSecretSeed(VoterPrivate);
        }

        public string ConfigFile { get; }
        public string BaseConfigFile { get; }
        public string MasterAccountPrivate { get; }
        private string MasterAccountPublic { get; }
        public string PlutoDAOSenderPublic { get; set; }
        public string? PlutoDAOSenderPrivate { get; set; }
        public KeyPair PlutoDAOSenderKeyPair { get; set; } = null!;
        public string PlutoDAOReceiverPublic { get; set; }
        public string? PlutoDAOReceiverPrivate { get; set; }
        public KeyPair PlutoDAOReceiverKeyPair { get; set; } = null!;
        public string PlutoDAOEscrowPublic { get; set; }
        public string PlutoDAOEscrowPrivate { get; set; }
        public KeyPair PlutoDAOEscrowKeyPair { get; set; } = null!;
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
