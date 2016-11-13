using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class SignedLong : ValueRepresentation
    {
        public SignedLong(Tag tag) : base("SL", tag)
        {
        }

        public override string ToLongString()
        {
            return "Signed Long (SL)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var multiValue = ToImproperMultiValue(bytes, 4);
            var number = new int[multiValue.Length];
            for (var i = 0; i < number.Length; i++)
                number[i] = BitConverter.ToInt32(
                    TransferSyntax.CorrectByteOrdering(multiValue[i]), 0);
            return number;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var multiValue = ToProperMultiValue(bytes, 4);
            if (bytes.Length == 4*multiValue.Length)
            {
                var number = new int[multiValue.Length];
                for (var i = 0; i < number.Length; i++)
                    number[i] = BitConverter.ToInt32(
                        TransferSyntax.CorrectByteOrdering(multiValue[i]), 0);
                return number;
            }
            throw new EncodingException(
                "A value of multiple 4 bytes is only allowed.", Tag,
                Name + "/value.Length", bytes.Length.ToString());
        }
    }
}