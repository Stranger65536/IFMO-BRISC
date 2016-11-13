using System;

namespace openDicom.Registry
{
    public enum UidType
    {
        TransferSyntax,
        SopClass,
        FrameOfReference,
        MetaSopClass,
        SopInstance,
        ServiceClass,
        PrinterSopInstance,
        PrintQueueSopInstance,
        CodingScheme,
        ApplicationContextName,
        LdapOid,
        Unknown
    }

    public sealed class UidDictionaryEntry : IComparable
    {
        public UidDictionaryEntry(string uid) : this(uid, null, null, null)
        {
        }

        public UidDictionaryEntry(Uid uid) :
            this(uid, null, UidType.Unknown, false)
        {
        }

        public UidDictionaryEntry(string uid, string name) :
            this(uid, name, null, null)
        {
        }

        public UidDictionaryEntry(Uid uid, string name) :
            this(uid, name, UidType.Unknown, false)
        {
        }

        public UidDictionaryEntry(string uid, string name, string type,
            string retired)
        {
            Uid = new Uid(uid);
            if (name == null)
                Name = "";
            else
                Name = name.Trim();
            if (type == null)
                Type = UidType.Unknown;
            else
                Type = (UidType) Enum.Parse(typeof(UidType), type);
            if (retired != null)
            {
                retired = retired.Trim().ToLower();
                IsRetired = retired == "ret" || retired == "retired" ||
                            retired == "true";
            }
        }

        public UidDictionaryEntry(Uid uid, string name, UidType type,
            bool retired)
        {
            Uid = uid;
            if (name == null)
                Name = "";
            else
                Name = name.Trim();
            Type = type;
            IsRetired = retired;
        }

        public Uid Uid { get; }
        public string Name { get; }
        public UidType Type { get; }
        public bool IsRetired { get; }

        public int CompareTo(object obj)
        {
            var uid = ((UidDictionaryEntry) obj).Uid;
            return Uid.CompareTo(uid);
        }

        public bool Equals(UidDictionaryEntry dictionaryEntry)
        {
            return CompareTo(dictionaryEntry) == 0;
        }
    }
}