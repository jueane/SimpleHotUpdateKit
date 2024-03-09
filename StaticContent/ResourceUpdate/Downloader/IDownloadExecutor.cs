using System.Collections;

public interface IDownloadExecutor
{
    IEnumerator Download(string url, string savePath);

    long GetDownloadedSize();
}
