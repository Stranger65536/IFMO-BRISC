using System.Collections;
using System.IO;
using openDicom.Encoding;

namespace openDicom.DataStructure.DataSet
{
    public class Sequence : IDicomStreamMember
    {
        public static readonly Tag DelimiterTag = new Tag("FFFE", "E0DD");
        protected ArrayList itemList = new ArrayList();
        protected long streamPosition = -1;
        private TransferSyntax transferSyntax = TransferSyntax.Default;

        public Sequence()
        {
        }

        public Sequence(Stream stream) : this(stream, null)
        {
        }

        public Sequence(Stream stream, TransferSyntax transferSyntax)
        {
            TransferSyntax = transferSyntax;
            LoadFrom(stream);
        }

        public DataElement this[int index]
        {
            get { return (DataElement) itemList[index]; }
        }

        public int Count
        {
            get { return itemList.Count; }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
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

        public long StreamPosition
        {
            get { return streamPosition; }
        }

        public virtual void LoadFrom(Stream stream)
        {
            streamPosition = stream.Position;
            var element = new DataElement(stream, TransferSyntax);
            var isTrailingPadding = false;
            while (!element.Tag.Equals(DelimiterTag) &&
                   stream.Position < stream.Length)
            {
                isTrailingPadding = element.Tag.Equals("(0000,0000)");
                if (!isTrailingPadding)
                    Add(element);
                element = new DataElement(stream, TransferSyntax);
            }
        }

        public virtual int Add(DataElement dataElement)
        {
            return itemList.Add(dataElement);
        }

        public void Add(Sequence sequence)
        {
            foreach (DataElement element in sequence)
                Add(element);
        }

        private Sequence TreeNodeToSequence(Sequence sequence)
        {
            var result = new Sequence();
            foreach (DataElement element in sequence)
            {
                result.Add(element);
                foreach (var value in element.Value)
                {
                    if (value is Sequence)
                        result.Add(TreeNodeToSequence((Sequence) value));
                }
            }
            return result;
        }

        public virtual Sequence GetJointSubsequences()
        {
            return TreeNodeToSequence(this);
        }

        public virtual void Clear()
        {
            itemList.Clear();
        }

        public virtual void Sort()
        {
            itemList.Sort();
        }

        public DataElement[] ToArray()
        {
            return (DataElement[]) itemList.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return itemList.GetEnumerator();
        }
    }
}