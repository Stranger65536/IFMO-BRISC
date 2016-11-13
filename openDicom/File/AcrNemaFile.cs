using System;
using System.IO;
using openDicom.DataStructure;
using openDicom.DataStructure.DataSet;
using openDicom.Encoding;
using openDicom.Image;

namespace openDicom.File
{
    public class AcrNemaFile
    {
        protected DataSet dataSet;
        protected PixelData pixelData;

        public AcrNemaFile(Stream stream) : this(stream, true)
        {
        }

        public AcrNemaFile(Stream stream, bool useStrictDecoding)
        {
            IsStrictDecoded = useStrictDecoding;
            LoadFrom(stream);
        }

        public AcrNemaFile(string fileName) : this(fileName, true)
        {
        }

        public AcrNemaFile(string fileName, bool useStrictDecoding)
        {
            IsStrictDecoded = useStrictDecoding;
            var fileStream = new FileStream(fileName, FileMode.Open,
                FileAccess.Read);
            try
            {
                LoadFrom(fileStream);
            }
            finally
            {
                fileStream.Close();
            }
        }

        public AcrNemaFile(string fileName, bool preloadToMemory,
            bool useStrictDecoding)
        {
            IsStrictDecoded = useStrictDecoding;
            var fileStream = new FileStream(fileName, FileMode.Open,
                FileAccess.Read);
            var buffer = new byte[fileStream.Length];
            try
            {
                fileStream.Read(buffer, 0, buffer.Length);
            }
            finally
            {
                fileStream.Close();
            }
            var memoryStream = new MemoryStream(buffer);
            try
            {
                LoadFrom(memoryStream);
            }
            finally
            {
                memoryStream.Close();
            }
        }

        public DataSet DataSet
        {
            get
            {
                if (dataSet != null)
                    return dataSet;
                throw new DicomException("Data set is null.",
                    "AcrNemaFile.DataSet");
            }
        }

        public PixelData PixelData
        {
            get
            {
                if (pixelData != null)
                    return pixelData;
                throw new DicomException("Pixel data is null.",
                    "AcrNemaFile.PixelData");
            }
        }

        public bool HasPixelData
        {
            get { return PixelData.HasPixelData(dataSet); }
        }

        public bool IsStrictDecoded
        {
            set { ValueRepresentation.IsStrictDecoded = value; }
            get { return ValueRepresentation.IsStrictDecoded; }
        }

        public static bool IsAcrNemaFile(string fileName)
        {
            var fileStream = new FileStream(fileName, FileMode.Open,
                FileAccess.Read);
            var isStrictDecoded = ValueRepresentation.IsStrictDecoded;
            ValueRepresentation.IsStrictDecoded = false;
            try
            {
                var tag = new Tag(fileStream, TransferSyntax.Default);
                var vr =
                    ValueRepresentation.LoadFrom(fileStream, tag);
                if (vr.IsUndefined) return false;
                var valueLength = new ValueLength(fileStream, vr);
                Value value = null;
                if (fileStream.Position + valueLength.Value <= fileStream.Length)
                    value = new Value(fileStream, vr, valueLength);
                else
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                fileStream.Close();
                ValueRepresentation.IsStrictDecoded = isStrictDecoded;
            }
        }

        public virtual void LoadFrom(Stream stream)
        {
            dataSet = new DataSet(stream);
            pixelData = new PixelData(dataSet);
        }

        public virtual DataSet GetJointDataSets()
        {
            return dataSet;
        }
    }
}