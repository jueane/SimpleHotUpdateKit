using System;
using System.IO;
using UnityEngine;

namespace Main.AssemblyLoading
{
    public class AssemblyLoadManager
    {
        public static bool LoadBytes(string filename, out byte[] bytes)
        {
            var filePath = Path.Combine(ApplicationConst.LoadRootPath, ApplicationConst.AssemblyFolder, $"{filename}");
            if (File.Exists(filePath))
            {
                bytes = File.ReadAllBytes(filePath);
                Debug.Log($"Load {filename} from application persistentDataPath data");
                return true;
            }

            if (UnityStreamingAssetLoader.LoadBytes(filename, out bytes))
            {
                Debug.Log($"Load {filename} from application streaming data");
                return true;
            }

            throw new Exception($"Error: {filename} not found");
        }
    }
}
