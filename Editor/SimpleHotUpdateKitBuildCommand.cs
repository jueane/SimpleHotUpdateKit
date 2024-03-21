using System;
using System.Collections;
using System.IO;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public static class SimpleHotUpdateKitBuildCommand
{
    public static IEnumerator Build(bool isFullPackage, bool includeResource, Action<string> buildResourceFunc)
    {
        Debug.Log($"Get remote version info");
        yield return VersionChecker.Fetch(100).AsCoroutine();
        Debug.Log($"Remote version info: {VersionChecker.LocalVersion},{VersionChecker.LocalResVersion}");

        if (!VersionChecker.Fetched)
        {
            throw new Exception("VersionChecker error");
        }

        BuildConst.GenerateBuildVersion();
        CheckURLs();

        ApplicationConst.config.VersionCode = BuildConst.BuildVersion;
        ApplicationConst.config.Save();

        yield return new EditorWaitForSeconds(1);

        Debug.Log($"HybridCLR enabled: {HybridCLRSettings.Instance.enable}");

        CleanUploadFolder();
        FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingData);
        if (includeResource)
            FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingDataRes);
        FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingDataNoCache);

        if (HybridCLRSettings.Instance.enable)
        {
            if (isFullPackage)
                PrebuildCommand.GenerateAll();

            CompileDllCommand.CompileDllActiveBuildTarget();

            if (isFullPackage)
                BuildAssemblyCommand.CopyHotAssemblyToStreamingAssets();

            BuildAssemblyCommand.CopyMetaDataToUploadFolder();

            BuildAssemblyCommand.PrepareData();
        }

        yield return new EditorWaitForSeconds(1);

        var versionInfo = new VersionInfo()
        {
            codeVersion = BuildConst.BuildVersion,
            resourceVersion = VersionChecker.LocalResVersion,
        };

        Debug.Log($"Build Resources: {includeResource}");
        if (includeResource)
        {
            versionInfo.resourceVersion = BuildConst.BuildVersion;
            buildResourceFunc?.Invoke(BuildConst.FullPathForUploadingDataRes);
        }

        AssetsListGenerator.SaveFileList(includeResource);

        var verJson = JsonConvert.SerializeObject(versionInfo);
        File.WriteAllText($"{BuildConst.FullPathForUploadingDataNoCache}/{ApplicationConst.DataPointerFile}", verJson);

        ApplicationConst.config.VersionCode = null;
        ApplicationConst.config.Save();
    }

    public static void CheckURLs()
    {
        if (!BuildAssemblyCommand.CheckURL())
        {
            throw new Exception("Check urls failed");
        }
    }

    [MenuItem("Build/CheckURLs")]
    public static void CheckURLsManual()
    {
        BuildConst.GenerateBuildVersion();
        CheckURLs();
    }

    public static void CleanUploadFolder()
    {
        var dir = BuildConst.FolderForUploadingData;
        if (!Directory.Exists(dir))
            return;
        Debug.Log($"Clean directory {dir}");
        Directory.Delete(dir, true);
    }
}
