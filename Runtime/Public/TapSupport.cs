using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TapTap.Support.Internal.Platform;

[assembly: UnityEngine.Scripting.Preserve]
namespace TapTap.Support {
    public class TapSupport {
        private static ITapSupport platformWrapper;

        static TapSupport() {
            Type interfaceType = typeof(ITapSupport);
            Type platformSupportType = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asssembly => asssembly.GetName().FullName.StartsWith("TapTap.Support"))
                .SelectMany(assembly => assembly.GetTypes())
                .SingleOrDefault(clazz => interfaceType.IsAssignableFrom(clazz) && clazz.IsClass);
            platformWrapper = Activator.CreateInstance(platformSupportType) as ITapSupport;
        }

        public static void Init(string serverUrl, string productID, TapSupportDelegate supportDelegate) {
            if (string.IsNullOrEmpty(serverUrl)) {
                throw new ArgumentNullException(nameof(serverUrl));
            }
            if (string.IsNullOrEmpty(productID)) {
                throw new ArgumentNullException(nameof(productID));
            }
            platformWrapper?.Init(serverUrl, productID, supportDelegate);
        }

        public static void SetDefaultMetaData(Dictionary<string, object> metaData) {
            platformWrapper?.SetDefaultMetaData(metaData);
        }

        public static void SetDefaultFieldsData(Dictionary<string, object> fieldsData) {
            platformWrapper?.SetDefaultFieldsData(fieldsData);
        }

        public static void LoginAnonymously(string userId) {
            platformWrapper?.LoginAnonymously(userId);
        }

        public static void Logout() {
            platformWrapper?.Logout();
        }

        public static Task<string> GetSupportWebUrl(string path = null,
            Dictionary<string, object> metaData = null,
            Dictionary<string, object> fieldsData = null) {
            return platformWrapper?.GetSupportWebUrl(path, metaData, fieldsData);
        }

        public static void OpenSupportView(string path = null,
            Dictionary<string, object> metaData = null,
            Dictionary<string, object> fieldsData = null) {
            platformWrapper?.OpenSupportView(path, metaData, fieldsData);
        }

        public static void CloseSupportView() {
            platformWrapper?.CloseSupportView();
        }

        public static void Resume() {
            platformWrapper?.Resume();
        }

        public static void Pause() {
            platformWrapper?.Pause();
        }

        public static void FetchUnReadStatus() {
            platformWrapper?.FetchUnReadStatus();
        }
    }
}
