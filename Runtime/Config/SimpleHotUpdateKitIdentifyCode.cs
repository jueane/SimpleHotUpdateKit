using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleHotUpdateKitIdentifyCode", menuName = "SimpleHotUpdateKitConfig/IdentifyCodeConfig", order = 1)]
public class SimpleHotUpdateKitIdentifyCode : ScriptableObject
{
    public string VersionCode;

    const string configLoadPath = "SimpleHotUpdateKitConfig/SimpleHotUpdateKitIdentifyCode";
    private static string configPath = $"Assets/Resources/{configLoadPath}";

    private static SimpleHotUpdateKitIdentifyCode instance;

    public static SimpleHotUpdateKitIdentifyCode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SimpleHotUpdateKitIdentifyCode>(configLoadPath);
                if (instance == null)
                {
                    Debug.LogError($"SimpleHotUpdateKitIdentifyCode not found. Please create a new configuration asset at path {configPath}.");
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
