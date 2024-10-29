using System;
using System.IO;
using System.Linq;
using AssetList;
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
        VersionChecker.SyncFetch();
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
                Debug.LogWarning($"Build Resources: no resource found");
                includeResource = false;
            }
        }
        else
        {
            versionInfo.resourceVersion = remoteResVersion;
        }

        var allAssetList = AssetsListGenerator.SaveFileList(includeResource, versionInfo);

        var verJson = JsonConvert.SerializeObject(versionInfo);
        File.WriteAllText(Path.Combine(BuildConst.FullPathForUploadingDataNoCache, ApplicationConst.DataPointerFile), verJson);

        // Copy bundled assets to Application.streamingAssetsPath
        if (isFullPackage)
        {
            var bundledAssetsPath = Path.Combine(Application.streamingAssetsPath, ApplicationConst.config.loadRootDirectory);
            File.WriteAllText(Path.Combine(bundledAssetsPath, ApplicationConst.DataPointerFile), verJson);

            foreach (var curAssetList in allAssetList)
            {
                if (curAssetList.assetCollectionType == EAssetCollectionType.Res && (!includeResource || !copyResourcesToStreamingData))
                    continue;

                var rootDirectory = curAssetList.rootDirectory;
                foreach (var curFileInfo in curAssetList.assetInfoList)
                {
                    var srcPath = $"{rootDirectory}/{curFileInfo.redirectRelativePath}";
                    var dstPath = $"{bundledAssetsPath}/{curFileInfo.relativePath}";
                    var dstChecksumPath = $"{dstPath}.checksum";
                    FolderUtility.EnsurePathExists(Directory.GetParent(dstChecksumPath).FullName);
                    File.Copy(srcPath, dstPath, true);
                    File.WriteAllText(dstChecksumPath, curFileInfo.crc.ToString());
                }
            }
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
