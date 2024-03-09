using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.Singleton;

public class DownloadScheduler : MonoSingleton<DownloadScheduler>
{
    const int CONCURRENT = 3;
    const int MAX_RETRY_COUNT = 5;

    private Queue<DownloadJob> downloadWaitingQueue = new Queue<DownloadJob>();
    private List<DownloadJob> downloadingList = new List<DownloadJob>();
    private Queue<DownloadJob> downloadFinishedQueue = new Queue<DownloadJob>();

    bool NoRemainingTask => downloadFinishedQueue.Count == (downloadingList.Count + downloadWaitingQueue.Count + downloadFinishedQueue.Count);

    public bool IsAllDownloadFinished { get; private set; }

    protected override void Init()
    {
    }

    public void Add(DownloadDetailInfo downloadDetailInfo)
    {
        var newDownload = new DownloadJob
        {
            downloadDetailInfo = downloadDetailInfo,
            downloader = new DownloadTest4(),
        };

        IsAllDownloadFinished = false;
        downloadWaitingQueue.Enqueue(newDownload);
    }

    void Update()
    {
        foreach (var curDownloading in downloadingList)
        {
            curDownloading.downloadDetailInfo.downloadBytes = curDownloading.downloader.GetDownloadedSize();
        }

        for (int i = downloadingList.Count - 1; i >= 0; i--)
        {
            var curDl = downloadingList[i];
            if (curDl.downloadDetailInfo.checksumPassed)
            {
                downloadingList.RemoveAt(i);
                downloadFinishedQueue.Enqueue(curDl);
                curDl.downloadDetailInfo.downloadBytes = curDl.downloadDetailInfo.totalBytes;
                var progressDesc = $"{downloadFinishedQueue.Count}/{downloadingList.Count + downloadWaitingQueue.Count + downloadFinishedQueue.Count}";
                var skippedDesc = curDl.downloadDetailInfo.skipped ? $"[Skipped]" : null;
                Debug.Log($"Download progress: {progressDesc} {skippedDesc}, {curDl.downloadDetailInfo.savePath}");
            }
        }

        while (downloadWaitingQueue.Count > 0 && downloadingList.Count < CONCURRENT)
        {
            var curDl = downloadWaitingQueue.Dequeue();
            downloadingList.Add(curDl);
            StartCoroutine(Download(curDl));
        }

        if (!IsAllDownloadFinished && NoRemainingTask)
        {
            IsAllDownloadFinished = true;
            downloadWaitingQueue.Clear();
            downloadingList.Clear();
            downloadFinishedQueue.Clear();
        }
    }

    IEnumerator Download(DownloadJob job)
    {
        var info = job.downloadDetailInfo;

        string url = info.url;
        var savePath = info.savePath;

        var dir = Path.GetDirectoryName(savePath);
        FolderUtility.EnsurePathExists(dir);

        if (File.Exists(info.savePath))
            File.Delete(info.savePath);

        yield return job.downloader.Download(url, savePath);

        var saved = File.Exists(savePath);
        if (saved)
        {
            var task = ChecksumAsync(info.savePath, info.checksum);
            // yield return task;
            yield return new WaitUntil(() => task.IsCompleted);
            info.checksumPassed = task.Result;
            File.WriteAllText(info.ChecksumFilePath, info.checksum.ToString());
        }

        if (!info.checksumPassed)
        {
            if (info.retryCount < MAX_RETRY_COUNT)
            {
                Debug.Log($"Download failed, saved:{saved}, checksum check passed:{info.checksumPassed}, retry {url}");
                info.retryCount++;
                StartCoroutine(Download(job));
            }
            else
            {
                Debug.Log($"Download failed {info.retryCount} times, give up {url}");
            }
        }
    }

    // 异步检验方法
    async Task<bool> ChecksumAsync(string filePath, long expectedChecksum)
    {
        var actualChecksum = await Task.Run((() => CRC32Calculator.CalculateCRC32(filePath)));
        return actualChecksum == expectedChecksum;
    }

    protected override void Dispose()
    {
    }
}
