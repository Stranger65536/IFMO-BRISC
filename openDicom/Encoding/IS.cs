using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class IntegerString : ValueRepresentation
    {
        public IntegerString(Tag tag) : base("IS", tag)
        {
        }

        public override string ToLongString()
        {
            return "Integer String (IS)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToImproperMultiValue(s);
            var intValue = new long[multiValue.Length];
            for (var i = 0; i < intValue.Length; i++)
            {
                var item = multiValue[i];
                item = item.Trim();
                try
                {
                    if (item.Length > 0)
                        intValue[i] = long.Parse(item);
                }
                catch (Exception)
                {
                    throw new EncodingException(
                        "Integer string format is invalid.", Tag,
                        Name + "/item", item);
                }
            }
            return intValue;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToProperMultiValue(s);
            var intValue = new long[multiValue.Length];
            for (var i = 0; i < intValue.Length; i++)
            {
                var item = multiValue[i];
                if (item.Length <= 12)
                {
                    item = item.Trim();
                    try
                    {
                        if (item.Length > 0)
                            intValue[i] = long.Parse(item);
                    }
                    catch (Exception)
                    {
                        throw new EncodingException(
                            "Integer string format is invalid.", Tag,
                            Name + "/item", item);
                    }
                }
                else
                    throw new EncodingException(
                        "A value of max. 12 bytes is only allowed.", Tag,
                        Name + "/item", item);
            }
            return intValue;
        }
    }
}