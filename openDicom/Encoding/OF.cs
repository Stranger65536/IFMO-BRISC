using System;
using openDicom.DataStructure;
using openDicom.Registry;

namespace openDicom.Encoding
{
    public sealed class OtherFloatString : ValueRepresentation
    {
        public OtherFloatString(Tag tag) : base("OF", tag)
        {
        }

        public override string ToLongString()
        {
            return "Other Float String (OF)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var floatValue = new float[(int) Math.Floor((double) bytes.Length/4)];
            var buffer = new byte[4];
            for (var i = 0; i < floatValue.Length; i++)
            {
                Array.Copy(bytes, i*4, buffer, 0, 4);
                floatValue[i] = BitConverter.ToSingle(
                    TransferSyntax.CorrectByteOrdering(buffer), 0);
            }
            return new float[1][] {floatValue};
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var vm = Tag.GetDictionaryEntry().VM;
            if (vm.Equals(1) || vm.IsUndefined)
            {
                if (bytes.Length%4 != 0)
                    throw new EncodingException(
                        "A value of multiple 4 bytes is only allowed.", Tag,
                        Name + "/value.Length", bytes.Length.ToString());
                if (bytes.Length <= 0xFFFFFFB)
                {
                    var floatValue = new float[bytes.Length/4];
                    var buffer = new byte[4];
                    for (var i = 0; i < floatValue.Length; i++)
                    {
                        Array.Copy(bytes, i*4, buffer, 0, 4);
                        floatValue[i] = BitConverter.ToSingle(
                            TransferSyntax.CorrectByteOrdering(buffer), 0);
                    }
                    return new float[1][] {floatValue};
                }
                throw new EncodingException(
                    "A value of max. 2^32 - 4 bytes is only allowed.",
                    Tag, Name + "/value.Length", bytes.Length.ToString());
            }
            throw new EncodingException(
                "Multiple values are not allowed within this field.",
                Tag, Name + "/VM", vm.ToString());
        }
    }
}