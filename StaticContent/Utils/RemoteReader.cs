using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class RemoteReader
{
    public static async Task GetRemoteValue(string url, Action<bool, string> callback, int retryCount = int.MaxValue, bool failed = false)
    {
        retryCount--;
        using (UnityWebRequest hashRequest = UnityWebRequest.Get(url))
        {
            hashRequest.timeout = 10;

            // 异步发送网络请求
            var operation = hashRequest.SendWebRequest();

            // 等待请求完成
            while (!operation.isDone)
            {
                await Task.Delay(100); // 延迟一段时间后继续检查是否完成
            }

            // 处理请求结果
            string remoteValue = null;

            if (hashRequest.result == UnityWebRequest.Result.Success || hashRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                if (hashRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // 处理重定向
                    string newUrl = hashRequest.GetResponseHeader("Location");
                    if (!string.IsNullOrEmpty(newUrl))
                    {
                        await GetRemoteValue(newUrl, callback, retryCount);
                        return;
                    }
                }

                remoteValue = hashRequest.downloadHandler.text.Trim();
            }
            else
            {
                await Task.Delay(2000);
                if (!failed)
                {
                    failed = true;
                    Debug.LogError("Failed to download, retrying for: " + url);
                }

                if (retryCount > 0)
                {
                    await GetRemoteValue(url, callback, retryCount, failed);
                }

                return;
            }

            var success = hashRequest.result == UnityWebRequest.Result.Success;
            // 在主线程中调用回调函数
            callback?.Invoke(success, remoteValue);
        }
    }

    public static async Task GetRemoteValueList(string url, Action<bool, List<string>> callback)
    {
        await GetRemoteValue(url, (success, value) =>
        {
            if (success)
            {
                List<string> valueList = new List<string>(value.Split('\n'));
                callback?.Invoke(true, valueList);
            }
            else
            {
                callback?.Invoke(false, null);
            }
        });
    }
}
