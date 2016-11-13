using System.Text.RegularExpressions;
using openDicom.DataStructure;

namespace openDicom.Registry
{
    public sealed class ValueMultiplicity
    {
        private string vm = "0";
        private readonly ValueRepresentation vr;

        public ValueMultiplicity(ValueRepresentation vr)
        {
            this.vr = vr;
        }

        public ValueMultiplicity(ValueRepresentation vr, int vm) :
            this(vr, vm.ToString())
        {
        }

        public ValueMultiplicity(ValueRepresentation vr, string vm) : this(vr)
        {
            Value = vm;
        }

        public ValueRepresentation VR
        {
            get
            {
                if (vr != null)
                    return vr;
                throw new DicomException("ValueMultiplicity.VR is null.");
            }
        }

        public string Value
        {
            set
            {
                if (value == null || value.Equals("")) value = "0";
                value.ToLower();
                value = value.Replace(" ", null);
                if (Regex.IsMatch(value,
                    "^ (([0-9]*n | [0-9]+n?) | [0-9]+ - ([0-9]*n | [0-9]+n?)) $",
                    RegexOptions.IgnorePatternWhitespace))
                {
                    vm = value;
                    var s = vm.Split('-');
                    if (s.Length == 2)
                    {
                        LowerFactor = int.Parse(s[0]);
                        IsUnbounded = Regex.IsMatch(s[1], "^[0-9]*n$");
                        if (IsUnbounded)
                        {
                            s[1] = s[1].Replace("n", null);
                            if (s[1].Equals("")) s[1] = "1";
                        }
                        UpperFactor = int.Parse(s[1]);
                    }
                    else
                    {
                        IsUnbounded = Regex.IsMatch(s[0], "^[0-9]*n$");
                        if (IsUnbounded)
                        {
                            s[0] = s[0].Replace("n", null);
                            if (s[0].Equals("")) s[0] = "1";
                        }
                        LowerFactor = int.Parse(s[0]);
                        UpperFactor = LowerFactor;
                    }
                }
                else
                    throw new DicomException("VM is invalid.", "VM", value);
            }
            get { return vm; }
        }

        public int LowerFactor { get; private set; }
        public int UpperFactor { get; private set; }
        public bool IsUnbounded { get; private set; }

        public bool IsUndefined
        {
            get { return LowerFactor == 0 || UpperFactor == 0; }
        }

        public bool IsValid(int valueCount)
        {
            if (IsUndefined && valueCount >= 1)
                return true;
            if (valueCount >= LowerFactor)
            {
                if (IsUnbounded)
                {
                    if (UpperFactor > 1)
                    {
                        return valueCount%UpperFactor == 0;
                    }
                    return true;
                }
                return valueCount <= UpperFactor;
            }
            return false;
        }

        public bool Equals(int valueCount)
        {
            return !IsUndefined && !IsUnbounded &&
                   LowerFactor == UpperFactor && valueCount == LowerFactor;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}