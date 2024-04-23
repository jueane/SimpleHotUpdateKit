using Downloader;
using UnityEngine;

public class DownloadJob
{
    public DownloadDetailInfo downloadDetailInfo;

    public IDownloadExecutor downloader;

    public void CreateNewDownloader()
    {
        downloader = new Downloader1()
        {
            timeout = Mathf.Max(60, 50 * (int)(downloadDetailInfo.totalBytes / (1024 * 1024)))
        };
    }

    public void Reset()
    {
        downloader.Dispose();
        downloadDetailInfo.Reset();
    }
}
