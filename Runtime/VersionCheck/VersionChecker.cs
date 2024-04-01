using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class VersionChecker
{
    public static string VersionFilepath => $"{ApplicationConst.LoadRootPath}/{ApplicationConst.DataPointerFile}";

    public static string dataPointerUrl => $"{ApplicationConst.BaseRemoteURLNoCache}/{ApplicationConst.DataPointerFile}";

    public static VersionInfo versionInfo = new VersionInfo();

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
        await RemoteReader.GetRemoteValue(dataPointerUrl, VersionFetchCallback, retryCount);
        return Fetched;
    }

    public static void FetchSync()
    {
        var rValue = RemoteReader.GetRemoteValue(dataPointerUrl);
        VersionFetchCallback(true, rValue);
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
                Debug.LogError(e);
                return;
            }

            FetchedValue = remoteValue;
            var remoteVer = remoteVersionInfo.codeVersion;
            var remoteResVer = remoteVersionInfo.resourceVersion;
            if (versionInfo.codeVersion == remoteVer && versionInfo.resourceVersion == remoteResVer)
            {
                Debug.Log($"System up-to-date, no updates needed. [{versionInfo.codeVersion},{versionInfo.resourceVersion}]");
                isNewest = true;
            }
            else
            {
                Debug.Log($"New version found {versionInfo.codeVersion},{versionInfo.resourceVersion} -> {remoteVer},{remoteResVer}");
                versionInfo = remoteVersionInfo;
            }
        }
        else
        {
            Debug.Log($"Get version failed, FetchedRemoteValue: {remoteValue}");
        }
    }

    public static void ForceSetInEditor(String newVer)
    {
        versionInfo = new VersionInfo()
        {
            codeVersion = newVer
        };
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
            versionInfo = JsonConvert.DeserializeObject<VersionInfo>(json);
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
