#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR

using System.Collections.Generic;
using LC.Newtonsoft.Json;
using TapTap.Common;

[assembly: UnityEngine.Scripting.Preserve]
namespace TapTap.Support.Internal.Platform {
    public class TapSupportMobile : ITapSupport {
        const string SERVICE_NAME = "TDSTapSupportService";

        private const string ServiceClz = "com.tds.tapsupport.wrapper.TapSupportService";

        private const string ServiceImpl = "com.tds.tapsupport.wrapper.TapSupportServiceImpl";

        public TapSupportMobile() {
            EngineBridge.GetInstance().Register(ServiceClz, ServiceImpl);
        }

        public void Init(string serverUrl, string productID, TapSupportDelegate supportDelegate) {
            Dictionary<string, object> config = new Dictionary<string, object> {
                { "server", serverUrl },
                { "productID", productID }
            };
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("setConfig")
                .Args("config", JsonConvert.SerializeObject(config))
                .Callback(true)
                .OnceTime(false)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command, result => {
                UnreadStatusResult unreadStatusResult = JsonConvert.DeserializeObject<UnreadStatusResult>(result.content);
                if (unreadStatusResult.IsSuccessful) {
                    supportDelegate.OnUnreadStatusChanged?.Invoke(unreadStatusResult.HasUnread);
                } else {
                    supportDelegate.OnGetUnreadStatusError?.Invoke(
                        new TapException(unreadStatusResult.Code, unreadStatusResult.ErrorMessage));
                }
            });
        }
        
        public void OpenSupportView(string path) {
            Command.Builder cmdBuilder = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("openFullUrl");
            cmdBuilder.Args("openFullUrl", path);
            Command command = cmdBuilder.CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void CloseSupportView() {
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("closeSupportView")
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }
    }
}

#endif