using System;
using System.IO;
using UnityEngine;

public class ToggleDebugButtons : MonoBehaviour
{
    public GameObject obj;

    public static string filePath => $"{Application.persistentDataPath}/showdebug.txt";

    void Start()
    {
        var exist = File.Exists(filePath);
        obj?.SetActive(exist);
    }
}
