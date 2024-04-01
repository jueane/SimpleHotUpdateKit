using System.IO;
using UnityEngine;

public static class ApplicationConst
{
    public static SimpleHotUpdateKitConfig config => SimpleHotUpdateKitConfig.Instance;

    public static string ServerAddress => config.updateServerURL;

    public static string LoadRootPath => Path.Combine(Application.persistentDataPath, config.LoadRootDirectory);

    public static string CacheDir => config.CacheDir;
    public static string NoCacheDir => config.NoCacheDir;

    public static string MainBranch => config.MainBranch;

    public static string BuildBranch => config.BuildBranch;

    public static string NoCacheRelativePath;
    public static string CacheRelativePath;

    public static string BaseRemoteURLNoCache;

    public static string BaseRemoteURL;
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
        NoCacheRelativePath = $"{NoCacheDir}/{MainBranch}_{BuildBranch}_{PlatformMappingService.GetPlatformPathSubFolder()}";
        CacheRelativePath = $"{CacheDir}_{MainBranch}_{BuildBranch}_{PlatformMappingService.GetPlatformPathSubFolder()}";
        BaseRemoteURLNoCache = $"{ServerAddress}/{NoCacheRelativePath}";
        var cachedDir = $"{ServerAddress}/{CacheRelativePath}";
        BaseRemoteURL = $"{cachedDir}/{VersionChecker.versionInfo.codeVersion}";
        BaseRemoteURL_RESOURCE = $"{cachedDir}/{VersionChecker.versionInfo.resourceVersion}{config.ResourceFolderSuffix}";
    }
}
