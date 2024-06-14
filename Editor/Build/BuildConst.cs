using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildConst
{
    public static string BuildDirectory => SimpleHotUpdateKitConfig.Instance.buildDirectory;

    public static string FolderForUploadingData => SimpleHotUpdateKitConfig.Instance.folderForUploadingData;

    public static string ProjectPath => Directory.GetParent(Application.dataPath).FullName;

    public static string BuildVersion;

    public static string FullPathForUploadingData => Path.Combine(FolderForUploadingData, ApplicationConst.CdnDownloadRelativePath, BuildVersion);
    public static string FullPathForUploadingDataRes => Path.Combine(FolderForUploadingData, ApplicationConst.CdnDownloadRelativePath, BuildVersion + ApplicationConst.config.resourceFolderSuffix);
    public static string FullPathForUploadingDataNoCache => Path.Combine(FolderForUploadingData, ApplicationConst.CheckUpdateRelativePath);

    // 构建过程中补充元数据的DLL保存目录
    public static string aot_save_dir_path => Path.Combine(ProjectPath, FullPathForUploadingData, ApplicationConst.AOT_Dll_Dir);

    public static void GenerateBuildVersion()
    {
        BuildVersion = DatetimeUtil.GetSimpleTimeOfCurrent();

        VersionChecker.ForceSetInEditor(BuildConst.BuildVersion);
        ApplicationConst.RefreshValues();
    }
}
