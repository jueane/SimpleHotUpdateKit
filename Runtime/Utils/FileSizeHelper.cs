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

    public static string CalcMemoryMensurableUnit(this long bytes)
    {
        return CalcMemoryMensurableUnit((double)bytes);
    }

    public static string CalcMemoryMensurableUnit(this double bytes)
    {
        double kb = bytes / 1024; // · 1024 Bytes = 1 Kilobyte 
        double mb = kb / 1024; // · 1024 Kilobytes = 1 Megabyte 
        double gb = mb / 1024; // · 1024 Megabytes = 1 Gigabyte 
        double tb = gb / 1024; // · 1024 Gigabytes = 1 Terabyte 

        string result =
            tb > 1 ? $"{tb:0.##}TB" :
            gb > 1 ? $"{gb:0.##}GB" :
            mb > 1 ? $"{mb:0.##}MB" :
            kb > 1 ? $"{kb:0.##}KB" :
            $"{bytes:0.##}B";

        result = result.Replace("/", ".");
        return result;
    }
}
