using System;
using System.IO;
using openDicom.Encoding;

namespace openDicom.DataStructure
{
    public sealed class ValueLength : IDicomStreamMember
    {
        private readonly ValueRepresentation vr;

        public ValueLength(ValueRepresentation vr)
        {
            this.vr = vr;
        }

        public ValueLength(ValueRepresentation vr, int length) : this(vr)
        {
            Value = length;
        }

        public ValueLength(Stream stream, ValueRepresentation vr) : this(vr)
        {
            LoadFrom(stream);
        }

        public ValueRepresentation VR
        {
            get
            {
                if (vr != null)
                    return vr;
                throw new DicomException("ValueLength.VR is null.",
                    (Tag) null);
            }
        }

        public int Value { get; private set; }

        public bool IsUndefined
        {
            get { return Value < 0; }
        }

        public TransferSyntax TransferSyntax
        {
            get { return VR.TransferSyntax; }
        }

        public long StreamPosition { get; private set; } = -1;

        public void LoadFrom(Stream stream)
        {
            StreamPosition = stream.Position;
            DicomContext.Set(stream, VR.Tag);
            var count = 2;
            var isCertainVR = VR.Name.Equals("OB") || VR.Name.Equals("OW") ||
                              VR.Name.Equals("OF") || VR.Name.Equals("SQ") ||
                              VR.Name.Equals("UT") || VR.Name.Equals("UN");
            if (isCertainVR && !VR.IsImplicit)
            {
                // explicit value representation for certain VRs
                var reserved = new byte[2];
                stream.Read(reserved, 0, 2);
                if (TransferSyntax.CorrectByteOrdering(
                    BitConverter.ToUInt16(reserved, 0)) != 0)
                    throw new ArgumentException("Reserved 2 bytes block is " +
                                                "not 0x0000.");
                count = 4;
            }
            else if (VR.IsImplicit)
                // implicit value representation
                count = 4;
            var buffer = new byte[count];
            stream.Read(buffer, 0, count);
            if (count == 2)
            {
                Value = TransferSyntax.CorrectByteOrdering(
                    BitConverter.ToUInt16(buffer, 0));
            }
            else
            {
                var len = TransferSyntax.CorrectByteOrdering(
                    BitConverter.ToUInt32(buffer, 0));
                if (len == 0xFFFFFFFF)
                    // undefined length
                    Value = -1;
                else if (len > int.MaxValue)
                    // casting problem from uint32 to int32
                    throw new DicomException("Value length is " +
                                             "too big for this implementation.", VR.Tag, "len",
                        len.ToString());
                else
                    Value = (int) len;
            }
            DicomContext.Reset();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}