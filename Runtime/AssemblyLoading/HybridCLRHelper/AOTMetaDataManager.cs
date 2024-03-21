using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using HybridCLR;
using Debug = UnityEngine.Debug;

public class AOTMetaDataManager
{
    public static void Startup()
    {
        var timer = Stopwatch.StartNew();

        LoadMetadataForAOTAssemblies();

        var min = timer.Elapsed.TotalMilliseconds;
        Debug.Log($"Load meta data cost time: {min:N2} ms");
        timer.Stop();
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("LoadMetadataForAOTAssembly");

        // foreach (var aotDllName in AOTGenericReferences.PatchedAOTAssemblyList)
        foreach (var aotDllName in GetAotList())
        {
            string asFilepath = Path.Combine(ApplicationConst.aot_load_dir_path, $"{aotDllName}.bytes");

            if (!File.Exists(asFilepath))
            {
                Debug.Log($"File {asFilepath} not found!");
                continue;
            }

            var dllBytes = File.ReadAllBytes(asFilepath);

            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            sb.AppendLine($"  {aotDllName}. mode:{mode} ret:{err}");
        }

        Debug.Log(sb.ToString());
    }

    public static IReadOnlyList<string> GetAotList()
    {
        // 加载目标程序集
        Assembly targetAssembly = ApplicationLaunch.GetAssembly("Assembly-CSharp");
        // 获取类型
        Type targetType = targetAssembly.GetType("AOTGenericReferences");

        if (targetType != null)
        {
            // 获取字段
            FieldInfo field = targetType.GetField("PatchedAOTAssemblyList", BindingFlags.Public | BindingFlags.Static);

            if (field != null)
            {
                // 获取字段值
                IReadOnlyList<string> assemblyList = (IReadOnlyList<string>)field.GetValue(null);

                return assemblyList;
            }
            else
            {
                Debug.LogError("字段未找到");
            }
        }
        else
        {
            Debug.LogError("类型未找到");
        }
        return null;
    }
}
