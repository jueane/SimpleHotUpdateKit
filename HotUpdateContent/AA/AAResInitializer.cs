using UnityEngine;
using UnityEngine.AddressableAssets;

public class AAResInitializer
{
    public static void InitConfig()
    {
        Addressables.LoadContentCatalogAsync($"{Application.persistentDataPath}/{AAResConst.aa_config_file}");
        Addressables.InternalIdTransformFunc = location =>
        {
            var key = location.InternalId;
            if (key.EndsWith(".bundle"))
            {
                key = key.Replace(AAResConst.aa_placeholder, Application.persistentDataPath);
                return key;
            }

            return location.InternalId;
        };
    }
}
