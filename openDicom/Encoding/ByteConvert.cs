using System;

namespace openDicom.Encoding
{
    public class ByteConvert
    {
        public static string ToString(byte[] bytes,
            CharacterRepertoire characterRepertoire)
        {
            return ToString(bytes, 0, bytes.Length, characterRepertoire);
        }

        public static string ToString(byte[] bytes, int count,
            CharacterRepertoire characterRepertoire)
        {
            return ToString(bytes, 0, count, characterRepertoire);
        }

        public static string ToString(byte[] bytes, int offset, int count,
            CharacterRepertoire characterRepertoire)
        {
            if (characterRepertoire != null)
                return characterRepertoire.Encoding.GetString(bytes, offset,
                    count);
            throw new EncodingException("characterRepertoire", "null");
        }

        public static byte[] ToBytes(string s,
            CharacterRepertoire characterRepertoire)
        {
            if (characterRepertoire != null)
                return characterRepertoire.Encoding.GetBytes(s);
            throw new EncodingException("characterRepertoire", "null");
        }

        public static byte[] ToBytes(ushort[] words)
        {
            var bytes = new byte[words.Length*2];
            for (var i = 0; i < words.Length; i++)
                Array.Copy(BitConverter.GetBytes(words[i]), 0,
                    bytes, i*2, 2);
            return bytes;
        }

        public static byte[] ToBytes(short[] words)
        {
            var bytes = new byte[words.Length*2];
            for (var i = 0; i < words.Length; i++)
                Array.Copy(BitConverter.GetBytes(words[i]), 0,
                    bytes, i*2, 2);
            return bytes;
        }

        public static ushort[] ToUnsignedWords(byte[] bytes)
        {
            if (bytes.Length%2 == 0)
            {
                var words = new ushort[bytes.Length/2];
                var buffer = new byte[2];
                for (var i = 0; i < words.Length; i++)
                    words[i] = BitConverter.ToUInt16(buffer, i*2);
                return words;
            }
            throw new EncodingException("Odd count of bytes. Cannot " +
                                        "convert to words.", "bytes.Length",
                bytes.Length.ToString());
        }

        public static short[] ToSignedWords(byte[] bytes)
        {
            if (bytes.Length%2 == 0)
            {
                var words = new short[bytes.Length/2];
                var buffer = new byte[2];
                for (var i = 0; i < words.Length; i++)
                    words[i] = BitConverter.ToInt16(buffer, i*2);
                return words;
            }
            throw new EncodingException("Odd count of bytes. Cannot " +
                                        "convert to words.", "bytes.Length",
                bytes.Length.ToString());
        }

        public static ushort SwapBytes(ushort word)
        {
            var bytes = BitConverter.GetBytes(word);
            bytes = SwapBytes(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static short SwapBytes(short word)
        {
            var bytes = BitConverter.GetBytes(word);
            bytes = SwapBytes(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static uint SwapBytes(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes = SwapBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static int SwapBytes(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes = SwapBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static ulong SwapBytes(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes = SwapBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static long SwapBytes(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes = SwapBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static float SwapBytes(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes = SwapBytes(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double SwapBytes(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes = SwapBytes(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static byte[] SwapBytes(byte[] bytes)
        {
            var buffer = new byte[bytes.Length];
            for (var i = 0; i < bytes.Length; i++)
                buffer[i] = bytes[bytes.Length - 1 - i];
            return buffer;
        }
    }
}