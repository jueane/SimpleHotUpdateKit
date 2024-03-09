using System;
using System.IO;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

public class OpenFileExplorer : MonoBehaviour
{
    Button btn;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);

        FileBrowser.AddQuickLink("persistent", Application.persistentDataPath, null);
    }

    void BtnClick()
    {
        OpenFB();
    }

    public static void OpenFB()
    {
        // !!! Uncomment any of the examples below to show the file browser !!!

        // Example 1: Show a save file dialog using callback approach
        // onSuccess event: not registered (which means this dialog is pretty useless)
        // onCancel event: not registered
        // Save file/folder: file, Allow multiple selection: false
        // Initial path: "C:\", Initial filename: "Screenshot.png"
        // Title: "Save As", Submit button text: "Save"
        // FileBrowser.ShowSaveDialog( null, null, FileBrowser.PickMode.Files, false, "C:\\", "Screenshot.png", "Save As", "Save" );

        // Example 2: Show a select folder dialog using callback approach
        // onSuccess event: print the selected folder's path
        // onCancel event: print "Canceled"
        // Load file/folder: folder, Allow multiple selection: false
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Select Folder", Submit button text: "Select"
        FileBrowser.ShowLoadDialog((paths) =>
            {
                Debug.Log("Selected: " + paths[0]);


                FileInfo fi = new FileInfo(paths[0]);
                Debug.Log($"File size: {fi.Length}, path: {fi.FullName}");
            },
            () =>
            {
                Debug.Log("Canceled");
            },
            FileBrowser.PickMode.FilesAndFolders, false, null, null, "Select Folder", "Select");

        // Example 3: Show a select file dialog using coroutine approach
        // StartCoroutine( ShowLoadDialogCoroutine() );
    }
}
