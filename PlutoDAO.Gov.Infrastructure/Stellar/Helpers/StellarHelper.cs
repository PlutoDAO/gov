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
            var proposalMicropaymentSenderFinalXlmBalance =
                await GetAccountXlmBalance(source.AccountId);
            var returnFundsFee = 0.00001M;
            var exceedingFunds = proposalMicropaymentSenderFinalXlmBalance - initialXlmBalance - returnFundsFee;

            var destinationKeyPair = KeyPair.FromAccountId(destination);
            var txBuilder = new TransactionBuilder(source);
            var paymentOp = new PaymentOperation.Builder(destinationKeyPair, new AssetTypeNative(),
                Convert.ToString(exceedingFunds, CultureInfo.InvariantCulture)).Build();
            txBuilder.AddOperation(paymentOp);
            var tx = txBuilder.Build();
            tx.Sign(sourceKeyPair);
            await Server.SubmitTransaction(tx);
        }
        
        private static async Task<string> GenerateAssetCode(string proposalMicropaymentReceiverPublicKey, string proposalMicropaymentSenderPublicKey)
        {
            IList<string> assetList = new List<string>();
            var response =
                await Server.Payments.ForAccount(proposalMicropaymentSenderPublicKey).Limit(200).Execute();
            while (response.Embedded.Records.Count != 0)
            {
                foreach (var payment in response.Records.OfType<PaymentOperationResponse>())
                    if (payment.SourceAccount == proposalMicropaymentReceiverPublicKey && payment.AssetCode.Contains("PROP"))
                        assetList.Add(payment.AssetCode);
                response = await response.NextPage();
            }

            var uniqueAssetCount = assetList.Distinct().Count();
            return $"PROP{uniqueAssetCount + 1}";
        }
        
        private static async Task<SubmitTransactionResponse> ClaimClaimableBalance(Account proposalMicropaymentSender,
            KeyPair proposalMicropaymentSenderKeyPair, string proposalCreator)
        {
            var sponsor = KeyPair.FromAccountId(proposalCreator);
            var claimableBalance = await Server.ClaimableBalances.ForClaimant(proposalMicropaymentSenderKeyPair)
                .ForAsset(new AssetTypeNative()).ForSponsor(sponsor).Execute();
            var balanceId = claimableBalance.Records.First().Id;

            var claimClaimableBalanceOp = new ClaimClaimableBalanceOperation.Builder(balanceId).Build();
            var transactionBuilder = new TransactionBuilder(proposalMicropaymentSender);
            transactionBuilder.AddOperation(claimClaimableBalanceOp);
            var tx = transactionBuilder.Build();
            tx.Sign(proposalMicropaymentSenderKeyPair);
            return await Server.SubmitTransaction(tx);
        }
    }
}
