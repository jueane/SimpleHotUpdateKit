using System;
using System.IO;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEngine;

public class SimpleHotUpdateKitBuildCommand
{
    public static void BuildFull()
    {
        BuildConst.GenerateBuildVersion();
        CheckURLs();

        Debug.Log($"HybridCLR enabled: {HybridCLRSettings.Instance.enable}");

        // if (!runInCmd)
        //     AddVersion();

        CleanUploadFolder();
        FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingData);
        FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingDataNoCache);

        // SetBuildSettingScene();

        Debug.Log("Build Resources");
        AddressableAssetsBuilder.Build(BuildConst.FullPathForUploadingData);

        if (HybridCLRSettings.Instance.enable)
        {
            PrebuildCommand.GenerateAll();
            CompileDllCommand.CompileDllActiveBuildTarget();

            BuildAssemblyCommand.CopyHotAssemblyToStreamingAssets();
            BuildAssemblyCommand.CopyMetaDataToUploadFolder();

            BuildAssemblyCommand.PrepareData();
        }

        AssetsListGenerator.SaveFileList();
        File.WriteAllText($"{BuildConst.FullPathForUploadingDataNoCache}/{ApplicationConst.DataPointerFile}", BuildConst.BuildVersion);
    }

    public static void BuildUpdate()
    {
        BuildConst.GenerateBuildVersion();
        CheckURLs();

        // if (!runInCmd)
        //     AddVersion();

        CleanUploadFolder();
        FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingData);
        FolderUtility.EnsurePathExists(BuildConst.FullPathForUploadingDataNoCache);

        Debug.Log("Build Resources");
        AddressableAssetsBuilder.Build(BuildConst.FullPathForUploadingData);

        // 程序集
        if (HybridCLRSettings.Instance.enable)
        {
            CompileDllCommand.CompileDllActiveBuildTarget();
            BuildAssemblyCommand.CopyMetaDataToUploadFolder();
            BuildAssemblyCommand.PrepareData();
        }

        AssetsListGenerator.SaveFileList();
        File.WriteAllText($"{BuildConst.FullPathForUploadingDataNoCache}/{ApplicationConst.DataPointerFile}", BuildConst.BuildVersion);

        Debug.Log($"Build version: {BuildConst.BuildVersion}");
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
