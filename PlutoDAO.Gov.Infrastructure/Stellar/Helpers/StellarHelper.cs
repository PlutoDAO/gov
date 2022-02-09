using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Helpers
{
    public static class StellarHelper
    {
        private static Server Server { get; set; } = null!;

        private static async Task<decimal> GetAccountXlmBalance(string publicKey)
        {
            var account = await Server.Accounts.Account(publicKey);
            var balance = account.Balances
                .First(balance => balance.AssetType == "native").BalanceString;
            return Convert.ToDecimal(balance, CultureInfo.InvariantCulture);
        }
        
        private static async Task PayBackExceedingFunds(decimal initialXlmBalance, Account source, KeyPair sourceKeyPair,
            string destination)
        {
            var proposalSenderFinalXlmBalance =
                await GetAccountXlmBalance(source.AccountId);
            var returnFundsFee = 0.00001M;
            var exceedingFunds = proposalSenderFinalXlmBalance - initialXlmBalance - returnFundsFee;

            var destinationKeyPair = KeyPair.FromAccountId(destination);
            var txBuilder = new TransactionBuilder(source);
            var paymentOp = new PaymentOperation.Builder(destinationKeyPair, new AssetTypeNative(),
                Convert.ToString(exceedingFunds, CultureInfo.InvariantCulture)).Build();
            txBuilder.AddOperation(paymentOp);
            var tx = txBuilder.Build();
            tx.Sign(sourceKeyPair);
            await Server.SubmitTransaction(tx);
        }
        
        private static async Task<string> GenerateAssetCode(string proposalReceiverPublicKey, string proposalSenderPublicKey)
        {
            IList<string> assetList = new List<string>();
            var response =
                await Server.Payments.ForAccount(proposalSenderPublicKey).Limit(200).Execute();
            while (response.Embedded.Records.Count != 0)
            {
                foreach (var payment in response.Records.OfType<PaymentOperationResponse>())
                    if (payment.SourceAccount == proposalReceiverPublicKey && payment.AssetCode.Contains("PROP"))
                        assetList.Add(payment.AssetCode);
                response = await response.NextPage();
            }

            var uniqueAssetCount = assetList.Distinct().Count();
            return $"PROP{uniqueAssetCount + 1}";
        }
        
        private static async Task<SubmitTransactionResponse> ClaimClaimableBalance(Account proposalSender,
            KeyPair proposalSenderKeyPair, string proposalCreator)
        {
            var sponsor = KeyPair.FromAccountId(proposalCreator);
            var claimableBalance = await Server.ClaimableBalances.ForClaimant(proposalSenderKeyPair)
                .ForAsset(new AssetTypeNative()).ForSponsor(sponsor).Execute();
            var balanceId = claimableBalance.Records.First().Id;

            var claimClaimableBalanceOp = new ClaimClaimableBalanceOperation.Builder(balanceId).Build();
            var transactionBuilder = new TransactionBuilder(proposalSender);
            transactionBuilder.AddOperation(claimClaimableBalanceOp);
            var tx = transactionBuilder.Build();
            tx.Sign(proposalSenderKeyPair);
            return await Server.SubmitTransaction(tx);
        }
    }
}
