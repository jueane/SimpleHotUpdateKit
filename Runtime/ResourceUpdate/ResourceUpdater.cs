using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityCommunity.UnitySingleton;

public class ResourceUpdater : PersistentMonoSingleton<ResourceUpdater>
{
    public enum CheckUpdateStatus
    {
        None,
        Checking,
        Updating,
        Finished,
    }

    public List<DownloadDetailInfo> taskList = new List<DownloadDetailInfo>();

    public CheckUpdateStatus updateStatus { get; private set; }

    public long downloadBytes;

    public long totalBytes;

    public double downloadSpeed;

    public IEnumerator UpdateAll()
    {
        updateStatus = CheckUpdateStatus.Checking;

        List<URLListHandler> urlListHandlers = new List<URLListHandler>()
        {
            new URLListHandler()
            {
                baseUrl = $"{ApplicationConst.BaseRemoteURL_CODE}",
                versionCode = $"{VersionChecker.VersionInfo.codeVersion}",
            },
            new URLListHandler()
            {
                baseUrl = $"{ApplicationConst.BaseRemoteURL_RESOURCE}",
                versionCode = $"{VersionChecker.VersionInfo.resourceVersion}",
            },
        };

        foreach (var curURLListHandler in urlListHandlers)
        {
            yield return RemoteReader.GetRemoteValueList(curURLListHandler.ListFileUrl, curURLListHandler.OnReadFile).AsCoroutine();
        }

        List<string> skipList = new List<string>();

        foreach (var curURLListHandler in urlListHandlers)
        {
            taskList.AddRange(curURLListHandler.taskList);
            totalBytes += curURLListHandler.totalBytes;
            skipList.AddRange(curURLListHandler.skipList);
        }

        StringBuilder skipListLog = new StringBuilder();
        foreach (var curSkip in skipList)
        {
            skipListLog.AppendLine(curSkip);
        }

        skipListLog.Insert(0, $"Skipping download for {skipList.Count} files as their local checksum matches the remote checksum.\n");
        Debug.Log(skipListLog.ToString());

        if (taskList.Count == 0)
        {
            updateStatus = ResourceUpdater.CheckUpdateStatus.Finished;
        }
        else
        {
            updateStatus = ResourceUpdater.CheckUpdateStatus.Updating;

            var timer = Stopwatch.StartNew();
            StringBuilder strDownloadTable = new StringBuilder();

            foreach (var taskInfo in taskList)
            {
                DownloadScheduler.Instance.Add(taskInfo);
                strDownloadTable.Append($"{taskInfo.url}, save path: \n{taskInfo.savePath}");
            }

            Debug.Log($"New download task: {totalBytes.CalcMemoryMensurableUnit()}, {taskList.Count} URLs\n{strDownloadTable.ToString()}");

            yield return new WaitUntil(() => DownloadScheduler.Instance.IsAllDownloadFinished);
            DownloadScheduler.DestroyInstance();
            updateStatus = CheckUpdateStatus.Finished;

            var min1 = timer.Elapsed.TotalMinutes;
            Debug.Log($"Download all resources cost time: {min1:N2} minutes");
            timer.Stop();
        }

        Debug.Log($"Downloaded {taskList.Count} files");
        taskList.Clear();

        VersionChecker.WriteVersionFile();
    }

    void Update()
    {
        if (taskList.Count > 0)
        {
            long newSize = 0;
            double speed = 0;
            foreach (var curDl in taskList)
            {
                if (curDl.checksumPassed)
                {
                    newSize += curDl.totalBytes;
                }
                else if (curDl.downloadStarted)
                {
                    newSize += curDl.downloadBytes;
                    speed += curDl.downloadSpeed;
                }
            }

            downloadBytes = newSize;
            downloadSpeed = speed;
        }
    }
}
