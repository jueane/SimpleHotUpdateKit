using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class DownloadTest1 : IDownloadExecutor
{
    DownloadHandler downloadHandler;

    public IEnumerator Download(string url, string savedPath)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.error != null)
            Debug.LogError(request.error);

        if (request.result == UnityWebRequest.Result.Success)
        {
            downloadHandler = request.downloadHandler;
            byte[] downloadedData = request.downloadHandler.data;

            while (true)
            {
                try
                {
                    var dir = Path.GetDirectoryName(savedPath);
                    FolderUtility.EnsurePathExists(dir);
                    File.WriteAllBytes(savedPath, downloadedData);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
        else
        {
            // 下载失败
            Debug.LogError($"Resource download failed: {request.error}, url: {url}");
        }
    }

    public long GetDownloadedSize()
    {
        return downloadHandler?.data.Length ?? 0;
    }

    public double GetDownloadedSpeed()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
