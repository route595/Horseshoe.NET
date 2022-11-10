using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace Horseshoe.NET.ActiveDirectory
{
    public class UserInfo
    {
        /* * * * * * * * * * * * * * * * * * 
         * Scalar Properties
         * * * * * * * * * * * * * * * * * */
        public string SAMAccountName { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public string Department { get; set; }
        public string PhysicalOfficeName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastLogon { get; set; }
        public DateTime? AccountExpires { get; set; }
        public string UserPrincipalName { get; set; }
        public string Manager { get; set; }
        public string ExtensionAttribute1 { get; set; }
        public string ParentPath { get; set; }
        public OUInfo OU { get; set; }
        public IEnumerable<string> GroupNames { get; set; }

        public override string ToString()
        {
            return DisplayName + " OU=" + string.Join(" > ", OU.PseudoPath);
        }
    }
}
