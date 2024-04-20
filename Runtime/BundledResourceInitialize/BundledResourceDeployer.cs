using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class BundledResourceDeployer
{
    public static string BundledResVersionFilepath => $"{Application.streamingAssetsPath}/{ApplicationConst.config.LoadRootDirectory}/{ApplicationConst.DataPointerFile}";

    static VersionInfo versionInfo;

    public static void TryDeploy()
    {
        try
        {
            Debug.Log($"Check bundled version info {BundledResVersionFilepath}");
            if (!VersionInfo.TryReadFromFile(BundledResVersionFilepath, out var bundledVersionInfo))
            {
                Debug.Log($"No bundled assets exist");
                return;
            }

            var localInfoExist = VersionInfo.TryReadFromFile(VersionChecker.LocalVersionFilepath, out var localVersionInfo);
            var isBundledInfoNewer = bundledVersionInfo.IsNewerThan(localVersionInfo);

            Debug.Log($"Bundled version info is newer than local: {isBundledInfoNewer}");

            if (!localInfoExist || isBundledInfoNewer)
            {
                CopyBundledAssetsToPersistentDataPath();
            }
            else
            {
                Debug.Log($"Skipping extraction of bundled resources as local resources are newer");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public static void CopyBundledAssetsToPersistentDataPath()
    {
        var srcDir = Path.Combine(Application.streamingAssetsPath, ApplicationConst.config.LoadRootDirectory);
        var dstDir = ApplicationConst.LoadRootPath;

#if UNITY_ANDROID
        FolderUtilityForAndroid.CopyDirectory(srcDir, dstDir, new List<string>() { ".meta" });
#else
        FolderUtility.CopyDirectory(srcDir, dstDir, new List<string>() { ".meta" });
#endif

        Debug.Log($"Bundled assets deployed successfully to {ApplicationConst.LoadRootPath}");
    }
}
