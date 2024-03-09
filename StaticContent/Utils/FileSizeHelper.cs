using System;

public static class FileSizeHelper
{
    static string[] units = {"B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"};

    public static string ReadableFileSize(double size, int unit = 0)
    {
        while (size >= 1024)
        {
            size /= 1024;
            ++unit;
        }

        return String.Format("{0:G4} {1}", size, units[unit]);
    }
}
