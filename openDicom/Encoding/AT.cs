using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class AttributeTag : ValueRepresentation
    {
        public AttributeTag(Tag tag) : base("AT", tag)
        {
        }

        public override string ToLongString()
        {
            return "Attribute Tag (AT)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var multiValue = ToImproperMultiValue(bytes, 4);
            var tagValue = new Tag[multiValue.Length];
            for (var i = 0; i < tagValue.Length; i++)
            {
                int group = TransferSyntax.CorrectByteOrdering(
                    BitConverter.ToUInt16(multiValue[i], 0));
                int element = TransferSyntax.CorrectByteOrdering(
                    BitConverter.ToUInt16(multiValue[i], 2));
                tagValue[i] = new Tag(group, element);
            }
            return tagValue;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var multiValue = ToProperMultiValue(bytes, 4);
            if (bytes.Length == 4*multiValue.Length)
            {
                var tagValue = new Tag[multiValue.Length];
                for (var i = 0; i < tagValue.Length; i++)
                {
                    int group = TransferSyntax.CorrectByteOrdering(
                        BitConverter.ToUInt16(multiValue[i], 0));
                    int element = TransferSyntax.CorrectByteOrdering(
                        BitConverter.ToUInt16(multiValue[i], 2));
                    tagValue[i] = new Tag(group, element);
                }
                return tagValue;
            }
            throw new EncodingException(
                "A value of multiple 4 bytes is only allowed.", Tag,
                Name + "/value.Length", bytes.Length.ToString());
        }
    }
}