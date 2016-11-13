using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class SignedShort : ValueRepresentation
    {
        public SignedShort(Tag tag) : base("SS", tag)
        {
        }

        public override string ToLongString()
        {
            return "Signed Short (SS)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var multiValue = ToImproperMultiValue(bytes, 2);
            var number = new short[multiValue.Length];
            for (var i = 0; i < number.Length; i++)
                number[i] = BitConverter.ToInt16(
                    TransferSyntax.CorrectByteOrdering(multiValue[i]), 0);
            return number;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var multiValue = ToProperMultiValue(bytes, 2);
            if (bytes.Length == 2*multiValue.Length)
            {
                var number = new short[multiValue.Length];
                for (var i = 0; i < number.Length; i++)
                    number[i] = BitConverter.ToInt16(
                        TransferSyntax.CorrectByteOrdering(multiValue[i]), 0);
                return number;
            }
            throw new EncodingException(
                "A value of multiple 2 bytes is only allowed.",
                Tag, Name + "/value.Length", bytes.Length.ToString());
        }
    }
}