using System;
using System.Text;

namespace Builder
{
    public class Helper
    {
        static Random r = new Random();

        public static string RandomString(int Length)
        {
            string chars = "qwertzuiopasdfghjklyxcvbnmQWERTZUIOPASDFGHJKLYXCVBNM";
            string ret = chars[r.Next(10, chars.Length)].ToString();
            for (int i = 1; i < Length; i++)
                ret += chars[r.Next(chars.Length)];
            return ret;
        }
        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(str.ToCharArray()[i]);
            }
            return bytes;
        }
        public static byte[] Encrypt(byte[] data, string pass)
        {
            byte[] key = Encoding.UTF8.GetBytes(pass);
            int kind = 0;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte) (data[i] ^ key[kind++]);
                if (kind == key.Length)
                    kind = 0;
            }

            return data;
        }

        public static class RC4
        {
            public static byte[] Encrypt(byte[] data, string pass)
            {
                byte[] key = GetBytes(pass);
                int kind = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)(data[i] ^ key[kind++]);
                    if (kind == key.Length)
                        kind = 0;
                }
                return data;
            }
            private static byte[] GetBytes(string str)
            {
                byte[] bytes = new byte[str.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(str.ToCharArray()[i]);
                }

                return bytes;
            }
            public static byte[] PolyRecEncrypt(byte[] byte_0, string string_0)
            {
                byte num = (byte)new Random().Next(1, 0xff);
                byte[] bytes = GetBytes(string_0);
                byte[] array = new byte[byte_0.Length + 1];
                int index = 0;
                for (int i = 0; i <= (byte_0.Length - 1); i++)
                {
                    array[i] = (byte)((byte_0[i] ^ bytes[index]) ^ num);
                    Array.Reverse(bytes);
                    if (index == (bytes.Length - 1))
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }
                }
                Array.Resize<byte>(ref array, array.Length);
                array[array.Length - 1] = num;
                Array.Reverse(array);
                return array;
            }

            public static byte[] PolyRevDecrypt(byte[] data, string pass)
            {
                Array.Reverse(data);
                byte rndByte = data[data.Length - 1];
                byte[] passByte = GetBytes(pass);
                byte[] Out = new byte[data.Length + 1];
                int u = 0;
                for (int i = 0; i <= data.Length - 1; i++)
                {
                    Out[i] = (byte)((data[i] ^ rndByte) ^ passByte[u]);
                    Array.Reverse(passByte);
                    if (u == passByte.Length - 1) u = 0;
                    else u += 1;
                }

                Array.Resize(ref Out, Out.Length - 2);
                return Out;
            }
        }
        public static string RandomPassNew()
        {
            Random rnd = new Random();
            uint rOne = (uint)rnd.Next(52345, 52348);
            uint rTwo = (uint)rnd.Next(39327, 39329);
            uint rMul = rOne * rTwo;
            return rMul.ToString();
        }

        public static class MD5
        {
            private static string ToHex(uint value)
            {
                StringBuilder output = new StringBuilder();
                byte hex;
                char[] hexRes = new char[3];
                for (; value != 0; value /= 256)
                {
                    hex = (byte)(value % 256);
                    for (int i = 0; i < Convert.ToString(hex, 16).Length; i++)
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
                int length = input.Length;
                int rest = length % 64;
                int size = 0;

                if (rest < 56)
                {
                    size = length - rest + 56 + 8;
                }
                else
                {
                    size = length + 64 - rest + 56 + 8;
                }

                byte[] msgForEncode = new byte[size];
                for (int i = 0; i < length; i++)
                {
                    msgForEncode[i] = (byte)input[i];
                }

                msgForEncode[length] = 0x80;

                for (int i = length + 1; i < size; i++)
                {
                    msgForEncode[i] = 0;
                }

                Int64 bitLength = (uint)(length) * 8;
                for (int i = 0; i < 8; i++)
                {
                    msgForEncode[size - 8 + i] = (byte)(bitLength >> i * 8);
                }
                uint A = 0x67452301, B = 0xefcdab89, C = 0x98badcfe, D = 0x10325476;
                uint[] T = new uint[64];

                for (int i = 0; i < 64; i++)
                {
                    T[i] = (uint)(Math.Pow(2, 32) * Math.Abs(Math.Sin(i + 1)));
                }

                uint[] X = new uint[msgForEncode.Length / 4];
                for (int i = 0; i < msgForEncode.Length / 4; i++)
                {
                    X[i] = (uint)BitConverter.ToInt32(msgForEncode, i * 4);
                }
                uint AA, BB, CC, DD;

                for (int i = 0; i < size / 64; i++)
                {
                    AA = A; BB = B; CC = C; DD = D;
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

                string res = ToHex(A) + ToHex(B) + ToHex(C) + ToHex(D);
                return res;
            }
        }
    }
}
