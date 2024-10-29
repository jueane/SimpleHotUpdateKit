using System.IO;
using UnityEngine;

public static class ApplicationConst
{
    public static SimpleHotUpdateKitConfig config => SimpleHotUpdateKitConfig.Instance;
    public static SimpleHotUpdateKitIdentifyCode IdentifyCodeConfig => SimpleHotUpdateKitIdentifyCode.Instance;

    public static string CDNServerURL => config.cdnServerURL;

    public static string LoadRootPath => Path.Combine(Application.persistentDataPath, config.loadRootDirectory);

    public static string PackageSid => config.packageSid;

    public static string CheckUpdateRelativePath;
    public static string CdnDownloadRelativePath;

    public static string CheckUpdateBasePath;

    public static string BaseRemoteURL_CODE;
    public static string BaseRemoteURL_RESOURCE;

    public static string ListFile => config.listFile;
    public static string SeparateSymbol => config.separateSymbol;

    // 程序集 on cdn
    public static string AssemblyFolder => config.assemblyFolder;

    // 版本信息
    public static string DataPointerFile => config.dataPointerFile;

    // 补充元数据的DLL目录
    public static string MetaDllDir => config.additionDlls;

    public static string MetaDllLoadPath => Path.Combine(ApplicationConst.LoadRootPath, MetaDllDir);

    public const string DataFolder = "Data";
    public const string ResourceFolder = "Res";

    static ApplicationConst()
    {
        RefreshValues();
    }

    public static void RefreshValues()
    {
        Debug.Log($"{nameof(ApplicationConst)} {nameof(RefreshValues)}");
        CheckUpdateRelativePath = $"{PackageSid}_{PlatformMappingService.GetPlatformPathSubFolder()}";
        CdnDownloadRelativePath = $"{PackageSid}_{PlatformMappingService.GetPlatformPathSubFolder()}";
        CheckUpdateBasePath = $"{CDNServerURL}/{CheckUpdateRelativePath}";
        var downloadUrl = $"{CDNServerURL}/{CdnDownloadRelativePath}";
        BaseRemoteURL_CODE = $"{downloadUrl}/{ApplicationConst.DataFolder}";
        BaseRemoteURL_RESOURCE = $"{downloadUrl}/{ApplicationConst.ResourceFolder}";
    }
}
