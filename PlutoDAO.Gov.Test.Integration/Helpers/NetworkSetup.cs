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
            await CreateProposalCreatorAccounts(configuration);

            PrintConfigurationValues(
                new[]
                {
                    configuration.PlutoDAOSenderKeyPair,
                    configuration.PlutoDAOReceiverKeyPair,
                    configuration.PlutoDAOEscrowKeyPair,

                    configuration.ProposalCreator1KeyPair,
                    configuration.ProposalCreator2KeyPair,
                    configuration.VoterKeyPair
                },
                new[]
                {
                    TestConfiguration.PlutoDAOSenderConfigKey,
                    TestConfiguration.PlutoDAOReceiverConfigKey,
                    TestConfiguration.PlutoDAOEscrowConfigKey,

                    TestConfiguration.ProposalCreator1ConfigKey,
                    TestConfiguration.ProposalCreator2ConfigKey,
                    TestConfiguration.VoterConfigKey
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

            // PLUTODAO ESCROW
            configuration.PlutoDAOEscrowKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.PlutoDAOEscrowConfigKey, "PlutoDAO escrow account",
                configuration.PlutoDAOEscrowPrivate);
            configuration.PlutoDAOEscrowPublic = configuration.PlutoDAOEscrowKeyPair.AccountId;
            configuration.PlutoDAOEscrowPrivate = configuration.PlutoDAOEscrowKeyPair.SecretSeed;
        }

        private static async Task CreateProposalCreatorAccounts(TestConfiguration configuration)
        {
            //PROPOSAL CREATOR 1
            configuration.ProposalCreator1KeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.ProposalCreator1ConfigKey, "Proposal1Creator account",
                configuration.ProposalCreator1Private);
            configuration.ProposalCreator1Public = configuration.ProposalCreator1KeyPair.AccountId;
            configuration.ProposalCreator1Private = configuration.ProposalCreator1KeyPair.SecretSeed;

            //PROPOSAL CREATOR 2
            configuration.ProposalCreator2KeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.ProposalCreator2ConfigKey, "Proposal2Creator account",
                configuration.ProposalCreator2Private);
            configuration.ProposalCreator2Public = configuration.ProposalCreator2KeyPair.AccountId;
            configuration.ProposalCreator2Private = configuration.ProposalCreator2KeyPair.SecretSeed;

            //VOTER
            configuration.VoterKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.VoterConfigKey, "Voter account",
                configuration.VoterPrivate);
            configuration.VoterPublic = configuration.VoterKeyPair.AccountId;
            configuration.VoterPrivate = configuration.VoterKeyPair.SecretSeed;
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
