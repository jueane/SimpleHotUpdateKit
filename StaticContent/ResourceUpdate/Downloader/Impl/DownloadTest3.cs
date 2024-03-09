using System;
using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class DownloadTest3 : IDownloadExecutor
{
    DownloadHandlerFile downloadHandlerFile;

    public IEnumerator Download(string url, string savedPath)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // 使用 Streaming 模式，避免一次性加载整个文件到内存
            downloadHandlerFile = new DownloadHandlerFile(savedPath, true);
            request.downloadHandler = downloadHandlerFile;

            Debug.Log($"下载开始");
            var dir = Path.GetDirectoryName(savedPath);
            FolderUtility.EnsurePathExists(dir);

            // 发送请求并等待完成
            yield return request.SendWebRequest();

            Debug.Log($"下载完成");

            if (request.result == UnityWebRequest.Result.Success && File.Exists(savedPath))
            {
                Debug.Log("File downloaded successfully");
            }
            else
            {
                Debug.LogError($"Download failed: {request.error}, url: {url}");
            }
        }
    }

    public long GetDownloadedSize()
    {
        return downloadHandlerFile?.data?.Length ?? 0;
    }
}
