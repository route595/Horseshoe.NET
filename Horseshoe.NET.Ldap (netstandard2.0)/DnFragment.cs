using System;

namespace Horseshoe.NET.Ldap
{
    public abstract class DnFragment
    {
        public virtual string Name { get; set; }

        public DnFragment(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString()
        {
            return GetType().Name + "=" + Name;
        }

        public class CN : DnFragment { public CN(string name) : base(name) { } }

        public class DC : DnFragment { public DC(string name) : base(name) { } }

        public class OU : DnFragment { public OU(string name) : base(name) { } }

        public static DnFragment Parse(string value, string rawDn)   // DC=mycompany
        {
            var parts = value.Split('=');
            switch (parts[0].Trim().ToUpper())
            {
                case "CN":
                    return new CN(parts[1].Trim());
                case "DC":
                    return new DC(parts[1].Trim());
                case "OU":
                    return new OU(parts[1].Trim());
            }
            throw new Exception("Unrecognized DN fragment: " + parts[0] + " -- Orig string: " + rawDn);
        }
    }
}
