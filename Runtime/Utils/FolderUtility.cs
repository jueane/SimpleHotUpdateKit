using System;
using System.IO;

public class FolderUtility
{
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
            Console.WriteLine("Error occurred while creating folders: " + ex.Message);
            // You can handle the exception as per your requirement
        }
    }

    public static void CopyDirectory(string sourceDir, string destinationDir)
    {
        // 确保目标目录存在
        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        // 获取源目录中的所有文件和子目录
        string[] files = Directory.GetFiles(sourceDir);
        string[] subDirs = Directory.GetDirectories(sourceDir);

        // 复制所有文件
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string destinationFilePath = Path.Combine(destinationDir, fileName);
            File.Copy(file, destinationFilePath, true); // 如果目标文件已存在，覆盖
        }

        // 递归复制子目录
        foreach (string subDir in subDirs)
        {
            string dirName = Path.GetFileName(subDir);
            string destinationSubDir = Path.Combine(destinationDir, dirName);
            CopyDirectory(subDir, destinationSubDir);
        }
    }

    public static void ClearDirectory(string directoryPath)
    {
        // 获取目录中的所有文件和子目录
        string[] files = Directory.GetFiles(directoryPath);
        string[] directories = Directory.GetDirectories(directoryPath);

        // 删除目录中的文件
        foreach (string file in files)
        {
            File.Delete(file);
        }

        // 递归清空子目录
        foreach (string subDirectory in directories)
        {
            Directory.Delete(subDirectory, true);
        }
    }
}
