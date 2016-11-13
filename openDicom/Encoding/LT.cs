using System;
using openDicom.DataStructure;
using openDicom.Registry;

namespace openDicom.Encoding
{
    public sealed class LongText : ValueRepresentation
    {
        public LongText(Tag tag) : base("LT", tag)
        {
        }

        public override string ToLongString()
        {
            return "Long Text (LT)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var longText = TransferSyntax.ToString(bytes);
            longText = longText.TrimEnd(null);
            return new[] {longText};
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var longText = TransferSyntax.ToString(bytes);
            var vm = Tag.GetDictionaryEntry().VM;
            if (vm.Equals(1) || vm.IsUndefined)
            {
                if (longText.Length <= 10240)
                    longText = longText.TrimEnd(null);
                else
                    throw new EncodingException(
                        "A value of max. 10240 characters is only allowed.",
                        Tag, Name + "/longText", longText);
            }
            else
                throw new EncodingException(
                    "Multiple values are not allowed within this field.",
                    Tag, Name + "/longText", longText);
            return new[] {longText};
        }
    }
}