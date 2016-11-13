using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class Unknown : ValueRepresentation
    {
        public Unknown(Tag tag) : base("UN", tag)
        {
        }

        public override string ToLongString()
        {
            return "Unknown (UN)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            return DecodeProper(bytes);
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            return new byte[1][] {bytes};
        }
    }
}