using System;
using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class DownloadTest3 : IDownloadExecutor
{
    UnityWebRequest request;
    float startTime;

    public IEnumerator Download(string url, string savedPath)
    {
        request = UnityWebRequest.Get(url);
        request.timeout = int.MaxValue;
        request.downloadHandler = new DownloadHandlerFile(savedPath);

        var dir = Path.GetDirectoryName(savedPath);
        FolderUtility.EnsurePathExists(dir);

        this.startTime = Time.time;

        yield return request.SendWebRequest();
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
