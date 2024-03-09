public class AAResConst
{
    public static string aa_placeholder = "aa_placeholder";
    public static string aa_bundle_dir = "resbundles2";

    public const string aa_catalog_dir = "resconfig";
    public const string aa_catalog_file = "catalog.json";
    public static string aa_config_file => $"{aa_catalog_dir}/{aa_catalog_file}";

    public static string AABuildPath;
    public static string AALoadPath => $"{aa_placeholder}/{aa_bundle_dir}";
}
