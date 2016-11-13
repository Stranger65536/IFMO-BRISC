using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public sealed class PersonName : ValueRepresentation
    {
        public PersonName(Tag tag) : base("PN", tag)
        {
        }

        public override string ToLongString()
        {
            return "Person Name (PN)";
        }

        protected override Array DecodeImproper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            s = s.TrimEnd(null);
            var multiValue = ToImproperMultiValue(s);
            var personName = new Type.PersonName[multiValue.Length];
            for (var i = 0; i < personName.Length; i++)
            {
                var item = multiValue[i];
                personName[i] = new Type.PersonName(item);
            }
            return personName;
        }

        protected override Array DecodeProper(byte[] bytes)
        {
            var s = TransferSyntax.ToString(bytes);
            if (s.Length < 64*5)
            {
                s = s.TrimEnd(null);
                var multiValue = ToProperMultiValue(s);
                var personName =
                    new Type.PersonName[multiValue.Length];
                for (var i = 0; i < personName.Length; i++)
                {
                    var item = multiValue[i];
                    personName[i] = new Type.PersonName(item);
                }
                return personName;
            }
            throw new EncodingException(
                "A value of max. 64 * 5 characters is only allowed.",
                Tag, Name + "/s", s);
        }
    }
}