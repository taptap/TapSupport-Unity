using LC.Newtonsoft.Json;

namespace TapTap.Support.Internal.Platform {
    public class UnreadStatusResult {
        [JsonProperty("code")]
        public int Code { get; private set; }

        [JsonProperty("errorMsg")]
        public string ErrorMessage { get; private set; }

        [JsonProperty("hasUnRead")]
        private int hasUnraed { get; set; }

        public bool HasUnread => hasUnraed == 1;

        public bool IsSuccessful => Code == 0;
    }
}
