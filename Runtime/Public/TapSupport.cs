using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TapTap.Support.Internal.Platform;

namespace TapTap.Support {
    public class TapSupport {
        private static ITapSupport platformWrapper;

        static TapSupport() {
#if UNITY_STANDALONE || UNITY_EDITOR
            platformWrapper = new TapSupportStandalone();
#elif UNITY_ANDROID || UNITY_IOS
            platformWrapper = new TapSupportMobile();
#endif
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
