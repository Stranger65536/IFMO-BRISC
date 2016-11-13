using System.IO;
using openDicom.DataStructure.DataSet;
using openDicom.Encoding;
using openDicom.Image;
using openDicom.Registry;

namespace openDicom.File
{
    public class DicomFile : AcrNemaFile
    {
        private FileMetaInformation metaInformation;

        public DicomFile(Stream stream) : base(stream)
        {
        }

        public DicomFile(Stream stream, bool useStrictDecoding) :
            base(stream, useStrictDecoding)
        {
        }

        public DicomFile(string fileName) : base(fileName)
        {
        }

        public DicomFile(string fileName, bool useStrictDecoding) :
            base(fileName, useStrictDecoding)
        {
        }

        public DicomFile(string fileName, bool preloadToMemory,
            bool useStrictDecoding) :
                base(fileName, preloadToMemory, useStrictDecoding)
        {
        }

        public FileMetaInformation MetaInformation
        {
            get
            {
                if (metaInformation != null)
                    return metaInformation;
                throw new DicomException("DicomFile.MetaInformation is null.");
            }
        }

        public static bool IsDicomFile(string fileName)
        {
            var fileStream = new FileStream(fileName, FileMode.Open,
                FileAccess.Read);
            try
            {
                if (fileStream.Length > 132)
                {
                    var buffer = new byte[132];
                    fileStream.Read(buffer, 0, 132);
                    var dicomPrefix =
                        TransferSyntax.FileMetaInformation
                            .ToString(buffer, 128, 4);
                    return dicomPrefix.Equals(FileMetaInformation.DicomPrefix);
                }
                else return false;
            }
            finally
            {
                fileStream.Close();
            }
        }

        public override void LoadFrom(Stream stream)
        {
            metaInformation = new FileMetaInformation(stream);
            var transferSyntaxDataElement =
                MetaInformation[TransferSyntax.UidTag];
            var uid = (Uid) transferSyntaxDataElement.Value[0];
            dataSet = new DataSet(stream, new TransferSyntax(uid));
            pixelData = new PixelData(GetJointDataSets());
        }

        public override DataSet GetJointDataSets()
        {
            var dataSet = new DataSet();
            dataSet.Add(MetaInformation);
            dataSet.Add(DataSet);
            return dataSet;
        }
    }
}