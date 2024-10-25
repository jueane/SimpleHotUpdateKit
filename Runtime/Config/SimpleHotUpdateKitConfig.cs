using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleHotUpdateKitConfig", menuName = "SimpleHotUpdateKitConfig/Config", order = 1)]
public class SimpleHotUpdateKitConfig : ScriptableObject
{
    [Header("Branch Configurations")]
    public string channelCode = "debug";
    public string packageId = "development";

    [Header("Editor Build Settings")]
    public string buildDirectory = "Build";
    public string folderForUploadingData = "upload_content";
    public string aotListFilePath;

    [Header("Update Settings")]
    public bool enableUpdate;
    public bool forceUpdate = true;
    public string cdnServerURL = "http://yourupdateurl.com";
    public int downloadConcurrent = 5;

    [Header("Download Configurations")]
    public string loadRootDirectory = "download_cache";

    public string listFile = "downloadlist.txt";
    public string separateSymbol = ",";

    // 程序集 on cdn
    public string assemblyFolder = "assembly_files";

    // 版本信息
    public string dataPointerFile = "data_version";

    // 补充元数据的DLL目录
    public string additionDlls = "addition_dlls";

    [Header("Preprocess")]
    public List<string> methodList;

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
