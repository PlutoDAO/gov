using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Domain;
using PlutoDAO.Gov.Infrastructure.Stellar.Helpers;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses.operations;
using Asset = stellar_dotnet_sdk.Asset;
using DomainAsset = PlutoDAO.Gov.Domain.Asset;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Proposals
{
    public class ProposalRepository : IProposalRepository
    {
        private const string AssetCode = "PROPCOIN1";
        private readonly Server _server;
        private readonly SystemAccountConfiguration _systemAccountConfiguration;

        public ProposalRepository(SystemAccountConfiguration systemAccountConfiguration, Server server)
        {
            _systemAccountConfiguration = systemAccountConfiguration;
            _server = server;
        }

        public async Task<Proposal> FindProposal(string address)
        {
            return new Proposal(
                "name",
                "description",
                "creator",
                DateTime.Now.AddHours(1),
                DateTime.Today,
                new[]
                {
                    new WhitelistedAsset(new DomainAsset(new AccountAddress("STELLAR"), "XLM", true), 1)
                }
            );
        }

        public async Task SaveProposal(Proposal proposal)
        {
            var serializedProposal = JsonConvert.SerializeObject(proposal, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });

            var encodedProposalPayments = EncodingHelper.Encode(serializedProposal);

            var proposalSenderKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.SenderPrivateKey);
            var proposalReceiverKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.ReceiverPrivateKey);
            var senderAccountResponse = await _server.Accounts.Account(proposalSenderKeyPair.AccountId);
            var senderAccount = new Account(proposalSenderKeyPair.AccountId, senderAccountResponse.SequenceNumber);

            var txBuilder = new TransactionBuilder(senderAccount);
            var asset = Asset.CreateNonNativeAsset(AssetCode, proposalReceiverKeyPair.AccountId);

            var changeTrustLineOp = new ChangeTrustOperation.Builder(ChangeTrustAsset.Create(asset))
                .SetSourceAccount(senderAccount.KeyPair).Build();
            var paymentOp =
                new PaymentOperation.Builder(proposalSenderKeyPair, asset, EncodingHelper.MaxTokens.ToString())
            txBuilder.AddOperation(changeTrustLineOp).AddOperation(paymentOp);
                    .SetSourceAccount(proposalReceiverKeyPair).Build();

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

        public async Task<Proposal[]> GetProposals()
        {
            IList<object> transactionHashesAll = new List<object>();
            var proposalReceiverKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.ReceiverPrivateKey);
            IList<PaymentOperationResponse> retrievedRecords = new List<PaymentOperationResponse>();

            var response =
                await _server.Payments.ForAccount(proposalReceiverKeyPair.AccountId).Limit(200).Execute();
            var paymentRecords = response.Records.OfType<PaymentOperationResponse>()
                .Where(payment => payment.TransactionSuccessful).ToList();

            foreach (var record in paymentRecords)
                if (record.AssetCode == AssetCode && record.To == proposalReceiverKeyPair.AccountId)
                {
                    retrievedRecords.Add(record);
                    transactionHashesAll.Add(record.TransactionHash);
                }

            return EncodingHelper.Decode(retrievedRecords, transactionHashesAll);
        }
    }
}
