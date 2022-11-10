using System;
using System.Linq;
using System.Reflection;

namespace Horseshoe.NET.Ldap
{
    public class PersonAttribute
    {
        public string AttrName { get; }
        public LdapAttributeType AttrType { get; }
        //public object Value { get; set; }

        public PersonAttribute(string attrName, LdapAttributeType attrType = default)
        {
            AttrName = attrName ?? throw new ArgumentNullException(nameof(attrName));
            AttrType = attrType;
        }

        //public string GetStringValue() => Value as string;
        //public string[] GetStringArrayValue() => Value as string[];
        //public int? GetIntValue() => Value as int?;
        //public long? GetLongValue() => Value as long?;
        //public double? GetDoubleValue() => Value as double?;
        //public decimal? GetDecimalValue() => Value as decimal?;
        //public DateTime? GetDateTimeValue() => Value as DateTime?;
        //public byte[] GetByteArrayValue() => Value as byte[];

        public override string ToString()
        {
            return AttrName + " (" + AttrType + ")";
        }

        public static PersonAttribute Name { get; } = new PersonAttribute("name");
        public static PersonAttribute DisplayName { get; } = new PersonAttribute("displayName");

        /// <summary>
        /// common name, full LDAP path to current object (person)
        /// </summary>
        public static PersonAttribute Cn { get; } = new PersonAttribute("cn", LdapAttributeType.Dn);
        public static PersonAttribute DistinguishedName { get; } = new PersonAttribute("distinguishedName", LdapAttributeType.Dn);
        public static PersonAttribute SamAccountName { get; } = new PersonAttribute("sAMAccountName");
        public static PersonAttribute UserPrincipalName { get; } = new PersonAttribute("userPrincipalName");

        /// <summary>
        /// last name (surname)
        /// </summary>
        public static PersonAttribute Sn { get; } = new PersonAttribute("sn");

        /// <summary>
        /// first name
        /// </summary>
        public static PersonAttribute GivenName { get; } = new PersonAttribute("givenName");

        /// <summary>
        /// middle initial
        /// </summary>
        public static PersonAttribute Initials { get; } = new PersonAttribute("initials");

        /// <summary>
        /// AD groups
        /// </summary>
        public static PersonAttribute MemberOf { get; } = new PersonAttribute("memberOf", LdapAttributeType.DnArray);
        public static PersonAttribute Department { get; } = new PersonAttribute("department");
        public static PersonAttribute PhysicalDeliveryOfficeName { get; } = new PersonAttribute("physicalDeliveryOfficeName");
        public static PersonAttribute Title { get; } = new PersonAttribute("title");
        public static PersonAttribute Description { get; } = new PersonAttribute("description");
        public static PersonAttribute Mail { get; } = new PersonAttribute("mail");
        public static PersonAttribute TelephoneNumber { get; } = new PersonAttribute("telephoneNumber");
        public static PersonAttribute Mobile { get; } = new PersonAttribute("mobile");
        public static PersonAttribute MobileTelephoneNumber { get; } = new PersonAttribute("mobileTelephoneNumber");
        public static PersonAttribute StreetAddress { get; } = new PersonAttribute("streetAddress");
        public static PersonAttribute PostalAddress { get; } = new PersonAttribute("postalAddress");

        /// <summary>
        /// City
        /// </summary>
        public static PersonAttribute L { get; } = new PersonAttribute("l");

        /// <summary>
        /// address state
        /// </summary>
        public static PersonAttribute St { get; } = new PersonAttribute("st");

        /// <summary>
        /// address country 
        /// </summary>
        public static PersonAttribute C { get; } = new PersonAttribute("c");
        public static PersonAttribute PostalCode { get; } = new PersonAttribute("postalCode");
        public static PersonAttribute Manager { get; } = new PersonAttribute("manager");
        public static PersonAttribute LastLogon { get; } = new PersonAttribute("lastLogon", LdapAttributeType.DateTime);
        //public static PersonAttribute AccountExpires { get; } = new PersonAttribute("accountExpires", LdapAttributeType.DateTime);
        //public static PersonAttribute WhenCreated { get; } = new PersonAttribute("whenCreated", LdapAttributeType.DateTime);
        public static PersonAttribute ObjectClass { get; } = new PersonAttribute("objectClass", LdapAttributeType.StringArray);
        public static PersonAttribute ObjectCategory { get; } = new PersonAttribute("objectCategory", LdapAttributeType.Dn);

        public static PersonAttribute[] GetAllBuiltIn()
        {
            return typeof(PersonAttribute).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(PersonAttribute))
                .Select(p => p.GetValue(null) as PersonAttribute)
                .ToArray();
        }

        public static implicit operator string(PersonAttribute personAttribute) => personAttribute.AttrName;
    }
}

