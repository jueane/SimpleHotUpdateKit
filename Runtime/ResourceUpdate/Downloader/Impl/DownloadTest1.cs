using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class DownloadTest1 : IDownloadExecutor
{
    UnityWebRequest request;
    float startTime;

    public IEnumerator Download(string url, string savedPath)
    {
        request = UnityWebRequest.Get(url);
        this.startTime = Time.time;
        yield return request.SendWebRequest();

        if (request.error != null)
            Debug.LogError(request.error);

        if (request.result == UnityWebRequest.Result.Success)
        {
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
            Debug.LogError($"Resource download failed: {request.error}, url: {url}");
        }
    }

    public long GetDownloadedSize()
    {
        return (long)request.downloadedBytes;
    }

    public double GetDownloadedSpeed()
    {
        var timeElapsed = Time.time - startTime;
        if (timeElapsed <= 0)
            return 0;
        return request.downloadedBytes / timeElapsed;
    }

    public void Dispose()
    {
        request.Dispose();
    }
}
