using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using openDicom.Encoding;
using openDicom.Registry;

namespace openDicom.DataStructure
{
    public sealed class Tag : IComparable, IDicomStreamMember
    {
        private string element = "0000";
        private string group = "0000";
        private TransferSyntax transferSyntax;

        public Tag(string tag) : this(tag, (TransferSyntax) null)
        {
        }

        public Tag(string tag, TransferSyntax transferSyntax)
        {
            tag = tag.ToUpper().Replace(" ", null);
            if (Regex.IsMatch(tag, "^\\([0-9A-F]{4}\\,[0-9A-F]{4}\\)$"))
            {
                var s = tag.Split(',');
                Group = s[0].TrimStart('(').Trim();
                Element = s[1].TrimEnd(')').Trim();
                TransferSyntax = transferSyntax;
            }
            else
                throw new DicomException("Tag is invalid.", this, "tag", tag);
        }

        public Tag(string group, string element) : this(group, element, null)
        {
        }

        public Tag(string group, string element, TransferSyntax transferSyntax)
        {
            Group = group;
            Element = element;
            TransferSyntax = transferSyntax;
        }

        public Tag(int group, int element) : this(group, element, null)
        {
        }

        public Tag(int group, int element, TransferSyntax transferSyntax)
        {
            TransferSyntax = transferSyntax;
            SetInt(group, element);
        }

        public Tag(Stream stream) : this(stream, null)
        {
        }

        public Tag(Stream stream, TransferSyntax transferSyntax)
        {
            TransferSyntax = transferSyntax;
            LoadFrom(stream);
        }

        public string Group
        {
            get { return group; }
            set
            {
                if (value != null)
                {
                    if (Regex.IsMatch(value, "^[0-9A-Fa-f]{4}$"))
                        group = value.ToUpper();
                    else
                        throw new DicomException("Tag.Group is invalid.",
                            this, "Group", value);
                }
                else
                    throw new DicomException("Tag.Group is null.", this);
            }
        }

        public string Element
        {
            get { return element; }
            set
            {
                if (value != null)
                {
                    if (Regex.IsMatch(value, "^[0-9A-Fa-f]{4}$"))
                        element = value.ToUpper();
                    else
                        throw new DicomException("Tag.Element is invalid.",
                            this, "Element", value);
                }
                else
                    throw new DicomException("Tag.Element is null.");
            }
        }

        public bool IsUserDefined
        {
            get { return !DataElementDictionary.Global.Contains(this); }
        }

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(((Tag) obj).ToString());
        }

        public long StreamPosition { get; private set; } = -1;

        public TransferSyntax TransferSyntax
        {
            set { transferSyntax = value; }
            get
            {
                if (transferSyntax != null)
                    return transferSyntax;
                throw new DicomException("Transfer syntax is null. " +
                                         "Make sure you are not referencing a dictionary " +
                                         "entry tag.", "Tag.TransferSyntax");
            }
        }

        public void LoadFrom(Stream stream)
        {
            StreamPosition = stream.Position;
            DicomContext.Set(stream, null);
            if (stream != null)
            {
                var buffer = new byte[2];
                stream.Read(buffer, 0, 2);
                int group = BitConverter.ToUInt16(
                    TransferSyntax.CorrectByteOrdering(buffer), 0);
                stream.Read(buffer, 0, 2);
                int element = BitConverter.ToUInt16(
                    TransferSyntax.CorrectByteOrdering(buffer), 0);
                SetInt(group, element);
            }
            else
                throw new DicomException("Tag.LoadFrom.Stream is null.",
                    this);
            DicomContext.Reset();
        }

        private void SetInt(int group, int element)
        {
            if (group < 0x0000 || group > 0xFFFF)
                throw new ArgumentOutOfRangeException("Tag.Group");
            if (element < 0x0000 || element > 0xFFFF)
                throw new ArgumentOutOfRangeException("Tag.Element");
            Group = string.Format("{0:X4}", group);
            Element = string.Format("{0:X4}", element);
        }

        public void SaveTo(Stream stream)
        {
            StreamPosition = stream.Position;
            DicomContext.Set(stream, this);
            if (stream != null)
            {
                var group = BitConverter.GetBytes(ushort.Parse(Group,
                    NumberStyles.HexNumber));
                var element = BitConverter.GetBytes(ushort.Parse(Element,
                    NumberStyles.HexNumber));
                group = TransferSyntax.CorrectByteOrdering(group);
                element = TransferSyntax.CorrectByteOrdering(element);
                var buffer = new byte[4];
                Array.Copy(group, 0, buffer, 0, 2);
                Array.Copy(element, 0, buffer, 2, 2);
                stream.Write(buffer, 0, 4);
            }
            else
                throw new DicomException("Tag.SaveToStream.Stream is null.",
                    this);
            DicomContext.Reset();
        }

        public bool Equals(Tag tag)
        {
            return CompareTo(tag) == 0;
        }

        public bool Equals(string tag)
        {
            tag = tag.ToUpper().Replace(" ", null);
            if (Regex.IsMatch(tag, "^\\([0-9A-F]{4}\\,[0-9A-F]{4}\\)$"))
                return ToString().Equals(tag);
            throw new DicomException("Tag is invalid.", this, "tag", tag);
        }

        public DataElementDictionaryEntry GetDictionaryEntry()
        {
            if (IsUserDefined)
                // no data element dictionary entry exists
                return new DataElementDictionaryEntry(this, "Unknown");
            return DataElementDictionary.Global.GetDictionaryEntry(this);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", Group, Element);
        }
    }
}