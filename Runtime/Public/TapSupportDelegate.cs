using System;

namespace TapTap.Support {
    public class TapSupportDelegate {
        public Action<bool> OnUnreadStatusChanged { get; set; }
        public Action<Exception> OnGetUnreadStatusError { get; set; }
    }
}
