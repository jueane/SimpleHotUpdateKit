using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FolderUtilityForAndroid
{
    public static void CopyDirectory(string sourceDir, string destinationDir, List<string> ignoreFileTypeList = null)
    {
        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        // Debug.Log($"Source path {sourceDir}");
        string[] filesInSource = BetterStreamingAssets.GetFiles($"{sourceDir}", "*", SearchOption.AllDirectories);
        // Debug.Log($"File number: {files.Length}");

        // 复制所有文件
        foreach (string curSrcFile in filesInSource)
        {
            if (ignoreFileTypeList != null)
            {
                bool skipThisFile = false;
                foreach (var ignoreType in ignoreFileTypeList)
                {
                    if (curSrcFile.EndsWith(ignoreType))
                    {
                        skipThisFile = true;
                        break;
                    }
                }

                if (skipThisFile)
                    continue;
            }

            string fileName = Path.GetFileName(curSrcFile);

            string dstRelativeFilepath = curSrcFile;
            if (!string.IsNullOrEmpty(sourceDir))
                dstRelativeFilepath = dstRelativeFilepath.Replace($"{sourceDir}/", "");

            var destinationFilePath = Path.Combine(destinationDir, dstRelativeFilepath);
            var data = BetterStreamingAssets.ReadAllBytes(curSrcFile);
            if (destinationFilePath.LastIndexOf("/") >= 0)
            {
                var dstDir = destinationFilePath.Substring(0, destinationFilePath.LastIndexOf("/"));
                FolderUtility.EnsurePathExists(dstDir);
            }
            File.WriteAllBytes(destinationFilePath, data);
        }
    }
}
