using System.Collections;
using System.Collections.Generic;
using LC.Newtonsoft.Json;

namespace TapTap.Support.Internal {
    public class ErrorResponse {
        [JsonProperty("numCode")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
