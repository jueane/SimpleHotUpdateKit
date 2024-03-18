using System;
using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEngine;

public static class BuildAssemblyCommand
{
    static string baseRelativeDir => Path.Combine(BuildConst.FullPathForUploadingData, ApplicationConst.AssemblyFolder);

    public static string[] asList => HybridCLRSettings.Instance.hotUpdateAssemblies;

    public static bool CheckURL()
    {
        var uploadUrl = baseRelativeDir;
        uploadUrl = uploadUrl.Replace(BuildConst.FolderForUploadingData, "");
        uploadUrl = $"{ApplicationConst.ServerAddress}/{uploadUrl}";
        uploadUrl = uploadUrl.Replace("\\", "/");
        uploadUrl = uploadUrl.Replace("//", "/");

        var downloadUrl = $"{ApplicationConst.BaseRemoteURL}/{ApplicationConst.AssemblyFolder}";
        downloadUrl = downloadUrl.Replace("\\", "/");
        downloadUrl = downloadUrl.Replace("//", "/");

        Debug.Log($"url ^: {uploadUrl}");
        Debug.Log($"url v: {downloadUrl}");

        var isSame = (uploadUrl.Equals(downloadUrl)) || downloadUrl.Contains(uploadUrl);
        Debug.Log($"{isSame}");
        return isSame;
    }

    public static void CopyHotAssemblyToStreamingAssets()
    {
        string projectPath = Directory.GetParent(Application.dataPath).ToString();
        var asPath = Path.Combine(projectPath, $"HybridCLRData/HotUpdateDlls/{EditorUserBuildSettings.activeBuildTarget}");
        string outputPath = Path.Combine(Application.streamingAssetsPath);
        FolderUtility.EnsurePathExists(outputPath);

        foreach (var curAsName in asList)
        {
            var srcPath = Path.Combine(asPath, $"{curAsName}.dll");
            var dstPath = Path.Combine(outputPath, $"{curAsName}.bytes");
            Debug.Log($"Copy file from {srcPath} to {dstPath}");
            System.IO.File.Copy(srcPath, dstPath, true);
        }

        AssetDatabase.Refresh();
    }

    public static void PrepareData()
    {
        if (!HybridCLRSettings.Instance.enable)
            return;

        var target = EditorUserBuildSettings.activeBuildTarget.ToString();

        // 在project下创建assemblys_temp目录
        var baseDir = Path.Combine(BuildConst.ProjectPath, baseRelativeDir);
        FolderUtility.EnsurePathExists(baseDir);

        var asDir = baseDir;

        string projectPath = Directory.GetParent(Application.dataPath).ToString();
        var asFolderFrom = Path.Combine(projectPath, $"HybridCLRData/HotUpdateDlls/{target}");

        foreach (var curAsName in asList)
        {
            var curAsFilename = Path.Combine(asFolderFrom, $"{curAsName}.dll");

            var updateAsFilename = $"{curAsName}.bytes";
            var uploadFilePath = Path.Combine(asDir, updateAsFilename);
            var hashFile = Path.Combine(asDir, $"{updateAsFilename}.hash");

            File.Copy(curAsFilename, uploadFilePath, true);
        }
    }

    public static void CopyMetaDataToUploadFolder()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        string aotAssembliesDstDir = BuildConst.aot_save_dir_path;
        FolderUtility.EnsurePathExists(aotAssembliesDstDir);

        // foreach (var dll in AOTGenericReferences.PatchedAOTAssemblyList)
        foreach (var dll in AOTMetaDataManager.GetAotList())
        {
            string srcDllPath = $"{aotAssembliesSrcDir}/{dll}";
            if (!File.Exists(srcDllPath))
            {
                Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                continue;
            }

            string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.bytes";
            File.Copy(srcDllPath, dllBytesPath, true);
            // Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
        }

        AssetDatabase.Refresh();
    }
}
