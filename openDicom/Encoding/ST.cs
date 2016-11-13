using System;
using openDicom.DataStructure;
using openDicom.Registry;

namespace openDicom.Encoding
{
    public sealed class ShortText : ValueRepresentation
    {
        public ShortText(Tag tag) : base("ST", tag)
        {
        }

        public override string ToLongString()
        {
            return "Short Text (ST)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var shortText = TransferSyntax.ToString(bytes);
            shortText = shortText.TrimEnd(null);
            return new[] {shortText};
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var shortText = TransferSyntax.ToString(bytes);
            var vm = Tag.GetDictionaryEntry().VM;
            if (vm.Equals(1) || vm.IsUndefined)
            {
                if (shortText.Length <= 1024)
                    shortText = shortText.TrimEnd(null);
                else
                    throw new EncodingException(
                        "A value of max. 1024 characters is only allowed.",
                        Tag, Name + "/shortText", shortText);
            }
            else
                throw new EncodingException(
                    "Multiple values are not allowed within this field.", Tag,
                    Name + "/VM, " + Name + "/shortText",
                    vm + ", " + shortText);
            return new[] {shortText};
        }
    }
}