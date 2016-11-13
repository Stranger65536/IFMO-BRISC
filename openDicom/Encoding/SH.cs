using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class ShortName : ValueRepresentation
    {
        public ShortName(Tag tag) : base("SH", tag)
        {
        }

        public override string ToLongString()
        {
            return "Short Name (SH)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var shortName = ToImproperMultiValue(s);
            for (var i = 0; i < shortName.Length; i++)
            {
                var item = shortName[i];
                shortName[i] = item.Trim();
            }
            return shortName;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var shortName = ToProperMultiValue(s);
            for (var i = 0; i < shortName.Length; i++)
            {
                var item = shortName[i];
                if (item.Length <= 16)
                    shortName[i] = item.Trim();
                else
                    throw new EncodingException(
                        "A value of max. 16 characters is only allowed.",
                        Tag, Name + "/item", item);
            }
            return shortName;
        }
    }
}