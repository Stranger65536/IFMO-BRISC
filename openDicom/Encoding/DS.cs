using System;
using System.Globalization;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class DecimalString : ValueRepresentation
    {
        public DecimalString(Tag tag) : base("DS", tag)
        {
        }

        public override string ToLongString()
        {
            return "Decimal String (DS)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToImproperMultiValue(s);
            var decimalValue = new decimal[multiValue.Length];
            for (var i = 0; i < decimalValue.Length; i++)
            {
                var item = multiValue[i];
                item = item.Trim();
                try
                {
                    if (item.Length > 0)
                        decimalValue[i] = decimal.Parse(item,
                            NumberStyles.Float,
                            NumberFormatInfo.InvariantInfo);
                }
                catch (Exception)
                {
                    throw new EncodingException(
                        "Decimal string format is invalid.",
                        Tag, Name + "/item", item);
                }
            }
            return decimalValue;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToProperMultiValue(s);
            var decimalValue = new decimal[multiValue.Length];
            for (var i = 0; i < decimalValue.Length; i++)
            {
                var item = multiValue[i];
                if (item.Length <= 16)
                {
                    item = item.Trim();
                    try
                    {
                        if (item.Length > 0)
                            decimalValue[i] = decimal.Parse(item,
                                NumberStyles.Float,
                                NumberFormatInfo.InvariantInfo);
                    }
                    catch (Exception)
                    {
                        throw new EncodingException(
                            "Decimal string format is invalid.",
                            Tag, Name + "/item", item);
                    }
                }
                else
                    throw new EncodingException(
                        "A value of max. 16 bytes is only allowed.",
                        Tag, Name + "/item", item);
            }
            return decimalValue;
        }
    }
}