using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AddressableAssetsBuilder
{
    public static void Build(string savePath, bool useCache)
    {
        Debug.Log($" ---  Build resource to {savePath}, use cache: {useCache}");
        // 获取项目目录的绝对路径
        string projectPath = Directory.GetParent(Application.dataPath).FullName;

        // 设置输出路径
        string outputPath = savePath;
        outputPath = Path.Combine(outputPath, AAResConst.aa_bundle_dir);
        FolderUtility.EnsurePathExists(outputPath);

        var tempPath = Path.Combine(projectPath, "Build_AA_Cache");
        AAResConst.AABuildPath = tempPath;
        if (!useCache)
            buildAddressableContent();
        FolderUtility.CopyDirectory(tempPath, outputPath);
        Debug.Log($"AA build path: {AAResConst.AABuildPath}");

        CopyAAConfig(savePath);
    }

    static void CopyAAConfig(string basePath)
    {
        var path = Addressables.RuntimePath;

        var from = Path.Combine(BuildConst.ProjectPath, path, AAResConst.aa_catalog_file);

        // 获取项目目录的绝对路径
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        var to = Path.Combine(projectPath, basePath, AAResConst.aa_config_file);

        var toDir = Path.GetDirectoryName(to);
        FolderUtility.EnsurePathExists(toDir);

        File.Copy(from, to, true);
        Debug.Log($"File {AAResConst.aa_config_file} copied");
    }

    // public static string GetBuildPathForProfile(string profileName)
    // {
    //     // 获取 Addressables 配置
    //     AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
    //
    //     var p = settings.profileSettings.GetProfileDataById(AddressableAssetSettingsDefaultObject.Settings.activeProfileId);
    //
    //
    //     // 获取所有 Profile 的名称
    //     string[] allProfileNames = settings.profileSettings.GetAllProfileNames().ToArray();
    //
    //     // 查找匹配的 Profile
    //     if (System.Array.Exists(allProfileNames, name => name == profileName))
    //     {
    //         // 获取 Build Path
    //         string buildPath = settings.profileSettings.GetValueByName(profileName, "BuildPath");
    //         return buildPath;
    //     }
    //     else
    //     {
    //         // 如果找不到匹配的 Profile，返回空字符串或者默认路径
    //         return string.Empty;
    //     }
    // }

    public static bool buildAddressableContent()
    {
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
        bool success = string.IsNullOrEmpty(result.Error);

        if (!success)
        {
            Debug.LogError("Addressables build error encountered: " + result.Error);
            throw new Exception();
        }

        return success;
    }

    public static string build_script
        = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";

    public static string settings_asset
        = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

    public static string profile_name = "Default";
    private static AddressableAssetSettings settings;

    static void getSettingsObject(string settingsAsset)
    {
        // This step is optional, you can also use the default settings:
        //settings = AddressableAssetSettingsDefaultObject.Settings;

        settings
            = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                as AddressableAssetSettings;

        if (settings == null)
            Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                           $"a settings object.");
    }

    static void setProfile(string profile)
    {
        string profileId = settings.profileSettings.GetProfileId(profile);
        if (String.IsNullOrEmpty(profileId))
            Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                             $"using current profile instead.");
        else
            settings.activeProfileId = profileId;
    }

    static void setBuilder(IDataBuilder builder)
    {
        int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);

        if (index > 0)
            settings.ActivePlayerDataBuilderIndex = index;
        else
            Debug.LogWarning($"{builder} must be added to the " +
                             $"DataBuilders list before it can be made " +
                             $"active. Using last run builder instead.");
    }
}
