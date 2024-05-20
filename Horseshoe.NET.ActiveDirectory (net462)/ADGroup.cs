using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.ActiveDirectory
{
    public class ADGroup : ADObject
    {
        public int? MemberCount => GetObjectArrayProperty(ADConstants.GroupProperties.member)?.Length;

        public ADGroup()
        {
        }

        public ADGroup(IDictionary<string, object> data) : base(data)
        {
        }

        /// <summary>
        /// Returns the list of members returned by the LDAP query for this group.
        /// </summary>
        /// <param name="orderByAdsPath">If <c>true</c>, results will be sorted internally based on ADS path.  Default  is <c>false</c>.</param>
        /// <param name="descending">If <c>true</c>, ADS path ordering will be in descending order.  Default is <c>false</c>.</param>
        /// <param name="returnNullCollection">If <c>true</c>, method will return <c>null</c> (instead of an empty collection) if the member array returned by the LDAP query was null. Default is <c>false</c>.</param>
        /// <returns>A collection of type <c>ADMember</c>.</returns>
        public IEnumerable<ADMember> GetMembers(bool orderByAdsPath = false, bool descending = false, bool returnNullCollection = false)
        {
            IEnumerable<string> strings = GetObjectArrayPropertyAsStrings(ADConstants.GroupProperties.member);
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
            int? memberCount = MemberCount;
            return "AD Group: " + SAMAccountName + (memberCount.HasValue ? " (" + memberCount + " members)" : "");
        }
    }
}