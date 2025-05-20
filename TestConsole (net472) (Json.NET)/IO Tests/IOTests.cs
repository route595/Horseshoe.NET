using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;

namespace TestConsole.IOTests
{
    class IOTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Display file sizes",
                () =>
                {
                    Console.WriteLine("-1 B  =>  " + FileUtil.GetDisplayFileSize(-1));
                    Console.WriteLine("-1 B in KB  =>  " + FileUtil.GetDisplayFileSize(-1, unit: FileSizeUnit.KB));
                    Console.WriteLine("0  =>  " + FileUtil.GetDisplayFileSize(0));
                    Console.WriteLine("0 B in GB  =>  " + FileUtil.GetDisplayFileSize(0, unit: FileSizeUnit.GB));
                    Console.WriteLine("1000000  =>  " + FileUtil.GetDisplayFileSize(1000000));
                    Console.WriteLine("1000000 'bi'  =>  " + FileUtil.GetDisplayFileSize(1000000, bi: true));
                    Console.WriteLine("1000000 in B  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.B));
                    Console.WriteLine("1000000 in B w/o sep  =>  " + FileUtil.GetDisplayFileSize(1000000, addSeparators: false, unit: FileSizeUnit.B));
                    Console.WriteLine("1000000 B in KB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.KB));
                    Console.WriteLine("1000000 B in KiB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.KiB));
                    Console.WriteLine("1000000 B in GB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.GB));
                    Console.WriteLine("1000000 B in GiB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.GiB));
                    Console.WriteLine("1000000 B in GB w/ 3 dec  =>  " + FileUtil.GetDisplayFileSize(1000000, maxDecimalPlaces: 3, unit: FileSizeUnit.GB));
                    Console.WriteLine("1000000 B in GiB w/ 3 dec  =>  " + FileUtil.GetDisplayFileSize(1000000, maxDecimalPlaces: 3, unit: FileSizeUnit.GiB));
                }
            ),
            BuildMenuRoutine
            (
                "Test ChildOf()",
                () =>
                {
                    DirectoryPath dir1 = "C:\\";
                    DirectoryPath dir2 = "C:\\spandex";
                    Console.WriteLine(dir1 + " is child of " + dir2 + "? " + dir1.IsChildOf(dir2));
                    Console.WriteLine(dir2 + " is child of " + dir1 + "? " + dir2.IsChildOf(dir1));
                }
            ),
            BuildMenuRoutine
            (
                "Test ContainsParent()",
                () =>
                {
                    DirectoryPath dir1 = @"C:\dev\packages\simple-green";
                    DirectoryPath dir2 = @"C:\dev\Packages\simple-green";
                    Console.WriteLine(dir1 + " contains parent \"packages\"? " + dir1.ContainsParent("packages"));
                    Console.WriteLine(dir1 + " contains parent \"packages\" (ignore case)? " + dir1.ContainsParent("packages", ignoreCase: true));
                    Console.WriteLine(dir2 + " contains parent \"packages\"? " + dir2.ContainsParent("packages"));
                    Console.WriteLine(dir2 + " contains parent \"packages\" (ignore case)? " + dir2.ContainsParent("packages", ignoreCase: true));
                }
            )
        };

        private Regex BinPath { get; } = new Regex(@"[\\/]bin[\\/]?");

        private DirectoryPath GetBinParent()
        {
            DirectoryPath path = Directory.GetCurrentDirectory();

            Console.WriteLine("Start dir: " + path);
            while (BinPath.IsMatch(path.FullName))
            {
                if (!path.Parent.HasValue)
                    break;
                path = path.Parent.Value;
            }
            Console.WriteLine("Current dir: " + path);
            Console.WriteLine();

            return path;
        }
    }
}
