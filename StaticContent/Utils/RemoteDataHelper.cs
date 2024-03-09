using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Main.Utils
{
    public class RemoteDataHelper
    {
        public static IEnumerator GetRemoteValue(string url, Action<bool, string> callback)
        {
            UnityWebRequest hashRequest = UnityWebRequest.Get(url);
            yield return hashRequest.SendWebRequest();

            string remoteValue = null;

            var checkSuccess = hashRequest.result == UnityWebRequest.Result.Success;
            if (checkSuccess)
            {
                remoteValue = hashRequest.downloadHandler.text.Trim();

                // Debug.Log($"Remote url: {url}");
                // Debug.Log($"Remote value: {remoteValue}");
            }
            else
            {
                remoteValue = null;
                Debug.LogError("Failed to download hash for: " + url);
            }

            callback?.Invoke(checkSuccess, remoteValue);
        }

        public static IEnumerator CheckRemoteValue(string url, string value, Action<bool, bool> callback)
        {
            UnityWebRequest hashRequest = UnityWebRequest.Get(url);
            yield return hashRequest.SendWebRequest();

            var checkSuccess = hashRequest.result == UnityWebRequest.Result.Success;
            var valueSame = false;

            if (checkSuccess)
            {
                string remoteValue = hashRequest.downloadHandler.text.Trim();

                Debug.Log($"Compare url: {url}");
                Debug.Log($"Local value : {value}");
                Debug.Log($"Remote value: {remoteValue}");
                valueSame = value.Equals(remoteValue);
            }
            else
            {
                Debug.LogError("Failed to download hash for: " + url);
                valueSame = false;
            }

            callback?.Invoke(checkSuccess, valueSame);
        }
    }
}