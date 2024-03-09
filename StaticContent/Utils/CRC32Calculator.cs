using System.IO;
using ICSharpCode.SharpZipLib.Checksums;

public class CRC32Calculator
{
    public static long CalculateCRC32(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            Crc32 crc32 = new Crc32();
            byte[] buffer = new byte[8192]; // You can adjust the buffer size based on your needs

            int bytesRead;
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                crc32.Update(buffer, 0, bytesRead);
            }

            return crc32.Value;
        }
    }
}
