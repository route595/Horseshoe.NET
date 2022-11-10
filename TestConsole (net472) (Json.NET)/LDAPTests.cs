using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET;
using Horseshoe.NET.Ldap;
using Horseshoe.NET.ConsoleX;

namespace TestConsole
{
    class LDAPTests : RoutineX
    {
        static LDAPTests()
        {
            Settings.BlacklistedDnNamePatterns = new[]
            {
                new Regex("APP[0-9]+", RegexOptions.IgnoreCase),
                new Regex("S[0-9]+", RegexOptions.IgnoreCase),
                new Regex("FTP[0-9]+", RegexOptions.IgnoreCase),
                new Regex("[$]"),
                new Regex(".+ \\(((Disabled User)|(Domain Admin)|(Smart Phone)|(Smartphone)|(Cell))\\)"),
                new Regex("(SAPService).+"),
            };
            Settings.BlacklistedDnNames = new[]
            {
                "LGE-KUAdministrator",
                "Guest",
                "SharePointEnterprise-ApplicationAccount",
                "SystemMailbox{bb558c35-97f1-4cb9-8ff7-d53741dc928c}",
                "SystemMailbox{1f05a927-3634-4345-b7a0-9b3612ef4d44}",
                "krbtgt",
            };
            Settings.BlacklistedPersonFunctions = new Func<Person,bool>[]
            {
                (person) => person.ObjectClasses.Any(oc => oc == "computer"),
                (person) => person.SamAccountName == null,
            };
        }

        public override Title MenuTitle => "LDAP Tests";

        private Credential? CachedCredentials;

        private Person CachedPerson;

        public override Action<Exception> OnError => (ex) => 
        { 
            CachedCredentials = null; 
            RenderX.Exception(ex); 
        };

        public override IList<MenuObject> Menu => new MenuObject[]
        {
            new MenuHeader("USER ROUTINES"),
            BuildMenuRoutine
            (
                "Log in and display person data",
                () =>
                {
                    if (CachedCredentials == null)
                    {
                        var userName = PromptX.Input("User name");
                        var password = PromptX.Password("Password");
                        CachedCredentials = new Credential(userName, password, isEncryptedPassword: false, domain: "lgeenergy.int");
                    }
                    CachedPerson = LdapExec.Login_AD(CachedCredentials.Value.UserName, CachedCredentials.Value.Password, domain: CachedCredentials.Value.Domain);
                    Console.WriteLine("Logged in: " + CachedPerson);
                    Console.WriteLine("OU: " + CachedPerson.Ou);
                    Console.WriteLine("OU (rvrs): " + CachedPerson.Ou.ShortDn.ToReversePathString());
                    Console.WriteLine("Groups: " + string.Join(", ", CachedPerson.GroupNames.Take(2)));
                }
            ),
            BuildMenuRoutine
            (
                "Who are the users in my OU?",
                () =>
                {
                    if (CachedCredentials == null)
                    {
                        var userName = PromptX.Input("User name");
                        var password = PromptX.Password("Password");
                        CachedCredentials = new Credential(userName, password, isEncryptedPassword: false, domain: "lgeenergy.int");
                        CachedPerson = LdapExec.Login_AD(CachedCredentials.Value.UserName, CachedCredentials.Value.Password, domain: CachedCredentials.Value.Domain);
                    }
                    var people = LdapExec.ListPeopleByOu_AD(CachedPerson.Ou, CachedCredentials.Value.UserName, CachedCredentials.Value.Password, domain: CachedCredentials.Value.Domain);
                    int counter = 10;
                    foreach (var person in people)
                    {
                        Console.WriteLine(person);
                        if (--counter == 0)
                        {
                            Console.WriteLine((people.Count() - 10) + " more...");
                            break;
                        }
                    }
                }
            ),
            new MenuHeader("DOMAIN ROUTINES"),
            BuildMenuRoutine
            (
                "List top 10 OUs",
                () =>
                {
                    if (CachedCredentials == null)
                    {
                        var userName = PromptX.Input("User name");
                        var password = PromptX.Password("Password");
                        CachedCredentials = new Credential(userName, password, isEncryptedPassword: false, domain: "lgeenergy.int");
                    }
                    var ous = LdapExec.ListOus(CachedCredentials.Value.UserName, CachedCredentials.Value.Password, domain: CachedCredentials.Value.Domain, ou: null, recursive: false);
                    int counter = 10;
                    foreach (var ou in ous)
                    {
                        Console.WriteLine(ou.ShortDn.ToReversePathString());
                        if (--counter == 0)
                        {
                            Console.WriteLine((ous.Count() - 10) + " more...");
                            break;
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "List top 10 OUs (recursively)",
                () =>
                {
                    if (CachedCredentials == null)
                    {
                        var userName = PromptX.Input("User name");
                        var password = PromptX.Password("Password");
                        CachedCredentials = new Credential(userName, password, isEncryptedPassword: false, domain: "lgeenergy.int");
                    }
                    var ous = LdapExec.ListOus(CachedCredentials.Value.UserName, CachedCredentials.Value.Password, domain: CachedCredentials.Value.Domain, ou: null, recursive: true);
                    int counter = 10;
                    foreach (var ou in ous)
                    {
                        Console.WriteLine(ou.ShortDn.ToReversePathString());
                        if (--counter == 0)
                        {
                            Console.WriteLine((ous.Count() - 10) + " more...");
                            break;
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Search Users",
                () =>
                {
                    if (CachedCredentials == null)
                    {
                        var userName = PromptX.Input("User name");
                        var password = PromptX.Password("Password");
                        CachedCredentials = new Credential(userName, password, isEncryptedPassword: false, domain: "lgeenergy.int");
                    }
                    var searchText = PromptX.Input("Search text");
                    var people = LdapExec.SearchPeople_AD(searchText, CachedCredentials.Value.UserName, CachedCredentials.Value.Password, domain: CachedCredentials.Value.Domain, recursive: true);
                    int counter = 10;
                    foreach (var person in people)
                    {
                        Console.WriteLine(person);
                        if (--counter == 0)
                        {
                            Console.WriteLine((people.Count() - 10) + " more...");
                            break;
                        }
                    }
                }
            )
        };
    }
}
