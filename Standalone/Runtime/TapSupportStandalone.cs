#if UNITY_STANDALONE || UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using UnityEngine;
using LC.Newtonsoft.Json;
using TapTap.Common.Internal.Http;

[assembly: UnityEngine.Scripting.Preserve]
namespace TapTap.Support.Internal.Platform {
    public class TapSupportStandalone : ITapSupport {
        private static readonly string UNREAD_ENDPOINT = "/api/2/unread";
        private const int POLL_INTERVAL = 10;
        private const int MAX_POLL_INTERVAL = 300;

        private string serverUrl;
        private string productId;
        private TapSupportDelegate supportDelegate;
        private Dictionary<string, object> defaultMetaData;
        private Dictionary<string, object> defaultFieldsData;
        private string anonymousUserId;

        private TapSupportPoll poll;
        private Coroutine pollCoroutine;

        private HttpClient httpClient;

        private HttpClient HttpClient {
            get {
                if (httpClient == null) {
                    httpClient = new HttpClient();
                }
                return httpClient;
            }
        }

        private int currentPollInterval;

        private string lastUnreadStatus;

        public void Init(string serverUrl, string productId, TapSupportDelegate supportDelegate) {
            this.serverUrl = serverUrl;
            this.productId = productId;
            this.supportDelegate = supportDelegate;
        }

        public void SetDefaultMetaData(Dictionary<string, object> metaData) {
            defaultMetaData = metaData;
        }

        public void SetDefaultFieldsData(Dictionary<string, object> fieldsData) {
            defaultFieldsData = fieldsData;
        }

        public void LoginAnonymously(string userId) {
            if (string.IsNullOrEmpty(userId)) {
                throw new ArgumentNullException(nameof(userId));
            }
            if (anonymousUserId != null && anonymousUserId != userId) {
                // 已经登录情况
                Logout();
            }
            anonymousUserId = userId;
        }

        public void Logout() {
            anonymousUserId = null;
            Pause();
        }

        public Task<string> GetSupportWebUrl(string path = null, Dictionary<string, object> metaData = null, Dictionary<string, object> fieldsData = null) {
            if (string.IsNullOrEmpty(anonymousUserId)) {
                throw new ArgumentNullException(nameof(anonymousUserId));
            }

            if (string.IsNullOrEmpty(path)) {
                path = "/";
            }

            string url = $"{serverUrl}/in-app/v1/categories/{productId}{path}#anonymous-id={anonymousUserId}";

            if (metaData == null) {
                metaData = defaultMetaData;
            }
            string metaStr = GetEncodeData(metaData);
            if (!string.IsNullOrEmpty(metaStr)) {
                url = $"{url}&meta={metaStr}";
            }

            if (fieldsData == null) {
                fieldsData = defaultFieldsData;
            }
            string fieldStr = GetEncodeData(fieldsData);
            if (!string.IsNullOrEmpty(fieldStr)) {
                url = $"{url}&fields={fieldStr}";
            }

            return Task.FromResult(url);
        }

        public async void OpenSupportView(string path = null, Dictionary<string, object> metaData = null, Dictionary<string, object> fieldsData = null) {
            if (string.IsNullOrEmpty(anonymousUserId)) {
                throw new ArgumentNullException(nameof(anonymousUserId));
            }

            string url = await GetSupportWebUrl(path, metaData, fieldsData);
            Application.OpenURL(url);
        }

        public void CloseSupportView() {
            throw new NotImplementedException();
        }

        public void Resume() {
            if (string.IsNullOrEmpty(anonymousUserId)) {
                throw new ArgumentNullException(nameof(anonymousUserId));
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

        private IEnumerator Poll() {
            while (true) {
                Task<bool> task = FetchUnReadStatusInternal();
                while (!task.IsCompleted) {
                    yield return null;
                }
                bool hasUnread = task.Result;
                if (hasUnread) {
                    currentPollInterval = POLL_INTERVAL;
                } else {
                    currentPollInterval += POLL_INTERVAL;
                    currentPollInterval = Math.Min(currentPollInterval, MAX_POLL_INTERVAL);
                }
                yield return new WaitForSeconds(currentPollInterval);
            }
        }

        private async Task<bool> FetchUnReadStatusInternal() {
            try {
                HttpRequestMessage request = new HttpRequestMessage {
                    RequestUri = new Uri($"{serverUrl}{UNREAD_ENDPOINT}?product={productId}"),
                    Method = HttpMethod.Get,
                };
                request.Headers.Add("X-Anonymous-ID", anonymousUserId);
                TapHttpUtils.PrintRequest(HttpClient, request);

                HttpResponseMessage response = await HttpClient.SendAsync(request);
                request.Dispose();

                string resultString = await response.Content.ReadAsStringAsync();
                response.Dispose();
                TapHttpUtils.PrintResponse(response, resultString);

                bool hasUnread = "true".Equals(resultString);
                if (resultString != lastUnreadStatus) {
                    lastUnreadStatus = resultString;
                    supportDelegate?.OnUnreadStatusChanged?.Invoke(hasUnread);
                }
                return hasUnread;
            } catch (Exception e) {
                supportDelegate?.OnGetUnreadStatusError?.Invoke(e);
                return false;
            }
        }

        public void Pause() {
            if (pollCoroutine != null) {
                poll.StopCoroutine(pollCoroutine);
                pollCoroutine = null;
            }
        }

        public void FetchUnReadStatus() {
            if (string.IsNullOrEmpty(anonymousUserId)) {
                throw new ArgumentNullException(nameof(anonymousUserId));
            }
            _ = FetchUnReadStatusInternal();
        }

        private static string GetEncodeData(Dictionary<string, object> data) {
            if (data == null) {
                return null;
            }
            return Uri.EscapeDataString(JsonConvert.SerializeObject(data));
        }
    }
}

#endif
