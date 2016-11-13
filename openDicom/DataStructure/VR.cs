using System;
using System.IO;
using System.Text.RegularExpressions;
using openDicom.Encoding;
using DateTime = openDicom.Encoding.DateTime;

namespace openDicom.DataStructure
{
    public class ValueRepresentation : IDicomStreamMember
    {
        private readonly Tag tag;

        public ValueRepresentation(Tag tag) : this((string) null, tag)
        {
        }

        public ValueRepresentation(string vr, Tag tag)
        {
            if (vr == null)
                Name = "";
            else
                Name = vr.Trim().ToUpper();
            if (!(Regex.IsMatch(Name,
                "^(AE|AS|AT|CS|DA|DS|DT|FL|FD|IS|LO|LT|OB|OF|OW|PN|SH|SL|SQ|" +
                "SS|ST|TM|UI|UL|UN|US|UT)$") || Name.Equals("")))
                throw new DicomException(
                    "Value representation is not valid.", "vr", vr);
            this.tag = tag;
        }

        public ValueRepresentation(Stream stream, Tag tag)
        {
            this.tag = tag;
            LoadFrom(stream);
        }

        public string Name { get; private set; } = "";

        public Tag Tag
        {
            get
            {
                if (tag != null)
                    return tag;
                throw new DicomException(
                    "ValueRepresentation.Tag is null.", (Tag) null);
            }
        }

        public bool IsImplicit
        {
            get
            {
                if (IsUndefined)
                {
                    if (Tag.IsUserDefined)
                        // no data dictionary entry exists
                        return TransferSyntax.IsImplicitVR;
                    return true;
                }
                return TransferSyntax.IsImplicitVR;
            }
        }

        public bool IsUndefined
        {
            get { return Name.Equals(""); }
        }

        public bool IsUnknown
        {
            get { return Name.Equals("UN"); }
        }

        public static bool IsStrictDecoded { set; get; } = true;

        public TransferSyntax TransferSyntax
        {
            get { return Tag.TransferSyntax; }
        }

        public long StreamPosition { get; private set; } = -1;

        public void LoadFrom(Stream stream)
        {
            StreamPosition = stream.Position;
            DicomContext.Set(stream, tag);
            if (IsImplicit)
            {
                if (Tag.IsUserDefined)
                    // implicit but unknown value representation
                    Name = "UN";
                else
                // implicit but known value representation;
                // return new instance, dictionary entry do not have a
                // transfer syntax
                    Name = Tag.GetDictionaryEntry().VR.Name;
            }
            else
            {
                // explicit value representation
                var buffer = new byte[2];
                stream.Read(buffer, 0, 2);
                Name = ByteConvert.ToString(buffer,
                    CharacterRepertoire.Default);
            }
            DicomContext.Reset();
        }

        public static ValueRepresentation GetBy(Tag tag)
        {
            return GetBy(null, tag);
        }

        public static ValueRepresentation GetBy(string name, Tag tag)
        {
            switch (name)
            {
                case "AE":
                    return new ApplicationEntity(tag);
                case "AS":
                    return new AgeString(tag);
                case "AT":
                    return new AttributeTag(tag);
                case "CS":
                    return new CodeString(tag);
                case "DA":
                    return new Date(tag);
                case "DS":
                    return new DecimalString(tag);
                case "DT":
                    return new DateTime(tag);
                case "FL":
                    return new FloatingPointSingle(tag);
                case "FD":
                    return new FloatingPointDouble(tag);
                case "IS":
                    return new IntegerString(tag);
                case "LO":
                    return new LongString(tag);
                case "LT":
                    return new LongText(tag);
                case "OB":
                    return new OtherByteString(tag);
                case "OF":
                    return new OtherFloatString(tag);
                case "OW":
                    return new OtherWordString(tag);
                case "PN":
                    return new PersonName(tag);
                case "SH":
                    return new ShortName(tag);
                case "SL":
                    return new SignedLong(tag);
                case "SQ":
                    return new SequenceOfItems(tag);
                case "SS":
                    return new SignedShort(tag);
                case "ST":
                    return new ShortText(tag);
                case "TM":
                    return new Time(tag);
                case "UI":
                    return new UniqueIdentifier(tag);
                case "UL":
                    return new UnsignedLong(tag);
                case "UN":
                    return new Unknown(tag);
                case "US":
                    return new UnsignedShort(tag);
                case "UT":
                    return new UnlimitedText(tag);
                case null:
                case "":
                    return new ValueRepresentation(tag);
                default:
                    throw new DicomException(
                        "Value representation is not valid.", "name", name);
            }
        }

        public static bool IsImplicitBy(Tag tag)
        {
            if (tag.GetDictionaryEntry().VR.IsUndefined)
            {
                if (tag.IsUserDefined)
                    // no data dictionary entry exists
                    return tag.TransferSyntax.IsImplicitVR;
                return true;
            }
            return tag.TransferSyntax.IsImplicitVR;
        }

        public static ValueRepresentation LoadFrom(Stream stream, Tag tag)
        {
            DicomContext.Set(stream, tag);
            if (IsImplicitBy(tag))
            {
                if (tag.IsUserDefined)
                    // implicit but unknown value representation
                    return GetBy("UN", tag);
                return GetBy(tag.GetDictionaryEntry().VR.Name, tag);
            }
            // explicit value representation
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            var name = ByteConvert.ToString(buffer,
                CharacterRepertoire.Default);
            return GetBy(name, tag);
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual string ToLongString()
        {
            if (!Name.Equals(""))
                return "Undefined (" + Name + ")";
            return "Undefined";
        }

        protected byte[][] ToImproperMultiValue(byte[] jointMultiValue,
            int valueLength)
        {
            byte[][] result = null;
            if (jointMultiValue.Length > valueLength)
            {
                var count = (int) Math.Floor((double) jointMultiValue.Length/valueLength);
                result = new byte[count][];
                for (var i = 0; i < count; i++)
                {
                    result[i] = new byte[valueLength];
                    Array.Copy(jointMultiValue, i*valueLength, result[i], 0,
                        valueLength);
                }
            }
            else
            {
                if (jointMultiValue.Length > 0)
                    result = new byte[1][] {jointMultiValue};
                else
                    result = new byte[0][];
            }
            return result;
        }

        protected byte[][] ToProperMultiValue(byte[] jointMultiValue,
            int valueLength)
        {
            byte[][] result = null;
            if (jointMultiValue.Length > valueLength)
            {
                var count = 0;
                if (jointMultiValue.Length%valueLength == 0)
                    count = jointMultiValue.Length/valueLength;
                else
                    throw new EncodingException(
                        "Joint multi value cannot be seperated into single " +
                        "multi values by the specified value length.",
                        Name + "/jointMultiValue.Length, " + Name +
                        "/valueLength", jointMultiValue.Length +
                                        ", " + valueLength);
                if (jointMultiValue.Length > 0 &&
                    !Tag.GetDictionaryEntry().VM.IsValid(count))
                    throw new EncodingException("Count of values is invalid.",
                        Tag, Name + "/VM, " + Name + "/count",
                        Tag.GetDictionaryEntry().VM + ", " +
                        count);
                result = new byte[count][];
                for (var i = 0; i < count; i++)
                {
                    result[i] = new byte[valueLength];
                    Array.Copy(jointMultiValue, i*valueLength, result[i], 0,
                        valueLength);
                }
            }
            else
                result = new byte[1][] {jointMultiValue};
            return result;
        }

        protected byte[] ToJointMultiValue(byte[][] multiValue)
        {
            var jointLength = 0;
            foreach (var value in multiValue)
                jointLength += value.Length;
            var result = new byte[jointLength];
            var resultIndex = 0;
            var multiValueIndex = 0;
            while (multiValueIndex < multiValue.Length)
            {
                var value = multiValue[multiValueIndex];
                Array.Copy(value, 0, result, resultIndex, value.Length);
                resultIndex += value.Length;
                multiValueIndex++;
            }
            if (result.Length == 0 ||
                Tag.GetDictionaryEntry().VM.IsValid(multiValue.Length))
                return result;
            throw new EncodingException("Count of values is invalid.",
                Tag, Name + "/VM, " + Name + "/multiValue.Length",
                Tag.GetDictionaryEntry().VM + ", " +
                multiValue.Length);
        }

        protected string[] ToImproperMultiValue(string jointMultiValue)
        {
            var result = jointMultiValue.Split('\\');
            return result;
        }

        protected string[] ToProperMultiValue(string jointMultiValue)
        {
            var result = jointMultiValue.Split('\\');
            if (jointMultiValue.Length == 0 ||
                Tag.GetDictionaryEntry().VM.IsValid(result.Length))
                return result;
            throw new EncodingException("Count of values is invalid.",
                Tag, Name + "/VM, " + Name + "/result.Length",
                Tag.GetDictionaryEntry().VM + ", " +
                result.Length);
        }

        protected string ToJointMultiValue(string[] multiValue)
        {
            var result = "";
            foreach (var value in multiValue)
            {
                if (result.Equals("")) result = value;
                else result += "\\" + value;
            }
            if (result.Length == 0 ||
                Tag.GetDictionaryEntry().VM.IsValid(multiValue.Length))
                return result;
            throw new EncodingException("Count of values is invalid.",
                Tag, Name + "/VM, " + Name + "/multiValue.Length",
                Tag.GetDictionaryEntry().VM + ", " +
                multiValue.Length);
        }

        protected virtual Array DecodeProper(byte[] bytes)
        {
            return new byte[1][] {bytes};
        }

        protected virtual Array DecodeImproper(byte[] bytes)
        {
            return new byte[1][] {bytes};
        }

        public Array Decode(byte[] bytes)
        {
            if (IsStrictDecoded)
                return DecodeProper(bytes);
            return DecodeImproper(bytes);
        }

        public Value DecodeToValue(byte[] bytes)
        {
            return new Value(this, new ValueLength(this, bytes.Length),
                Decode(bytes));
        }
    }
}