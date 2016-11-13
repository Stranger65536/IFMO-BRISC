using System;
using openDicom.DataStructure;

namespace openDicom.Registry
{
    public sealed class DataElementDictionaryEntry : IComparable
    {
        public DataElementDictionaryEntry(string tag) :
            this(tag, null, null, null, null)
        {
        }

        public DataElementDictionaryEntry(Tag tag) :
            this(tag, null, null, null, false)
        {
        }

        public DataElementDictionaryEntry(string tag, string description) :
            this(tag, description, null, null, null)
        {
        }

        public DataElementDictionaryEntry(Tag tag, string description) :
            this(tag, description, null, null, false)
        {
        }

        public DataElementDictionaryEntry(string tag, string description,
            string vr, string vm, string retired)
        {
            Tag = new Tag(tag);
            if (description == null) description = "";
            Description = description.Trim();
            VR = ValueRepresentation.GetBy(vr, Tag);
            VM = new ValueMultiplicity(VR, vm);
            if (retired != null)
            {
                retired = retired.Trim().ToLower();
                IsRetired = retired == "ret" || retired == "retired" ||
                            retired == "true";
            }
        }

        public DataElementDictionaryEntry(Tag tag, string description,
            ValueRepresentation vr, ValueMultiplicity vm, bool retired)
        {
            Tag = tag;
            if (description == null) description = "";
            Description = description.Trim();
            if (vr == null)
                VR = ValueRepresentation.GetBy(Tag);
            else
                VR = vr;
            if (vm == null)
                VM = new ValueMultiplicity(VR);
            else
                VM = vm;
            IsRetired = retired;
        }

        public Tag Tag { get; }
        public string Description { get; } = string.Empty;
        public ValueRepresentation VR { get; }
        public ValueMultiplicity VM { get; }
        public bool IsRetired { get; }

        public int CompareTo(object obj)
        {
            var tag = ((DataElementDictionaryEntry) obj).Tag;
            return Tag.CompareTo(tag);
        }

        public bool Equals(DataElementDictionaryEntry dictionaryEntry)
        {
            return CompareTo(dictionaryEntry) == 0;
        }
    }
}