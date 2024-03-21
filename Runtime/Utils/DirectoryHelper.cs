using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class DirectoryHelper
{
    // 获取目录中指定扩展名的所有文件，但结果不含扩展名
    public static string[] GetFilesWithoutExtension(string directory, string extension)
    {
        try
        {
            // 获取指定目录下所有文件
            string[] allFiles = Directory.GetFiles(directory);

            // 使用 LINQ 过滤扩展名为 .bytes 的文件，并移除扩展名
            var filteredFiles = allFiles
                .Where(file => Path.GetExtension(file).Equals(extension, StringComparison.OrdinalIgnoreCase))
                .Select(file => Path.GetFileNameWithoutExtension(file))
                .ToArray();

            return filteredFiles;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return default;
        }
    }
}
