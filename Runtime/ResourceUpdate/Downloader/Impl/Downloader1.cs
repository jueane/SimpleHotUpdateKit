using System;
using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class Downloader1 : IDownloadExecutor
{
    UnityWebRequest request;
    float startTime;

    public int timeout;

    public IEnumerator Download(string url, string savedPath)
    {
        request = UnityWebRequest.Get(url);
        if (this.timeout > 0)
            request.timeout = this.timeout;
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
