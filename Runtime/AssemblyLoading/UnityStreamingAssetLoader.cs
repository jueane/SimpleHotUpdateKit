using System.IO;
using UnityEngine;

public class UnityStreamingAssetLoader
{
    public static bool LoadBytes(string filename, out byte[] bytes)
    {
        bytes = null;
        var asFilepath = Path.Combine(Application.streamingAssetsPath, $"{filename}");

        Debug.Log($"Path: {asFilepath}");

#if UNITY_ANDROID && !UNITY_EDITOR
        // 如果在Android平台上，需要使用UnityWebRequest来加载StreamingAssets目录下的文件
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(asFilepath);
        www.SendWebRequest();
        while (!www.isDone)
        {
        }

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("加载文件失败: " + www.error);
            return false;
        }

        bytes = www.downloadHandler.data;
#else

        if (!File.Exists(asFilepath))
        {
            Debug.Log($"File {filename} not found!");
            return false;
        }

        bytes = File.ReadAllBytes(asFilepath);

#endif

        return true;
    }
}
