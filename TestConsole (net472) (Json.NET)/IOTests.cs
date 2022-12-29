using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;

namespace TestConsole
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
            )
        };
    }
}
