using System.IO;
using openDicom.Encoding;

namespace openDicom.DataStructure
{
    public interface IDicomStreamMember
    {
        TransferSyntax TransferSyntax { get; }
        long StreamPosition { get; }
        void LoadFrom(Stream stream);
    }
}