using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.ActiveDirectory
{
    public class OUInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string[] PseudoPath { get; set; }
        public string DisplayPseudoPath => PseudoPath != null
            ? string.Join(" > ", PseudoPath)
            : "";

        public override string ToString()
        {
            return DisplayPseudoPath;
        }
    }
}
