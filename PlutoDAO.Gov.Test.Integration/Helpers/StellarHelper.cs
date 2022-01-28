using System;
using System.Linq;
using System.Threading.Tasks;
using stellar_dotnet_sdk;

namespace PlutoDAO.Gov.Test.Integration.Helpers
{
    public static class StellarHelper
    {
        public static KeyPair MasterAccount { get; set; } = null!;
        public static Server Server { get; set; } = null!;

        public static async Task AddXlmFunds(KeyPair destinationKeyPair)
        {
            var fundAccountOp = new PaymentOperation.Builder(destinationKeyPair, new AssetTypeNative(), "10000")
                .SetSourceAccount(MasterAccount)
                .Build();

            var masterAccount = await Server.Accounts.Account(MasterAccount.AccountId);
            var transaction = new TransactionBuilder(masterAccount).AddOperation(fundAccountOp).Build();
            transaction.Sign(MasterAccount);
            await Server.SubmitTransaction(transaction);
        }

        private static async Task FundAccountWithXlm(string address, string env)
        {
            if (env == "STAGING")
            {
                await Server.TestNetFriendBot.FundAccount(address).Execute();
                await Server.Accounts.Account(address);
            }
            else
            {
                var destinationKeyPair = KeyPair.FromAccountId(address);
                var createAccountOp = new CreateAccountOperation.Builder(destinationKeyPair, "10000")
                    .SetSourceAccount(MasterAccount)
                    .Build();

                var masterAccount = await Server.Accounts.Account(MasterAccount.AccountId);
                var transaction = new TransactionBuilder(masterAccount).AddOperation(createAccountOp).Build();
                transaction.Sign(MasterAccount);
                await Server.SubmitTransaction(transaction);
            }

            Console.WriteLine($"Account {address} funded successfully.");
        }

        public static async Task<KeyPair> GetOrCreateAccountKeyPair(
            string key,
            string description,
            string? secret = null
        )
        {
            Console.WriteLine($"Looking for {key} for {description}");

            if (secret != null)
            {
                Console.WriteLine($"Found {description}");
                return KeyPair.FromSecretSeed(secret);
            }

            Console.WriteLine($"Didn't find {description}, an account will be created and funded");
            var pair = KeyPair.Random();
            var env = Environment.GetEnvironmentVariable("ENVIRONMENT")!;
            await FundAccountWithXlm(pair.AccountId, env);

            Console.WriteLine($"{description} secret key is {pair.SecretSeed}");
            Console.WriteLine($"{description} public key is {pair.AccountId}");
            Console.WriteLine($"{pair.AccountId} funded successfully with XLM");

            return pair;
        }

        public static async Task CreateFeesPaymentClaimableBalance(KeyPair proposalCreator, KeyPair destination)
        {
            var proposalCreatorAccountResponse = await Server.Accounts.Account(proposalCreator.AccountId);
            var proposalCreatorAccount =
                new Account(proposalCreator.AccountId, proposalCreatorAccountResponse.SequenceNumber);

            var claimant = new Claimant
            {
                Destination = destination,
                Predicate = ClaimPredicate.Unconditional()
            };

            var txBuilder = new TransactionBuilder(proposalCreatorAccount);
            var claimableBalanceOp =
                new CreateClaimableBalanceOperation.Builder(new AssetTypeNative(), "5", new[] {claimant})
                    .SetSourceAccount(proposalCreator)
                    .Build();
            txBuilder.AddOperation(claimableBalanceOp);

            var tx = txBuilder.Build();
            tx.Sign(proposalCreator);
            await Server.SubmitTransaction(tx);
        }

        public static async Task<string> GetAccountXlmBalance(string publicKey)
        {
            var accountResponse = await Server.Accounts.Account(publicKey);
            return accountResponse.Balances.First(balance => balance.AssetType == "native").BalanceString;
        }
    }
}
