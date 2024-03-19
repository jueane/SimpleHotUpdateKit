using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class VersionChecker
{
    public static string VersionFilepath => $"{ApplicationConst.LoadRootPath}/{ApplicationConst.DataPointerFile}";

    public static string LocalVersion { get; private set; }

    public static string LocalResVersion { get; private set; }

    public static bool Fetched { get; private set; }
    static string FetchedValue;

    public static bool isNewest { get; private set; }

    public static IEnumerator Init()
    {
        LoadLocalVersion();
        var retryCount = ApplicationConst.config.forceUpdate ? int.MaxValue : 3;
        yield return Fetch(retryCount).AsCoroutine();
    }

    public static async Task<bool> Fetch(int retryCount)
    {
        var dataPointerUrl = $"{ApplicationConst.BaseRemoteURLNoCache}/{ApplicationConst.DataPointerFile}";
        await RemoteReader.GetRemoteValue(dataPointerUrl, VersionFetchCallback, retryCount);
        return Fetched;
    }

    static void VersionFetchCallback(bool checkSucceed, string remoteValue)
    {
        Fetched = remoteValue != null;
        if (checkSucceed && Fetched)
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

            FetchedValue = remoteValue;
            var remoteVer = remoteVersionInfo.codeVersion;
            var remoteResVer = remoteVersionInfo.resourceVersion;
            if (LocalVersion == remoteVer && LocalResVersion == remoteResVer)
            {
                Debug.Log($"System up-to-date, no updates needed. [{VersionChecker.LocalVersion},{VersionChecker.LocalResVersion}]");
                isNewest = true;
            }
            else
            {
                Debug.Log($"New version found {LocalVersion},{LocalResVersion} -> {remoteVer},{remoteResVer}");
                LocalVersion = remoteVer;
                LocalResVersion = remoteResVer;
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

    static void LoadLocalVersion()
    {
        if (!File.Exists(VersionFilepath))
        {
            Debug.Log($"VersionFilepath not exist");
            return;
        }

        try
        {
            var json = File.ReadAllText(VersionFilepath);
            var verInfo = JsonConvert.DeserializeObject<VersionInfo>(json);
            LocalVersion = verInfo.codeVersion;
            LocalResVersion = verInfo.resourceVersion;
        }
        catch (Exception e)
        {
            Debug.Log("Local version is invalid");
        }
    }

    public static void WriteVersionFile()
    {
        File.WriteAllText(VersionFilepath, FetchedValue);
    }

    public static bool IsLastDownloadFinished()
    {
        return File.Exists(VersionFilepath);
    }
}
