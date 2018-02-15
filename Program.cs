using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractRcodeMD5
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Not enough arguments!");
                return -1;
            }

            string filePath = args[0];
            string tempFile = args[1];
            string fileName = args[2];

            string md5 = GetMD5(fileName);

            if (md5 == string.Empty)
            {
                Console.WriteLine("Empty MD5!!!");
                return -1;
            }

            try
            {
                File.WriteAllText(tempFile, md5);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
            //Console.WriteLine(md5);

            return 0;
        }


        private static string GetMD5(string filename)
        {
            int score = 0;
            byte b;
            byte[] bytes = new byte[41];
            byte[] MD5bytes = new byte[16];

            try
            {
                //load file into string
                using (FileStream f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 8))
                using (BinaryReader readStream = new BinaryReader(f))
                {

                    while (true) //readStream.PeekChar()>-1)
                    {
                        b = readStream.ReadByte();
                        switch (b)
                        {
                            case 0x50:
                                if (score == 0)
                                {
                                    score = 1;
                                }
                                else
                                {
                                    score = 0;
                                }
                                break;
                            case 0x52:
                                if (score == 1)
                                {
                                    score = 2;
                                }
                                else if (score == 4)
                                {
                                    score = 5;
                                }
                                else
                                {
                                    score = 0;
                                }
                                break;
                            case 0x4F:
                                if (score == 2)
                                {
                                    score = 3;
                                }
                                else
                                {
                                    score = 0;
                                }
                                break;
                            case 0x47:
                                if (score == 3)
                                {
                                    score = 4;
                                }
                                else
                                {
                                    score = 0;
                                }
                                break;
                            case 0x45:
                                if (score == 5)
                                {
                                    score = 6;
                                }
                                else
                                {
                                    score = 0;
                                }
                                break;
                            case 0x53:
                                if (score == 6)
                                {
                                    score = 7;
                                }
                                else if (score == 7)
                                {
                                    score = 8;
                                }
                                else
                                {
                                    score = 0;
                                }
                                break;
                            default:
                                score = 0;
                                break;
                        }
                        if (score == 8)
                        {
                            bytes = readStream.ReadBytes(41);
                            break;
                        }
                    }
                    readStream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }

            //no MD5 found?
            if (score != 8)
            {
                return string.Empty;
            }

            Array.Copy(bytes, 25, MD5bytes, 0, 16);

            return ToHexString(MD5bytes);
        }


        static char[] hexDigits = {
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static string ToHexString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }


    }
}
