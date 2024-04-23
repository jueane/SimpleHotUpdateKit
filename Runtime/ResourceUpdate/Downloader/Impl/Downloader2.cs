using System.Collections;
using Downloader;
using UnityEngine;

public class Downloader2 : IDownloadExecutor
{
    IDownload downloader;

    long downloadedSize;

    double downloadSpeed;

    public IEnumerator Download(string url, string savePath)
    {
        var downloadOpt = new DownloadConfiguration
        {
            CheckDiskSizeBeforeDownload = false,
            MinimumSizeOfChunking = 128 * 1024,
            ParallelDownload = true, // download parts of file as parallel or not
            BufferBlockSize = 10240, // usually, hosts support max to 8000 bytes
            ChunkCount = 4, // file parts to download
            MaxTryAgainOnFailover = int.MaxValue, // the maximum number of times to fail.
            Timeout = 1000, // timeout (millisecond) per stream block reader
            MaximumBytesPerSecond = 0, //1024 * 1024, // speed limited to 1MB/s
            RequestConfiguration = // config and customize request headers
            {
                Accept = "*/*",
                KeepAlive = false,
                UseDefaultCredentials = false
            }
        };

        downloader = DownloadBuilder.New().WithUrl(url).WithFileLocation(savePath).WithConfiguration(downloadOpt).Build();

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
