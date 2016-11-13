using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class CodeString : ValueRepresentation
    {
        public CodeString(Tag tag) : base("CS", tag)
        {
        }

        public override string ToLongString()
        {
            return "Code String (CS)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var code = ToImproperMultiValue(s);
            for (var i = 0; i < code.Length; i++)
            {
                var item = code[i];
                code[i] = item.Trim();
            }
            return code;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var code = ToProperMultiValue(s);
            for (var i = 0; i < code.Length; i++)
            {
                var item = code[i];
                if (item.Length > 16)
                    throw new EncodingException(
                        "A value of max. 16 bytes is only allowed.", Tag,
                        Name + "/item", item);
                if (item.Length > 0)
                    code[i] = item.Trim();
            }
            return code;
        }
    }
}