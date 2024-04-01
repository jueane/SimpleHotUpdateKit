using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class AssemblyLoadManager
{
    public static void LoadAllAssembly()
    {
        var asList = VersionChecker.versionInfo.hotUpdateAssemblyList;
        foreach (var assemblyName in asList)
        {
            Debug.Log($"load assembly {assemblyName}");

#if UNITY_EDITOR
            AppDomain.CurrentDomain.GetAssemblies().First(curAssembly => curAssembly.GetName().Name.Equals(assemblyName));
            continue;
#endif

            LoadBytes($"{assemblyName}.bytes", out var loadBytes);
            try
            {
                Assembly ass = Assembly.Load(loadBytes);
                Debug.Log($"Assembly {assemblyName} loaded");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }

    public static bool LoadBytes(string filename, out byte[] bytes)
    {
        var filePath = Path.Combine(ApplicationConst.LoadRootPath, ApplicationConst.AssemblyFolder, $"{filename}");
        if (File.Exists(filePath))
        {
            bytes = File.ReadAllBytes(filePath);
            Debug.Log($"Load {filename} from application persistentDataPath data");
            return true;
        }

        if (UnityStreamingAssetLoader.LoadBytes(Path.Combine(ApplicationConst.AssemblyFolder, filename), out bytes))
        {
            Debug.Log($"Load {filename} from application streaming data");
            return true;
        }

        throw new Exception($"Error: {filename} not found");
    }
}
