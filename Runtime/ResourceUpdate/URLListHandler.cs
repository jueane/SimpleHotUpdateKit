using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class URLListHandler
{
    public string baseUrl;
    public string versionCode;
    public string ListFileUrl => $"{baseUrl}/{ApplicationConst.ListFile}{versionCode}";

    public List<string> skipList { get; private set; } = new List<string>();

    public List<DownloadDetailInfo> taskList = new List<DownloadDetailInfo>();

    public long totalBytes { get; private set; } = 0;

    public void OnReadFile(string json)
    {
        var assetList = JsonConvert.DeserializeObject<List<AssetList.AssetInfo>>(json);

        int skipCount = 0;
        Debug.Log($"Parsed {assetList.Count} from {ListFileUrl}");

        foreach (var curInfo in assetList)
        {
            var curUrl = curInfo.relativePath;
            var fileLen = curInfo.fileLength;
            var crc = curInfo.crc;

            var fileUrl = $"{baseUrl}/{curInfo.redirectRelativePath}";
            var savePath = Path.Combine(ApplicationConst.LoadRootPath, curInfo.relativePath);

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
