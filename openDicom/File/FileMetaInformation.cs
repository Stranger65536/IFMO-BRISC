using System.IO;
using openDicom.DataStructure.DataSet;
using openDicom.Encoding;

namespace openDicom.File
{
    public sealed class FileMetaInformation : DataSet
    {
        public static readonly string DicomPrefix =
            ByteConvert.ToString(new byte[4] {0x44, 0x49, 0x43, 0x4D},
                CharacterRepertoire.Ascii);

        public FileMetaInformation(Stream stream) :
            base(stream, TransferSyntax.FileMetaInformation)
        {
        }

        public string FilePreamble { get; private set; } = "";

        public override void LoadFrom(Stream stream)
        {
            this.streamPosition = stream.Position;
            DicomContext.Set(stream, null);
            var buffer = new byte[132];
            stream.Read(buffer, 0, 132);
            FilePreamble = TransferSyntax.ToString(buffer, 128);
            FilePreamble = FilePreamble.Replace("\0", null);
            var dicomPrefix = TransferSyntax.ToString(buffer, 128, 4);
            if (dicomPrefix.Equals(DicomPrefix))
            {
                // group length
                var element = new DataElement(stream, TransferSyntax);
                Add(element);
                var groupLength = (uint) element.Value[0];
                var streamPosition = stream.Position;
                // consider omission of current stream position
                buffer = new byte[streamPosition + groupLength];
                stream.Read(buffer, (int) streamPosition, (int) groupLength);
                var memoryStream = new MemoryStream(buffer);
                try
                {
                    memoryStream.Seek(streamPosition, 0);
                    base.LoadFrom(memoryStream);
                }
                finally
                {
                    memoryStream.Close();
                }
            }
            else
            {
                stream.Close();
                throw new DicomException("Working with a non-valid DICOM file.");
            }
            DicomContext.Reset();
        }

        public override void Clear()
        {
            base.Clear();
            FilePreamble = "";
        }
    }
}