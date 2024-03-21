using System.IO;
using System.Security.Cryptography;

namespace ApplicationStartup.UpdateManager.Utils
{
    public class FileHashCalculator
    {
        public static string CalculateSHA256Hash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    var stringBuilder = new System.Text.StringBuilder(hashBytes.Length * 2);
                    foreach (byte b in hashBytes)
                    {
                        stringBuilder.AppendFormat("{0:X2}", b);
                    }

                    return stringBuilder.ToString();
                }
            }
        }

        public static string CalculateSHA256Hash(byte[] byteData)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(byteData);
                var stringBuilder = new System.Text.StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    stringBuilder.AppendFormat("{0:X2}", b);
                }

                return stringBuilder.ToString();
            }
        }
    }
}