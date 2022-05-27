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
            await CreateStableCoinIssuerAccounts(configuration);
            await FundVoter(configuration);

            PrintConfigurationValues(
                new[]
                {
                    configuration.PlutoDAOMicropaymentSenderKeyPair,
                    configuration.PlutoDAOMicropaymentReceiverKeyPair,
                    configuration.PlutoDAOEscrowKeyPair,
                    configuration.PlutoDAOResultsKeyPair,
                    configuration.UsdcAssetIssuerKeyPair,
                    configuration.YusdcAssetIssuerKeyPair,
                    configuration.PlutoDAOpUSDIssuerKeyPair,

                    configuration.ProposalCreator1KeyPair,
                    configuration.ProposalCreator2KeyPair,
                    configuration.VoterKeyPair
                },
                new[]
                {
                    TestConfiguration.PlutoDAOMicropaymentSenderConfigKey,
                    TestConfiguration.PlutoDAOMicropaymentReceiverConfigKey,
                    TestConfiguration.PlutoDAOEscrowConfigKey,
                    TestConfiguration.PlutoDAOResultsConfigKey,
                    TestConfiguration.UsdcAssetIssuerConfigKey,
                    TestConfiguration.YusdcAssetIssuerConfigKey,
                    TestConfiguration.PlutoDAOpUSDIssuerConfigKey,

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
            configuration.PlutoDAOMicropaymentSenderKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.PlutoDAOMicropaymentSenderConfigKey,
                "Main PlutoDAO Micropayment Sender Account", configuration.PlutoDAOMicropaymentSenderPrivate
            );
            configuration.PlutoDAOMicropaymentSenderPublic = configuration.PlutoDAOMicropaymentSenderKeyPair.AccountId;
            configuration.PlutoDAOMicropaymentSenderPrivate =
                configuration.PlutoDAOMicropaymentSenderKeyPair.SecretSeed;

            // PLUTODAO RECEIVER
            configuration.PlutoDAOMicropaymentReceiverKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.PlutoDAOMicropaymentReceiverConfigKey,
                "Main PlutoDAO Micropayment Receiver Account", configuration.PlutoDAOMicropaymentReceiverPrivate
            );
            configuration.PlutoDAOMicropaymentReceiverPublic =
                configuration.PlutoDAOMicropaymentReceiverKeyPair.AccountId;
            configuration.PlutoDAOMicropaymentReceiverPrivate =
                configuration.PlutoDAOMicropaymentReceiverKeyPair.SecretSeed;

            // PLUTODAO ESCROW
            configuration.PlutoDAOEscrowKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.PlutoDAOEscrowConfigKey, "PlutoDAO escrow account",
                configuration.PlutoDAOEscrowPrivate);
            configuration.PlutoDAOEscrowPublic = configuration.PlutoDAOEscrowKeyPair.AccountId;
            configuration.PlutoDAOEscrowPrivate = configuration.PlutoDAOEscrowKeyPair.SecretSeed;

            // PLUTODAO RESULTS
            configuration.PlutoDAOResultsKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.PlutoDAOResultsConfigKey, "PlutoDAO escrow account",
                configuration.PlutoDAOResultsPrivate);
            configuration.PlutoDAOResultsPublic = configuration.PlutoDAOResultsKeyPair.AccountId;
            configuration.PlutoDAOResultsPrivate = configuration.PlutoDAOResultsKeyPair.SecretSeed;
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

        private static async Task CreateStableCoinIssuerAccounts(TestConfiguration configuration)
        {
            configuration.PlutoDAOpUSDIssuerKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.PlutoDAOpUSDIssuerConfigKey,
                "Main PlutoDAO pUSD Issuer Account", configuration.PlutoDAOpUSDIssuerPrivate
            );
            configuration.PlutoDAOpUSDIssuerPublic = configuration.PlutoDAOpUSDIssuerKeyPair.AccountId;
            configuration.PlutoDAOpUSDIssuerPrivate = configuration.PlutoDAOpUSDIssuerKeyPair.SecretSeed;
            configuration.PusdAsset = new AssetTypeCreditAlphaNum4("pUSD", configuration.PlutoDAOpUSDIssuerPublic);

            configuration.YusdcAssetIssuerKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.YusdcAssetIssuerConfigKey, "YusdcAssetIssuer account",
                configuration.YusdcAssetIssuerPrivate);
            configuration.YusdcAssetIssuerPublic = configuration.YusdcAssetIssuerKeyPair.AccountId;
            configuration.YusdcAssetIssuerPrivate = configuration.YusdcAssetIssuerKeyPair.SecretSeed;
            configuration.YusdcAsset = new AssetTypeCreditAlphaNum12("yUSDC", configuration.YusdcAssetIssuerPublic);

            configuration.UsdcAssetIssuerKeyPair = await StellarHelper.GetOrCreateAccountKeyPair(
                TestConfiguration.UsdcAssetIssuerConfigKey, "UsdcAssetIssuer account",
                configuration.UsdcAssetIssuerPrivate);
            configuration.UsdcAssetIssuerPublic = configuration.UsdcAssetIssuerKeyPair.AccountId;
            configuration.UsdcAssetIssuerPrivate = configuration.UsdcAssetIssuerKeyPair.SecretSeed;
            configuration.UsdcAsset = new AssetTypeCreditAlphaNum4("USDC", configuration.UsdcAssetIssuerPublic);
        }

        private static async Task FundVoter(TestConfiguration configuration)
        {
            await StellarHelper.Pay(
                configuration.PlutoDAOpUSDIssuerKeyPair,
                configuration.VoterKeyPair,
                configuration.PusdAsset.Code,
                10000
            );
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
