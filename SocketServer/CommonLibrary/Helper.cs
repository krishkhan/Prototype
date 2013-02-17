using System.IO;

namespace ServerCommonLibrary
{
    /// <summary>
    /// Static helper class
    /// </summary>
    public static class Helper
    {

        public static byte[] GetFile(string file)
        {
            if (!File.Exists(file)) return null;
            FileStream readIn = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[1024*1000];
            int nRead = readIn.Read(buffer, 0, 10240);
            int total = 0;
            while (nRead > 0)
            {
                total += nRead;
                nRead = readIn.Read(buffer, total, 10240);
            }
            readIn.Close();
            byte[] maxresponse_complete = new byte[total];
            System.Buffer.BlockCopy(buffer, 0, maxresponse_complete, 0, total);
            return maxresponse_complete;
        }
    }
}
