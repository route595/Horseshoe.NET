using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Ldap
{
    public class Person
    {
        private Dictionary<string, object> Attributes = new Dictionary<string, object>();

        public string[] ObjectClasses => Try(Attributes, PersonAttribute.ObjectClass) as string[];
        public Dn ObjectCategory => Try(Attributes, PersonAttribute.ObjectCategory) as Dn;
        public string ObjectCategoryName => ObjectCategory?.Name;
        public string Name => Try(Attributes, PersonAttribute.Name) as string;
        public string DisplayName => Try(Attributes, PersonAttribute.DisplayName) as string;
        public string UserPrincipalName => Try(Attributes, PersonAttribute.UserPrincipalName) as string;
        public string SamAccountName => Try(Attributes, PersonAttribute.SamAccountName) as string;
        public string FirstName => Try(Attributes, PersonAttribute.GivenName) as string;
        public string MiddleInitial => Try(Attributes, PersonAttribute.Initials) as string;
        public string LastName => Try(Attributes, PersonAttribute.Sn) as string;
        public string Title => Try(Attributes, PersonAttribute.Title) as string;
        public string Description => Try(Attributes, PersonAttribute.Description) as string;
        public string EmailAddress => Try(Attributes, PersonAttribute.Mail) as string;
        public string TelephoneNumber => Try(Attributes, PersonAttribute.TelephoneNumber) as string;
        public string MobileTelephoneNumber => Try(Attributes, PersonAttribute.MobileTelephoneNumber) as string;
        public string Mobile => Try(Attributes, PersonAttribute.Mobile) as string;
        public string Department => Try(Attributes, PersonAttribute.Department) as string;
        public string PhysicalDeliveryOfficeName => Try(Attributes, PersonAttribute.PhysicalDeliveryOfficeName) as string;
        public string StreetAddress => Try(Attributes, PersonAttribute.StreetAddress) as string;
        public string PostalAddress => Try(Attributes, PersonAttribute.PostalAddress) as string;
        public string City => Try(Attributes, PersonAttribute.L) as string;
        public string State => Try(Attributes, PersonAttribute.St) as string;
        public string PostalCode => Try(Attributes, PersonAttribute.PostalCode) as string;
        public string Country => Try(Attributes, PersonAttribute.C) as string;
        //public DateTime? DateCreated => Try(Attributes, PersonAttribute.WhenCreated) as DateTime?;
        public DateTime? LastLogon => Try(Attributes, PersonAttribute.LastLogon) as DateTime?;
        //public DateTime? AccountExpires => Try(Attributes, PersonAttribute.AccountExpires) as DateTime?;
        public string Manager => Try(Attributes, PersonAttribute.Manager) as string;
        public Dn DistinguishedName => Try(Attributes, PersonAttribute.DistinguishedName) as Dn;
        public Dn Ou => DistinguishedName?.Ou;
        public Dn[] Groups => Try(Attributes, PersonAttribute.MemberOf) as Dn[];
        public string[] GroupNames => Groups?.Select(dn => dn.Name).ToArray();

        internal void LoadAttribute(string attrName, object value)
        {
            if (value != null)
                Attributes[attrName] = value;
            else if (Attributes.ContainsKey(attrName))
                Attributes.Remove(attrName);
        }

        public string GetAttributeAsString(string attrName) => Try(Attributes, attrName) as string;
        public string[] GetAttributeAsStringArray(string attrName) => Try(Attributes, attrName) as string[];
        public int? GetAttributeAsInt(string attrName) => Try(Attributes, attrName) as int?;
        public long? GetAttributeAsLong(string attrName) => Try(Attributes, attrName) as long?;
        public double? GetAttributeAsDouble(string attrName) => Try(Attributes, attrName) as double?;
        public decimal? GetAttributeAsDecimal(string attrName) => Try(Attributes, attrName) as decimal?;
        public DateTime? GetAttributeAsDateTime(string attrName) => Try(Attributes, attrName) as DateTime?;
        public byte[] GetAttributeAsByteArray(string attrName) => Try(Attributes, attrName) as byte[];
        public byte[][] GetAttributeAsArrayOfByteArray(string attrName) => Try(Attributes, attrName) as byte[][];

        public override string ToString()
        {
            return DisplayName ?? SamAccountName ?? UserPrincipalName ?? Name ?? "[no name]";
        }

        private static object Try(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out object value))
            {
                return value;
            }
            return null;
        }
    }
}
