using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Common.Singleton;
using UnityEngine;

public class ResourceUpdater : MonoSingletonSimple<ResourceUpdater>
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
        // var listUrl = $"{bashUrl}/{ApplicationConst.ListFile}";
        // Debug.Log($"[Remote] Read file list, {listUrl}");
        updateStatus = CheckUpdateStatus.Checking;
        // RemoteReader.GetRemoteValueList(listUrl, OnReadFileList);

        List<URLListHandler> urlListHandlers = new List<URLListHandler>()
        {
            new URLListHandler()
            {
                baseUrl = $"{ApplicationConst.BaseRemoteURL}",
            },
            new URLListHandler()
            {
                baseUrl = $"{ApplicationConst.BaseRemoteURL_RESOURCE}",
            },
        };

        foreach (var curURLListHandler in urlListHandlers)
        {
            yield return RemoteReader.GetRemoteValueList(curURLListHandler.ListFileUrl, curURLListHandler.OnReadFileList).AsCoroutine();
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

            StringBuilder strDownloadTable = new StringBuilder();

            foreach (var taskInfo in taskList)
            {
                if (taskInfo.downloadStarted)
                    continue;

                taskInfo.downloadStarted = true;
                DownloadScheduler.Instance.Add(taskInfo);
                strDownloadTable.Append($"{taskInfo.url}, save path: \n{taskInfo.savePath}");
            }

            Debug.Log($"New download task: {totalBytes.CalcMemoryMensurableUnit()}, {taskList.Count} URLs\n{strDownloadTable.ToString()}");

            yield return new WaitUntil(() => DownloadScheduler.Instance.IsAllDownloadFinished);
            updateStatus = CheckUpdateStatus.Finished;
        }

        Debug.Log($"Downloaded {taskList.Count} files");
        taskList.Clear();

        VersionChecker.WriteVersionFile();
    }

    // List<string> list1;
    // List<string> list2;
    //
    // void OnReadFileList1(bool a, List<string> fileList)
    // {
    //     list1 = fileList;
    // }
    //
    // void OnReadFileList2(bool a, List<string> fileList)
    // {
    //     list2 = fileList;
    // }
    //
    // void OnReadFileListAll(bool a, List<string> fileList)
    // {
    // }
    //
    // void OnReadFileList(bool a, List<string> fileList)
    // {
    //     totalBytes = 0;
    //
    //     StringBuilder skipListLog = new StringBuilder();
    //     int skipCount = 0;
    //
    //     foreach (var curInfo in fileList)
    //     {
    //         if (string.IsNullOrEmpty(curInfo))
    //             continue;
    //
    //         var infoArray = curInfo.Split(ApplicationConst.SeparateSymbol.ToCharArray());
    //         var curUrl = infoArray[0];
    //         long.TryParse(infoArray[1], out var fileLen);
    //         long.TryParse(infoArray[2], out var crc);
    //
    //         var fileUrl = $"{bashUrl}/{curUrl}";
    //         var savePath = Path.Combine(Application.persistentDataPath, curUrl);
    //
    //         var newTask = new DownloadDetailInfo()
    //         {
    //             url = fileUrl,
    //             savePath = savePath,
    //             totalBytes = fileLen,
    //             checksum = crc,
    //         };
    //
    //         if (newTask.IsLocalFileExistWithSameChecksum())
    //         {
    //             newTask.skipped = true;
    //             newTask.checksumPassed = true;
    //             skipListLog.AppendLine(newTask.savePath);
    //             skipCount++;
    //             continue;
    //         }
    //
    //         totalBytes += fileLen;
    //         taskList.Add(newTask);
    //     }
    //
    //     skipListLog.Insert(0, $"Skipping download for {skipCount} files as their local checksum matches the remote checksum.\n");
    //     Debug.Log(skipListLog.ToString());
    //
    //     if (taskList.Count == 0)
    //     {
    //         updateStatus = CheckUpdateStatus.Finished;
    //     }
    //     else
    //     {
    //         updateStatus = CheckUpdateStatus.Updating;
    //
    //         StringBuilder strDownloadTable = new StringBuilder();
    //
    //         foreach (var taskInfo in taskList)
    //         {
    //             if (taskInfo.downloadStarted)
    //                 continue;
    //
    //             taskInfo.downloadStarted = true;
    //             DownloadScheduler.Instance.Add(taskInfo);
    //             strDownloadTable.Append($"{taskInfo.url}, save path: \n{taskInfo.savePath}");
    //         }
    //
    //         Debug.Log($"New download task: {totalBytes.CalcMemoryMensurableUnit()}, {taskList.Count} URLs\n{strDownloadTable.ToString()}");
    //     }
    // }

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
                else if (curDl.downloadStarted)
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
