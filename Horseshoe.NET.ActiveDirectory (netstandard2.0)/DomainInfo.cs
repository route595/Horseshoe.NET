using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.ActiveDirectory
{
    public class DomainInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public DomainInfo(string name, string path) 
        { 
            Name = name ?? throw new ValidationException("name cannot be null");
            Path = path;
        }

        public override string ToString()
        {
            return Name + (Path != null ? " (" + Path + ")" : "");
        }

        public static DomainInfo Default { get; } = new DomainInfo("default", null);
    }
}
