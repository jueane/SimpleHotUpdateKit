using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Downloader;

public class RemoteReader
{
    public static async Task<List<string>> Read(string url)
    {
        var downloader = new DownloadService();
        List<string> contentLines = new List<string>();

        using (Stream destinationStream = await downloader.DownloadFileTaskAsync(url))
        {
            using (StreamReader reader = new StreamReader(destinationStream))
            {
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    contentLines.Add(line);
                }
            }
        }

        return contentLines;
    }
}
