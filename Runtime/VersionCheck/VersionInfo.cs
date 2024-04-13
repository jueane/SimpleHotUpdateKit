using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class VersionInfo
{
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

    public static bool TryReadFromFile(string filePath, out VersionInfo newVersionInfo)
    {
        newVersionInfo = null;

        if (!File.Exists(filePath))
        {
            Debug.Log($"Version file not exist, path: {filePath}");
            return false;
        }

        var json = File.ReadAllText(filePath);
        if (!TryParse(json, out newVersionInfo))
        {
            Debug.Log($"Local version is invalid, path: {filePath}");
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
