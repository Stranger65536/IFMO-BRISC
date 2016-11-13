using System.Text.RegularExpressions;

namespace openDicom.Encoding.Type
{
    public enum AgeContext
    {
        Days,
        Weeks,
        Months,
        Years
    }

    public sealed class Age
    {
        private const char days = 'D';
        private const char weeks = 'W';
        private const char months = 'M';
        private const char years = 'Y';
        private int ageValue;

        public Age(string ageString)
        {
            if (Regex.IsMatch(ageString,
                "^[0-9]{3}[" + days + weeks + months + years + "]$"))
            {
                AgeValue = int.Parse(ageString.Substring(0, 3));
                var context = ageString[3];
                switch (context)
                {
                    case days:
                        IsDays = true;
                        break;
                    case weeks:
                        IsWeeks = true;
                        break;
                    case months:
                        IsMonths = true;
                        break;
                    case years:
                        IsYears = true;
                        break;
                    default:
                        throw new DicomException("Age context is invalid.",
                            "ageString", ageString);
                }
            }
            else
                throw new DicomException("Age string is invalid.", "ageString",
                    ageString);
        }

        public Age(int ageValue, AgeContext context)
        {
            Context = context;
            AgeValue = ageValue;
        }

        public Age(int ageValue, char context)
        {
            switch (context)
            {
                case days:
                    IsDays = true;
                    break;
                case weeks:
                    IsWeeks = true;
                    break;
                case months:
                    IsMonths = true;
                    break;
                case years:
                    IsYears = true;
                    break;
                default:
                    throw new DicomException("Age context is invalid.",
                        "context", context.ToString());
            }
            AgeValue = ageValue;
        }

        public AgeContext Context { get; private set; } = AgeContext.Days;

        public bool IsDays
        {
            set
            {
                if (value) Context = AgeContext.Days;
            }
            get { return Context == AgeContext.Days; }
        }

        public bool IsWeeks
        {
            set
            {
                if (value) Context = AgeContext.Weeks;
            }
            get { return Context == AgeContext.Weeks; }
        }

        public bool IsMonths
        {
            set
            {
                if (value) Context = AgeContext.Months;
            }
            get { return Context == AgeContext.Months; }
        }

        public bool IsYears
        {
            set
            {
                if (value) Context = AgeContext.Years;
            }
            get { return Context == AgeContext.Years; }
        }

        public int AgeValue
        {
            set
            {
                if (value >= 0)
                    ageValue = value;
                else
                    throw new DicomException("Age cannot be negativ.",
                        "Age.AgeValue", value.ToString());
            }
            get { return ageValue; }
        }

        public override string ToString()
        {
            var charContext = ' ';
            switch (Context)
            {
                case AgeContext.Days:
                    charContext = days;
                    break;
                case AgeContext.Weeks:
                    charContext = weeks;
                    break;
                case AgeContext.Months:
                    charContext = months;
                    break;
                case AgeContext.Years:
                    charContext = years;
                    break;
            }
            return string.Format("{0:D3}", AgeValue) + charContext;
        }
    }
}