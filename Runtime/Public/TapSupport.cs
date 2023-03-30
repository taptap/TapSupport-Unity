using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using LC.Newtonsoft.Json;
using TapTap.Common;
using TapTap.Support.Internal;
using TapTap.Support.Internal.Platform;
using UnityEngine;

[assembly: UnityEngine.Scripting.Preserve]
namespace TapTap.Support {
    public class TapSupport {
        public static readonly int UNAUTHORIZED = 9000;
        public static readonly int NOT_FOUND = 9001;
        public static readonly int INVALID_LOGIN_CREDENTIAL = 9002;
        public static readonly int USER_NOT_REGISTERED = 9003;
        public static readonly int CREDENTIAL_REQUIRED = 9004;
        public static readonly int WEAK_ANONYMOUS_ID = 9005;
        public static readonly int EXPIRED_CREDENTIAL = 9006;
        public static readonly int INTERNAL_SERVER_ERROR = 9500;

        private static ITapSupport platformWrapper;
        
        private const int POLL_INTERVAL = 10;
        private const int MAX_POLL_INTERVAL = 300;
        private const int NO_REGISTERED_USER_POLL_INTERVAL = 3600;
        
        private static string serverUrl;
        private static string productId;
        private static TapSupportDelegate supportDelegate;
        
        private static AuthData authData;
        
        public static Dictionary<string, object> DefaultFields { get; set; }
        
        private static int currentPollInterval;
        private static bool lastUnreadStatus;
        
        private static TapSupportPoll poll;
        private static Coroutine pollCoroutine;
        
        private static SupportHttpClient supportHttpClient;
        private static SupportHttpClient SupportHttpClient {
            get {
                if (supportHttpClient == null) {
                    supportHttpClient = new SupportHttpClient(serverUrl);
                }
                return supportHttpClient;
            }
        }

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
            TapSupport.serverUrl = serverUrl;
            TapSupport.productId = productID;
            TapSupport.supportDelegate = supportDelegate;
            platformWrapper.Init(serverUrl, productID, supportDelegate);
        }

        public static void SetDefaultMetaData(Dictionary<string, object> metaData) {
            if (DefaultFields == null) {
                DefaultFields = new Dictionary<string, object>();
            }
            if (metaData != null) {
                foreach (KeyValuePair<string, object> kv in metaData) {
                    DefaultFields[kv.Key] = kv.Value;
                }
            }
        }

        public static void SetDefaultFieldsData(Dictionary<string, object> fieldsData) {
            if (DefaultFields == null) {
                DefaultFields = new Dictionary<string, object>();
            }
            if (fieldsData != null) {
                foreach (KeyValuePair<string, object> kv in fieldsData) {
                    DefaultFields[kv.Key] = kv.Value;
                }
            }
        }

        public static void LoginAnonymously(string userId) {
            if (string.IsNullOrEmpty(userId)) {
                throw new ArgumentNullException(nameof(userId));
            }
            if (authData != null) {
                // 已经登录情况
                Logout();
            }

            authData = new AuthData(new KeyValuePair<string, string>(AuthData.ANONYMOUS_ID_FRAGMENT_KEY, userId),
                new KeyValuePair<string, string>(AuthData.ANONYMOUS_ID_HEADER_KEY, userId));
            Resume();
        }

        public static void Logout() {
            authData = null;
            Pause();
        }

        [Obsolete]
        public static Task<string> GetSupportWebUrl(string path = null,
            Dictionary<string, object> metaData = null,
            Dictionary<string, object> fieldsData = null) {
            if (authData == null) {
                throw new ArgumentNullException(nameof(authData));
            }

            if (string.IsNullOrEmpty(path)) {
                path = "/";
            }

            string url = $"{serverUrl}/in-app/v1/products/{productId}{path}#{authData.Fragment}";
            Dictionary<string, object> totalFields = new Dictionary<string, object>();
            if (DefaultFields != null) {
                foreach (KeyValuePair<string, object> kv in DefaultFields) {
                    totalFields[kv.Key] = kv.Value;
                }
            }
            if (metaData != null) {
                foreach (KeyValuePair<string, object> kv in metaData) {
                    totalFields[kv.Key] = kv.Value;
                }
            }
            if (fieldsData != null) {
                foreach (KeyValuePair<string, object> kv in fieldsData) {
                    totalFields[kv.Key] = kv.Value;
                }
            }
            if (totalFields.Count > 0) {
                string fieldStr = GetEncodeData(totalFields);
                if (!string.IsNullOrEmpty(fieldStr)) {
                    url = $"{url}&fields={fieldStr}";
                }
            }

            return Task.FromResult(url);
        }

        public static Task<string> GetSupportWebUrl(string path = null,
            Dictionary<string, object> fields = null) {
            return GetSupportWebUrl(path: path, fieldsData: fields);
        }

        [Obsolete]
        public static async void OpenSupportView(string path = null,
            Dictionary<string, object> metaData = null,
            Dictionary<string, object> fieldsData = null) {
            if (authData == null) {
                throw new ArgumentNullException(nameof(authData));
            }

            string url = await GetSupportWebUrl(path, metaData, fieldsData);
            platformWrapper.OpenSupportView(url);
            // TRICK 
            await Task.Delay(3000);
            Pause();
            Resume();
        }

        public static void OpenSupportView(string path = null,
            Dictionary<string, object> fields = null)
        {
            OpenSupportView(path: path, fieldsData: fields);
        }

        public static void CloseSupportView() {
            platformWrapper.CloseSupportView();
        }

        public static void Resume() {
            if (authData == null) {
                throw new ArgumentNullException(nameof(authData));
            }

            if (poll == null) {
                GameObject pollGo = new GameObject("_TapSupportPoll");
                poll = pollGo.AddComponent<TapSupportPoll>();
                UnityEngine.Object.DontDestroyOnLoad(pollGo);
            }
            if (pollCoroutine == null) {
                currentPollInterval = 0;
                pollCoroutine = poll.StartCoroutine(Poll());
            }
        }

        public static void Pause() {
            if (pollCoroutine != null) {
                poll.StopCoroutine(pollCoroutine);
                pollCoroutine = null;
            }
        }

        public static void FetchUnReadStatus() {
            if (authData == null) {
                throw new ArgumentNullException(nameof(authData));
            }
            _ = FetchUnReadStatusInternal();
        }

        public static void LoginWithCustomCredential(string credential) {
            if (string.IsNullOrEmpty(credential)) {
                throw new ArgumentNullException(nameof(credential));
            }
            if (authData != null) {
                Logout();
            }
            authData = new AuthData(new KeyValuePair<string, string>(AuthData.CUSTOM_CREDENTIAL_FRAGMENT_KEY, credential),
                new KeyValuePair<string, string>(AuthData.CUSTOM_CREDENTIAL_HEADER_KEY, credential));
            Resume();
        }

        public static async Task LoginWithTDSCredential(string credential) {
            if (string.IsNullOrEmpty(credential)) {
                throw new ArgumentNullException(nameof(credential));
            }
            if (authData != null) {
                Logout();
            }

            string path = "api/2/users/tds/token";
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "jwt", credential }
            };
            Dictionary<string, object> result = await SupportHttpClient.Post<Dictionary<string, object>>(path, data: data);
            string jwt = result["jwt"] as string;

            authData = new AuthData(new KeyValuePair<string, string>(AuthData.TDS_CREDENTIAL_FRAGMENT_KEY, jwt),
                new KeyValuePair<string, string>(AuthData.TDS_CREDENTIAL_HEADER_KEY, jwt));
            Resume();
        }

        private static Dictionary<string, object> MergeMetaAndFields(Dictionary<string, object> metaData, Dictionary<string, object> fieldsData) {
            Dictionary<string, object> fields = new Dictionary<string, object>();
            if (metaData != null) {
                foreach (KeyValuePair<string, object> kv in metaData) {
                    fields[kv.Key] = kv.Value;
                }
            }
            if (fieldsData != null) {
                foreach (KeyValuePair<string, object> kv in fieldsData) {
                    fields[kv.Key] = kv.Value;
                }
            }
            return fields;
        }
        
        private static string GetEncodeData(Dictionary<string, object> data) {
            if (data == null) {
                return null;
            }
            return Uri.EscapeDataString(JsonConvert.SerializeObject(data));
        }
        
        private static IEnumerator Poll() {
            while (true) {
                Task<bool> task = FetchUnReadStatusInternal();
                while (!task.IsCompleted) {
                    yield return null;
                }

                if (task.IsFaulted) {
                    Exception e = task.Exception.InnerException;
                    if (e is TapException ex) {

                        if (ex.Code == UNAUTHORIZED || ex.Code == NOT_FOUND || ex.Code == INVALID_LOGIN_CREDENTIAL ||
                            ex.Code == CREDENTIAL_REQUIRED || ex.Code == WEAK_ANONYMOUS_ID || ex.code == EXPIRED_CREDENTIAL ||
                            ex.Code == INTERNAL_SERVER_ERROR) {
                            throw ex;
                        }

                        if (ex.code == USER_NOT_REGISTERED) {
                            // 判断是否是未注册用户
                            currentPollInterval = NO_REGISTERED_USER_POLL_INTERVAL;
                        } else {
                            currentPollInterval = Math.Max(currentPollInterval, POLL_INTERVAL);
                        }
                    } else {
                        currentPollInterval = Math.Max(currentPollInterval, POLL_INTERVAL);
                    }
                } else {
                    bool hasUnread = task.Result;
                    if (hasUnread != lastUnreadStatus) {
                        lastUnreadStatus = hasUnread;
                        currentPollInterval = POLL_INTERVAL;
                        supportDelegate?.OnUnreadStatusChanged?.Invoke(hasUnread);
                    } else {
                        currentPollInterval += POLL_INTERVAL;
                        currentPollInterval = Math.Min(currentPollInterval, MAX_POLL_INTERVAL);
                    }
                }

                // 轮询
                yield return new WaitForSeconds(currentPollInterval);
            }
        }
        
        private static async Task<bool> FetchUnReadStatusInternal() {
            try {
                string path = "api/2/unread";
                Dictionary<string, object> headers = new Dictionary<string, object> {
                    { authData.Header.Key, authData.Header.Value }
                };
                Dictionary<string, object> queryParams = new Dictionary<string, object> {
                    { "product", productId }
                };
                
                return await SupportHttpClient.Get<bool>(path, headers, queryParams);
            } catch (Exception e) {
                supportDelegate?.OnGetUnreadStatusError?.Invoke(e);
                throw e;
            }
        }
    }
}
