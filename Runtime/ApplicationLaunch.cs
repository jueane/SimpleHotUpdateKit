using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ApplicationLaunch : MonoBehaviour
{
    WaitForSeconds wfs = new WaitForSeconds(1);

    public static bool SkipUpdate { get; private set; }

    static Assembly launcherAssembly;

    IEnumerator Start()
    {
        Debug.Log($"Build version: {ApplicationConst.IdentifyCodeConfig.VersionCode}");
        Debug.Log($"{nameof(ApplicationLaunch)}");
        SkipUpdate = !ApplicationConst.config.forceUpdate && !NetworkUtil.HasInternetConnectionCached && VersionChecker.IsLastDownloadFinished();
        Debug.Log($"Skip update: {SkipUpdate}");

        if (SkipUpdate)
        {
            Debug.Log($"No internet, update skipped");
        }
        else
        {
            Debug.Log($"Version init, {VersionChecker.dataPointerUrl}");
            yield return VersionChecker.Init();
            ApplicationConst.RefreshValues();

            if (!VersionChecker.isNewest)
            {
                Debug.Log($"Updating all files");
                yield return ResourceUpdater.Instance.UpdateAll();

                yield return new WaitForSeconds(1f); // wait for show
            }
        }

        AssemblyLoadManager.LoadAllAssembly();

        AOTMetaDataManager.Startup();

        if (VersionChecker.VersionInfo.preprocessMethodList != null)
        {
            foreach (var curMethod in VersionChecker.VersionInfo.preprocessMethodList)
            {
                yield return CallInit(curMethod);
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
