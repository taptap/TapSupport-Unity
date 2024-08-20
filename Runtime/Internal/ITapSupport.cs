using System.Collections.Generic;
using System.Threading.Tasks;

namespace TapTap.Support.Internal.Platform {
    public interface ITapSupport
    {
        void Init(string serverUrl, string productID, TapSupportDelegate supportDelegate);
        void OpenSupportView(string path);
        void CloseSupportView();
    }
}
