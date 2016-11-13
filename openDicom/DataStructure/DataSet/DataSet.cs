using System.Collections;
using System.IO;
using openDicom.Encoding;

namespace openDicom.DataStructure.DataSet
{
    public class DataSet : Sequence
    {
        // key index pair for access of Sequence as array
        private readonly Hashtable keys = new Hashtable();

        public DataSet()
        {
        }

        public DataSet(Stream stream) : base(stream)
        {
        }

        public DataSet(Stream stream, TransferSyntax transferSyntax) :
            base(stream, transferSyntax)
        {
        }

        public DataElement this[Tag tag]
        {
            get
            {
                if (keys.ContainsKey(tag.ToString()))
                {
                    return base[(int) keys[tag.ToString()]];
                }
                throw new InvalidDataException("Can't handle DICOM file without z-index!");
            }
        }

        public override void LoadFrom(Stream stream)
        {
            streamPosition = stream.Position;
            TransferSyntax.CharacterRepertoire = CharacterRepertoire.Default;
            var isTrailingPadding = false;
            while (stream.Position < stream.Length && !isTrailingPadding)
            {
                var element = new DataElement(stream, TransferSyntax);
                if (element.Tag.Equals(CharacterRepertoire.CharacterSetTag))
                    TransferSyntax.CharacterRepertoire =
                        new CharacterRepertoire((string) element.Value[0]);
                isTrailingPadding = element.Tag.Equals("(0000,0000)");
                if (!isTrailingPadding)
                    Add(element);
            }
        }

        public override int Add(DataElement dataElement)
        {
            var index = base.Add(dataElement);
            keys.Add(dataElement.Tag.ToString(), index);
            return index;
        }

        public void Add(DataSet dataSet)
        {
            foreach (DataElement element in dataSet)
                Add(element);
        }

        public override Sequence GetJointSubsequences()
        {
            return base.GetJointSubsequences();
        }

        public bool Contains(Tag tag)
        {
            return keys.Contains(tag.ToString());
        }

        public override void Clear()
        {
            keys.Clear();
            base.Clear();
        }

        public override void Sort()
        {
            base.Sort();
            for (var index = 0; index < Count; index++)
            {
                var tagKey = base[index].Tag.ToString();
                keys[tagKey] = index;
            }
        }
    }
}