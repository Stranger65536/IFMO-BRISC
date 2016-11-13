using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class FloatingPointDouble : ValueRepresentation
    {
        public FloatingPointDouble(Tag tag) : base("FD", tag)
        {
        }

        public override string ToLongString()
        {
            return "Floating Point Double (FD)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var multiValue = ToImproperMultiValue(bytes, 8);
            var number = new double[multiValue.Length];
            for (var i = 0; i < number.Length; i++)
                number[i] = BitConverter.ToDouble(
                    TransferSyntax.CorrectByteOrdering(multiValue[i]), 0);
            return number;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var multiValue = ToProperMultiValue(bytes, 8);
            if (bytes.Length == 8*multiValue.Length)
            {
                var number = new double[multiValue.Length];
                for (var i = 0; i < number.Length; i++)
                    number[i] = BitConverter.ToDouble(
                        TransferSyntax.CorrectByteOrdering(multiValue[i]), 0);
                return number;
            }
            throw new EncodingException(
                "A value of multiple 8 bytes is only allowed.", Tag,
                Name + "/value.Length", bytes.Length.ToString());
        }
    }
}