﻿using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildConst
{
    public static string BuildDirectory => SimpleHotUpdateKitConfig.Instance.BuildDirectory;

    public static string FolderForUploadingData => SimpleHotUpdateKitConfig.Instance.FolderForUploadingData;

    public static string ProjectPath => Directory.GetParent(Application.dataPath).FullName;

    public static string BuildVersion;

    public static string FullPathForUploadingData => Path.Combine(FolderForUploadingData, ApplicationConst.CacheRelativePath, BuildVersion);
    public static string FullPathForUploadingDataRes => Path.Combine(FolderForUploadingData, ApplicationConst.CacheRelativePath, BuildVersion + ApplicationConst.config.ResourceFolderSuffix);
    public static string FullPathForUploadingDataNoCache => Path.Combine(FolderForUploadingData, ApplicationConst.NoCacheRelativePath);

    // 构建过程中补充元数据的DLL保存目录
    public static string aot_save_dir_path => Path.Combine(ProjectPath, FullPathForUploadingData, ApplicationConst.AOT_Dll_Dir);

    public static void GenerateBuildVersion()
    {
        BuildVersion = DatetimeUtil.GetSimpleTimeOfCurrent();

        VersionChecker.ForceSetInEditor(BuildConst.BuildVersion);
        ApplicationConst.RefreshValues();
    }
}
