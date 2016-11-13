namespace openDicom.Encoding.Type
{
    public sealed class PersonName
    {
        private const int familyNameIndex = 0;
        private const int givenNameIndex = 1;
        private const int middleNameIndex = 2;
        private const int namePrefixIndex = 3;
        private const int nameSuffixIndex = 4;
        private string fullName;

        private readonly string[] innerArray = new string[5]
        {
            null, null, null, null,
            null
        };

        public PersonName()
        {
        }

        public PersonName(string fullName)
        {
            FullName = fullName;
        }

        public PersonName(string familyName, string givenName,
            string middleName, string namePrefix, string nameSuffix)
        {
            FamilyName = familyName;
            GivenName = givenName;
            MiddleName = middleName;
            NamePrefix = namePrefix;
            NameSuffix = nameSuffix;
        }

        public string this[int index]
        {
            set
            {
                if (value == null || value.Length < 64)
                    innerArray[index] = value;
                else
                    throw new DicomException(
                        "Length of new entry exceeds 64 characters.",
                        "PersonName[" + index + "]",
                        value);
            }
            get { return innerArray[index]; }
        }

        public string FamilyName
        {
            set
            {
                if (value == null || value.Length < 64)
                {
                    fullName = null;
                    innerArray[familyNameIndex] = value;
                }
                else
                    throw new DicomException(
                        "Length of family name exceeds 64 characters.",
                        "PersonName.FamilyName", value);
            }
            get { return innerArray[familyNameIndex]; }
        }

        public string GivenName
        {
            set
            {
                if (value == null || value.Length < 64)
                {
                    fullName = null;
                    innerArray[givenNameIndex] = value;
                }
                else
                    throw new DicomException(
                        "Length of given name exceeds 64 characters.",
                        "PersonName.GivenName", value);
            }
            get { return innerArray[givenNameIndex]; }
        }

        public string MiddleName
        {
            set
            {
                if (value == null || value.Length < 64)
                {
                    fullName = null;
                    innerArray[middleNameIndex] = value;
                }
                else
                    throw new DicomException(
                        "Length of middle name exceeds 64 characters.",
                        "PersonName.MiddleName", value);
            }
            get { return innerArray[middleNameIndex]; }
        }

        public string NamePrefix
        {
            set
            {
                if (value == null || value.Length < 64)
                {
                    fullName = null;
                    innerArray[namePrefixIndex] = value;
                }
                else
                    throw new DicomException(
                        "Length of name prefix exceeds 64 characters.",
                        "PersonName.NamePrefix", value);
            }
            get { return innerArray[namePrefixIndex]; }
        }

        public string NameSuffix
        {
            set
            {
                if (value == null || value.Length < 64)
                {
                    fullName = null;
                    innerArray[nameSuffixIndex] = value;
                }
                else
                    throw new DicomException(
                        "Length of name suffix exceeds 64 characters.",
                        "PersonName.NameSuffix", value);
            }
            get { return innerArray[nameSuffixIndex]; }
        }

        public string FullName
        {
            set
            {
                fullName = value;
                if (fullName != null)
                {
                    var s = fullName.Split('^');
                    int i;
                    for (i = 0; i < s.Length; i++)
                    {
                        if (s[i].Length < 64)
                            innerArray[i] = s[i];
                        else
                            throw new DicomException(
                                "Length of new entry exceeds 64 characters.",
                                "PersonName.FullName/s[ + " + i + "]",
                                s[i]);
                    }
                    for (var k = i; k < innerArray.Length; k++)
                        innerArray[k] = null;
                }
            }
            get
            {
                if (fullName == null)
                {
                    fullName = innerArray[0];
                    var isNotNull = fullName != null;
                    var i = 1;
                    while (isNotNull && i < innerArray.Length)
                    {
                        isNotNull = innerArray[i] != null;
                        if (isNotNull)
                            fullName += "^" + innerArray[i];
                        i++;
                    }
                }
                return fullName;
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}