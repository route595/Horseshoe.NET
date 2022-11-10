using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Db
{
    public class AutoSort<E>
    {
        public Func<E, IComparable> Sorter { get; }
        public IComparer<E> Comparer { get; }
        public Comparison<E> Comparison { get; }

        public AutoSort()
        {
        }

        public AutoSort(Func<E, IComparable> sorter)
        {
            Sorter = sorter;
        }

        public AutoSort(IComparer<E> comparer)
        {
            Comparer = comparer;
        }

        public AutoSort(Comparison<E> comparison)
        {
            Comparison = comparison;
        }
    }
}
