using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleHotUpdateKitIdentifyCode", menuName = "SimpleHotUpdateKitConfig/IdentifyCodeConfig", order = 1)]
public class SimpleHotUpdateKitIdentifyCode : ScriptableObject
{
    public string VersionCode;

    const string configLoadPath = "SimpleHotUpdateKitConfig/SimpleHotUpdateKitIdentifyCode";
    static readonly string configPath = $"Assets/Resources/{configLoadPath}.asset";

    static SimpleHotUpdateKitIdentifyCode instance;

    public static SimpleHotUpdateKitIdentifyCode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SimpleHotUpdateKitIdentifyCode>(configLoadPath);
                if (instance == null)
                {
                    Debug.LogWarning($"SimpleHotUpdateKitIdentifyCode not found. Creating a new one at path {configPath}.");
                    instance = CreateInstance<SimpleHotUpdateKitIdentifyCode>();

#if UNITY_EDITOR
                    string directory = System.IO.Path.GetDirectoryName(configPath);
                    if (!AssetDatabase.IsValidFolder(directory))
                    {
                        string parentDirectory = System.IO.Path.GetDirectoryName(directory);
                        string newFolder = System.IO.Path.GetFileName(directory);
                        AssetDatabase.CreateFolder(parentDirectory, newFolder);
                    }

                    AssetDatabase.CreateAsset(instance, configPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log($"SimpleHotUpdateKitIdentifyCode created at {configPath}.");
#endif
                }
            }

            return instance;
        }
    }

    public void Save()
    {
#if UNITY_EDITOR
        Debug.Log("Save config");
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}
