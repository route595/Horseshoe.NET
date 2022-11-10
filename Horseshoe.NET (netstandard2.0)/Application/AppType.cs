namespace Horseshoe.NET.Application
{
    public enum AppType
    {
        Web,
        Mvc,
        WindowsDesktop,
        Wpf,
        WinForms,
        Console,
        Xamarin,
        Assembly,
        Sql,     // reserved for SQL (e.g. stored procedures) likely never used directly in .NET projects
        Test
    }
}
