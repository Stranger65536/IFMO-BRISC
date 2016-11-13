using System;
using System.Text.RegularExpressions;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class Date : ValueRepresentation
    {
        public Date(Tag tag) : base("DA", tag)
        {
        }

        public override string ToLongString()
        {
            return "Date (DA)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToImproperMultiValue(s);
            var date = new System.DateTime[multiValue.Length];
            for (var i = 0; i < date.Length; i++)
            {
                var item = multiValue[i];
                if (Regex.IsMatch(item, "^[0-9]{4}\\.?[0-9]{2}\\.?[0-9]{2}$"))
                {
                    item = item.Replace(".", null);
                    var year = item.Substring(0, 4);
                    var month = item.Substring(4, 2);
                    var day = item.Substring(6, 2);
                    try
                    {
                        date[i] = new System.DateTime(int.Parse(year),
                            int.Parse(month), int.Parse(day));
                    }
                    catch (Exception)
                    {
                        throw new EncodingException("Date format is invalid.",
                            Tag, Name + "/item", item);
                    }
                }
            }
            return date;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToProperMultiValue(s);
            var date = new System.DateTime[multiValue.Length];
            for (var i = 0; i < date.Length; i++)
            {
                var item = multiValue[i];
                if (item.Length > 0)
                {
                    if (Regex.IsMatch(item, "^[0-9]{4}\\.?[0-9]{2}\\.?[0-9]{2}$"))
                    {
                        item = item.Replace(".", null);
                        var year = item.Substring(0, 4);
                        var month = item.Substring(4, 2);
                        var day = item.Substring(6, 2);
                        try
                        {
                            date[i] = new System.DateTime(int.Parse(year),
                                int.Parse(month), int.Parse(day));
                        }
                        catch (Exception)
                        {
                            throw new EncodingException("Date format is invalid.",
                                Tag, Name + "/item", item);
                        }
                    }
                    else
                        throw new EncodingException("Date format is invalid.",
                            Tag, Name + "/item", item);
                }
            }
            return date;
        }
    }
}