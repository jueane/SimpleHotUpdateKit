using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
namespace AssetList
{
    public class AssetCollection
    {
        public string rootDirectory;
        public List<AssetInfo> assetInfoList = new List<AssetInfo>();
        public string savePath;

        public void GenerateList()
        {
            List<string> relativePaths = new List<string>();
            ProcessDirectory(rootDirectory, rootDirectory, relativePaths);

            // 加入文件尺寸
            for (int i = 0; i < relativePaths.Count; i++)
            {
                var curLine = relativePaths[i];
                var filePath = Path.Combine(rootDirectory, curLine);

                var len = new FileInfo(filePath).Length;
                relativePaths[i] += $"{ApplicationConst.SeparateSymbol}{len}";

                var crc = CRC32Calculator.CalculateCRC32(filePath);
                relativePaths[i] += $"{ApplicationConst.SeparateSymbol}{crc}";

                assetInfoList.Add(new AssetInfo()
                {
                    relativePath = curLine,
                    fileLength = len,
                    crc = crc,
                });
            }
        }

        public void Redirect()
        {
            foreach (var info in assetInfoList)
            {
                info.Redirect(rootDirectory);
            }
        }

        public void SaveInfosToFile()
        {
            var json = JsonConvert.SerializeObject(assetInfoList);

            var savePath = $"{BuildConst.ProjectPath}/{BuildConst.FullPathForUploadingData}/{ApplicationConst.ListFile}";
            File.WriteAllText(savePath, json);
            // 打印结果
            Debug.Log(json);
        }

        static void ProcessDirectory(string targetDirectory, string rootDirectory, List<string> relativePaths)
        {
            // 处理目录下的文件
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string filePath in fileEntries)
            {
                string relativePath = GetRelativePath(filePath, rootDirectory);
                relativePaths.Add(relativePath);
            }

            // 处理目录下的子目录
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory, rootDirectory, relativePaths);
            }
        }

        static string GetRelativePath(string fullPath, string rootDirectory)
        {
            rootDirectory = Path.GetFullPath(rootDirectory) + Path.DirectorySeparatorChar;
            fullPath = Path.GetFullPath(fullPath);

            fullPath = fullPath.Replace(rootDirectory, null);
            return fullPath.Replace(Path.DirectorySeparatorChar, '/');
        }
    }
}
