using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    public TextMeshProUGUI downloadStatus;
    public TextMeshProUGUI downloadProgress;

    public Slider slider;

    public bool downloadFinished;

    void Awake()
    {
        downloadStatus.text = "";
        downloadProgress.text = "";
    }

    void Update()
    {
        if (downloadFinished)
            return;

        if (downloadProgress != null)
        {
            if (ResourceUpdater.Instance != null)
            {
                var size1 = ResourceUpdater.Instance.downloadBytes;
                var size2 = ResourceUpdater.Instance.totalBytes;
                downloadProgress.text = $"{FileSizeHelper.ReadableFileSize(size1)}/{FileSizeHelper.ReadableFileSize(size2)}";
                if (size2 > 0)
                    slider.value = (float)size1 / size2;
            }
        }

        switch (ResourceUpdater.Instance.updateStatus)
        {
            case ResourceUpdater.CheckUpdateStatus.Checking:
                downloadStatus.text = "Checking for updates...";
                break;
            case ResourceUpdater.CheckUpdateStatus.Updating:
                downloadStatus.text = "Updating...";
                break;
            case ResourceUpdater.CheckUpdateStatus.Finished:
                downloadStatus.text = "Update finished.";
                downloadFinished = true;
                break;
            default:
                downloadStatus.text = "";
                break;
        }
    }
}
