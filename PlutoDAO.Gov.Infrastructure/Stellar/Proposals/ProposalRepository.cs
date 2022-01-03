using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Domain;
using PlutoDAO.Gov.Infrastructure.Stellar.Helpers;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;
using Asset = stellar_dotnet_sdk.Asset;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Proposals
{
    public class ProposalRepository : IProposalRepository
    {
        private readonly Server _server;
        private readonly SystemAccountConfiguration _systemAccountConfiguration;

        public ProposalRepository(SystemAccountConfiguration systemAccountConfiguration, Server server)
        {
            _systemAccountConfiguration = systemAccountConfiguration;
            _server = server;
        }

        public async Task SaveProposal(Proposal proposal)
        {
            const int maximumProposalLength = 637;
            if (proposal.Name.Length > EncodingHelper.MemoTextMaximumCharacters)
                throw new ArgumentOutOfRangeException("The proposal name cannot exceed 28 characters");

            var serializedProposal = JsonConvert.SerializeObject(proposal, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });

            var proposalSenderKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.SenderPrivateKey);
            var proposalReceiverKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.ReceiverPrivateKey);
            var senderAccountResponse = await _server.Accounts.Account(proposalSenderKeyPair.AccountId);
            var senderAccount = new Account(proposalSenderKeyPair.AccountId, senderAccountResponse.SequenceNumber);
            var assetCode = await GenerateAssetCode(proposalReceiverKeyPair.AccountId, proposalSenderKeyPair.AccountId);

            var claimClaimableBalanceResponse =
                await ClaimClaimableBalance(senderAccount, proposalSenderKeyPair,
                    proposal.Creator);

            if (claimClaimableBalanceResponse.IsSuccess())
                if (serializedProposal.Length <= maximumProposalLength)
                    await SaveProposal(serializedProposal,
                        proposal.Name,
                        assetCode,
                        proposalSenderKeyPair,
                        proposalReceiverKeyPair,
                        senderAccount);
                else
                    for (var i = 0; i < serializedProposal.Length; i += maximumProposalLength)
                    {
                        var serializedProposalSection = serializedProposal.Substring(i,
                            serializedProposal.Length - i > maximumProposalLength
                                ? maximumProposalLength
                                : serializedProposal.Length - i);
                        await SaveProposal(
                            serializedProposalSection,
                            proposal.Name,
                            assetCode,
                            proposalSenderKeyPair,
                            proposalReceiverKeyPair,
                            senderAccount);
                    }
        }

        public async Task<Proposal> GetProposal(string assetCode)
        {
            IList<object> transactionHashesAll = new List<object>();
            var proposalReceiverKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.ReceiverPrivateKey);
            IList<PaymentOperationResponse> retrievedRecords = new List<PaymentOperationResponse>();

            var response =
                await _server.Payments.ForAccount(proposalReceiverKeyPair.AccountId).Limit(200).Execute();
            while (response.Embedded.Records.Count != 0)
            {
                var paymentRecords = response.Records.OfType<PaymentOperationResponse>()
                    .Where(payment => payment.TransactionSuccessful).ToList();

                foreach (var record in paymentRecords)
                    if (record.AssetCode == assetCode && record.To == proposalReceiverKeyPair.AccountId)
                    {
                        retrievedRecords.Add(record);
                        transactionHashesAll.Add(record.TransactionHash);
                    }

                response = await response.NextPage();
            }

            var decodedProposal = EncodingHelper.Decode(retrievedRecords, transactionHashesAll);
            return JsonConvert.DeserializeObject<Proposal>(decodedProposal);
        }

        public async Task<ProposalIdentifier[]> GetProposalList()
        {
            IList<ProposalIdentifier> proposalList = new List<ProposalIdentifier>();
            var proposalReceiverPublicKey =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.ReceiverPrivateKey).AccountId;
            var proposalSenderPublicKey =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.SenderPrivateKey).AccountId;

            var response =
                await _server.Transactions.ForAccount(proposalReceiverPublicKey).Limit(200).Execute();
            var transactionRecords = response.Records
                .Where(transaction => transaction.SourceAccount == proposalSenderPublicKey);

            foreach (var record in transactionRecords)
                if (proposalList.All(identifier => identifier.Name != record.MemoValue))
                {
                    var assetCode = (await _server.Payments.ForTransaction(record.Hash)
                            .Execute()).Records.OfType<PaymentOperationResponse>()
                        .First()
                        .AssetCode;

                    proposalList.Add(new ProposalIdentifier {Id = assetCode, Name = record.MemoValue});
                }

            return proposalList.ToArray();
        }

        private async Task SaveProposal(
            string serializedProposalSection,
            string proposalName,
            string assetCode,
            KeyPair proposalSenderKeyPair,
            KeyPair proposalReceiverKeyPair,
            Account senderAccount)
        {
            var encodedProposalPayments = EncodingHelper.Encode(serializedProposalSection);

            var txBuilder = new TransactionBuilder(senderAccount);
            var asset = Asset.CreateNonNativeAsset(assetCode, proposalReceiverKeyPair.AccountId);

            var changeTrustLineOp = new ChangeTrustOperation.Builder(ChangeTrustAsset.Create(asset))
                .SetSourceAccount(senderAccount.KeyPair).Build();
            var paymentOp =
                new PaymentOperation.Builder(proposalSenderKeyPair, asset, EncodingHelper.MaxTokens.ToString())
                    .SetSourceAccount(proposalReceiverKeyPair).Build();
            txBuilder.AddOperation(changeTrustLineOp).AddOperation(paymentOp).AddMemo(new MemoText(proposalName));

            foreach (var payment in encodedProposalPayments.EncodedProposalMicropayments)
            {
                var encodedTextPaymentOp = new PaymentOperation.Builder(
                        proposalReceiverKeyPair,
                        asset,
                        payment.ToString(CultureInfo.CreateSpecificCulture("en-us"))
                    )
                    .SetSourceAccount(senderAccount.KeyPair)
                    .Build();
                txBuilder.AddOperation(encodedTextPaymentOp);
            }

            txBuilder.AddOperation(new PaymentOperation.Builder(proposalReceiverKeyPair, asset,
                encodedProposalPayments.ExcessTokens.ToString(CultureInfo.CreateSpecificCulture("en-us"))).Build());

            var tx = txBuilder.Build();
            tx.Sign(proposalSenderKeyPair);
            tx.Sign(proposalReceiverKeyPair);

            var transactionResponse = await _server.SubmitTransaction(tx);

            if (!transactionResponse.IsSuccess())
                throw new ApplicationException(
                    transactionResponse
                        .SubmitTransactionResponseExtras
                        .ExtrasResultCodes
                        .OperationsResultCodes
                        .Aggregate("", (acc, code) => $"{acc}, {code}")
                );
        }

        private async Task<SubmitTransactionResponse> ClaimClaimableBalance(Account proposalSender, KeyPair proposalSenderKeyPair, string proposalCreator)
        {
            var sponsor = KeyPair.FromAccountId(proposalCreator);
            var claimableBalance = _server.ClaimableBalances.ForClaimant(proposalSenderKeyPair)
                .ForAsset(new AssetTypeNative()).ForSponsor(sponsor).Execute();
            var response = claimableBalance.Result.Records;
            var balanceId = response.First().Id;

            var claimClaimableBalanceOp = new ClaimClaimableBalanceOperation.Builder(balanceId).Build();
            var transactionBuilder = new TransactionBuilder(proposalSender);
            transactionBuilder.AddOperation(claimClaimableBalanceOp);
            var tx = transactionBuilder.Build();
            tx.Sign(proposalSenderKeyPair);
            return await _server.SubmitTransaction(tx);
        }

        private async Task<string> GenerateAssetCode(string proposalReceiverPublicKey, string proposalSenderAccount)
        {
            IList<string> assetList = new List<string>();
            var response =
                await _server.Payments.ForAccount(proposalSenderAccount).Limit(200).Execute();
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
    }
}
