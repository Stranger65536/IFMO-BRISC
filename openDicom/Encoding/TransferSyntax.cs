using System;
using System.Text.RegularExpressions;
using openDicom.DataStructure;
using openDicom.DataStructure.DataSet;
using openDicom.Registry;

namespace openDicom.Encoding
{
    public class TransferSyntax
    {
        public static readonly Tag UidTag = new Tag("0002", "0010");
        public static readonly TransferSyntax Default = new TransferSyntax();

        public static readonly TransferSyntax FileMetaInformation =
            new TransferSyntax("1.2.840.10008.1.2.1");

        private CharacterRepertoire characterRepertoire =
            CharacterRepertoire.Default;

        private Uid uid = new Uid("1.2.840.10008.1.2");

        public TransferSyntax()
        {
        }

        public TransferSyntax(string uid) : this(new Uid(uid), null)
        {
        }

        public TransferSyntax(string uid,
            CharacterRepertoire characterRepertoire) :
                this(new Uid(uid), characterRepertoire)
        {
        }

        public TransferSyntax(Uid uid) : this(uid, null)
        {
        }

        public TransferSyntax(Uid uid, CharacterRepertoire characterRepertoire)
        {
            Uid = uid;
            CharacterRepertoire = characterRepertoire;
        }

        public TransferSyntax(DataElement transferSyntaxUid) :
            this(transferSyntaxUid, null)
        {
        }

        public TransferSyntax(DataElement transferSyntaxUid,
            CharacterRepertoire characterRepertoire)
        {
            CharacterRepertoire = characterRepertoire;
            LoadFrom(transferSyntaxUid);
        }

        public TransferSyntax(DataSet dataSet) : this(dataSet, null)
        {
        }

        public TransferSyntax(DataSet dataSet,
            CharacterRepertoire characterRepertoire)
        {
            CharacterRepertoire = characterRepertoire;
            LoadFrom(dataSet);
        }

        public bool IsImplicitVR { get; private set; } = true;
        public bool IsLittleEndian { get; private set; } = true;

        public bool IsMachineLittleEndian
        {
            get { return BitConverter.IsLittleEndian; }
        }

        public CharacterRepertoire CharacterRepertoire
        {
            set
            {
                if (value == null)
                    characterRepertoire = CharacterRepertoire.Default;
                else
                    characterRepertoire = value;
            }
            get { return characterRepertoire; }
        }

        public Uid Uid
        {
            set
            {
                if (value != null)
                {
                    uid = value;
                    if (Regex.IsMatch(uid.ToString(),
                        "^1\\.2\\.840\\.10008\\.1\\.2"))
                    {
                        switch (uid.ToString())
                        {
                            case "1.2.840.10008.1.2":
                                IsImplicitVR = true;
                                IsLittleEndian = true;
                                break;
                            case "1.2.840.10008.1.2.1":
                                IsImplicitVR = false;
                                IsLittleEndian = true;
                                break;
                            case "1.2.840.10008.1.2.2":
                                IsImplicitVR = false;
                                IsLittleEndian = false;
                                break;
                            case "1.2.840.10008.1.2.99":
                                throw new DicomException("The deflated " +
                                                         "transfer syntax is not supported.",
                                    "uid", uid.ToString());
                            default:
                                // defaults for transfer syntax for JPEG
                                // (1.2.840.10008.1.2.4.*) and RLE
                                // (1.2.840.10008.1.2.5) encoding according
                                // to the DICOM standard
                                IsImplicitVR = false;
                                IsLittleEndian = true;
                                break;
                        }
                    }
                    else
                        throw new DicomException("UID is not a valid transfer " +
                                                 "syntax UID.", "TransferSyntax.Uid", uid.ToString());
                }
                else
                    throw new DicomException("UID is null.", "Uid.Uid");
            }
            get { return uid; }
        }

        public void LoadFrom(DataSet dataSet)
        {
            // character repertoire content cannot be read from data set,
            // because data set is already read in.
            if (dataSet.Contains(UidTag))
                LoadFrom(dataSet[UidTag]);
            else
                throw new DicomException("Data set does not contain a " +
                                         "transfer syntax UID.", "dataSet");
        }

        public void LoadFrom(DataElement transferSyntaxUid)
        {
            if (transferSyntaxUid != null)
            {
                if (transferSyntaxUid.Tag.Equals(UidTag))
                    Uid = (Uid) transferSyntaxUid.Value[0];
                else
                    throw new DicomException("Data element is not a transfer " +
                                             "syntax UID.", "transferSyntaxUID.Tag",
                        transferSyntaxUid.Tag.ToString());
            }
            else
                throw new DicomException("Data element is null.",
                    "transferSyntaxUID");
        }

        public bool Equals(TransferSyntax transferSyntax)
        {
            if (transferSyntax != null)
                return Uid.Equals(transferSyntax.Uid);
            return false;
        }

        public string ToString(byte[] bytes)
        {
            return ByteConvert.ToString(bytes, CharacterRepertoire);
        }

        public string ToString(byte[] bytes, int count)
        {
            return ByteConvert.ToString(bytes, count, CharacterRepertoire);
        }

        public virtual string ToString(byte[] bytes, int offset, int count)
        {
            return ByteConvert.ToString(bytes, offset, count,
                CharacterRepertoire);
        }

        public virtual byte[] ToBytes(string s)
        {
            return ByteConvert.ToBytes(s, CharacterRepertoire);
        }

        public ushort CorrectByteOrdering(ushort word)
        {
            if ((IsMachineLittleEndian && !IsLittleEndian) ||
                (!IsMachineLittleEndian && IsLittleEndian))
                return ByteConvert.SwapBytes(word);
            return word;
        }

        public short CorrectByteOrdering(short word)
        {
            if ((IsMachineLittleEndian && !IsLittleEndian) ||
                (!IsMachineLittleEndian && IsLittleEndian))
                return ByteConvert.SwapBytes(word);
            return word;
        }

        public uint CorrectByteOrdering(uint value)
        {
            if ((IsMachineLittleEndian && !IsLittleEndian) ||
                (!IsMachineLittleEndian && IsLittleEndian))
                return ByteConvert.SwapBytes(value);
            return value;
        }

        public int CorrectByteOrdering(int value)
        {
            if ((IsMachineLittleEndian && !IsLittleEndian) ||
                (!IsMachineLittleEndian && IsLittleEndian))
                return ByteConvert.SwapBytes(value);
            return value;
        }

        public byte[] CorrectByteOrdering(byte[] bytes)
        {
            if ((IsMachineLittleEndian && !IsLittleEndian) ||
                (!IsMachineLittleEndian && IsLittleEndian))
                return ByteConvert.SwapBytes(bytes);
            return bytes;
        }

        public override string ToString()
        {
            return Uid.ToString();
        }
    }
}