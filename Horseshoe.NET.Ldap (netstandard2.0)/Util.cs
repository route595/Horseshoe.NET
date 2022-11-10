using System;
using System.Linq;
using System.Text.RegularExpressions;

using Novell.Directory.Ldap;

namespace Horseshoe.NET.Ldap
{
    public static class Util
    {
        public static Person ParseUser(LdapEntry ldapEntry, string[] attrFilter, bool isActiveDirectory = false)
        {
            var dn = tryGetDn(ldapEntry, "distinguishedName") ?? Dn.Parse(ldapEntry.Dn);
            if (filterOutDn(dn))
            {
                return null;
            }

            // person attributes
            PersonAttribute[] attributes;
            if (attrFilter == null)
            {
                attributes = PersonAttribute.GetAllBuiltIn();
            }
            else
            {
                attrFilter = attrFilter.Select(s => s.ToLower()).ToArray();
                attributes = PersonAttribute.GetAllBuiltIn()
                    .Where(pa => attrFilter.Contains(pa.AttrName.ToLower()))
                    .ToArray();
            }

            // build person object
            var user = new Person();
            foreach (var attr in attributes)
            {
                switch (attr.AttrType)
                {
                    case LdapAttributeType.String:
                    default:
                        user.LoadAttribute(attr.AttrName, tryGetString(ldapEntry, attr.AttrName));
                        break;
                    case LdapAttributeType.StringArray:
                        user.LoadAttribute(attr.AttrName, tryGetStringArray(ldapEntry, attr.AttrName));
                        break;
                    case LdapAttributeType.Int:
                        user.LoadAttribute(attr.AttrName, Zap.NInt(tryGetString(ldapEntry, attr.AttrName)));
                        break;
                    case LdapAttributeType.Long:
                        user.LoadAttribute(attr.AttrName, Zap.NLong(tryGetString(ldapEntry, attr.AttrName)));
                        break;
                    case LdapAttributeType.Double:
                        user.LoadAttribute(attr.AttrName, Zap.NDouble(tryGetString(ldapEntry, attr.AttrName)));
                        break;
                    case LdapAttributeType.Decimal:
                        user.LoadAttribute(attr.AttrName, Zap.NDecimal(tryGetString(ldapEntry, attr.AttrName)));
                        break;
                    case LdapAttributeType.DateTime:
                        if (isActiveDirectory)
                        {
                            user.LoadAttribute(attr.AttrName, tryGetDateTimeTicksFrom1601(ldapEntry, attr.AttrName));
                        }
                        else
                        {
                            user.LoadAttribute(attr.AttrName, Zap.NDateTime(tryGetString(ldapEntry, attr.AttrName)));
                        }
                        break;
                    case LdapAttributeType.Dn:
                        user.LoadAttribute(attr.AttrName, tryGetDn(ldapEntry, attr.AttrName));
                        break;
                    case LdapAttributeType.DnArray:
                        user.LoadAttribute(attr.AttrName, tryGetDnArray(ldapEntry, attr.AttrName));
                        break;
                    case LdapAttributeType.ByteArray:
                        user.LoadAttribute(attr.AttrName, tryGetByteArray(ldapEntry, attr.AttrName));
                        break;
                    case LdapAttributeType.ArrayOfByteArray:
                        user.LoadAttribute(attr.AttrName, tryGetArrayOfByteArray(ldapEntry, attr.AttrName));
                        break;
                }
            }
            if (filterOutPerson(user))
            {
                return null;
            }
            return user;
        }

        static string tryGetString(LdapEntry ldapEntry, string attrName)
        {
            try
            {
                return ldapEntry.GetAttribute(attrName).StringValue.Trim();
            }
            catch
            {
                return null;
            }
        }

        static string[] tryGetStringArray(LdapEntry ldapEntry, string attrName)
        {
            try
            {
                return ldapEntry.GetAttribute(attrName).StringValueArray;
            }
            catch
            {
                return null;
            }
        }

        static DateTime? tryGetDateTimeTicksFrom1601(LdapEntry ldapEntry, string attrName)
        {
            var longValue = Zap.NLong(tryGetString(ldapEntry, attrName));
            if (longValue.HasValue)
            {
                var dateTime = new DateTime(1601, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddTicks(longValue.Value).ToLocalTime();
                return dateTime;
            }
            return null;
        }

        static Dn tryGetDn(LdapEntry ldapEntry, string attrName)
        {
            var value = tryGetString(ldapEntry, attrName);
            if (value != null)
            {
                return Dn.Parse(value);
            }
            return null;
        }

        static Dn[] tryGetDnArray(LdapEntry ldapEntry, string attrName)
        {
            var values = tryGetStringArray(ldapEntry, attrName);
            if (values != null)
            {
                return values
                    .Select(s => Dn.Parse(s))
                    .OrderBy(dn => dn.Name)
                    .ToArray();
            }
            return null;
        }

        static byte[] tryGetByteArray(LdapEntry ldapEntry, string attrName)
        {
            try
            {
                return ldapEntry.GetAttribute(attrName).ByteValue;
            }
            catch
            {
                return null;
            }
        }

        static byte[][] tryGetArrayOfByteArray(LdapEntry ldapEntry, string attrName)
        {
            try
            {
                return ldapEntry.GetAttribute(attrName).ByteValueArray;
            }
            catch
            {
                return null;
            }
        }

        static bool filterOutDn(Dn dn)
        {
            foreach (var filter in Settings.BlacklistedDnNamePatterns ?? Enumerable.Empty<Regex>())
            {
                if (filter.IsMatch(dn.Name))
                {
                    return true;
                }
            }
            foreach (var blacklistedName in Settings.BlacklistedDnNames ?? Enumerable.Empty<string>())
            {
                if (string.Equals(blacklistedName, dn.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        static bool filterOutPerson(Person person)
        {
            foreach (var func in Settings.BlacklistedPersonFunctions ?? Enumerable.Empty<Func<Person,bool>>())
            {
                if (func.Invoke(person))
                {
                    return true;
                }
            }
            foreach (var filter in Settings.BlacklistedDnNamePatterns ?? Enumerable.Empty<Regex>())
            {
                if (filter.IsMatch(person.SamAccountName))
                {
                    return true;
                }
            }
            foreach (var blacklistedName in Settings.BlacklistedDnNames ?? Enumerable.Empty<string>())
            {
                if (string.Equals(blacklistedName, person.SamAccountName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
