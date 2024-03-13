using System;
using System.Collections;

public class SimpleHotUpdateKitInitializer : IAssemblyInitializer
{
    public IEnumerator Initialize()
    {
        if (ApplicationConst.config.enableAutoUpdate)
        {
            AOTMetaDataManager.Startup();
            AAResInitializer.InitConfig();
        }

        yield break;
    }
}
