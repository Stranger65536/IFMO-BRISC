using System;
using System.Text.RegularExpressions;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class Time : ValueRepresentation
    {
        public Time(Tag tag) : base("TM", tag)
        {
        }

        public override string ToLongString()
        {
            return "Time (TM)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToImproperMultiValue(s);
            var time = new TimeSpan[multiValue.Length];
            for (var i = 0; i < time.Length; i++)
            {
                var item = multiValue[i];
                if (item.Length > 0)
                {
                    item = item.TrimEnd(null);
                    if (Regex.IsMatch(item,
                        "^[0-9]{2}(:?[0-9]{2}(:?[0-9]{2}(\\.[0-9]{1,6})?)?)?$"))
                    {
                        item = item.Replace(":", null).Replace(".", null);
                        var hour = item.Substring(0, 2);
                        var minute = "0";
                        if (item.Length > 2) minute = item.Substring(2, 2);
                        var second = "0";
                        if (item.Length > 4) second = item.Substring(4, 2);
                        var millisecond = "0";
                        if (item.Length > 6)
                            millisecond = item.Substring(6, item.Length - 6);
                        try
                        {
                            time[i] = new TimeSpan(0, int.Parse(hour),
                                int.Parse(minute), int.Parse(second),
                                int.Parse(millisecond));
                        }
                        catch (Exception)
                        {
                            throw new EncodingException(
                                "Time format is invalid.",
                                Tag, Name + "/item", item);
                        }
                    }
                    else
                        throw new EncodingException("Time format is invalid.",
                            Tag, Name + "/item", item);
                }
            }
            return time;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var multiValue = ToProperMultiValue(s);
            var time = new TimeSpan[multiValue.Length];
            for (var i = 0; i < time.Length; i++)
            {
                var item = multiValue[i];
                if (item.Length > 0)
                {
                    item = item.TrimEnd(null);
                    if (Regex.IsMatch(item,
                        "^[0-9]{2}(:?[0-9]{2}(:?[0-9]{2}(\\.[0-9]{1,6})?)?)?$"))
                    {
                        item = item.Replace(":", null).Replace(".", null);
                        var hour = item.Substring(0, 2);
                        var minute = "0";
                        if (item.Length > 2) minute = item.Substring(2, 2);
                        var second = "0";
                        if (item.Length > 4) second = item.Substring(4, 2);
                        var millisecond = "0";
                        if (item.Length > 6)
                            millisecond = item.Substring(6, item.Length - 6);
                        try
                        {
                            time[i] = new TimeSpan(0, int.Parse(hour),
                                int.Parse(minute), int.Parse(second),
                                int.Parse(millisecond));
                        }
                        catch (Exception)
                        {
                            throw new EncodingException(
                                "Time format is invalid.",
                                Tag, Name + "/item", item);
                        }
                    }
                    else
                        throw new EncodingException("Time format is invalid.",
                            Tag, Name + "/item", item);
                }
            }
            return time;
        }
    }
}