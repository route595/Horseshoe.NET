using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.ConsoleX.Plugins
{
    public class FileSystemNavigatorOptions
    {
        public string StartDirectory { get; set; }
        public bool DirectoryModeOn { get; set; }
        public bool AllowTraversalOutsideStartDirectory { get; set; }
    }
}
