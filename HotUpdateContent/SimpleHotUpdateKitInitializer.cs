using System;
using System.Reflection;

public class SimpleHotUpdateKitInitializer
{
    public static void Startup()
    {
        if (ApplicationConst.config.enableAutoUpdate)
            AOTMetaDataManager.Startup();
    }
}
