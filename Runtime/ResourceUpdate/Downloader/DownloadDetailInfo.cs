using System.IO;

public class DownloadDetailInfo
{
    public string url;

    public string savePath;

    public string ChecksumFilePath => $"{savePath}.checksum";

    public long checksum;

    public long downloadBytes;

    public long totalBytes;

    public double downloadSpeed;

    public bool downloadStarted;

    public bool skipped;

    public bool saved;

    public bool checksumPassed;

    public int retryCount;

    public bool IsLocalFileExistWithSameChecksum()
    {
        if (File.Exists(this.savePath) && File.Exists(this.ChecksumFilePath))
        {
            var strLocalChecksum = File.ReadAllText(this.ChecksumFilePath);
            if (long.TryParse(strLocalChecksum, out var localChecksum))
            {
                return localChecksum == this.checksum;
            }
        }

        return false;
    }

    public void Reset()
    {
        downloadBytes = 0;
        downloadSpeed = 0;
        downloadStarted = false;
        saved = false;
        checksumPassed = false;
    }
}
