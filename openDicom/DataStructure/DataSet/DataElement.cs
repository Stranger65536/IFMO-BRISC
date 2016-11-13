using System;
using System.IO;
using openDicom.Encoding;

namespace openDicom.DataStructure.DataSet
{
    public sealed class DataElement : IComparable, IDicomStreamMember
    {
        private Tag tag;
        private TransferSyntax transferSyntax = TransferSyntax.Default;
        private Value value;
        private ValueLength valueLength;
        private ValueRepresentation vr;

        public DataElement(Stream stream) : this(stream, null)
        {
        }

        public DataElement(Stream stream, TransferSyntax transferSyntax)
        {
            TransferSyntax = transferSyntax;
            LoadFrom(stream);
        }

        public DataElement(string tag, string vr) : this(tag, vr, null)
        {
        }

        public DataElement(string tag, string vr, TransferSyntax transferSyntax)
        {
            TransferSyntax = transferSyntax;
            this.tag = new Tag(tag, TransferSyntax);
            this.vr = ValueRepresentation.GetBy(vr, Tag);
        }

        public DataElement(Tag tag, ValueRepresentation vr)
        {
            this.tag = tag;
            TransferSyntax = Tag.TransferSyntax;
            if (vr == null) vr = ValueRepresentation.GetBy(Tag);
            this.vr = vr;
        }

        public Tag Tag
        {
            get
            {
                if (tag != null)
                    return tag;
                throw new DicomException("DataElement.Tag is null.",
                    (Tag) null);
            }
        }

        public ValueRepresentation VR
        {
            get
            {
                if (vr != null)
                    return vr;
                throw new DicomException("DataElement.VR is null.",
                    Tag);
            }
        }

        public ValueLength ValueLength
        {
            get
            {
                if (valueLength != null)
                    return valueLength;
                throw new DicomException("DataElement.ValueLength is null.",
                    Tag);
            }
        }

        public Value Value
        {
            get
            {
                if (value != null)
                    return value;
                throw new DicomException("DataElement.Value is null.",
                    Tag);
            }
        }

        public int CompareTo(object obj)
        {
            return Tag.CompareTo(((DataElement) obj).Tag);
        }

        public long StreamPosition
        {
            get { return Tag.StreamPosition; }
        }

        public TransferSyntax TransferSyntax
        {
            set
            {
                if (value == null)
                    transferSyntax = TransferSyntax.Default;
                else
                    transferSyntax = value;
            }
            get { return transferSyntax; }
        }

        public void LoadFrom(Stream stream)
        {
            tag = new Tag(stream, TransferSyntax);
            vr = ValueRepresentation.LoadFrom(stream, Tag);
            valueLength = new ValueLength(stream, VR);
            value = new Value(stream, VR, ValueLength);
        }
    }
}