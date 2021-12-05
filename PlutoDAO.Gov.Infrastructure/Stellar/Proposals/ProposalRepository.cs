using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Domain;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses.operations;
using Asset = stellar_dotnet_sdk.Asset;
using DomainAsset = PlutoDAO.Gov.Domain.Asset;

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
            const string assetCode = "PROPCOIN1";
            var serializedProposal = JsonConvert.SerializeObject(proposal, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });
            const long maxTokens = 100000000000;
            const decimal stellarPrecision = 10000000;
            IList<decimal> extraPayments = new List<decimal>();
            decimal encodedDataPayment;
            decimal totalPayments = 0;

            serializedProposal =
                serializedProposal.Substring(0, serializedProposal.Length < 638 ? serializedProposal.Length : 638);
            var proposalSenderKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.SenderPrivateKey);
            var proposalReceiverKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.ReceiverPrivateKey);

            var extraDigits = HexToDecimal(StringToHex(serializedProposal));

            for (var i = 0; i < extraDigits.Length; i += 16)
            {
                var encodedDataDecimalSection =
                    decimal.Parse(extraDigits.Substring(i, extraDigits.Length - i > 16 ? 16 : extraDigits.Length - i));
                encodedDataPayment = encodedDataDecimalSection / stellarPrecision;

                if (encodedDataPayment == 0) encodedDataPayment = 1000000000;

                extraPayments.Add(encodedDataPayment);
                totalPayments += encodedDataPayment;
            }

            decimal lastSequenceDigitCount = extraDigits.Length % 16;
            if (lastSequenceDigitCount == 0) lastSequenceDigitCount = 16;

            encodedDataPayment = lastSequenceDigitCount / stellarPrecision;
            extraPayments.Add(encodedDataPayment);
            totalPayments += encodedDataPayment;
            var excessTokens = maxTokens - totalPayments;

            // Stellar Transaction START
            var senderAccountResponse = await _server.Accounts.Account(proposalSenderKeyPair.AccountId);
            var senderAccount = new Account(proposalSenderKeyPair.AccountId, senderAccountResponse.SequenceNumber);
            var receiver = KeyPair.FromAccountId(proposalReceiverKeyPair.AccountId);

            var txBuilder = new TransactionBuilder(senderAccount);
            var asset = Asset.CreateNonNativeAsset(assetCode, proposalReceiverKeyPair.AccountId);

            var changeTrustLineOp = new ChangeTrustOperation.Builder(ChangeTrustAsset.Create(asset))
                .SetSourceAccount(senderAccount.KeyPair).Build();
            var paymentOp = new PaymentOperation.Builder(proposalSenderKeyPair, asset, maxTokens.ToString())
                .SetSourceAccount(receiver).Build();
            txBuilder.AddOperation(changeTrustLineOp).AddOperation(paymentOp);

            foreach (var payment in extraPayments)
            {
                var encodedTextPaymentOp = new PaymentOperation.Builder(
                        receiver,
                        asset,
                        payment.ToString(CultureInfo.CreateSpecificCulture("en-us"))
                    )
                    .SetSourceAccount(senderAccount.KeyPair)
                    .Build();
                txBuilder.AddOperation(encodedTextPaymentOp);
            }

            txBuilder.AddOperation(new PaymentOperation.Builder(receiver, asset,
                excessTokens.ToString(CultureInfo.CreateSpecificCulture("en-us"))).Build());

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
            const decimal stellarPrecision = 10000000;
            IList<object> transactionHashesAll = new List<object>();
            IEnumerable<object> transactionHashesUnique = null;
            var proposalReceiverKeyPair = KeyPair.FromSecretSeed(_systemAccountConfiguration.ReceiverPrivateKey);
            IEnumerable<DecodedProposal> decodedProposals = new List<DecodedProposal>();
            const string assetCode = "PROPCOIN1";
            IList<Payment> retrievedPayments = new List<Payment>();

            var response =
                await _server.Payments.ForAccount(proposalReceiverKeyPair.AccountId).Limit(200).Execute();
            var paymentRecords = response.Records.OfType<PaymentOperationResponse>()
                .Where(payment => payment.TransactionSuccessful).ToList();

            foreach (var record in paymentRecords)
                if (record.AssetCode == assetCode && record.To == proposalReceiverKeyPair.AccountId)
                {
                    retrievedPayments.Add(new Payment(record.Amount, record.SourceAccount, record.TransactionHash,
                        DateTimeOffset.Parse(record.CreatedAt).ToUnixTimeSeconds()));
                    transactionHashesAll.Add(record.TransactionHash);
                }

            transactionHashesUnique = transactionHashesAll.Select(transactionHash => transactionHash).Distinct();

            // Concatenate Payments assigned to each Transaction Hash
            foreach (var uniqueTransactionHash in transactionHashesUnique.ToArray())
            {
                var paymentsForOneTransaction = retrievedPayments.Where(payment =>
                    payment.TransactionHash == (string) uniqueTransactionHash).ToList();

                var paymentsLength = paymentsForOneTransaction.Count;

                if (paymentsForOneTransaction.Count > 2)
                {
                    var encodedDigits = "";

                    for (var i = 0; i < paymentsLength - 3; i++)
                        encodedDigits += new BigInteger(decimal.Parse(paymentsForOneTransaction.ElementAt(i).Amount,
                                CultureInfo.InvariantCulture) * stellarPrecision)
                            .ToString()
                            .PadLeft(16, '0');

                    var lastPaymentAmount =
                        new BigInteger(decimal.Parse(paymentsForOneTransaction.ElementAt(paymentsLength - 3).Amount,
                                CultureInfo.InvariantCulture) * stellarPrecision)
                            .ToString();
                    var lastPaymentDigits =
                        (int) (decimal.Parse(paymentsForOneTransaction.ElementAt(paymentsLength - 2).Amount,
                            CultureInfo.InvariantCulture) * stellarPrecision);
                    encodedDigits += lastPaymentAmount.PadLeft(lastPaymentDigits, '0');
                    var finalDecodedProposals = HexToString(DecimalToHex(encodedDigits));
                    decodedProposals = decodedProposals.Append(new DecodedProposal(
                        paymentsForOneTransaction.ElementAt(0).From, finalDecodedProposals,
                        paymentsForOneTransaction.ElementAt(0).Timestamp));
                }
            }

            decodedProposals = decodedProposals.OrderByDescending(m => m.Timestamp);
            return decodedProposals.Select(m => JsonConvert.DeserializeObject<Proposal>(m.Content)).ToArray();
        }

        private static string DecimalToHex(string dec)
        {
            return BigInteger.Parse(dec).ToString("X");
        }

        private static string StringToHex(string decString)
        {
            var bytes = Encoding.Default.GetBytes(decString);
            var hexString = BitConverter.ToString(bytes);
            return hexString.Replace("-", "");
        }

        private static string HexToDecimal(string hexString)
        {
            return BigInteger.Parse(hexString, NumberStyles.HexNumber).ToString();
        }

        private static string HexToString(string hex)
        {
            hex = hex.Replace("-", "");
            var raw = new byte[hex.Length / 2];
            for (var i = 0; i < raw.Length; i++) raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return Encoding.ASCII.GetString(raw);
        }

        private class Payment
        {
            public readonly string Amount;
            public readonly string From;
            public readonly long Timestamp;
            public readonly string TransactionHash;

            public Payment(string amount, string from, string transactionHash, long timestamp)
            {
                Amount = amount;
                From = from;
                Timestamp = timestamp;
                TransactionHash = transactionHash;
            }
        }

        private class DecodedProposal
        {
            public readonly string Content;
            public readonly long Timestamp;
            public string From;

            public DecodedProposal(string from, string content, long timestamp)
            {
                From = from;
                Content = content;
                Timestamp = timestamp;
            }
        }
    }
}
