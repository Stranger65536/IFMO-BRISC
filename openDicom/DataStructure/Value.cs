using System;
using System.Collections;
using System.IO;
using openDicom.DataStructure.DataSet;
using openDicom.Encoding;

namespace openDicom.DataStructure
{
    public sealed class Value : IComparable, IDicomStreamMember
    {
        private readonly ValueLength valueLength;
        private readonly ArrayList valueList = new ArrayList();
        private readonly ValueRepresentation vr;

        public Value(ValueRepresentation vr, ValueLength length)
        {
            this.vr = vr;
            valueLength = length;
        }

        public Value(ValueRepresentation vr, ValueLength length,
            object value) : this(vr, length)
        {
            Add(value);
        }

        public Value(ValueRepresentation vr, ValueLength length,
            Array multiValue) : this(vr, length)
        {
            Add(multiValue);
        }

        public Value(Stream stream, ValueRepresentation vr, ValueLength length) :
            this(vr, length)
        {
            LoadFrom(stream);
        }

        public object this[int index]
        {
            get { return valueList[index]; }
        }

        public int Count
        {
            get { return valueList.Count; }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public bool IsUndefined
        {
            get { return VR.IsUndefined; }
        }

        public bool IsMultiValue
        {
            get { return Count > 1; }
        }

        public bool IsUnknown
        {
            get { return VR.Name.Equals("UN"); }
        }

        public bool IsSequence
        {
            get
            {
                return VR.Name.Equals("SQ") ||
                       (ValueLength.IsUndefined && IsPixelData);
            }
        }

        public bool IsNestedDataSet
        {
            get { return ValueLength.IsUndefined && VR.Tag.Equals("(FFFE,E000)"); }
        }

        public bool IsPixelData
        {
            get { return VR.Tag.Equals("(7FE0,0010)"); }
        }

        public bool IsString
        {
            get
            {
                return VR.Name.Equals("AE") || VR.Name.Equals("CS") ||
                       VR.Name.Equals("LO") || VR.Name.Equals("LT") ||
                       VR.Name.Equals("SH") || VR.Name.Equals("ST") ||
                       VR.Name.Equals("UI") || VR.Name.Equals("UT");
            }
        }

        public bool IsNumeric
        {
            get
            {
                return VR.Name.Equals("DS") || VR.Name.Equals("FL") ||
                       VR.Name.Equals("FD") || VR.Name.Equals("IS") ||
                       VR.Name.Equals("OF") || VR.Name.Equals("OW") ||
                       VR.Name.Equals("SS") || VR.Name.Equals("SL") ||
                       VR.Name.Equals("UL") || VR.Name.Equals("US");
            }
        }

        public bool IsTag
        {
            get { return VR.Name.Equals("AT"); }
        }

        public bool IsUid
        {
            get { return VR.Name.Equals("UI"); }
        }

        public bool IsPersonName
        {
            get { return VR.Name.Equals("PN"); }
        }

        public bool IsAge
        {
            get { return VR.Name.Equals("AS"); }
        }

        public bool IsDate
        {
            get { return VR.Name.Equals("DA") || VR.Name.Equals("DT"); }
        }

        public bool IsTime
        {
            get { return VR.Name.Equals("TM"); }
        }

        public bool IsArray
        {
            get
            {
                return (VR.Name.Equals("OB") || VR.Name.Equals("OW") ||
                        VR.Name.Equals("UN") || VR.IsUndefined) &&
                       !IsSequence && !IsNestedDataSet;
            }
        }

        public ValueRepresentation VR
        {
            get
            {
                if (vr != null)
                    return vr;
                throw new DicomException("Value.VR is null.", (Tag) null);
            }
        }

        public ValueLength ValueLength
        {
            get
            {
                if (valueLength != null)
                    return valueLength;
                throw new DicomException("Value.ValueLength is null.",
                    VR.Tag);
            }
        }

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(((Value) obj).ToString());
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
            if (ValueLength.IsUndefined)
            {
                // use of delimitation (undefined length)
                if (IsSequence)
                {
                    // sequence delimitation
                    var sq = new Sequence(stream, TransferSyntax);
                    Add(sq);
                }
                else if (IsNestedDataSet)
                {
                    // item delimitation
                    var ds = new NestedDataSet(stream,
                        TransferSyntax);
                    Add(ds);
                }
                else
                    throw new DicomException(
                        "Value length is undefined but value is whether " +
                        "sequence nor data set.", VR.Tag);
            }
            else
            {
                if (stream.Position + valueLength.Value <= stream.Length)
                {
                    // use of length value
                    var buffer = new byte[ValueLength.Value];
                    stream.Read(buffer, 0, ValueLength.Value);
                    var multiValue = VR.Decode(buffer);
                    Add(multiValue);
                }
                else
                    throw new DicomException("Value length is out of bounds.",
                        "Value/stream.Length, Value/ValueLength.Value",
                        stream.Length + ", " +
                        ValueLength.Value);
            }
            DicomContext.Reset();
        }

        public void Add(object value)
        {
            if (valueList.Count > 0)
            {
                if (valueList[0].GetType().Equals(value.GetType()))
                    valueList.Add(value);
                else
                    throw new DicomException("Only values of the same type " +
                                             "are allowed to be added.", VR.Tag, "value",
                        value.GetType().ToString());
            }
            else
                valueList.Add(value);
        }

        public void Add(Array multiValue)
        {
            if (multiValue is byte[])
                Add((object) multiValue);
            else
                foreach (var item in multiValue)
                    Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return valueList.GetEnumerator();
        }

        public object[] ToArray()
        {
            return valueList.ToArray();
        }

        // TODO: Value.ToString() implementation!
    }
}