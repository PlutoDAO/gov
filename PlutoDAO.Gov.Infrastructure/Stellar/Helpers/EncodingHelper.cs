using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using stellar_dotnet_sdk.responses.operations;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Helpers
{
    public static class EncodingHelper
    {
        private const decimal StellarPrecision = 10000000M;
        public const long MaxTokens = 100000000000;
        public const int MemoTextMaximumCharacters = 28;
        private const int MaximumFiguresPerPayment = 16;

        public static EncodedProposalPayment Encode(string serializedProposal)
        {
            IList<decimal> extraPayments = new List<decimal>();
            decimal encodedDataPayment;
            decimal totalPayments = 0;

            var extraDigits = HexToDecimal(StringToHex(serializedProposal));

            for (var i = 0; i < extraDigits.Length; i += MaximumFiguresPerPayment)
            {
                var encodedDataDecimalSection =
                    decimal.Parse(
                        extraDigits.Substring(i,
                            extraDigits.Length - i > MaximumFiguresPerPayment
                                ? MaximumFiguresPerPayment
                                : extraDigits.Length - i),
                        CultureInfo.InvariantCulture);
                encodedDataPayment = encodedDataDecimalSection / StellarPrecision;

                if (encodedDataPayment == 0) encodedDataPayment = 1000000000;

                extraPayments.Add(encodedDataPayment);
                totalPayments += encodedDataPayment;
            }

            decimal lastSequenceDigitCount = extraDigits.Length % MaximumFiguresPerPayment;
            if (lastSequenceDigitCount == 0) lastSequenceDigitCount = MaximumFiguresPerPayment;

            encodedDataPayment = lastSequenceDigitCount / StellarPrecision;
            extraPayments.Add(encodedDataPayment);
            totalPayments += encodedDataPayment;
            var excessTokens = MaxTokens - totalPayments;

            return new EncodedProposalPayment(extraPayments, excessTokens);
        }

        public static string Decode(IList<PaymentOperationResponse> retrievedRecords,
            IEnumerable<object> transactionHashesAll)
        {
            var decodedProposal = "";
            var transactionHashesUnique = transactionHashesAll.Select(transactionHash => transactionHash).Distinct();

            foreach (var uniqueTransactionHash in transactionHashesUnique.ToArray())
            {
                var paymentsForOneTransaction = retrievedRecords.Where(payment =>
                    payment.TransactionHash == (string) uniqueTransactionHash).ToList();
                var paymentsLength = paymentsForOneTransaction.Count;

                if (paymentsForOneTransaction.Count > 2)
                {
                    var encodedDigits = "";

                    for (var i = 0; i < paymentsLength - 3; i++)
                        encodedDigits += new BigInteger(decimal.Parse(paymentsForOneTransaction.ElementAt(i).Amount,
                                CultureInfo.InvariantCulture) * StellarPrecision)
                            .ToString()
                            .PadLeft(MaximumFiguresPerPayment, '0');

                    var lastPaymentAmount =
                        new BigInteger(decimal.Parse(paymentsForOneTransaction.ElementAt(paymentsLength - 3).Amount,
                                CultureInfo.InvariantCulture) * StellarPrecision)
                            .ToString();
                    var lastPaymentDigits =
                        (int) (decimal.Parse(paymentsForOneTransaction.ElementAt(paymentsLength - 2).Amount,
                            CultureInfo.InvariantCulture) * StellarPrecision);
                    encodedDigits += lastPaymentAmount.PadLeft(lastPaymentDigits, '0');
                    decodedProposal += HexToString(DecimalToHex(encodedDigits));
                }
            }

            return decodedProposal;
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

        public static string EncodeSeqNumberToBase48(long sequenceNumber)
        {
            var map = new[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K',
                'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
                'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
                'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't',
                'u', 'v', 'x', 'y', 'z', '2', '3', '4'
            };

            var sequenceNumberString = sequenceNumber.ToString();
            var slicedSequenceNumber = sequenceNumberString.Length <= 13
                ? long.Parse(sequenceNumberString)
                : long.Parse(sequenceNumberString.Substring(sequenceNumberString.Length - 13));

            var b = map.Length;

            var toChar = map.Select((v, i) => new {Value = v, Index = i}).ToDictionary(i => i.Index, i => i.Value);
            var result = "";
            if (slicedSequenceNumber == 0) return "" + toChar[0];
            while (slicedSequenceNumber > 0)
            {
                var val = (int) (slicedSequenceNumber % b);
                slicedSequenceNumber = slicedSequenceNumber / b;
                result += toChar[val];
            }

            return result;
        }
    }
}
