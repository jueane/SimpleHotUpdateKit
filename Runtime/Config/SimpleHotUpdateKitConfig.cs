using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleHotUpdateKitConfig", menuName = "SimpleHotUpdateKitConfig/Config", order = 1)]
public class SimpleHotUpdateKitConfig : ScriptableObject
{
    [Header("Editor Build Settings")] public string BuildDirectory = "Build";
    public string FolderForUploadingData = "cdn_ready_content";
    public bool upload = false;
    public string aotListFilePath;

    [Header("Update Settings")] public string updateServerURL = "http://yourupdateurl.com";
    public bool enableAutoUpdate = true;
    public bool forceUpdate = true;
    public int downloadConcurrent = 5;

    [Header("Preprocess")] public List<string> methodList;

    [Header("Branch Configurations")] public string CacheDir = "cache";
    public string NoCacheDir = "nocache";
    public string MainBranch = "default";
    public string BuildBranch = "development";

    [Header("Directory Configurations")] public string LoadRootDirectory = "download_cache";
    public string ResourceFolderSuffix = "res";

    public string ListFile = "downloadlist.txt";
    public string SeparateSymbol = ",";

    // 程序集 on cdn
    public string AssemblyFolder = "assembly_files";

    // 版本信息
    public string DataPointerFile = "data_version";

    // 补充元数据的DLL目录
    public string AdditionDlls = "addition_dlls";

    const string configLoadPath = "SimpleHotUpdateKitConfig/SimpleHotUpdateKitConfig";
    private static string configPath = $"Assets/Resources/{configLoadPath}";

    private static SimpleHotUpdateKitConfig instance;

    public static SimpleHotUpdateKitConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SimpleHotUpdateKitConfig>(configLoadPath);
                if (instance == null)
                {
                    Debug.LogError($"SimpleHotUpdateKitConfig not found. Please create a new configuration asset at path {configPath}.");
                }
            }

            return instance;
        }
    }

    public void Save()
    {
#if UNITY_EDITOR
        Debug.Log($"Save config");
        // 保存修改到磁盘上
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); // 如果需要，刷新资源管理器
#endif
    }
}
