using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityCommunity.UnitySingleton;

public class DownloadScheduler : PersistentMonoSingleton<DownloadScheduler>
{
    const int MAX_RETRY_COUNT = int.MaxValue;
    int downloadConcurrent = ApplicationConst.config.downloadConcurrent;

    int taskCount;

    private Queue<DownloadJob> waitingQueue = new Queue<DownloadJob>();
    private List<DownloadJob> downloadingList = new List<DownloadJob>();
    private Queue<DownloadJob> finishedQueue = new Queue<DownloadJob>();

    public bool IsAllDownloadFinished => finishedQueue.Count == taskCount;

    public void Add(DownloadDetailInfo downloadDetailInfo)
    {
        var newJob = new DownloadJob
        {
            downloadDetailInfo = downloadDetailInfo,
        };

        taskCount++;
        StartJob(newJob);
    }

    void StartJob(DownloadJob job)
    {
        job.CreateNewDownloader();
        waitingQueue.Enqueue(job);
    }

    void Update()
    {
        foreach (var curDownloading in downloadingList)
        {
            curDownloading.downloadDetailInfo.downloadBytes = curDownloading.downloader.GetDownloadedSize();
            curDownloading.downloadDetailInfo.downloadSpeed = curDownloading.downloader.GetDownloadedSpeed();
        }

        while (waitingQueue.Count > 0 && downloadingList.Count < downloadConcurrent)
        {
            var curDl = waitingQueue.Dequeue();
            downloadingList.Add(curDl);
            curDl.downloadDetailInfo.downloadStarted = true;
            StartCoroutine(Download(curDl));
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
        downloadingList.Remove(job);
        job.downloader.Dispose();

        var saved = File.Exists(savePath);
        if (saved)
        {
            info.saved = true;

            var task = ChecksumAsync(info.savePath, info.checksum);
            yield return task.AsCoroutine();
            info.checksumPassed = task.Result;
            if (info.checksumPassed)
                File.WriteAllText(info.ChecksumFilePath, info.checksum.ToString());
        }

        if (info.checksumPassed)
        {
            finishedQueue.Enqueue(job);
            info.downloadBytes = info.totalBytes;
            var progressDesc = $"{finishedQueue.Count}/{taskCount}";
            var sizeDesc = info.totalBytes.CalcMemoryMensurableUnit();
            Debug.Log($"Download progress: {progressDesc} [{sizeDesc}], retried {info.retryCount} times, {info.url}\n{info.savePath}");
        }
        else
        {
            if (info.retryCount < MAX_RETRY_COUNT)
            {
                yield return new WaitForSeconds(3);
                Debug.Log($"Download failed, saved:{saved}, checksum check passed:{info.checksumPassed}, retried {info.retryCount} times, [{info.totalBytes.CalcMemoryMensurableUnit()}], {url}");
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

    public override void ClearSingleton()
    {
        waitingQueue.Clear();
        downloadingList.Clear();
        finishedQueue.Clear();
    }
}
