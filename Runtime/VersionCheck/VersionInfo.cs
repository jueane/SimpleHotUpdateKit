using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class VersionInfo
{
    public static string LocalVersionFilepath => $"{ApplicationConst.LoadRootPath}/{ApplicationConst.DataPointerFile}";

    public string codeVersion;
    public string resourceVersion;
    public List<string> hotUpdateAssemblyList = new List<string>();
    public List<string> preprocessMethodList = new List<string>();

    public bool IsNewerThan(VersionInfo target)
    {
        return target == null || CompareVersions(codeVersion, target.codeVersion);
    }

    static bool CompareVersions(string version1, string version2)
    {
        DateTime dateTime1 = DateTime.ParseExact(version1, "yyMMdd_HHmmss", null);
        DateTime dateTime2 = DateTime.ParseExact(version2, "yyMMdd_HHmmss", null);
        return dateTime1 > dateTime2;
    }

    public static bool TryReadFromBundle(out VersionInfo bundledVersionInfo)
    {
        bundledVersionInfo = null;

        string relativePath = $"{ApplicationConst.config.loadRootDirectory}/{ApplicationConst.DataPointerFile}";
        Debug.Log($"Check bundled version info: {relativePath}");

        if (!BetterStreamingAssets.FileExists(relativePath))
        {
            Debug.Log($"Bundled version is not exist: {relativePath}");
            return false;
        }

        var json = BetterStreamingAssets.ReadAllText(relativePath);
        if (!TryParse(json, out bundledVersionInfo))
        {
            Debug.Log($"Bundled version is invalid: {relativePath}");
            return false;
        }

        return true;
    }

    public static bool TryReadFromLocal(out VersionInfo newVersionInfo)
    {
        Debug.Log($"Check local version info: {LocalVersionFilepath}");

        newVersionInfo = null;

        if (!File.Exists(LocalVersionFilepath))
        {
            Debug.Log($"Local version is not exist: {LocalVersionFilepath}");
            return false;
        }

        var json = File.ReadAllText(LocalVersionFilepath);
        if (!TryParse(json, out newVersionInfo))
        {
            Debug.Log($"Local version is not exist: {LocalVersionFilepath}");
            return false;
        }

        return true;
    }

    public static bool TryParse(string json, out VersionInfo newVersionInfo)
    {
        newVersionInfo = null;
        try
        {
            newVersionInfo = JsonConvert.DeserializeObject<VersionInfo>(json);
            return true;
        }
        catch (Exception e)
        {
        }

        return false;
    }
}
