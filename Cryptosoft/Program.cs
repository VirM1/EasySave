using System;
using System.IO;

namespace CryptoSoft
{
    class Program
    {
        public static void CheckExist(string path)
        {

        }
        
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing arguments");
                return -1;
            }
            string src = args[0];
            string dst = args[1];

            if (!File.Exists(src))
            {
                Console.WriteLine("src file doesn't exist.");
                return -1;
            }

            try
            {
                byte[] byteToEncrypt = File.ReadAllBytes(src);
                byte[] byteKey = new byte[8] { 17, 25, 100, 145, 7, 55, 150, 248 };
                byte[] byteCrypted = new byte[byteToEncrypt.Length];

                for (int i = 0; i < byteToEncrypt.Length; i++)
                {
                    byteCrypted[i] = (byte)(byteToEncrypt[i] ^ byteKey[i % byteKey.Length]);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(dst));
                File.WriteAllBytes(dst, byteCrypted);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return -1;
            }
        }
    }
}
