using System;
using System.IO;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class SimpleHotUpdateKitBuildCommand
{
    public static void Build(bool isFullPackage, bool includeResource, Action<string> buildResourceFunc)
    {
        Debug.Log($"Build hot update content, isFullPackage: {isFullPackage}");
        if (!isFullPackage)
        {
            Debug.Log($"Get remote version info");
            VersionChecker.FetchSync();
            Debug.Log($"Remote version info: {VersionChecker.LocalVersion},{VersionChecker.LocalResVersion}");

            if (!VersionChecker.Fetched)
            {
                throw new Exception("VersionChecker error");
            }
        }

        BuildConst.GenerateBuildVersion();
        CheckURLs();

        ApplicationConst.config.VersionCode = BuildConst.BuildVersion;
        ApplicationConst.config.Save();

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
    }

    public static void CheckURLs()
    {
        if (!BuildAssemblyCommand.CheckURL())
        {
            throw new Exception("Check urls failed");
        }
    }

    // [MenuItem("Build/CheckURLs")]
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
