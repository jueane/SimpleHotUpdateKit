using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

internal class ApplicationLaunch : MonoBehaviour
{
    WaitForSeconds wfs = new WaitForSeconds(1);

    public static bool SkipUpdate { get; private set; }

    static Assembly launcherAssembly;

    IEnumerator Start()
    {
        Debug.Log($"{nameof(ApplicationLaunch)}");
        Debug.Log($"{nameof(ApplicationConst.config.packageSid)}: {ApplicationConst.config.packageSid}");
        Debug.Log($"{nameof(ApplicationConst.IdentifyCodeConfig.VersionCode)}: {ApplicationConst.IdentifyCodeConfig.VersionCode}");

        BundledResourceDeployer.TryDeploy();

        SkipUpdate = (!ApplicationConst.config.forceUpdate && !NetworkUtil.HasInternetConnectionCached && VersionChecker.IsLocalVersionFileExist());

        if (SkipUpdate)
        {
            if (!NetworkUtil.HasInternetConnectionCached)
            {
                Debug.Log($"No internet");
            }

            Debug.Log($"Skip update: {SkipUpdate}");
        }
        else
        {
            yield return VersionChecker.Init();
            ApplicationConst.RefreshValues();

            if (ApplicationConst.config.enableUpdate && !VersionChecker.isNewest)
            {
                Debug.Log($"Updating all files");
                yield return ResourceUpdater.Instance.UpdateAll();

                yield return new WaitForSeconds(1f); // wait for show
            }
        }

        if (ApplicationConst.config.enableUpdate)
        {
            AssemblyLoadManager.LoadAllAssembly();
            AOTMetaDataManager.Startup();
        }

        var methodList = VersionChecker.VersionInfo.GetPreprocessMethodList();
        if (methodList != null)
        {
            foreach (var curMethod in methodList)
            {
                Debug.Log($"Call method: {curMethod}");
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
