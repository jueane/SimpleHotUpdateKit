using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.Singleton;

public class DownloadScheduler : MonoSingletonSimple<DownloadScheduler>
{
    const int CONCURRENT = 5;
    const int MAX_RETRY_COUNT = int.MaxValue;

    private Queue<DownloadJob> waitingQueue = new Queue<DownloadJob>();
    private List<DownloadJob> downloadingList = new List<DownloadJob>();
    private List<DownloadJob> savedList = new List<DownloadJob>();
    private Queue<DownloadJob> finishedQueue = new Queue<DownloadJob>();

    bool NoRemainingTask => finishedQueue.Count == (waitingQueue.Count + downloadingList.Count + savedList.Count + finishedQueue.Count);

    public bool IsAllDownloadFinished { get; private set; }

    protected override void Init()
    {
    }

    public void Add(DownloadDetailInfo downloadDetailInfo)
    {
        var newJob = new DownloadJob
        {
            downloadDetailInfo = downloadDetailInfo,
            downloader = new DownloadTest4(),
        };

        StartJob(newJob);
    }

    void StartJob(DownloadJob job)
    {
        IsAllDownloadFinished = false;
        waitingQueue.Enqueue(job);
    }

    void Update()
    {
        foreach (var curDownloading in downloadingList)
        {
            curDownloading.downloadDetailInfo.downloadBytes = curDownloading.downloader.GetDownloadedSize();
            curDownloading.downloadDetailInfo.downloadSpeed = curDownloading.downloader.GetDownloadedSpeed();
        }

        while (waitingQueue.Count > 0 && downloadingList.Count < CONCURRENT)
        {
            var curDl = waitingQueue.Dequeue();
            downloadingList.Add(curDl);
            StartCoroutine(Download(curDl));
        }

        if (!IsAllDownloadFinished && NoRemainingTask)
        {
            IsAllDownloadFinished = true;
            waitingQueue.Clear();
            downloadingList.Clear();
            finishedQueue.Clear();
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
            info.saved = true;

            downloadingList.Remove(job);
            savedList.Add(job);

            var task = ChecksumAsync(info.savePath, info.checksum);
            yield return task.AsCoroutine();
            info.checksumPassed = task.Result;
            if (info.checksumPassed)
                File.WriteAllText(info.ChecksumFilePath, info.checksum.ToString());
        }

        savedList.Remove(job);

        if (info.checksumPassed)
        {
            finishedQueue.Enqueue(job);
            info.downloadBytes = info.totalBytes;
            var progressDesc = $"{finishedQueue.Count}/{downloadingList.Count + waitingQueue.Count + finishedQueue.Count}";
            var skippedDesc = info.skipped ? $"[Skipped]" : null;
            var sizeDesc = info.totalBytes.CalcMemoryMensurableUnit();
            Debug.Log($"Download progress: {progressDesc} {skippedDesc} [{sizeDesc}], {info.savePath}");
        }
        else
        {
            if (info.retryCount < MAX_RETRY_COUNT)
            {
                yield return new WaitForSeconds(3);
                Debug.Log($"Download failed, saved:{saved}, checksum check passed:{info.checksumPassed}, retry {url}");
                info.retryCount++;
                job.Reset();
                StartJob(job);
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
