using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.Ldap
{
    public class Dn
    {
        private List<DnFragment> _fragments = new List<DnFragment>();

        public string Name => _fragments.FirstOrDefault()?.Name;
        public Type Type => _fragments.FirstOrDefault()?.GetType();
        public Dn Dc => getDc();
        public Dn Ou => getOu();
        public Dn Parent => getParent();
        public Dn ShortDn => getShortDn();

        public Dn() { }

        public Dn(IEnumerable<DnFragment> fragments) { _fragments.AddRange(fragments); }

        public Dn(string domain) : this(domain.Split('.').Select(s => new DnFragment.DC(s))) { }

        public void Add(DnFragment frag)
        {
            _fragments.Add(frag);
        }

        public void Insert(int index, DnFragment frag)
        {
            _fragments.Insert(index, frag);
        }

        public bool Any()
        {
            return _fragments.Any();
        }

        public DnFragment[] ToArray()
        {
            return _fragments.ToArray();
        }

        public override string ToString()
        {
            return string.Join(",", _fragments);
        }

        public string ToReversePathString()
        {
            var list = new List<string>(_fragments.Select(f => f.Name));
            list.Reverse();
            return string.Join(" > ", list);
        }

        private Dn getDc()
        {
            var dn = new Dn();
            for (int i = _fragments.Count - 1; i >= 0; i--)
            {
                if (_fragments[i] is DnFragment.DC)
                {
                    dn.Insert(0, _fragments[i]);
                }
                else break;
            }
            return dn.Any() ? dn : null;
        }

        private Dn getOu()
        {
            var dn = new Dn();
            foreach (var frag in _fragments)
            {
                if (frag is DnFragment.OU || dn.Any())
                {
                    dn.Add(frag);
                }
            }
            return dn.Any() ? dn : null;
        }

        private Dn getParent()
        {
            var dn = new Dn();
            for (int i = 1; i < _fragments.Count; i++)
            {
                dn.Insert(0, _fragments[i]);
            }
            return dn.Any() ? dn : null;
        }

        private Dn getShortDn()
        {
            var dn = new Dn();
            foreach (var frag in _fragments)
            {
                if (frag.GetType() == Type)
                {
                    dn.Add(frag);
                }
                else break;
            }
            return dn;
        }

        private static Regex fragmentKeyPattern = new Regex("[A-Z]+=", RegexOptions.IgnoreCase);

        public static Dn Parse(string value)  // DC=mycompany,DC=com or LDAP://DC=mycompany,DC=com
        {
            var matches = fragmentKeyPattern.Matches(value);
            var list = new List<string>();
            for (int i = 0; i < matches.Count; i++)
            {
                string raw;

                // extract raw fragments
                if (i < matches.Count - 1)
                {
                    raw = value.Substring(matches[i].Index, matches[i + 1].Index - matches[i].Index);
                }
                else
                {
                    raw = value.Substring(matches[i].Index);
                }

                // cleanup raw fragments 
                if (raw.EndsWith(","))
                {
                    raw = raw.Substring(0, raw.Length - 1);
                }
                raw = raw.Replace("\\,", ",");

                // add to list
                list.Add(raw);
            }
            var fragments = list
                .Select(s => DnFragment.Parse(s, value))  // DC=mycompany
                .ToList();
            var dn = new Dn(fragments);
            return dn;
        }

        public static implicit operator string(Dn dn) => dn.ToString();
    }
}
