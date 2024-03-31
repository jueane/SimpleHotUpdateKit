using Downloader;
using UnityEngine;

public class DownloadJob
{
    public DownloadDetailInfo downloadDetailInfo;

    public IDownloadExecutor downloader;

    public void Reset()
    {
        downloader.Dispose();
        downloader = new DownloadTest4();
        downloadDetailInfo.Reset();
    }
}
