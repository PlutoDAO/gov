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
