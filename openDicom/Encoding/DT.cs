using System;
using System.Text.RegularExpressions;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class DateTime : ValueRepresentation
    {
        public DateTime(Tag tag) : base("DT", tag)
        {
        }

        public override string ToLongString()
        {
            return "Date Time (DT)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToImproperMultiValue(s);
            var dateTime = new System.DateTime[multiValue.Length];
            for (var i = 0; i < dateTime.Length; i++)
            {
                var item = multiValue[i];
                if (item.Length > 0)
                {
                    if (Regex.IsMatch(item, "^ [0-9]{10}" +
                                            "([0-9]{2} ([0-9]{2} (\\.[0-9]{6}" +
                                            "([\\+\\-][0-9]{4})? )? )? )? $",
                        RegexOptions.IgnorePatternWhitespace))
                    {
                        item = item.Replace(".", null);
                        var year = item.Substring(0, 4);
                        var month = item.Substring(4, 2);
                        var day = item.Substring(6, 2);
                        var hour = "0";
                        if (item.Length > 8) hour = item.Substring(8, 2);
                        var minute = "0";
                        if (item.Length > 10) minute = item.Substring(10, 2);
                        var second = "0";
                        if (item.Length > 12) second = item.Substring(12, 2);
                        var millisecond = "0";
                        if (item.Length > 14)
                            millisecond = item.Substring(14, 6);
                        var timeZone = "+0";
                        if (item.Length > 20)
                            timeZone = item.Substring(20, 5);
                        // TODO: What to do with the time zone?
                        try
                        {
                            dateTime[i] = new System.DateTime(int.Parse(year),
                                int.Parse(month), int.Parse(day), int.Parse(hour),
                                int.Parse(minute), int.Parse(second),
                                int.Parse(millisecond));
                        }
                        catch (Exception)
                        {
                            throw new EncodingException(
                                "Date time format is invalid.", Tag,
                                Name + "/item", item);
                        }
                    }
                    else
                        throw new EncodingException(
                            "Date time format is invalid.", Tag, Name + "/item",
                            item);
                }
            }
            return dateTime;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToProperMultiValue(s);
            var dateTime = new System.DateTime[multiValue.Length];
            for (var i = 0; i < dateTime.Length; i++)
            {
                var item = multiValue[i];
                if (item.Length > 0)
                {
                    if (Regex.IsMatch(item, "^ [0-9]{10}" +
                                            "([0-9]{2} ([0-9]{2} (\\.[0-9]{6}" +
                                            "([\\+\\-][0-9]{4})? )? )? )? $",
                        RegexOptions.IgnorePatternWhitespace))
                    {
                        item = item.Replace(".", null);
                        var year = item.Substring(0, 4);
                        var month = item.Substring(4, 2);
                        var day = item.Substring(6, 2);
                        var hour = "0";
                        if (item.Length > 8) hour = item.Substring(8, 2);
                        var minute = "0";
                        if (item.Length > 10) minute = item.Substring(10, 2);
                        var second = "0";
                        if (item.Length > 12) second = item.Substring(12, 2);
                        var millisecond = "0";
                        if (item.Length > 14)
                            millisecond = item.Substring(14, 6);
                        var timeZone = "+0";
                        if (item.Length > 20)
                            timeZone = item.Substring(20, 5);
                        // TODO: What to do with the time zone?
                        try
                        {
                            dateTime[i] = new System.DateTime(int.Parse(year),
                                int.Parse(month), int.Parse(day), int.Parse(hour),
                                int.Parse(minute), int.Parse(second),
                                int.Parse(millisecond));
                        }
                        catch (Exception)
                        {
                            throw new EncodingException(
                                "Date time format is invalid.", Tag,
                                Name + "/item", item);
                        }
                    }
                    else
                        throw new EncodingException(
                            "Date time format is invalid.", Tag, Name + "/item",
                            item);
                }
            }
            return dateTime;
        }
    }
}