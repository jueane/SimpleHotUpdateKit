using System.IO;
using UnityEngine;

namespace Main.Utils
{
    public class FileUtils
    {
        public static bool SafeDeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return true;
                }

                if (!File.Exists(filePath))
                {
                    return true;
                }

                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SafeDeleteFile failed! path = {filePath} with err: {ex.Message}");
                return false;
            }
        }

        public static bool IsFileExistOnAndroid(string filePath)
        {
            return BetterStreamingAssets.FileExists(filePath);
        }

        public static string ReadAllTextFromStreamingDataOnAndroid(string filePath)
        {
            return BetterStreamingAssets.ReadAllText(filePath);
        }

        public static string[] ReadAllLinesFromStreamingDataOnAndroid(string filePath)
        {
            return BetterStreamingAssets.ReadAllLines(filePath);
        }

        public static byte[] ReadAllBytesFromStreamingDataOnAndroid(string filePath)
        {
            return BetterStreamingAssets.ReadAllBytes(filePath);
        }
    }
}
