using System;
using openDicom.DataStructure;
using openDicom.Encoding.Type;

namespace openDicom.Encoding
{
    public sealed class AgeString : ValueRepresentation
    {
        public AgeString(Tag tag) : base("AS", tag)
        {
        }

        public override string ToLongString()
        {
            return "Age String (AS)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToImproperMultiValue(s);
            var age = new Age[multiValue.Length];
            for (var i = 0; i < age.Length; i++)
            {
                var item = multiValue[i];
                try
                {
                    if (item == null || item.Equals(""))
                        item = "000D";
                    age[i] = new Age(item);
                }
                catch (Exception)
                {
                    throw new EncodingException("Age string format is invalid.",
                        Name + "/item", item);
                }
            }
            return age;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToProperMultiValue(s);
            var age = new Age[multiValue.Length];
            for (var i = 0; i < age.Length; i++)
            {
                var item = multiValue[i];
                try
                {
                    age[i] = new Age(item);
                }
                catch (Exception)
                {
                    throw new EncodingException("Age string format is invalid.",
                        Name + "/item", item);
                }
            }
            return age;
        }
    }
}