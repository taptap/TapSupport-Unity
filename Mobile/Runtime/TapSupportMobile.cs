#if UNITY_ANDROID || UNITY_IOS

using System.Collections.Generic;
using System.Threading.Tasks;
using LC.Newtonsoft.Json;
using TapTap.Common;

namespace TapTap.Support.Internal.Platform {
    public class TapSupportMobile : ITapSupport {
        const string SERVICE_NAME = "TDSTapSupportService";

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

        public void SetDefaultMetaData(Dictionary<string, object> metaData) {
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("setDefaultMetaData")
                .Args("setDefaultMetaData", JsonConvert.SerializeObject(metaData))
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void SetDefaultFieldsData(Dictionary<string, object> fieldsData) {
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("setDefaultFieldsData")
                .Args("setDefaultFieldsData", JsonConvert.SerializeObject(fieldsData))
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void LoginAnonymously(string userId) {
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("loginAnonymously")
                .Args("loginAnonymously", userId)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void Logout() {
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("logout")
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public async Task<string> GetSupportWebUrl(string path = null,
            Dictionary<string, object> metaData = null,
            Dictionary<string, object> fieldsData = null) {
            Command.Builder cmdBuilder = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("getSupportWebUrl")
                .Callback(true);
            cmdBuilder.Args("path", path);
            cmdBuilder.Args("metaData", metaData);
            cmdBuilder.Args("fieldsData", fieldsData);
            Command command = cmdBuilder.CommandBuilder();
            Result result = await EngineBridge.GetInstance().Emit(command);
            if (!EngineBridge.CheckResult(result)) {
                throw new TapException((int)TapErrorCode.ERROR_CODE_BRIDGE_EXECUTE, result.message);
            }

            SupportUrlResult supportUrlResult = JsonConvert.DeserializeObject<SupportUrlResult>(result.content);
            return supportUrlResult.Url;
        }

        public void OpenSupportView(string path = null,
            Dictionary<string, object> metaData = null,
            Dictionary<string, object> fieldsData = null) {
            Command.Builder cmdBuilder = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("openSupportView");
            cmdBuilder.Args("openSupportView", path);
            cmdBuilder.Args("metaData", metaData);
            cmdBuilder.Args("fieldsData", fieldsData);
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

        public void Resume() {
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("resume")
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void Pause() {
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("pause")
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void FetchUnReadStatus() {
            Command command = new Command.Builder()
                .Service(SERVICE_NAME)
                .Method("fetchUnreadStatus")
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }
    }
}

#endif