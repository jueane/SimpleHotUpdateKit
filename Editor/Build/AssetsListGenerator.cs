﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetsListGenerator
{
    public static void SaveFileList()
    {
        // 指定目录路径
        string rootDirectory = Path.Combine(BuildConst.ProjectPath, BuildConst.FullPathForUploadingData);

        // 生成相对路径列表
        string result = GenerateRelativePaths(rootDirectory);

        // 打印结果
        Debug.Log(result);

        var savePath = $"{BuildConst.ProjectPath}/{BuildConst.FullPathForUploadingData}/{ApplicationConst.ListFile}";
        File.WriteAllText(savePath, result);
    }

    static string GenerateRelativePaths(string rootDirectory)
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
        }

        return string.Join(Environment.NewLine, relativePaths);
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
        var relativePath = Path.GetRelativePath(rootDirectory, fullPath);
        return relativePath.Replace(Path.DirectorySeparatorChar, '/');
    }
}
