using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class VersionChecker
{
    public static string VersionFilepath => $"{Application.persistentDataPath}/{ApplicationConst.DataPointerFile}";

    public static string LocalVersion { get; private set; }

    public static string LocalResVersion { get; private set; }

    public static bool FetchedRemoteValue { get; private set; }

    public static bool isNewest { get; private set; }

    public static IEnumerator Init()
    {
        LocalVersion = LoadLocalVersion();
        var retryCount = ApplicationConst.config.forceUpdate ? int.MaxValue : 3;
        yield return Fetch(retryCount).AsCoroutine();
    }

    public static async Task<bool> Fetch(int retryCount)
    {
        var dataPointerUrl = $"{ApplicationConst.BaseRemoteURLNoCache}/{ApplicationConst.DataPointerFile}";
        await RemoteReader.GetRemoteValue(dataPointerUrl, VersionFetchCallback, retryCount);
        return FetchedRemoteValue;
    }

    static void VersionFetchCallback(bool checkSucceed, string remoteValue)
    {
        FetchedRemoteValue = remoteValue != null;
        if (checkSucceed && FetchedRemoteValue)
        {
            VersionInfo remoteVersionInfo = null;
            try
            {
                remoteVersionInfo = JsonConvert.DeserializeObject<VersionInfo>(remoteValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            var remoteVer = remoteVersionInfo.codeVersion;
            if (LocalVersion == remoteVer)
            {
                isNewest = true;
            }
            else
            {
                Debug.Log($"New version found {LocalVersion} -> {remoteVer}");
                LocalVersion = remoteVer;
                LocalResVersion = remoteVersionInfo.resourceVersion;
            }
        }
        else
        {
            Debug.Log($"Get version failed, FetchedRemoteValue: {remoteValue}");
        }
    }

    public static void ForceSetInEditor(String newVer)
    {
        LocalVersion = newVer;
    }

    static string LoadLocalVersion()
    {
        return File.Exists(VersionFilepath) ? File.ReadAllText(VersionFilepath) : null;
    }

    public static void WriteVersionFile()
    {
        File.WriteAllText(VersionFilepath, LocalVersion);
    }

    public static bool IsLastDownloadFinished()
    {
        return File.Exists(VersionFilepath);
    }
}
