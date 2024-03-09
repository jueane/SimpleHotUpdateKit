using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Common.Singleton;
using UnityEngine;

public class ResourceUpdater : MonoSingleton<ResourceUpdater>
{
    public enum CheckUpdateStatus
    {
        None,
        Checking,
        Updating,
        Finished,
    }

    string bashUrl => $"{ApplicationConst.BaseRemoteURL}";

    public List<DownloadDetailInfo> taskList = new List<DownloadDetailInfo>();

    public CheckUpdateStatus updateStatus { get; private set; }

    public long downloadBytes;

    public long totalBytes;

    public IEnumerator UpdateAll()
    {
        var listUrl = $"{bashUrl}/{ApplicationConst.ListFile}";
        Debug.Log($"[Remote] Read file list, {listUrl}");
        updateStatus = CheckUpdateStatus.Checking;
        RemoteReader.Read(listUrl).ContinueWith(OnReadFileList, TaskScheduler.FromCurrentSynchronizationContext());

        yield return new WaitUntil(() => (updateStatus == CheckUpdateStatus.Finished || updateStatus == CheckUpdateStatus.Updating));

        if (updateStatus == CheckUpdateStatus.Updating)
        {
            yield return new WaitUntil(() => DownloadScheduler.Instance.IsAllDownloadFinished);
            updateStatus = CheckUpdateStatus.Finished;
        }

        Debug.Log($"Downloaded {taskList.Count} files");
        taskList.Clear();

        VersionChecker.WriteVersionFile();
    }

    void OnReadFileList(Task<List<string>> fileList)
    {
        totalBytes = 0;

        StringBuilder skipListLog = new StringBuilder();
        int skipCount = 0;

        foreach (var curInfo in fileList.Result)
        {
            if (string.IsNullOrEmpty(curInfo))
                continue;

            var infoArray = curInfo.Split(ApplicationConst.SeparateSymbol);
            var curUrl = infoArray[0];
            long.TryParse(infoArray[1], out var fileLen);
            long.TryParse(infoArray[2], out var crc);

            var fileUrl = $"{bashUrl}/{curUrl}";
            var savePath = Path.Combine(Application.persistentDataPath, curUrl);

            var newTask = new DownloadDetailInfo()
            {
                url = fileUrl,
                savePath = savePath,
                totalBytes = fileLen,
                checksum = crc,
            };

            if (newTask.IsLocalFileExistWithSameChecksum())
            {
                newTask.skipped = true;
                newTask.checksumPassed = true;
                skipListLog.AppendLine(newTask.savePath);
                skipCount++;
                continue;
            }

            totalBytes += fileLen;
            taskList.Add(newTask);
        }

        skipListLog.Insert(0, $"Skipping download for {skipCount} files as their local checksum matches the remote checksum.\n");
        Debug.Log(skipListLog.ToString());

        if (taskList.Count == 0)
        {
            updateStatus = CheckUpdateStatus.Finished;
        }
        else
        {
            updateStatus = CheckUpdateStatus.Updating;

            StringBuilder strDownloadTable = new StringBuilder();

            foreach (var taskInfo in taskList)
            {
                if (taskInfo.downloadStarted)
                    continue;

                taskInfo.downloadStarted = true;
                DownloadScheduler.Instance.Add(taskInfo);
                strDownloadTable.Append($"{taskInfo.url}, save path: \n{taskInfo.savePath}");
            }

            Debug.Log($"New download task: {FileSizeHelper.ReadableFileSize(totalBytes)}, {taskList.Count} URLs\n{strDownloadTable.ToString()}");
        }
    }

    void Update()
    {
        if (taskList.Count > 0)
        {
            long newSize = 0;
            foreach (var curDl in taskList)
            {
                if (curDl.checksumPassed)
                {
                    newSize += curDl.totalBytes;
                }
                else if (!curDl.downloadStarted)
                {
                    newSize += curDl.downloadBytes;
                }
            }

            downloadBytes = newSize;
        }
    }

    protected override void Dispose()
    {
    }
}
