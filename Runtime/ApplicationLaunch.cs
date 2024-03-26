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
        Debug.Log($"Build version: {ApplicationConst.config.VersionCode}");
        Debug.Log($"{nameof(ApplicationLaunch)}");
        SkipUpdate = !ApplicationConst.config.forceUpdate && !NetworkUtil.HasInternetConnectionCached && VersionChecker.IsLastDownloadFinished();
        Debug.Log($"Skip update: {SkipUpdate}");

        if (SkipUpdate)
        {
            Debug.Log($"No internet, update skipped");
        }
        else
        {
            Debug.Log($"Version init");

            yield return VersionChecker.Init();
            ApplicationConst.RefreshValues();

            if (VersionChecker.isNewest)
            {
                Debug.Log($"System up-to-date, no updates needed. [{VersionChecker.LocalVersion},{VersionChecker.LocalResVersion}]");
            }
            else
            {
                Debug.Log($"Updating all files");
                yield return ResourceUpdater.Instance.UpdateAll();

                yield return new WaitForSeconds(1f); // wait for show
            }
        }

        LoadLauncherAssembly();

        AOTMetaDataManager.Startup();

        if (ApplicationConst.config.methodList != null)
        {
            foreach (var curMethod in ApplicationConst.config.methodList)
            {
                yield return CallInit(curMethod);
            }
        }

        yield return CallInit($"{ApplicationConst.config.InvokeAssembly}.{ApplicationConst.config.InvokeClassName}");
    }

    static void LoadLauncherAssembly()
    {
#if UNITY_EDITOR
        launcherAssembly = AppDomain.CurrentDomain.GetAssemblies().First(curAssembly => curAssembly.GetName().Name.Equals(ApplicationConst.launcherAssemblyName));
        return;
#endif

        string targetDirectory = Path.Combine(ApplicationConst.LoadRootPath, ApplicationConst.AssemblyFolder);
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

    public static Assembly GetAssembly(string assemblyName)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies().First(curAssembly => curAssembly.GetName().Name.Equals(assemblyName));
        if (assembly == null)
        {
            Debug.Log($"{assembly.FullName} is null");
            return null;
        }

        return assembly;
    }

    static IEnumerator CallInit(string fullName)
    {
        var nameArray = fullName.Split('.');
        if (nameArray.Length < 2)
        {
            throw new Exception($"error call {fullName}");
        }

        yield return GetAssembly(nameArray[0]).Initialize(nameArray[1]);
    }
}
