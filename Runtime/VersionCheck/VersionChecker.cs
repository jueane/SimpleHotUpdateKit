using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class VersionChecker
{
    public static string dataPointerUrl => $"{ApplicationConst.CheckUpdateBasePath}/{ApplicationConst.DataPointerFile}";

    private static VersionInfo _versionInfo;
    public static VersionInfo VersionInfo => _versionInfo ??= new VersionInfo();

    public static bool Fetched { get; private set; }
    static string FetchedRemoteValue;

    public static bool isNewest { get; private set; }

    public static IEnumerator Init()
    {
        if (VersionInfo.TryReadFromLocal(out var ver))
        {
            _versionInfo = ver;
        }

        if (ApplicationConst.config.enableUpdate)
        {
            yield return Fetch().AsCoroutine();
        }
    }

    public static async Task<bool> Fetch()
    {
        Debug.Log($"Get remote version {dataPointerUrl}");
        await RemoteReader.GetRemoteValue(dataPointerUrl, VersionFetchCallback);
        return Fetched;
    }

    public static void FetchSync()
    {
        Debug.Log($"Get remote version {dataPointerUrl}");
        var rValue = RemoteReader.GetRemoteValue(dataPointerUrl);
        VersionFetchCallback(true, rValue);
    }

    static void VersionFetchCallback(bool checkSucceed, string remoteValue)
    {
        Fetched = remoteValue != null;
        if (checkSucceed && Fetched)
        {
            Debug.Log($"Remote value: {remoteValue}");

            if (!VersionInfo.TryParse(remoteValue, out var remoteVersionInfo))
            {
                Debug.Log($"Remote version is invalid");
                return;
            }

            FetchedRemoteValue = remoteValue;
            var remoteVer = remoteVersionInfo.codeVersion;
            var remoteResVer = remoteVersionInfo.resourceVersion;
            if (VersionInfo.codeVersion == remoteVer && VersionInfo.resourceVersion == remoteResVer)
            {
                Debug.Log($"System up-to-date, no updates needed. [{VersionInfo.codeVersion},{VersionInfo.resourceVersion}]");
                isNewest = true;
            }
            else
            {
                Debug.Log($"New version found {VersionInfo.codeVersion},{VersionInfo.resourceVersion} -> {remoteVer},{remoteResVer}");
                _versionInfo = remoteVersionInfo;
            }
        }
        else
        {
            Debug.Log($"Get version failed, FetchedRemoteValue: {remoteValue}");
        }
    }

    public static void ForceSetInEditor(String newVer)
    {
        _versionInfo = new VersionInfo()
        {
            codeVersion = newVer
        };
    }

    public static void WriteVersionFile()
    {
        if (FetchedRemoteValue != null)
            File.WriteAllText(VersionInfo.LocalVersionFilepath, FetchedRemoteValue);
    }

    public static bool IsLastDownloadFinished()
    {
        return File.Exists(VersionInfo.LocalVersionFilepath);
    }
}
