﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    public Text downloadStatus;
    public Text downloadProgress;
    public Text downloadSpeed;

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
                downloadProgress.text = $"{size1.CalcMemoryMensurableUnit()}/{size2.CalcMemoryMensurableUnit()}";
                if (size2 > 0)
                    slider.value = (float)size1 / size2;
                downloadSpeed.text = $"{ResourceUpdater.Instance.downloadSpeed.CalcMemoryMensurableUnit()}/s";
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
