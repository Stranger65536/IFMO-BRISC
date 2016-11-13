using System.IO;
using openDicom.DataStructure;
using openDicom.Registry;

namespace openDicom
{
    public sealed class DicomContext
    {
        public static DataElementDictionary DataElementDictionary
        {
            set { DataElementDictionary.Global = value; }
            get { return DataElementDictionary.Global; }
        }

        public static UidDictionary UidDictionary
        {
            set { UidDictionary.Global = value; }
            get { return UidDictionary.Global; }
        }

        public static Tag CurrentTag { set; get; }
        public static Stream BaseStream { set; get; }

        public static long StreamPosition
        {
            get
            {
                if (BaseStream != null)
                    return BaseStream.Position;
                return -1;
            }
        }

        public static void Set(Stream baseStream, Tag currentTag)
        {
            BaseStream = baseStream;
            CurrentTag = currentTag;
        }

        public static void Reset()
        {
            CurrentTag = null;
            BaseStream = null;
        }
    }
}