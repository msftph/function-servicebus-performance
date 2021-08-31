using Newtonsoft.Json;
using System;

namespace FuncSbPerf.Shared
{
    public class MessageModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }
}
