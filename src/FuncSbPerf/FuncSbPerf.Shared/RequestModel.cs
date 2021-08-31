using Newtonsoft.Json;
using System.Collections.Generic;

namespace FuncSbPerf.Shared
{
    public class RequestModel
    {
        [JsonProperty(PropertyName = "messages")]
        public List<MessageModel> Messages { get; set; }

        public RequestModel() 
        {
            Messages = new List<MessageModel>();
        }
    }
}
