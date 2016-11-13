using System;
using System.Text.RegularExpressions;
using openDicom.Encoding;

namespace openDicom.Registry
{
    public class Uid : IComparable
    {
        private string uid;

        public Uid(string uid)
        {
            Value = uid;
        }

        public string Value
        {
            set
            {
                if (value != null)
                {
                    var uid = value.Trim();
                    if (uid.Length > 64)
                        throw new DicomException("Uid exceeds max. length of " +
                                                 "64 characters.", "Uid.Value.Length",
                            uid.Length.ToString());
                    if (Regex.IsMatch(uid, "^[0-9]+(\\.[0-9]+)*$"))
                        this.uid = uid;
                    else
                        throw new DicomException("Uid is invalid.",
                            "Uid.Value", uid);
                }
                else
                    throw new DicomException("UID is null.", "Uid.Value");
            }
            get { return uid; }
        }

        public bool IsUserDefined
        {
            get { return !UidDictionary.Global.Contains(this); }
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(((Uid) obj).Value);
        }

        public bool Equals(string uid)
        {
            return Value.Equals(uid);
        }

        public bool Equals(Uid uid)
        {
            if (uid != null)
                return Value.Equals(uid.Value);
            return false;
        }

        public TransferSyntax GetTransferSyntax()
        {
            return new TransferSyntax(this);
        }

        public UidDictionaryEntry GetDictionaryEntry()
        {
            if (IsUserDefined)
                // no UID dictionary entry exists
                return new UidDictionaryEntry(this, "Unknown");
            return UidDictionary.Global.GetDictionaryEntry(this);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}