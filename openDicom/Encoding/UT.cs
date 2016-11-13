using System;
using openDicom.DataStructure;
using openDicom.Registry;

namespace openDicom.Encoding
{
    public sealed class UnlimitedText : ValueRepresentation
    {
        public UnlimitedText(Tag tag) : base("UT", tag)
        {
        }

        public override string ToLongString()
        {
            return "Unlimited Text (UT)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var unlimitedText = TransferSyntax.ToString(bytes);
            unlimitedText = unlimitedText.TrimEnd(null);
            return new[] {unlimitedText};
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var unlimitedText = TransferSyntax.ToString(bytes);
            var vm = Tag.GetDictionaryEntry().VM;
            if (vm.Equals(1) || vm.IsUndefined)
            {
                // 0xFFFFFFFF is reserved!
#pragma warning disable
                if (unlimitedText.Length <= 0xFFFFFFFE)
                    unlimitedText = unlimitedText.TrimEnd(null);
                else
                    throw new EncodingException(
                        "A value of max. 2^32 - 2 characters is only allowed.",
                        Tag, Name + "/unlimitedText", unlimitedText);
#pragma warning restore
            }
            else
                throw new EncodingException(
                    "Multiple values are not allowed within this field.", Tag,
                    Name + "/unlimitedText", unlimitedText);
            return new[] {unlimitedText};
        }
    }
}