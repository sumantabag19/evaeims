using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EVA.EIMS.Common
{
    /// <summary>
    /// ReturnResult can be used to return result in specified format 
    /// </summary>
    public class ReturnResult
    {
        object _data;
        string _result;  

        [JsonProperty(PropertyName = "Success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "Result")]
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;          
            }
        }

        [JsonProperty(PropertyName = "Data")]
        public object Data
        {
            get { return _data; }
            set
            {
                _data = value;
            }
        }
    }
}
