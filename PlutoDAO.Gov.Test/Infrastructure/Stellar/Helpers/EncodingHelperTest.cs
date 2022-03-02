using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using PlutoDAO.Gov.Domain;
using PlutoDAO.Gov.Infrastructure.Stellar.Helpers;
using PlutoDAO.Gov.Test.Helpers;
using stellar_dotnet_sdk.responses.operations;
using Xunit;

namespace PlutoDAO.Gov.Test.Infrastructure.Stellar.Helpers
{
    public class EncodingHelperTest
    {
        [Fact]
        public void TestEncodeProposal()
        {
            var proposal = new Proposal(
                ProposalHelper.GetName(),
                ProposalHelper.GetDescription(),
                ProposalHelper.GetFakeCreator(),
                WhitelistedAssetHelper.GetWhitelistedAssets(),
                DateTime.Parse("9/12/2021 00:00:00", CultureInfo.InvariantCulture),
                DateTime.Parse("9/12/2021 00:00:01", CultureInfo.InvariantCulture),
                null
            );
            var serializedProposal = JsonConvert.SerializeObject(proposal, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            });

            var encodedProposal = EncodingHelper.Encode(serializedProposal);

            Assert.Equal(49291324564.1189122M, encodedProposal.ExcessTokens);
            Assert.Equal(103, encodedProposal.EncodedProposalMicropayments.Count);
        }

        [Fact]
        public void TestDecodeProposal()
        {
            var record1 = JsonConvert.DeserializeObject<PaymentOperationResponse>(
                @"{""type_i"":1,""amount"":""214879170.8693511"",""asset_code"":""PROPCOIN1"",""transaction_hash"":""69930aa243a7b7ec59b1553cf18fd1cc2477b217f6f659fc755d9695414da956"",""from"":""GAPGGFKPOYMNJ4PNGHI33FLV3OGOKATOCBHL36RI4HLZRVY4U7XKPPL4"",""created_at"":""12/10/2021 19:46:37""}");
            var record2 = JsonConvert.DeserializeObject<PaymentOperationResponse>(
                @"{""type_i"":1,""amount"":""369485.906546"",""asset_code"":""PROPCOIN1"",""transaction_hash"":""69930aa243a7b7ec59b1553cf18fd1cc2477b217f6f659fc755d9695414da956"",""from"":""GAPGGFKPOYMNJ4PNGHI33FLV3OGOKATOCBHL36RI4HLZRVY4U7XKPPL4"",""created_at"":""12/10/2021 19:46:37""}");
            var record3 = JsonConvert.DeserializeObject<PaymentOperationResponse>(
                @"{""type_i"":1,""amount"":""0.0000013"",""asset_code"":""PROPCOIN1"",""transaction_hash"":""69930aa243a7b7ec59b1553cf18fd1cc2477b217f6f659fc755d9695414da956"",""from"":""GAPGGFKPOYMNJ4PNGHI33FLV3OGOKATOCBHL36RI4HLZRVY4U7XKPPL4"",""created_at"":""12/10/2021 19:46:37""}");
            var record4 = JsonConvert.DeserializeObject<PaymentOperationResponse>(
                @"{""type_i"":1,""amount"":""0"",""asset_code"":""PROPCOIN1"",""transaction_hash"":""69930aa243a7b7ec59b1553cf18fd1cc2477b217f6f659fc755d9695414da956"",""from"":""GAPGGFKPOYMNJ4PNGHI33FLV3OGOKATOCBHL36RI4HLZRVY4U7XKPPL4"",""created_at"":""12/10/2021 19:46:37""}");

            IList<PaymentOperationResponse> retrievedRecords = new List<PaymentOperationResponse>();
            var transactionHashes = new List<string> {"69930aa243a7b7ec59b1553cf18fd1cc2477b217f6f659fc755d9695414da956"};

            retrievedRecords.Add(record1);
            retrievedRecords.Add(record2);
            retrievedRecords.Add(record3);
            retrievedRecords.Add(record4);
            var decodedProposal = EncodingHelper.Decode(retrievedRecords, transactionHashes);
            
            Assert.Equal("Encoded text", decodedProposal);
        }

        [Fact]
        public void TestEncodeSequenceNumberToBase48()
        {
            Assert.Equal("4qXPrAFC", EncodingHelper.EncodeSeqNumberToBase48(91581235487423215));
        }
    }
}
