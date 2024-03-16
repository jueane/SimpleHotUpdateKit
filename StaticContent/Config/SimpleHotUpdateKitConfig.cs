using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleHotUpdateKitConfig", menuName = "SimpleHotUpdateKitConfig/Config", order = 1)]
public class SimpleHotUpdateKitConfig : ScriptableObject
{
    [Header("Editor Build Settings")] public string BuildDirectory = "Build";

    public string FolderForUploadingData = "cdn_ready_content";
    public bool upload = false;

    [Header("Update Settings")] public string updateServerURL = "http://yourupdateurl.com";
    public bool enableAutoUpdate = true;
    public bool forceUpdate;

    [Header("Preprocess")] public List<string> methodList;

    [Header("Launch Class")] public string InvokeAssembly = "Assembly-CSharp";
    public string InvokeClassName = "GameLogicLoader";

    [Header("Version Control")] public int currentVersion = 1;

    [Header("Other Configurations")] public bool debugMode = false;

    public string CacheDir = "cache";
    public string NoCacheDir = "nocache";
    public string MainBranch = "default";
    public string BuildBranch = "Development";

    public string ResourceFolderSuffix = "Res";

    public string ListFile = "downloadlist.txt";
    public string SeparateSymbol = ",";

    // 程序集 on cdn
    public string AssemblyFolder = "assembly_files";

    public string AssetRootDirectory = "assetspackage";

    // 版本信息
    public string DataPointerFile = "data_version";

    // 补充元数据的DLL目录
    public string AOT_Dll_Dir = "aot_dlls";

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
}
