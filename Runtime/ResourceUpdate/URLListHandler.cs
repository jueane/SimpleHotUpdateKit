using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class URLListHandler
{
    public string baseUrl;
    public string ListFileUrl => $"{baseUrl}/{ApplicationConst.ListFile}";

    public List<string> skipList { get; private set; } = new List<string>();

    public List<DownloadDetailInfo> taskList = new List<DownloadDetailInfo>();

    public long totalBytes { get; private set; } = 0;

    public void OnReadFileList(bool a, List<string> fileList)
    {
        // StringBuilder skipListLog = new StringBuilder();
        int skipCount = 0;
        Debug.Log($"Parsed {fileList.Count} from {ListFileUrl}");

        foreach (var curInfo in fileList)
        {
            if (string.IsNullOrEmpty(curInfo))
                continue;

            var infoArray = curInfo.Split(ApplicationConst.SeparateSymbol.ToCharArray());
            var curUrl = infoArray[0];
            long.TryParse(infoArray[1], out var fileLen);
            long.TryParse(infoArray[2], out var crc);

            var fileUrl = $"{baseUrl}/{curUrl}";
            var savePath = Path.Combine(ApplicationConst.LoadRootPath, curUrl);

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
                skipList.Add(newTask.savePath);
                skipCount++;
                continue;
            }

            totalBytes += fileLen;
            taskList.Add(newTask);
        }
    }
}
