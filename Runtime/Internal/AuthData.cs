using System.Collections.Generic;

namespace TapTap.Support.Internal.Platform {
    public class AuthData {
        public static readonly string ANONYMOUS_ID_FRAGMENT_KEY = "anonymous-id";
        public static readonly string ANONYMOUS_ID_HEADER_KEY = "X-Anonymous-ID";

        public static readonly string CUSTOM_CREDENTIAL_FRAGMENT_KEY = "credential";
        public static readonly string CUSTOM_CREDENTIAL_HEADER_KEY = "X-Credential";

        public static readonly string TDS_CREDENTIAL_FRAGMENT_KEY = "tds-credential";
        public static readonly string TDS_CREDENTIAL_HEADER_KEY = "X-TDS-Credential";

        public KeyValuePair<string, string> Fragment;

        public AuthData(KeyValuePair<string, string> fragment, KeyValuePair<string, string> header) {
            this.Fragment = fragment;
            Header = header;
        }

        public KeyValuePair<string, string> Header { get; set; }
    }
}
