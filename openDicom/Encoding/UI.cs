using System;
using openDicom.DataStructure;
using openDicom.Registry;

namespace openDicom.Encoding
{
    public sealed class UniqueIdentifier : ValueRepresentation
    {
        public UniqueIdentifier(Tag tag) : base("UI", tag)
        {
        }

        public override string ToLongString()
        {
            return "Unique Identifier (UI)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToImproperMultiValue(s);
            var uidValue = new Uid[multiValue.Length];
            for (var i = 0; i < uidValue.Length; i++)
            {
                var item = multiValue[i];
                // trailing zero padding
                if (item.Length > 0)
                {
                    var b = (byte) item[item.Length - 1];
                    if (b == 0)
                        item = item.Remove(item.Length - 1, 1);
                    item = item.Replace(" ", null);
                }
                if (item == null || item.Equals(""))
                    item = "0";
                uidValue[i] = new Uid(item);
            }
            return uidValue;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToProperMultiValue(s);
            var uidValue = new Uid[multiValue.Length];
            for (var i = 0; i < uidValue.Length; i++)
            {
                var item = multiValue[i];
                if (item.Length > 64)
                    throw new EncodingException(
                        "A value of max. 64 characters is only allowed.",
                        Tag, Name + "/item", item);
                if (item.Length > 0)
                {
                    // trailing zero padding
                    var b = (byte) item[item.Length - 1];
                    if (b == 0)
                        item = item.Remove(item.Length - 1, 1);
                    item = item.Replace(" ", null);
                }
                if (item == null || item.Equals(""))
                    throw new EncodingException("Uid is empty.", Tag,
                        Name + "/item", item);
                uidValue[i] = new Uid(item);
            }
            return uidValue;
        }
    }
}