using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using stellar_dotnet_sdk;

namespace PlutoDAO.Gov.Test.Integration.Helpers
{
    public static class NetworkSetup
    {
        public static async Task Setup(Server server, TestConfiguration configuration)
        {
            StellarHelper.Server = server;
            StellarHelper.MasterAccount = KeyPair.FromSecretSeed(configuration.MasterAccountPrivate);

            await CreatePlutoDAOAccounts(configuration);

            PrintConfigurationValues(
                new[]
                {
                    configuration.PlutoDAOSenderKeyPair,
                    configuration.PlutoDAOReceiverKeyPair
                },
                new[]
                {
                    TestConfiguration.PlutoDAOSenderConfigKey,
                    TestConfiguration.PlutoDAOReceiverConfigKey
                },
                configuration.BaseConfigFile
            );
        }

        private static async Task CreatePlutoDAOAccounts(TestConfiguration configuration)
        {
            // PLUTODAO SENDER
            configuration.PlutoDAOSenderKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.PlutoDAOSenderConfigKey,
                "Main PlutoDAO Sender Account", configuration.PlutoDAOSenderPrivate
            );
            configuration.PlutoDAOSenderPublic = configuration.PlutoDAOSenderKeyPair.AccountId;
            configuration.PlutoDAOSenderPrivate = configuration.PlutoDAOSenderKeyPair.SecretSeed;

            // PLUTODAO RECEIVER
            configuration.PlutoDAOReceiverKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.PlutoDAOReceiverConfigKey,
                "Main PlutoDAO Receiver Account", configuration.PlutoDAOReceiverPrivate
            );
            configuration.PlutoDAOReceiverPublic = configuration.PlutoDAOReceiverKeyPair.AccountId;
            configuration.PlutoDAOReceiverPrivate = configuration.PlutoDAOReceiverKeyPair.SecretSeed;
        }

        private static void PrintConfigurationValues(
            IReadOnlyCollection<KeyPair> accountKeyPairs,
            string[] descriptions,
            string baseConfigFile
        )
        {
            var template = File.ReadAllText(baseConfigFile);
            var configObject = JObject.Parse(template);

            for (var i = 0; i < accountKeyPairs.Count; i++)
            {
                var configKeyBase = descriptions.ElementAt(i);
                var publicKeyConfigKey = $"{configKeyBase}_PUBLIC_KEY";
                var privateKeyConfigKey = $"{configKeyBase}_PRIVATE_KEY";
                var keyPair = accountKeyPairs.ElementAt(i);
                var publicKey = keyPair.AccountId;
                var privateKey = keyPair.SecretSeed;
                Environment.SetEnvironmentVariable(publicKeyConfigKey, publicKey);
                Environment.SetEnvironmentVariable(privateKeyConfigKey, privateKey);
                Console.WriteLine($"\"{publicKeyConfigKey}\" : \"{publicKey}\",");
                Console.WriteLine($"\"{privateKeyConfigKey}\" : \"{privateKey}\",");
                configObject.Add(publicKeyConfigKey, publicKey);
                configObject.Add(privateKeyConfigKey, privateKey);
            }

            File.WriteAllText(@"./appsettings.test-result.json", configObject.ToString());
        }
    }
}
