using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FolderUtility
{
    public static bool IsDirectoryEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }

    public static void EnsurePathExists(string path)
    {
        try
        {
            // Check if the given path is null or empty
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.");
            }

            // Get the full path and normalize it
            string fullPath = Path.GetFullPath(path);

            // Split the path into individual folder names
            string[] folders = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Create each folder one by one
            string currentPath = folders[0] + Path.AltDirectorySeparatorChar;
            for (int i = 1; i < folders.Length; i++)
            {
                currentPath = Path.Combine(currentPath, folders[i]);
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error occurred while creating folders: " + ex.Message);
            // You can handle the exception as per your requirement
        }
    }

    public static void CopyDirectory(string sourceDir, string destinationDir, List<string> ignoreFileTypeList = null)
    {
        if (!Directory.Exists(sourceDir))
        {
            Debug.Log($"No assets found in directory: {sourceDir}");
            return;
        }

        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        string[] files = Directory.GetFiles(sourceDir);
        string[] subDirs = Directory.GetDirectories(sourceDir);

        foreach (string file in files)
        {
            if (ignoreFileTypeList != null)
            {
                bool skipThisFile = false;
                foreach (var ignoreType in ignoreFileTypeList)
                {
                    if (file.EndsWith(ignoreType))
                    {
                        skipThisFile = true;
                        break;
                    }
                }

                if (skipThisFile)
                    continue;
            }

            string fileName = Path.GetFileName(file);
            string destinationFilePath = Path.Combine(destinationDir, fileName);
            File.Copy(file, destinationFilePath, true); // 如果目标文件已存在，覆盖
        }

        foreach (string subDir in subDirs)
        {
            string dirName = Path.GetFileName(subDir);
            string destinationSubDir = Path.Combine(destinationDir, dirName);
            CopyDirectory(subDir, destinationSubDir, ignoreFileTypeList);
        }
    }

    public static void ClearDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return;

        string[] files = Directory.GetFiles(directoryPath);
        string[] directories = Directory.GetDirectories(directoryPath);

        foreach (string file in files)
        {
            File.Delete(file);
        }

        foreach (string subDirectory in directories)
        {
            Directory.Delete(subDirectory, true);
        }
    }
}
