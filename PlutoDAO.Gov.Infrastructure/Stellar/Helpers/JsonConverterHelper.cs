using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlutoDAO.Gov.Domain;

namespace PlutoDAO.Gov.Infrastructure.Stellar.Helpers
{
    class JsonConverterHelper : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Proposal));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var name = (string)jo["Name"];
            var description = (string)jo["Description"];
            var creator = (string)jo["Creator"];
            var whitelistedAssets = JArray.Parse(jo["WhitelistedAssets"].ToString()).Select(x => new WhitelistedAsset(new Asset(new AccountAddress(x["Asset"]["Issuer"]["Address"].ToString()), x["Asset"]["Code"].ToString(), (bool)x["Asset"]["IsNative"]), (decimal)x["Multiplier"])).ToArray();
            var created = (DateTime) jo["Created"];
            var deadline = (DateTime) jo["Deadline"];
            var options = JArray.Parse(jo["Options"].ToString()).Select(x => new Option(x["Name"].ToString())).ToArray();

            var proposal = new Proposal(name, description, creator, whitelistedAssets, created, deadline, options);
            return proposal;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
