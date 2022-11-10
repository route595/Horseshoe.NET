namespace Horseshoe.NET.Application
{
    public enum AppType
    {
        Assembly,
        Web,
        Mvc,
        WebForms,
        WindowsDesktop,
        Wpf,
        WinForms,
        Console,
        Sql,     // reserved for SQL (e.g. stored procedures) likely never used directly in .NET projects
        Test
    }
}
