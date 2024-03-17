using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using Unity.EditorCoroutines.Editor;

public class SimpleBuildWindow : EditorWindow
{
    [MenuItem("Window/" + nameof(SimpleBuildWindow))]
    static void Init()
    {
        SimpleBuildWindow window = (SimpleBuildWindow)EditorWindow.GetWindow(typeof(SimpleBuildWindow));
        window.Show();
    }

    void OnGUI()
    {
        NewButton("Build Full Package", BuildLauncher.BuildFullPackageCoroutine(false, true));
        NewButton("Build Full Package (No Res)", BuildLauncher.BuildFullPackageCoroutine(false, false));

        GUILayout.Space(50);

        NewButton("Build Update", BuildLauncher.BuildUpdate(false, true));
        NewButton("Build Update (No Res)", BuildLauncher.BuildUpdate(false, false));

        GUILayout.Space(50);
        DrawSeparator();

        NewButton("Open PersistentDataPath", OpenFolder);
    }

    private void DrawSeparator()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        GUILayout.Space(10);
    }

    void NewButton(string name, IEnumerator action)
    {
        GUILayout.Space(20);
        EditorGUILayout.Separator();
        if (GUILayout.Button(name))
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(action);
        }
    }

    void NewButton(string name, Action action)
    {
        GUILayout.Space(50);
        if (GUILayout.Button(name))
        {
            action?.Invoke();
        }
    }

    private static void OpenFolder()
    {
        string path = Application.persistentDataPath;
        EditorUtility.RevealInFinder(path);
    }
}
