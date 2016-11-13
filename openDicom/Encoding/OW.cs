using System;
using openDicom.DataStructure;
using openDicom.Registry;

namespace openDicom.Encoding
{
    public sealed class OtherWordString : ValueRepresentation
    {
        public OtherWordString(Tag tag) : base("OW", tag)
        {
        }

        public override string ToLongString()
        {
            return "Other Word String (OW)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var wordValue = new ushort[(int) Math.Floor((double) bytes.Length/2)];
            var buffer = new byte[2];
            for (var i = 0; i < wordValue.Length; i++)
            {
                Array.Copy(bytes, i*2, buffer, 0, 2);
                wordValue[i] = BitConverter.ToUInt16(
                    TransferSyntax.CorrectByteOrdering(buffer), 0);
            }
            return new ushort[1][] {wordValue};
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var vm = Tag.GetDictionaryEntry().VM;
            if (vm.Equals(1) || vm.IsUndefined)
            {
                if (bytes.Length%2 != 0)
                    throw new EncodingException(
                        "A value of multiple 2 bytes is only allowed.", Tag,
                        Name + "/value.Length", bytes.Length.ToString());
                // TODO: Get allowed length from transfer syntax.
                var wordValue = new ushort[bytes.Length/2];
                var buffer = new byte[2];
                for (var i = 0; i < wordValue.Length; i++)
                {
                    Array.Copy(bytes, i*2, buffer, 0, 2);
                    wordValue[i] = BitConverter.ToUInt16(
                        TransferSyntax.CorrectByteOrdering(buffer), 0);
                }
                return new ushort[1][] {wordValue};
            }
            throw new EncodingException(
                "Multiple values are not allowed within this field.", Tag,
                Name + "/VM", vm.ToString());
        }
    }
}