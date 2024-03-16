using System;
using System.IO;
using System.Threading.Tasks;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class SimpleHotUpdateKitBuildCommand
{
    public static async Task<bool> Build(bool isFullPackage, bool includeResource, bool useCache)
    {
        var versionChecked = await VersionChecker.Fetch(100);
        Debug.Log($"Remote version info: {VersionChecker.LocalVersion},{VersionChecker.LocalResVersion}");

        if (versionChecked)
        {
            if (!VersionChecker.Fetched)
            {
                throw new Exception("VersionChecker error");
            }

            BuildConst.GenerateBuildVersion();
            CheckURLs();

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
                AddressableAssetsBuilder.Build(BuildConst.FullPathForUploadingDataRes, useCache);
            }

            AssetsListGenerator.SaveFileList(includeResource);

            var verJson = JsonConvert.SerializeObject(versionInfo);
            File.WriteAllText($"{BuildConst.FullPathForUploadingDataNoCache}/{ApplicationConst.DataPointerFile}", verJson);
            return true;
        }

        return false;
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
