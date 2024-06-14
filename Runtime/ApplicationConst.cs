using System.IO;
using UnityEngine;

public static class ApplicationConst
{
    public static SimpleHotUpdateKitConfig config => SimpleHotUpdateKitConfig.Instance;
    public static SimpleHotUpdateKitIdentifyCode IdentifyCodeConfig => SimpleHotUpdateKitIdentifyCode.Instance;

    public static string ServerAddress => config.updateCheckServerURL;
    public static string CDNServerAddress => config.cdnServerURL;

    public static string LoadRootPath => Path.Combine(Application.persistentDataPath, config.LoadRootDirectory);

    public static string CdnDownloadDir => config.CdnDownloadDir;
    public static string UpdateInfoDir => config.UpdateInfoDir;

    public static string MainBranch => config.MainBranch;

    public static string BuildBranch => config.BuildBranch;

    public static string CheckUpdateRelativePath;
    public static string CdnDownloadRelativePath;

    public static string CheckUpdateBasePath;

    public static string BaseRemoteURL_CODE;
    public static string BaseRemoteURL_RESOURCE;

    public static string ListFile => config.ListFile;
    public static string SeparateSymbol => config.SeparateSymbol;

    // 程序集 on cdn
    public static string AssemblyFolder => config.AssemblyFolder;

    // 版本信息
    public static string DataPointerFile => config.DataPointerFile;

    public static bool PackageMode;

    // 补充元数据的DLL目录
    public static string AOT_Dll_Dir => config.AdditionDlls;

    public static string aot_load_dir_path => Path.Combine(ApplicationConst.LoadRootPath, AOT_Dll_Dir);

    static ApplicationConst()
    {
        RefreshValues();
    }

    public static void RefreshValues()
    {
        Debug.Log($"{nameof(ApplicationConst)} {nameof(RefreshValues)}");
        CheckUpdateRelativePath = $"{UpdateInfoDir}/{MainBranch}_{BuildBranch}_{PlatformMappingService.GetPlatformPathSubFolder()}";
        CdnDownloadRelativePath = $"{CdnDownloadDir}/{MainBranch}_{BuildBranch}_{PlatformMappingService.GetPlatformPathSubFolder()}";
        CheckUpdateBasePath = $"{ServerAddress}/{CheckUpdateRelativePath}";
        var downloadUrl = $"{CDNServerAddress}/{CdnDownloadRelativePath}";
        BaseRemoteURL_CODE = $"{downloadUrl}/{VersionChecker.VersionInfo.codeVersion}";
        BaseRemoteURL_RESOURCE = $"{downloadUrl}/{VersionChecker.VersionInfo.resourceVersion}{config.ResourceFolderSuffix}";
    }
}
