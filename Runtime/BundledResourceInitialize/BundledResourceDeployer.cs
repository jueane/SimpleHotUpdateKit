using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal static class BundledResourceDeployer
{
    public static void TryDeploy()
    {
        try
        {
            Debug.Log($"Trying deploying bundled resources");

            BetterStreamingAssets.Initialize();

            var bundledVersionFileExist = VersionInfo.TryReadFromBundle(out var bundledVersionInfo);
            if (!bundledVersionFileExist)
            {
                Debug.Log($"Bundled version file does not exist");
                return;
            }

            var localVersionValid = VersionInfo.TryReadFromLocal(out var localVersionInfo);

            var isBundledInfoNewer = bundledVersionInfo.IsNewerThan(localVersionInfo);

            Debug.Log($"Bundled version info is newer than local: {isBundledInfoNewer}");

            if (!localVersionValid || isBundledInfoNewer)
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

    static void CopyBundledAssetsToPersistentDataPath()
    {
        var dstDir = ApplicationConst.LoadRootPath;
        var ignoreTypeList = new List<string>() {".meta"};

        if (Application.platform == RuntimePlatform.Android)
        {
            var srcDir = Path.Combine(ApplicationConst.config.loadRootDirectory);
            FolderUtilityForAndroid.CopyDirectory(srcDir, dstDir, ignoreTypeList);
        }
        else
        {
            var srcDir = Path.Combine(Application.streamingAssetsPath, ApplicationConst.config.loadRootDirectory);
            FolderUtility.CopyDirectory(srcDir, dstDir, ignoreTypeList);
        }

        Debug.Log($"Bundled assets deployed successfully to {ApplicationConst.LoadRootPath}");
    }
}
