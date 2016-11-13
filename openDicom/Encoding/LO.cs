using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class LongString : ValueRepresentation
    {
        public LongString(Tag tag) : base("LO", tag)
        {
        }

        public override string ToLongString()
        {
            return "Long String (LO)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var longString = ToImproperMultiValue(s);
            for (var i = 0; i < longString.Length; i++)
            {
                var item = longString[i];
                longString[i] = item.Trim();
            }
            return longString;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var longString = ToProperMultiValue(s);
            for (var i = 0; i < longString.Length; i++)
            {
                var item = longString[i];
                if (item.Length <= 64)
                    longString[i] = item.Trim();
                else
                    throw new EncodingException(
                        "A value of max. 64 characters is only allowed.",
                        Tag, Name + "/item", item);
            }
            return longString;
        }
    }
}