using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using PlutoDAO.Gov.Domain;
using stellar_dotnet_sdk.responses.operations;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Helpers
{
    public class EncodingHelper
    {
        private const decimal StellarPrecision = 10000000;
        public const long MaxTokens = 100000000000;

        public static (IList<decimal>, decimal) Encode(string serializedProposal)
        {
            IList<decimal> extraPayments = new List<decimal>();
            decimal encodedDataPayment;
            decimal totalPayments = 0;

            serializedProposal =
                serializedProposal.Substring(0, serializedProposal.Length < 638 ? serializedProposal.Length : 638);
            var extraDigits = HexToDecimal(StringToHex(serializedProposal));

            for (var i = 0; i < extraDigits.Length; i += 16)
            {
                var encodedDataDecimalSection =
                    decimal.Parse(extraDigits.Substring(i, extraDigits.Length - i > 16 ? 16 : extraDigits.Length - i));
                encodedDataPayment = encodedDataDecimalSection / StellarPrecision;

                if (encodedDataPayment == 0) encodedDataPayment = 1000000000;

                extraPayments.Add(encodedDataPayment);
                totalPayments += encodedDataPayment;
            }

            decimal lastSequenceDigitCount = extraDigits.Length % 16;
            if (lastSequenceDigitCount == 0) lastSequenceDigitCount = 16;

            encodedDataPayment = lastSequenceDigitCount / StellarPrecision;
            extraPayments.Add(encodedDataPayment);
            totalPayments += encodedDataPayment;
            var excessTokens = MaxTokens - totalPayments;

            return (extraPayments, excessTokens);
        }

        public static Proposal[] Decode(IList<PaymentOperationResponse> retrievedRecords,
            IEnumerable<object> transactionHashesAll)
        {
            IEnumerable<DecodedProposal> decodedProposals = new List<DecodedProposal>();
            var encodedDigits = "";

            var transactionHashesUnique = transactionHashesAll.Select(transactionHash => transactionHash).Distinct();

            foreach (var uniqueTransactionHash in transactionHashesUnique.ToArray())
            {
                var paymentsForOneTransaction = retrievedRecords.Where(payment =>
                    payment.TransactionHash == (string) uniqueTransactionHash).ToList();
                var paymentsLength = paymentsForOneTransaction.Count;

                if (paymentsForOneTransaction.Count > 2)
                {
                    for (var i = 0; i < paymentsLength - 3; i++)
                        encodedDigits += new BigInteger(decimal.Parse(paymentsForOneTransaction.ElementAt(i).Amount,
                                CultureInfo.InvariantCulture) * StellarPrecision)
                            .ToString()
                            .PadLeft(16, '0');

                    var lastPaymentAmount =
                        new BigInteger(decimal.Parse(paymentsForOneTransaction.ElementAt(paymentsLength - 3).Amount,
                                CultureInfo.InvariantCulture) * StellarPrecision)
                            .ToString();
                    var lastPaymentDigits =
                        (int) (decimal.Parse(paymentsForOneTransaction.ElementAt(paymentsLength - 2).Amount,
                            CultureInfo.InvariantCulture) * StellarPrecision);
                    encodedDigits += lastPaymentAmount.PadLeft(lastPaymentDigits, '0');

                    var finalDecodedProposals = HexToString(DecimalToHex(encodedDigits));

                    decodedProposals = decodedProposals.Append(new DecodedProposal(
                        paymentsForOneTransaction.ElementAt(0).From, finalDecodedProposals,
                        DateTimeOffset.Parse(paymentsForOneTransaction.ElementAt(0).CreatedAt).ToUnixTimeSeconds()));
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
