using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Main.AssemblyLoading;
using UnityEngine;

public class ApplicationLaunch : MonoBehaviour
{
    WaitForSeconds wfs = new WaitForSeconds(1);

    public static bool SkipUpdate { get; private set; }

    static Assembly launcherAssembly;

    IEnumerator Start()
    {
        // 强制联网
        while (true)
        {
            Debug.Log($"Version init");
            yield return VersionChecker.Init();
            if (VersionChecker.FetchedRemoteValue)
            {
                ApplicationConst.RefreshValues();
                break;
            }

            Debug.Log($"Please check network connection.");
            yield return wfs;
        }

        SkipUpdate = !NetworkUtil.HasInternetConnection && VersionChecker.IsLastDownloadFinished();

        if (VersionChecker.isNewest)
        {
            Debug.Log($"System up-to-date, no updates needed. [{VersionChecker.LocalVersion}]");
        }
        else if (SkipUpdate)
        {
            Debug.Log($"No internet, assembly update skipped");
        }
        else
        {
            yield return new WaitUntil(() => hasNetwork);

            Debug.Log($"Updating all files");
            yield return ResourceUpdater.Instance.UpdateAll();

            yield return new WaitForSeconds(1f); // wait for show
        }

        LoadAssembly();

        InitLauncherAssembly(launcherAssembly);

        Debug.Log("Call hotfix assembly");
        CallAssembly(launcherAssembly);
    }

    void Update()
    {
        RefreshNetworkStatus();
    }

    float checkNetworkCooldown;
    bool hasNetwork;

    void RefreshNetworkStatus()
    {
        checkNetworkCooldown -= Time.deltaTime;
        if (checkNetworkCooldown < 0)
        {
            checkNetworkCooldown = 1;
            hasNetwork = NetworkUtil.CheckHasInternetConnection(false);
        }
    }

    public static void LoadAssembly()
    {
#if UNITY_EDITOR
        launcherAssembly = AppDomain.CurrentDomain.GetAssemblies().First(curAssembly => curAssembly.GetName().Name.Equals(ApplicationConst.launcherAssemblyName));
        return;
#endif

        string targetDirectory = Path.Combine(Application.persistentDataPath, ApplicationConst.AssemblyFolder);
        var files = DirectoryHelper.GetFilesWithoutExtension(targetDirectory, ".bytes");

        foreach (var assemblyName in files)
        {
            Debug.Log($"load assembly {assemblyName}");
            AssemblyLoadManager.LoadBytes($"{assemblyName}.bytes", out var loadBytes);
            try
            {
                Assembly ass = Assembly.Load(loadBytes);
                Debug.Log($"Assembly {assemblyName} loaded");

                if (assemblyName.Contains(ApplicationConst.launcherAssemblyName))
                {
                    launcherAssembly = ass;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }

    static void InitLauncherAssembly(Assembly assembly)
    {
        if (assembly == null)
        {
            Debug.Log($"{assembly.FullName} is null");
            return;
        }

        Type entryType = assembly.GetType("SimpleHotUpdateKitInitializer");
        MethodInfo method = entryType.GetMethod("Startup");
        method.Invoke(null, null);
    }

    static void CallAssembly(Assembly assembly)
    {
        if (assembly == null)
        {
            Debug.Log($"{assembly.FullName} is null");
            return;
        }

        Type entryType = assembly.GetType(ApplicationConst.config.InvokeClassName);
        MethodInfo method = entryType.GetMethod(ApplicationConst.config.InvokeMethod);
        method.Invoke(null, null);
        Debug.Log($"Call method {method.DeclaringType.FullName}.");
    }
}
