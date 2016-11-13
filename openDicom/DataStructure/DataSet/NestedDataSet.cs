using System.IO;
using openDicom.Encoding;

namespace openDicom.DataStructure.DataSet
{
    public sealed class NestedDataSet : DataSet
    {
        public new static readonly Tag DelimiterTag = new Tag("FFFE", "E00D");

        public NestedDataSet(Stream stream) : base(stream)
        {
        }

        public NestedDataSet(Stream stream, TransferSyntax transferSyntax) :
            base(stream, transferSyntax)
        {
        }

        public override void LoadFrom(Stream stream)
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
    }
}