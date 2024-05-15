using System;
using System.IO;
using System.Linq;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class SimpleHotUpdateKitBuildCommand
{
    public static void Build(bool isFullPackage, bool includeResource, bool copyResourcesToStreamingData, Action<string> buildResourceFunc)
    {
        Debug.Log($"Build hot update content, isFullPackage: {isFullPackage}, includeResource: {includeResource}");

        Debug.Log($"Get remote version info");
        VersionChecker.FetchSync();
        if (!isFullPackage)
        {
            if (!VersionChecker.Fetched)
            {
                throw new Exception("VersionChecker error");
            }
        }

        Debug.Log($"Remote version info: {VersionChecker.VersionInfo.codeVersion},{VersionChecker.VersionInfo.resourceVersion}");

        var remoteResVersion = VersionChecker.VersionInfo.resourceVersion;

        BuildConst.GenerateBuildVersion();
        CheckURLs();

        ApplicationConst.IdentifyCodeConfig.VersionCode = BuildConst.BuildVersion;
        ApplicationConst.IdentifyCodeConfig.Save();

        Debug.Log($"HybridCLR enabled: {HybridCLRSettings.Instance.enable}");

        CleanUploadFolder();
        FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingData);
        FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingDataNoCache);

        var versionInfo = new VersionInfo()
        {
            codeVersion = BuildConst.BuildVersion,
            resourceVersion = VersionChecker.VersionInfo.resourceVersion,
        };

        versionInfo.hotUpdateAssemblyList = HybridCLRSettings.Instance.hotUpdateAssemblies.ToList();
        versionInfo.preprocessMethodList = ApplicationConst.config.methodList;

        if (HybridCLRSettings.Instance.enable)
        {
            if (isFullPackage)
                PrebuildCommand.GenerateAll();

            CompileDllCommand.CompileDllActiveBuildTarget();

            BuildAssemblyCommand.CopyMetaDataToUploadFolder();

            BuildAssemblyCommand.PrepareData();
        }

        Debug.Log($"Build Resources: {includeResource}");
        if (includeResource)
        {
            FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingDataRes);
            versionInfo.resourceVersion = BuildConst.BuildVersion;
            buildResourceFunc?.Invoke(BuildConst.FullPathForUploadingDataRes);
            if (FolderUtility.IsDirectoryEmpty(BuildConst.FullPathForUploadingDataRes))
            {
                Debug.Log($"Build Resources: no resource found");
                includeResource = false;
            }
        }
        else
        {
            versionInfo.resourceVersion = remoteResVersion;
        }

        // Copy bundled assets to Application.streamingAssetsPath
        string bundledAssetsPath = null;
        if (isFullPackage)
        {
            bundledAssetsPath = Path.Combine(Application.streamingAssetsPath, ApplicationConst.config.LoadRootDirectory);
            FolderUtility.ClearDirectory(bundledAssetsPath);
            FolderUtility.CopyDirectory(BuildConst.FullPathForUploadingData, bundledAssetsPath);
            if (includeResource && copyResourcesToStreamingData)
                FolderUtility.CopyDirectory(BuildConst.FullPathForUploadingDataRes, bundledAssetsPath);
        }

        AssetsListGenerator.SaveFileList(includeResource);

        var verJson = JsonConvert.SerializeObject(versionInfo);
        File.WriteAllText(Path.Combine(BuildConst.FullPathForUploadingDataNoCache, ApplicationConst.DataPointerFile), verJson);

        if (isFullPackage)
        {
            File.WriteAllText(Path.Combine(bundledAssetsPath, ApplicationConst.DataPointerFile), verJson);
        }

        AssetDatabase.Refresh();

        Debug.Log($"Generated version info: {verJson}");
    }

    public static void CheckURLs()
    {
        if (!BuildAssemblyCommand.CheckURL())
        {
            throw new Exception("Check urls failed");
        }
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
