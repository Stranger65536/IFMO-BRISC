using System;
using System.IO;
using openDicom.DataStructure;
using openDicom.DataStructure.DataSet;

namespace openDicom.Encoding
{
    public sealed class SequenceOfItems : ValueRepresentation
    {
        public SequenceOfItems(Tag tag) : base("SQ", tag)
        {
        }

        public override string ToLongString()
        {
            return "Sequence Of Items (SQ)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            return DecodeProper(bytes);
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            // sequences are special cases that are normally taken care of by
            // the class Value, use this method if you know what you are doing
            var memoryStream = new MemoryStream(bytes);
            var sq = new Sequence(memoryStream);
            return new Sequence[1] {sq};
        }
    }
}