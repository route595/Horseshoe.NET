using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.DirectoryCrawler;
using Horseshoe.NET.Text;
using Horseshoe.NET.XmlDoc;

namespace TestConsole
{
    class XmlDocTests : RoutineX
    {
        private XmlDoc XmlDoc { get; set; } = new XmlDoc();

        public override IList<MenuObject> Menu => new MenuObject[]
        {
            new MenuHeader("USER ROUTINES"),
            BuildMenuRoutine
            (
                "Parse documentation XML",
                () =>
                {
                    XmlDoc.Fill
                    (
                        "Horseshoe.NET.xml",
                        fillInMissingTypes: true,
                        journal: new TraceJournal((level, entry) => Console.WriteLine((level >= 0 ? new string(' ', level * 2) : "<") + entry))
                    );
                    Console.WriteLine();
                    Console.WriteLine("Namespaces");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, XmlDoc.GetDocumentedNamespaces()));
                    Console.WriteLine();
                    Console.WriteLine("Verified Types");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, XmlDoc.VerifiedTypes.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key + " -> " + kvp.Value.FullName)));
                    Console.WriteLine();
                    Console.WriteLine("Verified Namespaces");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, XmlDoc.VerifiedNamespaces));
                }
            ),
            BuildMenuRoutine
            (
                "Render HTML docs",
                () =>
                {
                    new DirectoryCrawler
                    (
                        @"C:\Users\E029791\OneDrive - lge-ku.com\Desktop\horseshoe.net\docs",
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.OnInit:
                                    Console.WriteLine();
                                    Console.WriteLine("scanning: " + dir);
                                    break;
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine(new string(' ', metadata.Level) + "entered: " + dir);
                                    break;
                                case DirectoryCrawlEvent.DirectorySkipped:
                                    Console.WriteLine(new string(' ', metadata.Level) + "skipped: " + dir);
                                    Console.WriteLine(new string(' ', metadata.Level) + "skipped: " + metadata.SkipReason);
                                    break;
                                case DirectoryCrawlEvent.OnHalt:
                                    Console.WriteLine();
                                    Console.WriteLine("Halted!");
                                    if (metadata.Exception != null)
                                        Console.WriteLine(metadata.Exception.RenderMessage());
                                    break;
                                case DirectoryCrawlEvent.OnComplete:
                                    Console.WriteLine();
                                    Console.WriteLine("Statistics...");
                                    Console.WriteLine(string.Join(Environment.NewLine, metadata.Statistics.Display().Select(s => "  " + s)));
                                    break;
                            }
                        },
                        fileCrawled: (@event, file, metadata) =>
                        {
                            switch (@event)
                            {
                                case FileCrawlEvent.FileFound:
                                    Console.WriteLine(new string(' ', metadata.Level) + "found: " + file);
                                    break;
                                case FileCrawlEvent.FileSkipped:
                                    Console.WriteLine(new string(' ', metadata.Level) + "skipped: " + file);
                                    Console.WriteLine(new string(' ', metadata.Level) + "skipped: " + metadata.SkipReason + ": " + TextUtil.Reveal(metadata.SkipComment));
                                    break;
                            }
                        }
                    ).Go();
                    new RecursiveDelete
                    (
                        @"C:\Users\E029791\OneDrive - lge-ku.com\Desktop\horseshoe.net\docs",
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.OnInit:
                                    Console.WriteLine();
                                    Console.WriteLine("deleting: " + dir);
                                    break;
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine(new string(' ', metadata.Level) + "entered: " + dir);
                                    break;
                                case DirectoryCrawlEvent.DirectorySkipped:
                                    Console.WriteLine(new string(' ', metadata.Level) + "skipped: " + dir);
                                    Console.WriteLine(new string(' ', metadata.Level) + "skipped: " + metadata.SkipReason);
                                    break;
                                case DirectoryCrawlEvent.OnHalt:
                                    Console.WriteLine();
                                    Console.WriteLine("Halted!");
                                    if (metadata.Exception != null)
                                        Console.WriteLine(metadata.Exception.RenderMessage());
                                    break;
                                case DirectoryCrawlEvent.OnComplete:
                                    Console.WriteLine();
                                    Console.WriteLine("Statistics...");
                                    Console.WriteLine(string.Join(Environment.NewLine, metadata.Statistics.Display().Select(s => "  " + s)));
                                    break;
                            }
                        },
                        fileCrawled: (@event, file, metadata) =>
                        {
                            switch (@event)
                            {
                                case FileCrawlEvent.FileSkipped:
                                    Console.WriteLine(new string(' ', metadata.Level) + "skipped: " + file);
                                    Console.WriteLine(new string(' ', metadata.Level) + "skipped: " + metadata.SkipReason + ": " + TextUtil.Reveal(metadata.SkipComment));
                                    break;
                            }
                        },
                        deletingFile: (file, metadata) =>
                        {
                            Console.WriteLine(new string(' ', metadata.Level) + "deleted: " + file + " (" + FileUtil.GetDisplayFileSize(file) + ")");
                        },
                        directoryDeleted: (dir, metadata) =>
                        {
                            Console.WriteLine(new string(' ', metadata.Level) + "deleted: " + dir + "    (dir)");
                        },
                        precludeRootDirectory: true
                    ).Go();

                    //foreach (var member in XmlDoc.Members)
                    //{
                    //    if (!string.IsNullOrEmpty(member.MemberType.Namespace))
                    //    {
                    //        System.IO.Directory.CreateDirectory(member.MemberType.Namespace.ToLower());
                    //    }
                    //    var strb = new StringBuilder("<!DOCTYPE html>")
                    //        .AppendLine("<html>")
                    //        .AppendLine("<head>")
                    //        .AppendLine("<title>" + member.Name + " - Horseshoe.NET docs</title>")
                    //        .AppendLine("</head>")
                    //        .AppendLine("<body>")
                    //        .AppendLine("</body>")
                    //        .AppendLine("</html>");
                    //}
                }
            )
        };
    }
}