using System.IO;
namespace AssetList
{
    public class AssetInfo
    {
        public string relativePath;
        public string redirectRelativePath;
        public long fileLength;
        public long crc;

        public bool isRedirected => !string.IsNullOrEmpty(redirectRelativePath);

        public void Redirect(string rootDirectory)
        {
            redirectRelativePath = relativePath + "_" + crc + ".data";

            var src = Path.Combine(rootDirectory, relativePath);
            var dst = Path.Combine(rootDirectory, redirectRelativePath);
            if (File.Exists(src))
            {
                File.Move(src, dst);
            }
        }
    }
}
