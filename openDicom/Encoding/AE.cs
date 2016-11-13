using System;
using System.Text.RegularExpressions;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class ApplicationEntity : ValueRepresentation
    {
        public ApplicationEntity(Tag tag) : base("AE", tag)
        {
        }

        public override string ToLongString()
        {
            return "Application Entity (AE)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var applicationName = ToImproperMultiValue(s);
            for (var i = 0; i < applicationName.Length; i++)
            {
                var item = applicationName[0];
                applicationName[i] = item.Trim();
            }
            return applicationName;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            var applicationName = ToProperMultiValue(s);
            for (var i = 0; i < applicationName.Length; i++)
            {
                var item = applicationName[0];
                if (item.Length > 16)
                    throw new EncodingException(
                        "A value of max. 16 bytes is only allowed.", Tag,
                        Name + "/item", item);
                if (Regex.IsMatch(item, "^[\\s]{16}$"))
                    throw new EncodingException(
                        "No application name specified.", Tag, Name + "/item",
                        item);
                if (item.Length > 0)
                    applicationName[i] = item.Trim();
            }
            return applicationName;
        }
    }
}