using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Common
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class Key
    {
        public string use { get; set; }
        public string kty { get; set; }
        public string alg { get; set; }
        [JsonProperty(PropertyName = "e")]
        public string Exponent { get; set; }
        [JsonProperty(PropertyName = "n")]
        public string Modulus { get; set; }
    }

    public class JWKSET
    {
        public List<Key> keys { get; set; }

    }
}
