using System.Text.RegularExpressions;
using openDicom.DataStructure;
using openDicom.DataStructure.DataSet;
using openDicom.Encoding;

namespace openDicom.Image
{
    public sealed class PixelData
    {
        public static readonly Tag SamplesPerPixelTag =
            new Tag("0028", "0002");

        public static readonly Tag PlanarConfigurationTag =
            new Tag("0028", "0006");

        public static readonly Tag RowsTag = new Tag("0028", "0010");
        public static readonly Tag ColumnsTag = new Tag("0028", "0011");
        public static readonly Tag BitsAllocatedTag = new Tag("0028", "0100");
        public static readonly Tag BitsStoredTag = new Tag("0028", "0101");
        public static readonly Tag HighBitTag = new Tag("0028", "0102");
        public static readonly Tag PixelDataTag = new Tag("7FE0", "0010");
        private int bitsAllocated;
        private int bitsStored;
        private int columns;
        private DataElement data;
        private int highBit = -1;
        private int planarConfiguration = -1;
        private int rows;
        private int samplesPerPixel;
        private TransferSyntax transferSyntax;

        public PixelData(int samplesPerPixel, int planarConfiguration, int rows,
            int columns, int bitsAllocated, int bitsStored, int highBit,
            DataElement data, TransferSyntax transferSyntax)
        {
            LoadFrom(samplesPerPixel, planarConfiguration, rows, columns,
                bitsAllocated, bitsStored, highBit, data, transferSyntax);
        }

        public PixelData(DataElement samplesPerPixel,
            DataElement planarConfiguration, DataElement rows,
            DataElement columns, DataElement bitsAllocated,
            DataElement bitsStored, DataElement highBit, DataElement data)
        {
            LoadFrom(samplesPerPixel, planarConfiguration, rows, columns,
                bitsAllocated, bitsStored, highBit, data);
        }

        public PixelData(DataSet dataSet)
        {
            LoadFrom(dataSet);
        }

        public int SamplesPerPixel
        {
            get
            {
                if (samplesPerPixel > 0)
                    return samplesPerPixel;
                throw new DicomException("Pixel data samples per pixel " +
                                         "are invalid.", "PixelData.SamplesPerPixel",
                    samplesPerPixel.ToString());
            }
        }

        public int PlanarConfiguration
        {
            get
            {
                if (planarConfiguration >= 0 && planarConfiguration <= 1)
                    return planarConfiguration;
                throw new DicomException("Pixel data planar configuration " +
                                         "is invalid.", "PixelData.PlanarConfiguration",
                    planarConfiguration.ToString());
            }
        }

        public int Rows
        {
            get
            {
                if (rows > 0)
                    return rows;
                throw new DicomException("Pixel data rows are invalid.",
                    "PixelData.Rows", rows.ToString());
            }
        }

        public int Columns
        {
            get
            {
                if (columns > 0)
                    return columns;
                throw new DicomException("Pixel data columns are invalid.",
                    "PixelData.Columns", columns.ToString());
            }
        }

        public int BitsAllocated
        {
            get
            {
                if (bitsAllocated > 0)
                    return bitsAllocated;
                throw new DicomException("Pixel data bits allocated is invalid.",
                    "PixelData.BitsAllocated", bitsAllocated.ToString());
            }
        }

        public int BitsStored
        {
            get
            {
                if (bitsStored > 0)
                    return bitsStored;
                throw new DicomException("Pixel data bits stored is invalid.",
                    "PixelData.BitsStored", bitsStored.ToString());
            }
        }

        public int HighBit
        {
            get
            {
                if (highBit > -1)
                    return highBit;
                throw new DicomException("Pixel data high bit is invalid.",
                    "PixelData.HighBit", highBit.ToString());
            }
        }

        public DataElement Data
        {
            get
            {
                if (data != null)
                    return data;
                throw new DicomException("Pixel data is null.",
                    "PixelData.Data");
            }
        }

        public bool IsJpeg
        {
            get
            {
                if (transferSyntax != null)
                    return Regex.IsMatch(transferSyntax.Uid.ToString(),
                        "^1\\.2\\.840\\.10008\\.1\\.2\\.4");
                return false;
            }
        }

        public void LoadFrom(int samplesPerPixel, int planarConfiguration,
            int rows, int columns, int bitsAllocated, int bitsStored,
            int highBit, DataElement data, TransferSyntax transferSyntax)
        {
            this.samplesPerPixel = samplesPerPixel;
            this.planarConfiguration = planarConfiguration;
            this.rows = rows;
            this.columns = columns;
            this.bitsAllocated = bitsAllocated;
            this.bitsAllocated = bitsStored;
            this.highBit = highBit;
            this.data = data;
            this.transferSyntax = transferSyntax;
        }

        public void LoadFrom(DataElement samplesPerPixel,
            DataElement planarConfiguration, DataElement rows,
            DataElement columns, DataElement bitsAllocated,
            DataElement bitsStored, DataElement highBit, DataElement data)
        {
            this.samplesPerPixel = ToValue(samplesPerPixel);
            this.planarConfiguration = ToValue(planarConfiguration);
            this.rows = ToValue(rows);
            this.columns = ToValue(columns);
            this.bitsAllocated = ToValue(bitsAllocated);
            this.bitsStored = ToValue(bitsStored);
            this.highBit = ToValue(highBit);
            this.data = data;
        }

        public void LoadFrom(DataSet dataSet)
        {
            if (dataSet != null)
            {
                foreach (DataElement element in dataSet)
                {
                    if (element.Tag.Equals(SamplesPerPixelTag))
                        samplesPerPixel = ToValue(element);
                    else if (element.Tag.Equals(PlanarConfigurationTag))
                        planarConfiguration = ToValue(element);
                    else if (element.Tag.Equals(RowsTag))
                        rows = ToValue(element);
                    else if (element.Tag.Equals(ColumnsTag))
                        columns = ToValue(element);
                    else if (element.Tag.Equals(BitsAllocatedTag))
                        bitsAllocated = ToValue(element);
                    else if (element.Tag.Equals(BitsStoredTag))
                        bitsStored = ToValue(element);
                    else if (element.Tag.Equals(HighBitTag))
                        highBit = ToValue(element);
                    else if (element.Tag.Equals(PixelDataTag))
                        data = element;
                    else if (element.Tag.Equals(TransferSyntax.UidTag))
                        transferSyntax = new TransferSyntax(element);
                }
            }
            else
                throw new DicomException("Data set is null.", "dataSet");
        }

        private int ToValue(DataElement element)
        {
            if (element != null)
            {
                if (!element.Tag.Equals(PixelDataTag))
                    return (ushort) element.Value[0];
                throw new DicomException("Data element does not belong " +
                                         "to pixel data.", "element.Tag",
                    element.Tag.ToString());
            }
            throw new DicomException("Data element is null.", "element");
        }

        public static bool HasPixelData(DataSet dataSet)
        {
            if (dataSet != null)
                return dataSet.Contains(PixelDataTag);
            return false;
        }

        public static bool IsValid(DataSet dataSet)
        {
            if (dataSet != null)
                return dataSet.Contains(SamplesPerPixelTag)
                       && dataSet.Contains(PlanarConfigurationTag)
                       && dataSet.Contains(RowsTag)
                       && dataSet.Contains(ColumnsTag)
                       && dataSet.Contains(BitsAllocatedTag)
                       && dataSet.Contains(BitsStoredTag)
                       && dataSet.Contains(HighBitTag)
                       && dataSet.Contains(PixelDataTag);
            return false;
        }

        public object[] ToArray()
        {
            if (Data.Value.IsSequence)
            {
                var sq = (Sequence) Data.Value[0];
                var array = new object[sq.Count];
                for (var i = 0; i < sq.Count; i++)
                    array[i] = sq[i].Value[0];
                return array;
            }
            return new object[1] {Data.Value[0]};
        }

        public byte[][] ToBytesArray()
        {
            byte[][] bytesArray;
            if (Data.Value.IsSequence)
            {
                var sq = (Sequence) Data.Value[0];
                bytesArray = new byte[sq.Count][];
                for (var i = 0; i < sq.Count; i++)
                {
                    if (sq[i].Value[0] is ushort[])
                        bytesArray[i] = ByteConvert.ToBytes(
                            (ushort[]) sq[i].Value[0]);
                    else
                        bytesArray[i] = (byte[]) sq[i].Value[0];
                }
            }
            else
            {
                bytesArray = new byte[1][];
                if (Data.Value[0] is ushort[])
                    bytesArray[0] = ByteConvert.ToBytes(
                        (ushort[]) Data.Value[0]);
                else
                    bytesArray[0] = (byte[]) Data.Value[0];
            }
            return bytesArray;
        }
    }
}