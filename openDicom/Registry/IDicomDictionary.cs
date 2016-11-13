namespace openDicom.Registry
{
    public enum DictionaryFileFormat
    {
        BinaryFile,
        PropertyFile,
        XmlFile,
        CsvFile
    }

    public interface IDicomDictionary
    {
        void LoadFrom(string fileName, DictionaryFileFormat fileFormat);
        void SaveTo(string fileName, DictionaryFileFormat fileFormat);
    }
}