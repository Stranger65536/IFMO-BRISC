using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class UnsignedLong : ValueRepresentation
    {
        public UnsignedLong(Tag tag) : base("UL", tag)
        {
        }

        public override string ToLongString()
        {
            return "Unsigned Long (UL)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var multiValue = ToImproperMultiValue(bytes, 4);
            var number = new uint[multiValue.Length];
            for (var i = 0; i < number.Length; i++)
                number[i] = BitConverter.ToUInt32(
                    TransferSyntax.CorrectByteOrdering(multiValue[i]), 0);
            return number;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var multiValue = ToProperMultiValue(bytes, 4);
            if (bytes.Length == 4*multiValue.Length)
            {
                var number = new uint[multiValue.Length];
                for (var i = 0; i < number.Length; i++)
                    number[i] = BitConverter.ToUInt32(
                        TransferSyntax.CorrectByteOrdering(multiValue[i]), 0);
                return number;
            }
            throw new EncodingException(
                "A value of multiple 4 bytes is only allowed.", Tag,
                Name + "/value.Length", bytes.Length.ToString());
        }
    }
}