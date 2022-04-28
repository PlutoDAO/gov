using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Application.Providers;
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
        private readonly DateTimeProvider _dateTimeProvider;

        public ProposalRepository(SystemAccountConfiguration systemAccountConfiguration, Server server,
            DateTimeProvider dateTimeProvider)
        {
            _systemAccountConfiguration = systemAccountConfiguration;
            _server = server;
            _dateTimeProvider = dateTimeProvider;
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

            var proposalMicropaymentSenderKeyPair =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.MicropaymentSenderPrivateKey);
            var proposalMicropaymentReceiverKeyPair =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.MicropaymentReceiverPrivateKey);
            var micropaymentSenderAccountResponse =
                await _server.Accounts.Account(proposalMicropaymentSenderKeyPair.AccountId);
            var micropaymentSenderAccount = new Account(proposalMicropaymentSenderKeyPair.AccountId,
                micropaymentSenderAccountResponse.SequenceNumber);
            var proposalCreatorAccount = await _server.Accounts.Account(proposal.Creator);
            var assetCode = EncodingHelper.EncodeSeqNumberToBase48(proposalCreatorAccount.SequenceNumber);
            var proposalMicropaymentSenderInitialXlmBalance =
                await GetAccountXlmBalance(proposalMicropaymentSenderKeyPair.AccountId);

            var claimClaimableBalanceResponse =
                await ClaimClaimableBalance(micropaymentSenderAccount, proposalMicropaymentSenderKeyPair,
                    proposal.Creator);

            while (claimClaimableBalanceResponse.Result is TransactionResultBadSeq)
                claimClaimableBalanceResponse =
                    await ClaimClaimableBalance(micropaymentSenderAccount, proposalMicropaymentSenderKeyPair,
                        proposal.Creator);

            if (claimClaimableBalanceResponse.IsSuccess())
            {
                for (var i = 0; i <= serializedProposal.Length; i += maximumProposalLength)
                {
                    var serializedProposalSection = serializedProposal.Substring(i,
                        serializedProposal.Length - i > maximumProposalLength
                            ? maximumProposalLength
                            : serializedProposal.Length - i);

                    var transactionResponse = await SaveProposal(
                        serializedProposalSection,
                        proposal.Name,
                        assetCode,
                        proposalMicropaymentSenderKeyPair,
                        proposalMicropaymentReceiverKeyPair,
                        micropaymentSenderAccount);

                    while (transactionResponse.Result is TransactionResultBadSeq)
                        transactionResponse = await SaveProposal(
                            serializedProposalSection,
                            proposal.Name,
                            assetCode,
                            proposalMicropaymentSenderKeyPair,
                            proposalMicropaymentReceiverKeyPair,
                            micropaymentSenderAccount);

                    if (!transactionResponse.IsSuccess())
                        throw new ApplicationException(
                            transactionResponse
                                .SubmitTransactionResponseExtras
                                .ExtrasResultCodes
                                .OperationsResultCodes
                                .Aggregate("", (acc, code) => $"{acc}, {code}")
                        );
                }

                await PayBackExceedingFunds(proposalMicropaymentSenderInitialXlmBalance, micropaymentSenderAccount,
                    proposalMicropaymentSenderKeyPair,
                    proposal.Creator);
            }
        }

        public async Task<Proposal> GetProposal(string assetCode)
        {
            IList<object> transactionHashesAll = new List<object>();
            var proposalMicropaymentReceiverPublicKey =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.MicropaymentReceiverPrivateKey).AccountId;
            IList<PaymentOperationResponse> retrievedRecords = new List<PaymentOperationResponse>();

            var response =
                await _server.Payments.ForAccount(proposalMicropaymentReceiverPublicKey).Limit(200).Execute();
            while (response.Embedded.Records.Count != 0)
            {
                var paymentRecords = response.Records.OfType<PaymentOperationResponse>()
                    .Where(payment => payment.TransactionSuccessful).ToList();

                foreach (var record in paymentRecords)
                    if (record.AssetCode == assetCode && record.To == proposalMicropaymentReceiverPublicKey &&
                        record.AssetIssuer == proposalMicropaymentReceiverPublicKey)
                    {
                        retrievedRecords.Add(record);
                        transactionHashesAll.Add(record.TransactionHash);
                    }

                response = await response.NextPage();
            }

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new JsonConverterHelper());
            var decodedProposal = EncodingHelper.Decode(retrievedRecords, transactionHashesAll);
            return JsonConvert.DeserializeObject<Proposal>(decodedProposal, settings);
        }

        public async Task<ProposalIdentifier[]> GetProposalList()
        {
            IList<ProposalIdentifier> proposalList = new List<ProposalIdentifier>();
            var proposalMicropaymentReceiverPublicKey =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.MicropaymentReceiverPrivateKey).AccountId;
            var proposalMicropaymentSenderPublicKey =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.MicropaymentSenderPrivateKey).AccountId;

            var response =
                await _server.Transactions.ForAccount(proposalMicropaymentReceiverPublicKey).Limit(200).Execute();
            var transactionRecords = response.Records
                .Where(transaction => transaction.SourceAccount == proposalMicropaymentSenderPublicKey);

            foreach (var record in transactionRecords)
                if (proposalList.All(identifier => identifier.Name != record.MemoValue))
                {
                    var assetCode = (await _server.Payments.ForTransaction(record.Hash)
                            .Execute()).Records.OfType<PaymentOperationResponse>()
                        .First()
                        .AssetCode;

                    var proposalClosingDay = DateTime.Parse(record.CreatedAt).AddDays(31).Date;
                    var minutesUntilProposalClosing = (float) (proposalClosingDay - _dateTimeProvider.Now).TotalMinutes;

                    proposalList.Add(new ProposalIdentifier
                        {Id = assetCode, Name = record.MemoValue, RemainingMinutes = minutesUntilProposalClosing});
                }

            return proposalList.ToArray();
        }

        private async Task<SubmitTransactionResponse> SaveProposal(
            string serializedProposalSection,
            string proposalName,
            string assetCode,
            KeyPair proposalMicropaymentSenderKeyPair,
            KeyPair proposalMicropaymentReceiverKeyPair,
            Account micropaymentSenderAccount)
        {
            var encodedProposalPayments = EncodingHelper.Encode(serializedProposalSection);

            var txBuilder = new TransactionBuilder(micropaymentSenderAccount);
            var asset = Asset.CreateNonNativeAsset(assetCode, proposalMicropaymentReceiverKeyPair.AccountId);

            var changeTrustLineOp = new ChangeTrustOperation.Builder(ChangeTrustAsset.Create(asset))
                .SetSourceAccount(micropaymentSenderAccount.KeyPair).Build();
            var paymentOp =
                new PaymentOperation.Builder(proposalMicropaymentSenderKeyPair, asset,
                        EncodingHelper.MaxTokens.ToString())
                    .SetSourceAccount(proposalMicropaymentReceiverKeyPair).Build();
            txBuilder.AddOperation(changeTrustLineOp).AddOperation(paymentOp).AddMemo(new MemoText(proposalName));

            foreach (var payment in encodedProposalPayments.EncodedProposalMicropayments)
            {
                var encodedTextPaymentOp = new PaymentOperation.Builder(
                        proposalMicropaymentReceiverKeyPair,
                        asset,
                        payment.ToString(CultureInfo.CreateSpecificCulture("en-us"))
                    )
                    .SetSourceAccount(micropaymentSenderAccount.KeyPair)
                    .Build();
                txBuilder.AddOperation(encodedTextPaymentOp);
            }

            txBuilder.AddOperation(new PaymentOperation.Builder(proposalMicropaymentReceiverKeyPair, asset,
                encodedProposalPayments.ExcessTokens.ToString(CultureInfo.CreateSpecificCulture("en-us"))).Build());

            var tx = txBuilder.Build();
            tx.Sign(proposalMicropaymentSenderKeyPair);
            tx.Sign(proposalMicropaymentReceiverKeyPair);

            return await _server.SubmitTransaction(tx);
        }

        private async Task<SubmitTransactionResponse> ClaimClaimableBalance(Account proposalMicropaymentSender,
            KeyPair proposalMicropaymentSenderKeyPair, string proposalCreator)
        {
            var sponsor = KeyPair.FromAccountId(proposalCreator);
            var claimableBalance = await _server.ClaimableBalances.ForClaimant(proposalMicropaymentSenderKeyPair)
                .ForAsset(new AssetTypeNative()).ForSponsor(sponsor).Execute();
            var balanceId = claimableBalance.Records.First().Id;

            var claimClaimableBalanceOp = new ClaimClaimableBalanceOperation.Builder(balanceId).Build();
            var transactionBuilder = new TransactionBuilder(proposalMicropaymentSender);
            transactionBuilder.AddOperation(claimClaimableBalanceOp);
            var tx = transactionBuilder.Build();
            tx.Sign(proposalMicropaymentSenderKeyPair);
            return await _server.SubmitTransaction(tx);
        }

        private async Task PayBackExceedingFunds(decimal initialXlmBalance, Account source, KeyPair sourceKeyPair,
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
            await _server.SubmitTransaction(tx);
        }

        private async Task<decimal> GetAccountXlmBalance(string publicKey)
        {
            var account = await _server.Accounts
                .Account(publicKey);
            var balance = account.Balances
                .First(balance => balance.AssetType == "native").BalanceString;
            return Convert.ToDecimal(balance, CultureInfo.InvariantCulture);
        }

        public async Task<int> GetVotingResult(string assetCode)
        {
            var plutoDaoResultsPublicKey =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.ResultsPrivateKey).AccountId;
            var proposalMicropaymentReceiverPublicKey =
                KeyPair.FromSecretSeed(_systemAccountConfiguration.MicropaymentReceiverPrivateKey).AccountId;
            var response =
                await _server.Payments.ForAccount(plutoDaoResultsPublicKey).Limit(200).Execute();
            while (response.Embedded.Records.Count != 0)
            {
                var paymentRecords = response.Records.OfType<PaymentOperationResponse>()
                    .Where(payment => payment.TransactionSuccessful).ToList();

                foreach (var record in paymentRecords)
                    if (record.AssetCode == assetCode && record.To == plutoDaoResultsPublicKey &&
                        record.AssetIssuer == proposalMicropaymentReceiverPublicKey)
                        return Convert.ToInt32(decimal.Parse(record.Amount, CultureInfo.InvariantCulture));

                response = await response.NextPage();
            }

            return -1;
        }
    }
}
