using System;
using openDicom.DataStructure;
using openDicom.Registry;

namespace openDicom.Encoding
{
    public sealed class OtherByteString : ValueRepresentation
    {
        public OtherByteString(Tag tag) : base("OB", tag)
        {
        }

        public override string ToLongString()
        {
            return "Other Byte String (OB)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            // TODO: How to get trailing zero padding from byte array?
            return new byte[1][] {bytes};
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            // TODO: How to get trailing zero padding from byte array?
            var vm = Tag.GetDictionaryEntry().VM;
            if (vm.Equals(1) || vm.IsUndefined)
                // TODO: Get allowed length from transfer syntax
                return new byte[1][] {bytes};
            throw new EncodingException(
                "Multiple values are not allowed within this field.", Tag,
                Name + "/VM", vm.ToString());
        }
    }
}