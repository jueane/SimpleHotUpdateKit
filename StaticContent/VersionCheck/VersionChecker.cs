using System;
using System.Collections;
using System.IO;
using Main.Utils;
using UnityEngine;

public class VersionChecker
{
    public static string VersionFilepath => $"{Application.persistentDataPath}/{ApplicationConst.DataPointerFile}";

    public static string LocalVersion { get; private set; }

    public static bool FetchedRemoteValue { get; private set; }

    public static bool isNewest { get; private set; }

    public static IEnumerator Init()
    {
        LocalVersion = LoadLocalVersion();
        yield return GetBuildVersion();
    }

    static IEnumerator GetBuildVersion()
    {
        var dataPointerUrl = $"{ApplicationConst.BaseRemoteURLNoCache}/{ApplicationConst.DataPointerFile}";

        yield return RemoteReader.GetRemoteValue(dataPointerUrl, (checkSucceed, remoteValue) =>
        {
            if (checkSucceed)
            {
                FetchedRemoteValue = remoteValue != null;
                if (LocalVersion == remoteValue)
                {
                    isNewest = true;
                }
                else
                {
                    Debug.Log($"New version found {LocalVersion} -> {remoteValue}");
                    LocalVersion = remoteValue;
                }
            }
            else
            {
                Debug.Log($"Get version failed");
            }
        });
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
