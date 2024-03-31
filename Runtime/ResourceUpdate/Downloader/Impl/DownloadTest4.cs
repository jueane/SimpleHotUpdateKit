using System.Collections;
using Downloader;
using UnityEngine;

public class DownloadTest4 : IDownloadExecutor
{
    IDownload downloader;

    long downloadedSize;

    double downloadSpeed;

    public IEnumerator Download(string url, string savePath)
    {
        var downloadOpt = new DownloadConfiguration()
        {
            CheckDiskSizeBeforeDownload = false,

            // ChunkCount = 8, // file parts to download, the default value is 1
            // ParallelDownload = true // download parts of the file as parallel or not. The default value is false
        };
        downloader = DownloadBuilder.New()
            .WithUrl(url)
            .WithFileLocation(savePath)
            .WithConfiguration(downloadOpt)
            .Build();

        downloader.DownloadProgressChanged += DownloaderOnDownloadProgressChanged;

        var task = downloader.StartAsync();

        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.Exception != null)
        {
            Debug.LogException(task.Exception);
        }
    }

    void DownloaderOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        downloadedSize = e.ReceivedBytesSize;
        downloadSpeed = e.BytesPerSecondSpeed;
    }

    public long GetDownloadedSize()
    {
        return downloadedSize;
    }

    public double GetDownloadedSpeed()
    {
        return downloadSpeed;
    }

    public void Dispose()
    {
        downloader.Dispose();
    }
}
