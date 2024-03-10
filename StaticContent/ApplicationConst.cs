using System.IO;
using UnityEngine;

public static class ApplicationConst
{
    public static SimpleHotUpdateKitConfig config => SimpleHotUpdateKitConfig.Instance;

    public static string launcherAssemblyName => config.InvokeAssembly;

    public static string ServerAddress => config.updateServerURL;

    public static string CacheDir => config.CacheDir;
    public static string NoCacheDir => config.NoCacheDir;

    public static string MainBranch => config.MainBranch;

    public static string BuildBranch => config.BuildBranch;

    public static string NoCacheRelativePath;
    public static string CacheRelativePath;

    public static string BaseRemoteURLNoCache;

    public static string BaseRemoteURL;

    public static string ListFile => config.ListFile;
    public static string SeparateSymbol => config.SeparateSymbol;

    // 程序集 on cdn
    public static string AssemblyFolder => config.AssemblyFolder;

    public static string AssetRootDirectory => config.AssetRootDirectory;

    // 版本信息
    public static string DataPointerFile => config.DataPointerFile;

    public static bool PackageMode;

    // 补充元数据的DLL目录
    public static string AOT_Dll_Dir => config.AOT_Dll_Dir;

    public static string aot_load_dir_path => Path.Combine(Application.persistentDataPath, AOT_Dll_Dir);

    static ApplicationConst()
    {
        RefreshValues();
    }

    public static void RefreshValues()
    {
        NoCacheRelativePath = $"{NoCacheDir}/{MainBranch}_{BuildBranch}_{PlatformMappingService.GetPlatformPathSubFolder()}";
        CacheRelativePath = $"{CacheDir}_{MainBranch}_{BuildBranch}_{PlatformMappingService.GetPlatformPathSubFolder()}";
        BaseRemoteURLNoCache = $"{ServerAddress}/{NoCacheRelativePath}";
        BaseRemoteURL = $"{ServerAddress}/{CacheRelativePath}/{VersionChecker.LocalVersion}";
    }
}
