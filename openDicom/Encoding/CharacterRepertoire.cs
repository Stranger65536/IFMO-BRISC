using openDicom.DataStructure;


namespace openDicom.Encoding
{
    public sealed class CharacterRepertoire
    {
        public static readonly Tag CharacterSetTag = new Tag("0008", "0005");


        public static readonly CharacterRepertoire Default =
            new CharacterRepertoire("ISO_IR 6");


        public static readonly CharacterRepertoire Ascii =
            new CharacterRepertoire("ASCII");


        public static readonly CharacterRepertoire Utf8 =
            new CharacterRepertoire("UTF-8");


        public static readonly CharacterRepertoire G0 =
            new CharacterRepertoire("ISO_IR 6");


        public static readonly CharacterRepertoire G1 =
            new CharacterRepertoire("ISO_IR 100");


        public CharacterRepertoire() : this(null)
        {
        }


        public CharacterRepertoire(string characterSet)
        {
            if (characterSet == null) characterSet = "";
            characterSet = characterSet.ToUpper()
                .Replace(" ", null)
                .Replace("-", null)
                .Replace("_", null);
            switch (characterSet)
            {
                case "":
                case "ISOIR6":
                case "ASCII":
                    Encoding = System.Text.Encoding.ASCII;
                    break;
                case "ISOIR100":
                case "ISO88591":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-1");
                    break;
                case "ISOIR101":
                case "ISO88592":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-2");
                    break;
                case "ISOIR109":
                case "ISO88593":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-3");
                    break;
                case "ISOIR110":
                case "ISO88594":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-4");
                    break;
                case "ISOIR144":
                case "ISO88595":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-5");
                    break;
                case "ISOIR127":
                case "ISO88596":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-6");
                    break;
                case "ISOIR126":
                case "ISO88597":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-7");
                    break;
                case "ISOIR138":
                case "ISO88598":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-8");
                    break;
                case "ISOIR148":
                case "ISO88599":
                    Encoding =
                        System.Text.Encoding.GetEncoding("ISO-8859-9");
                    break;
                case "ISOIR192":
                case "UTF8":
                    Encoding = System.Text.Encoding.UTF8;
                    break;
                default:
                    throw new DicomException("Encoding is not supported.",
                        "characterSet", characterSet);
            }
        }


        public System.Text.Encoding Encoding { get; }
    }
}