using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.ActiveDirectory
{
    public class ADUser : ADObject
    {
        public string UserPrincipalName
        {
            get => GetStringProperty(ADConstants.UserProperties.userprincipalname);
            set => SetProperty(ADConstants.UserProperties.userprincipalname, value);
        }

        public string FirstName
        {
            get => GetStringProperty(ADConstants.UserProperties.givenname);
            set => SetProperty(ADConstants.UserProperties.givenname, value);
        }

        public string Initial
        {
            get => GetStringProperty(ADConstants.UserProperties.initial);
            set => SetProperty(ADConstants.UserProperties.initial, value);
        }

        public string LastName
        {
            get => GetStringProperty(ADConstants.UserProperties.sn);
            set => SetProperty(ADConstants.UserProperties.sn, value);
        }

        public string DisplayName
        {
            get => GetStringProperty(ADConstants.UserProperties.displayname);
            set => SetProperty(ADConstants.UserProperties.displayname, value);
        }

        public string Email
        {
            get => GetStringProperty(ADConstants.UserProperties.mail);
            set => SetProperty(ADConstants.UserProperties.mail, value);
        }

        public int? GroupCount => GetObjectArrayProperty(ADConstants.UserProperties.memberof)?.Length;

        public ADUser(IDictionary<string, object> data) : base(data)
        {
        }

        public ADUser() : base()
        {
        }

        /// <summary>
        /// Returns the list of groups returned by the LDAP query for this user.
        /// </summary>
        /// <param name="orderByAdsPath">If <c>true</c>, results will be sorted internally based on ADS path.  Default  is <c>false</c>.</param>
        /// <param name="descending">If <c>true</c>, ADS path ordering will be in descending order.  Default is <c>false</c>.</param>
        /// <param name="returnNullCollection">If <c>true</c>, method will return <c>null</c> (instead of an empty collection) if the member array returned by the LDAP query was null. Default is <c>false</c>.</param>
        /// <returns>A collection of type <c>ADMember</c>.</returns>
        public IEnumerable<ADMember> GetGroups(bool orderByAdsPath = false, bool descending = false, bool returnNullCollection = false)
        {
            IEnumerable<string> strings = GetObjectArrayPropertyAsStrings(ADConstants.UserProperties.memberof);
            if (strings == null)
            {
                return returnNullCollection
                    ? null
                    : Enumerable.Empty<ADMember>();
            }
            if (orderByAdsPath || descending)
            {
                strings = descending
                    ? strings.OrderByDescending(s => s)
                    : strings.OrderBy(s => s);
            }
            return strings.Select(ADMember.FromAdsPath);
        }

        public override string ToString()
        {
            int? groupCount = GroupCount;
            return "AD User: " +
                   (DisplayName ?? CN) +
                   ((UserPrincipalName ?? Email) != null ? " (" + (UserPrincipalName ?? Email) + ")" : "") +
                   " " + SAMAccountName +
                   (groupCount.HasValue ? " (" + groupCount + " groups)" : "");
        }
    }
}