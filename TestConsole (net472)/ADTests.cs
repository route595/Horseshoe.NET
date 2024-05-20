using System;
using System.Collections.Generic;

using Horseshoe.NET;
using Horseshoe.NET.ActiveDirectory;
using Horseshoe.NET.ConsoleX;

namespace TestConsole
{
    class ADTests : RoutineX
    {
        public override Title MenuTitle => new Title(base.MenuTitle, xtra: " - Hello " + UserName);

        public string UserName { get; set; } = Environment.UserName;

        public override Action<MenuSelection<MenuObject>> OnMenuSelection => (selection) => 
        {
            if (selection.SelectedItem != null)
            {
                Console.WriteLine(selection.SelectedItem.Text + " - [" + UserName + "]");
                Console.WriteLine();
            }
        };

        public override IList<MenuObject> Menu => new MenuObject[]
        {
            new MenuHeader("USER ROUTINES"),
            BuildMenuRoutine
            (
                "Change user",
                () =>
                {
                    UserName = Zap.String(PromptX.Value<string>("New user name (leave blank for current user)")) ?? Environment.UserName;
                }
            ),
            BuildMenuRoutine
            (
                "Who am I? (+department, +extensionattribute1)",
                () =>
                {
                    var info = ADUtil.GetUser(UserName, propertiesToLoad: ADConstants.UserProperties.Default + "|department|extensionattribute1");
                    Console.WriteLine(info.DisplayName + " -- " + info.Email + " -- Dept. = " + info.GetStringProperty("department") + " -- Extn 1 = " + info.GetStringProperty("extensionattribute1"));
                }
            ),
            BuildMenuRoutine
            (
                "What is my OU?",
                () =>
                {
                    var info = ADUtil.GetUser(UserName);
                    Console.WriteLine("OU = " + info.OU);
                    Console.WriteLine("OU Path = " + info.RawOU);
                }
            ),
            BuildMenuRoutine
            (
                "Who are the users in my OU?",
                () =>
                {
                    var info = ADUtil.GetUser(UserName);
                    Console.WriteLine("OU = " + info.OU);
                    Console.WriteLine("OU Path = " + info.RawOU);
                    Console.WriteLine();
                    Console.WriteLine("Listing users in OU...");
                    //RenderX.List(ADUtil.ListUsersByOU(info.OU));
                }
            ),
            BuildMenuRoutine
            (
                "Authenticate",
                () =>
                {
                    var passWord = PromptX.Password("Enter the password for " + UserName);
                    ADUser user;
                    try
                    {
                        user = ADUtil.Authenticate(UserName, passWord);
                        if (user != null)
                        {
                            Console.WriteLine("Authenticated.");
                        }
                        else
                        {
                            Console.WriteLine("Incorrect user name or password.");
                        }
                    }
                    catch (Exception ex)
                    {
                        RenderX.Exception(ex);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Display Group Membership",
                () =>
                {
                    var user = ADUtil.GetUser(UserName, propertiesToLoad: ADConstants.UserProperties.DefaultIncludingGroups);
                    if (user != null)
                    {
                        Console.WriteLine("Listing...");
                        Console.WriteLine();
                        RenderX.List(user.GetGroups(orderByAdsPath: true), title: new Title("Group Membership", " (for " + UserName + ")"));
                    }
                    else
                    {
                        RenderX.Alert("User not found");
                    }
                }
            ),            
            new MenuHeader("DOMAIN ROUTINES"),
            BuildMenuRoutine
            (
                "What is my domain controller?",
                () =>
                {
                    var dc = ADEngine.GetDomainContext().ConnectedServer;
                    Console.WriteLine(dc);
                }
            ),
            BuildMenuRoutine
            (
                "List OUs",
                () =>
                {
                    RenderX.List(ADUtil.ListOUs());
                }
            ),
            //BuildMenuRoutine
            //(
            //    "List OUs (recursively)",
            //    () =>
            //    {
            //        RenderX.List(ADUtil.ListOUs(recursive: true));
            //    }
            //)
        };
    }
}
